using System;
using ILOG.CP;
using ILOG.Concert;
using ILOG.CPLEX;
using ILOG.OPL;
using System.Collections;
using System.IO;

namespace SubProblemCP
{
    public class CPModelRosterGeneration
    {
        public CP roster;
        public INumToNumStepFunction resource_AveIntern;
        public INumToNumStepFunction[][] resource_AveDisc_dh; // if hospital has this discipline + if intern can pass this discipline in this hospital
            
        public IIntervalVar[] disc_d;
        public IIntervalVar[][] discHosp_dh;
        public ICumulFunctionExpr allK;        
        public IIntervalVar[] discNReNR_d;
        public ICumulFunctionExpr[] hospChange_h;
        public IIntervalVar[][] discHospNReNR_dh;
        public IIntervalSequenceVar sequence;
        public IIntVar[][] change_dD;
        public IIntVar[][] y_dD;
        public IIntVar desire;
        public IIntVar waitingTime;
        

        public DataLayer.AllData data;
        public int theIntern;

        public void initialVar() 
        {
            roster = new CP();

            // intern availbility
            resource_AveIntern = roster.NumToNumStepFunction(0, data.General.TimePriods, 100, "AvailibilityOfIntern");
            for (int t = 0; t < data.General.TimePriods; t++)
            {
                if (!data.Intern[theIntern].Ave_t[t])
                {
                    resource_AveIntern.SetValue(t, t + 1, 0);
                }
            }

            // availibility of the discipline in the hospital considering intern ability
            resource_AveDisc_dh = new INumToNumStepFunction[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                resource_AveDisc_dh[d] = new INumToNumStepFunction[data.General.Hospitals];
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    resource_AveDisc_dh[d][h] = roster.NumToNumStepFunction(0, data.General.TimePriods, 100, "AvailibilityOfIntern");
                    bool flag = false;
                    for (int w = 0; w < data.General.HospitalWard && !flag; w++)
                    {
                        if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[theIntern].Abi_dh[d][h]) // existance of discipline in hospital + ability
                        {
                            flag = true;
                        }
                    }

                    if (flag)
                    {
                        for (int t = 0; t < data.General.TimePriods; t++)
                        {
                            resource_AveIntern.SetValue(t, t + 1, 0);
                        }
                    }
                }
            }

            // discipline 
            disc_d = new IIntervalVar[data.General.Disciplines];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                disc_d[d] = roster.IntervalVar();
                disc_d[d].EndMax = data.General.TimePriods;
                disc_d[d].EndMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                disc_d[d].LengthMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                disc_d[d].LengthMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                disc_d[d].SizeMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                disc_d[d].SizeMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                disc_d[d].StartMax = data.General.TimePriods;
                disc_d[d].StartMin = 0;
                disc_d[d].SetIntensity(resource_AveIntern, 100);
                disc_d[d].SetOptional();
            }



            // hospital assignment 
            discHosp_dh = new IIntervalVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discHosp_dh[d] = new IIntervalVar[data.General.Hospitals];
                for (int h = 0; h < [data.General.Hospitals; h++)
                {
                    discHosp_dh[d][h] = roster.IntervalVar();
                    discHosp_dh[d][h].EndMax = data.General.TimePriods;
                    discHosp_dh[d][h].EndMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                    discHosp_dh[d][h].LengthMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                    discHosp_dh[d][h].LengthMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                    discHosp_dh[d][h].SizeMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                    discHosp_dh[d][h].SizeMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                    discHosp_dh[d][h].StartMax = data.General.TimePriods;
                    discHosp_dh[d][h].StartMin = 0;
                    discHosp_dh[d][h].SetIntensity(resource_AveDisc_dh[d][h], 100);
                    discHosp_dh[d][h].SetOptional();
                }
                
            }

            // total number of courses
            allK = roster.CumulFunctionExpr();

            // discipline for not renewable resoureces 
            discNReNR_d = new IIntervalVar[data.General.Disciplines];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discNReNR_d[d] = roster.IntervalVar();
                discNReNR_d[d].EndMax = data.General.TimePriods;
                discNReNR_d[d].EndMin = data.General.TimePriods;
                discNReNR_d[d].LengthMax = data.General.TimePriods;
                discNReNR_d[d].LengthMin = data.General.TimePriods;
                discNReNR_d[d].SizeMax = data.General.TimePriods;
                discNReNR_d[d].SizeMin = data.General.TimePriods;
                discNReNR_d[d].StartMax = 0;
                discNReNR_d[d].StartMin = 0;
                discNReNR_d[d].SetOptional();
                allK.Add(roster.Pulse(discNReNR_d[d], data.Discipline[d].CourseCredit_p[data.Intern[theIntern].ProgramID]));
                roster.IfThen(roster.PresenceOf(disc_d[d]), roster.PresenceOf(discNReNR_d[d]));
            }

            // change in hospital
            hospChange_h = new ICumulFunctionExpr[data.General.Hospitals];
            for (int h = 0; h < data.General.Hospitals; h++)
            {
                hospChange_h[h] = roster.CumulFunctionExpr();
            }

            // hospital assignment 
            discHospNReNR_dh = new IIntervalVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discHospNReNR_dh[d] = new IIntervalVar[data.General.Hospitals];
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    discHospNReNR_dh[d][h] = roster.IntervalVar();
                    discHospNReNR_dh[d][h].EndMax = data.General.TimePriods;
                    discHospNReNR_dh[d][h].EndMin = data.General.TimePriods;
                    discHospNReNR_dh[d][h].LengthMax = data.General.TimePriods;
                    discHospNReNR_dh[d][h].LengthMin = data.General.TimePriods;
                    discHospNReNR_dh[d][h].SizeMax = data.General.TimePriods;
                    discHospNReNR_dh[d][h].SizeMin = data.General.TimePriods;
                    discHospNReNR_dh[d][h].StartMax = 0;
                    discHospNReNR_dh[d][h].StartMin = 0;
                    discHospNReNR_dh[d][h].SetOptional();
                    allK.Add(roster.Pulse(discHospNReNR_dh[d][h], 1));
                    roster.IfThen(roster.PresenceOf(discHosp_dh[d][h]), roster.PresenceOf(discHospNReNR_dh[d][h]));
                }

            }


            //change between hospitals
            change_dD = new IIntVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                change_dD[d] = new IIntVar[data.General.Disciplines];
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    change_dD[d][dd] = roster.IntVar(0, 1);
                    if (d == dd)
                    {
                        change_dD[d][dd] = roster.IntVar(0, 0);
                    }
                }
            }
            y_dD = new IIntVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                y_dD[d] = new IIntVar[data.General.Disciplines];
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    y_dD[d][dd] = roster.IntVar(0, 1);
                    if (d == dd)
                    {
                        y_dD[d][dd] = roster.IntVar(0, 0);
                    }
                }
            }

            // desire
            desire = roster.IntVar(0,data.AlgSettings.BigM, "Desire");
            //waiting time 
            waitingTime = roster.IntVar(0, data.General.TimePriods, "WaitingTime");
        }

        public void setTheConstraint() 
        {
            // no overlap for disciplines
            roster.Add(roster.NoOverlap(disc_d));

            // 1 discipline 1 hospital
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                roster.Add(roster.Alternative(disc_d[d], discHosp_dh[d]));
            }

            // total discipline 
            roster.Add(roster.Le(allK, data.Intern[theIntern].K_AllDiscipline));

            // group assignment
            for (int g = 0; g < data.General.DisciplineGr; g++)
            {
                IIntExpr groupedCours_g = roster.IntExpr();
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    if (data.Intern[theIntern].DisciplineList_dg[d][g])
                    {
                        groupedCours_g = roster.Sum(groupedCours_g, roster.Prod(data.Discipline[d].CourseCredit_p[data.Intern[theIntern].ProgramID], roster.PresenceOf(discipline_d[d])));
                    }
                }
                roster.AddGe(groupedCours_g, data.Intern[theIntern].ShouldattendInGr_g[g]);
            }

            // total change in hospital 
            for (int h = 0; h < data.General.Hospitals; h++)
            {
                roster.Add(roster.Le(hospChange_h[h], data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp));
            }

            // change calculating from d => ... => D indirect precedence 
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (d != dd)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            for (int hh = 0; hh < data.General.Hospitals; hh++)
                            {
                                if (d != dd && h != hh && data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp > 1)
                                {
                                    IIntExpr yyy = roster.IntExpr();
                                    yyy = roster.Sum(yyy, roster.Prod(data.General.TimePriods, y_dD[d][dd]));
                                    yyy = roster.Sum(yyy, roster.Prod(1, roster.EndOf(discHosp_dh[dd][hh])));
                                    yyy = roster.Sum(yyy, roster.Prod(-1, roster.StartOf(discHosp_dh[d][h])));
                                    roster.Add(roster.IfThen(roster.And(roster.PresenceOf(discHosp_dh[d][h]), roster.PresenceOf(discHosp_dh[dd][hh])), roster.AddGe(yyy, 0)));
                                }
                            }
                        }
                    }
                }
            }
            
            // calculating total change d => D
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (d == dd)
                    {
                        continue;
                    }
                    IIntExpr change = roster.IntExpr();
                    change = roster.Sum(change, change_dD[dd][d]);
                    change = roster.Sum(change, roster.Prod(-1, y_dD[dd][d]));
                    for (int ddd = 0; ddd < data.General.Disciplines; ddd++)
                    {
                        if (ddd == d || ddd == dd)
                        {
                            continue;
                        }
                        change = roster.Sum(change, change_dD[dd][ddd]);
                    }
                    for (int ddd = 0; ddd < data.General.Disciplines; ddd++)
                    {
                        if (ddd == d || ddd == dd)
                        {
                            continue;
                        }
                        change = roster.Sum(change, change_dD[ddd][d]);
                    }
                    roster.Add(roster.IfThen(roster.And(roster.PresenceOf(disc_d[d]), roster.PresenceOf(disc_d[dd])), roster.AddEq(change, 0)));


                }

            }

            // skill related precedence 
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (data.Discipline[d].Skill4D_dp[d][data.Intern[theIntern].ProgramID])
                    {
                        roster.Add(roster.IfThen(roster.PresenceOf(disc_d[d]),roster.And(roster.Le(roster.EndOf(disc_d[dd]), roster.StartOf(disc_d[d])),roster.PresenceOf(disc_d[dd]))));
                    }
                }
            }

            // waiting time
            IIntExpr makespan = roster.IntExpr();
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                makespan = roster.Max(makespan, roster.EndOf(disc_d[d]));
            }
            IIntExpr waitConst = roster.IntExpr();
            waitConst = roster.Sum(waitConst, waitingTime);
            waitConst = roster.Sum(waitConst, roster.Prod(-1, makespan));
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                waitConst = roster.Sum(waitConst, roster.Prod(data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID], 
                                                              roster.PresenceOf(disc_d[d])));
            }
            roster.AddEq(waitConst, 0);

            // desire
            IIntExpr desCons= roster.IntExpr();
            // prf
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    desCons = roster.Sum(desCons, roster.Prod(-(data.Intern[theIntern].Prf_d[d] 
                                                                + data.Intern[theIntern].Prf_h[d] 
                                                                + data.TrainingPr[data.Intern[theIntern].ProgramID].Prf_d[d]), roster.PresenceOf(discHosp_dh[d][h])));
                }
            }
            // desire Waiting time
            desCons = roster.Sum(desCons, roster.Prod(-data.Intern[theIntern].wieght_w,waitingTime));
            //change 
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (d != dd)
                    {
                        desCons = roster.Sum(desCons, roster.Prod(-data.Intern[theIntern].wieght_ch, change_dD[dd][d]));
                    }
                    if (data.TrainingPr[data.Intern[theIntern].ProgramID].cns_dD[dd][d] > 0)
                    {
                        roster.IfThen(roster.Eq(roster.EndOf(disc_d[dd]), roster.EndOf(disc_d[d])), desCons = );
                    }
                }
            }

        }

        public void setTheRC() { }

        public void setTheObjectiveFunction() { }

        public void solveTheModel() { }
    }
}

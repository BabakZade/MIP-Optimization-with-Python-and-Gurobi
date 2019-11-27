using System;
using ILOG.CP;
using ILOG.Concert;
using System.Collections;
using System.IO;
using ColumnAndBranchInfo;

namespace SubProblemCP
{
    public class CPModelRosterGeneration
    {
        public CP roster;
        public INumToNumStepFunction resource_AveIntern;
        public INumToNumStepFunction[][] resource_AveDisc_dh; // if hospital has this discipline + if intern can pass this discipline in this hospital

        public IIntervalVar[] disc_d;
        public IIntervalVar[][] discHosp_dh;
        public IIntervalVar[] discHospOneLine_dh;
        public ICumulFunctionExpr allK;
        public IIntervalVar[] discNReNR_d;
        public ICumulFunctionExpr[] hospUsage_h;
        
        public IIntervalVar[][] discHospNReNR_dh;
        public IIntervalSequenceVar sequenceDis;
        public IIntervalSequenceVar sequenceDisHosp;
        public IIntVar desire;
        public IIntVar waitingTime;
        public IIntVar[] change_d;
        public IIntVar[][] cns_dD;
        public IIntVar internVar;
        public INumExpr ReducedCost;
        public INumToNumSegmentFunction[][] startTimeRC_dh;
        public ArrayList theColumns;
        public int[] typeHosp;
        public int[] typeDis;
        public double[][][] RCStart_tdh;
        public double[][][] RCDual_tdh;

        public DataLayer.AllData data;
        public int theIntern;

        public CPModelRosterGeneration(double[] dual,int theIntern,ArrayList allBranches, DataLayer.AllData alldata) 
        {
            initial(theIntern,alldata);
            CPAlg(dual, theIntern, alldata, allBranches);
        }
        public void CPAlg(double[] dual, int theIntern, DataLayer.AllData allData, ArrayList allBranches)
        {
            initialVar(allBranches);
            setTheConstraint();
            setTheObjectiveFunction(dual);
        }
        

        public void initial(int theIntern,DataLayer.AllData alldata) 
        {
            this.theIntern = theIntern;
            data = alldata;
            new DataLayer.ArrayInitializer().CreateArray(ref RCStart_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new DataLayer.ArrayInitializer().CreateArray(ref RCDual_tdh,data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
        }

        /// <summary>
        /// intial all CP var
        /// </summary>
        public void initialVar(ArrayList allBranches)
        {
            roster = new CP();
            // reduced cost
            ReducedCost = roster.NumExpr();
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
                    resource_AveDisc_dh[d][h] = roster.NumToNumStepFunction(0, data.General.TimePriods, 100, "AvailibilityFunction");
                    bool heCan = false;
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        if (data.Hospital[h].Hospital_dw[d][w]  && data.Intern[theIntern].Abi_dh[d][h])
                        {
                            heCan = true;
                            break;
                        }
                    }

                    if (!heCan)
                    {
                        resource_AveDisc_dh[d][h] = roster.NumToNumStepFunction(0, data.General.TimePriods, 0, "AvailibilityFunction");
                    }
                }
            }

            // discipline 
            disc_d = new IIntervalVar[data.General.Disciplines];
            typeDis = new int[data.General.Disciplines];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                disc_d[d] = roster.IntervalVar("Disc[" + d + "]");
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
                
                typeDis[d] = d;
            }
            // no overlap for disciplines
            sequenceDis = roster.IntervalSequenceVar(disc_d, typeDis, "sequenceDis");
            roster.Add(roster.NoOverlap(sequenceDis));


            // hospital assignment 
            discHosp_dh = new IIntervalVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discHosp_dh[d] = new IIntervalVar[data.General.Hospitals + 1];
                for (int h = 0; h < data.General.Hospitals + 1; h++)
                {
                    if (h < data.General.Hospitals)
                    {
                        discHosp_dh[d][h] = roster.IntervalVar("discHosp_dh[" + d + "][" + h + "]");
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
                    else // oversea request 
                    {
                        bool flag = true;
                        for (int t = 0; t < data.General.TimePriods; t++)
                        {
                            if (data.Intern[theIntern].OverSea_dt[d][t])
                            {

                                discHosp_dh[d][h] = roster.IntervalVar("discHosp_dh[" + d + "][" + h + "]");
                                discHosp_dh[d][h].EndMax = data.General.TimePriods;
                                discHosp_dh[d][h].EndMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                                discHosp_dh[d][h].LengthMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                                discHosp_dh[d][h].LengthMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                                discHosp_dh[d][h].SizeMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                                discHosp_dh[d][h].SizeMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                                discHosp_dh[d][h].StartMax = t;
                                discHosp_dh[d][h].StartMin = t;
                                discHosp_dh[d][h].SetPresent();
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            discHosp_dh[d][h] = roster.IntervalVar("discHosp_dh[" + d + "][" + h + "]");
                            discHosp_dh[d][h].EndMax = data.General.TimePriods;
                            discHosp_dh[d][h].EndMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                            discHosp_dh[d][h].LengthMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                            discHosp_dh[d][h].LengthMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                            discHosp_dh[d][h].SizeMax = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                            discHosp_dh[d][h].SizeMin = data.Discipline[d].Duration_p[data.Intern[theIntern].ProgramID];
                            discHosp_dh[d][h].StartMax = data.General.TimePriods;
                            discHosp_dh[d][h].StartMin = 0;
                            discHosp_dh[d][h].SetAbsent();
                        }
                    }

                }

            }



            // total number of courses
            allK = roster.CumulFunctionExpr("AllCourses");

            // discipline for not renewable resoureces 
            discNReNR_d = new IIntervalVar[data.General.Disciplines];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discNReNR_d[d] = roster.IntervalVar("discNReNR_d[" + d + "]");
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
                roster.Add(roster.IfThen(roster.PresenceOf(disc_d[d]), roster.PresenceOf(discNReNR_d[d])));
            }

            // change in hospital
            hospUsage_h = new ICumulFunctionExpr[data.General.Hospitals];
            for (int h = 0; h < data.General.Hospitals; h++)
            {
                hospUsage_h[h] = roster.CumulFunctionExpr("hospUsage_h[" + h + "]");
            }

            // hospital assignment 
            discHospNReNR_dh = new IIntervalVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                discHospNReNR_dh[d] = new IIntervalVar[data.General.Hospitals];
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    discHospNReNR_dh[d][h] = roster.IntervalVar("discHospNReNR_dh[" + d + "][" + h + "]");
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

            // one line schedule
            typeHosp = new int[data.General.Disciplines * (data.General.Hospitals + 1)];
            discHospOneLine_dh = new IIntervalVar[data.General.Disciplines * (data.General.Hospitals + 1)];
            for (int dh = 0; dh < data.General.Disciplines * (data.General.Hospitals + 1); dh++)
            {
                int dIndex = dh % data.General.Disciplines;
                int hIndex = dh / data.General.Disciplines;
                discHospOneLine_dh[dh] = roster.IntervalVar("DHOneLine" + "[" + dIndex + "][" + hIndex + "]");
                typeHosp[dh] = hIndex;
                discHospOneLine_dh[dh].SetOptional();
                discHospOneLine_dh[dh] = discHosp_dh[dIndex][hIndex];
            }

            //change between hospitals
            change_d = new IIntVar[data.General.Disciplines];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                change_d[d] = roster.IntVar(0, 1, "change_d[" + d + "]");
                IIntExpr cut = roster.IntExpr();
                cut = roster.Sum(cut, change_d[d]);
                cut = roster.Sum(cut, roster.Prod(-1, roster.PresenceOf(disc_d[d])));
                roster.AddLe(cut, 0);
            }
            //consecutiveness
            cns_dD = new IIntVar[data.General.Disciplines][];
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                cns_dD[d] = new IIntVar[data.General.Disciplines];
                for (int D = 0; D < data.General.Disciplines; D++)
                {
                    cns_dD[d][D] = roster.IntVar(0, 1, "cns_dD[" + d + "][" + D + "]");
                }

            }


            // desire
            desire = roster.IntVar(0, data.AlgSettings.BigM, "Desire");
            //waiting time 
            waitingTime = roster.IntVar(0, data.General.TimePriods, "WaitingTime");

            


            // Branches
            foreach (Branch item in allBranches)
            {
                if (item.BrIntern == theIntern)
                {
                    if (item.branch_status)
                    {
                        if (item.BrTypePrecedence)
                        {
                            roster.Add(roster.Eq(roster.PresenceOf(disc_d[item.BrPrDisc]), roster.PresenceOf(disc_d[item.BrDisc])));
                            roster.Add(roster.EndAtStart(disc_d[item.BrPrDisc], disc_d[item.BrDisc]));
                            discHosp_dh[item.BrDisc][item.BrHospital].SetPresent();
                        }
                        if (item.BrTypeStartTime)
                        {
                            discHosp_dh[item.BrDisc][item.BrHospital].SetPresent();
                            discHosp_dh[item.BrDisc][item.BrHospital].StartMax = item.BrTime;
                            discHosp_dh[item.BrDisc][item.BrHospital].StartMin = item.BrTime;
                        }
                    }
                    else
                    {
                        if (item.BrTypePrecedence)
                        {
                            IIntVar brLeft = roster.IntVar(0, 1, item.displayYdDHIndex());
                            roster.Add(roster.IfThen(roster.And(roster.PresenceOf(discHosp_dh[item.BrPrDisc][item.BrHospital]), roster.Eq(roster.TypeOfNext(sequenceDis, disc_d[item.BrPrDisc], item.BrPrDisc, item.BrPrDisc), item.BrDisc)), roster.AddEq(brLeft, 1)));
                            ReducedCost = roster.Sum(ReducedCost, roster.Prod(-data.AlgSettings.BigM, brLeft));
                        }
                        if (item.BrTypeStartTime)
                        {
                            roster.Add(roster.Neq(roster.StartOf(discHosp_dh[item.BrDisc][item.BrHospital]), item.BrTime));
                        }

                    }
                }

            }

        }


        /// <summary>
        /// create all constraint 
        /// </summary>
        public void setTheConstraint()
        {
            internVar = roster.IntVar(1, 1, "TheIntern_" + theIntern);
            

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
                        groupedCours_g = roster.Sum(groupedCours_g, roster.Prod(data.Discipline[d].CourseCredit_p[data.Intern[theIntern].ProgramID], roster.PresenceOf(disc_d[d])));
                    }
                }
                roster.AddGe(groupedCours_g, data.Intern[theIntern].ShouldattendInGr_g[g]);
            }

            // changes between hospital
            sequenceDisHosp = roster.IntervalSequenceVar(discHospOneLine_dh, typeHosp, "OneLineSeq");
            roster.Add(roster.NoOverlap(sequenceDisHosp));


            // total change in hospital 
            for (int h = 0; h < data.General.Hospitals; h++)
            {
                roster.Add(roster.Le(hospUsage_h[h], data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp));
            }
            for (int dh = 0; dh < data.General.Disciplines * (data.General.Hospitals + 1); dh++)
            {
                int dIndex = dh % data.General.Disciplines;
                int hIndex = dh / data.General.Disciplines;
                
                IIntExpr chngD = roster.IntExpr();
                chngD = roster.Sum(chngD, change_d[dIndex]);
                
                roster.Add(roster.IfThen(roster.Neq(roster.TypeOfNext(sequenceDisHosp, discHospOneLine_dh[dh], typeHosp[dh], typeHosp[dh]), typeHosp[dh]), roster.Eq(chngD, 1, "ChangeD[" + dIndex + "]")));
                
            }
            // consecutiveness
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    IIntExpr cnsecutiveConst = roster.IntExpr();
                    cnsecutiveConst = roster.Sum(cnsecutiveConst, cns_dD[dd][d]);
                    roster.Add(roster.IfThenElse(roster.And(roster.PresenceOf(disc_d[dd]), roster.Eq(roster.TypeOfNext(sequenceDis, disc_d[dd],dd, dd), d)), roster.Eq(cnsecutiveConst, 1), roster.Eq(cnsecutiveConst, 0, "cnsDd[" + dd + "][" + d + "]")));
                }
            }

            // skill related precedence 
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (data.Discipline[d].Skill4D_dp[d][data.Intern[theIntern].ProgramID])
                    {
                        roster.Add(roster.Ge(roster.PresenceOf(disc_d[dd]), roster.PresenceOf(disc_d[d])));
                        roster.Add(roster.Before(sequenceDis, disc_d[dd], disc_d[d]));
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
            INumExpr desCons = roster.NumExpr();
            desCons = roster.Sum(desCons, desire);
            // prf
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    double x =   data.Intern[theIntern].wieght_d * data.Intern[theIntern].Prf_d[d]
                               + data.Intern[theIntern].wieght_h * data.Intern[theIntern].Prf_h[h]
                               + data.TrainingPr[data.Intern[theIntern].ProgramID].weight_p * data.TrainingPr[data.Intern[theIntern].ProgramID].Prf_d[d];
                    desCons = roster.Sum(desCons, roster.Prod(-x, roster.PresenceOf(discHosp_dh[d][h])));
                }
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    if (data.Intern[theIntern].OverSea_dt[d][t])
                    {
                        double x = data.Intern[theIntern].wieght_d * data.Intern[theIntern].Prf_d[d]
                              + data.TrainingPr[data.Intern[theIntern].ProgramID].weight_p * data.TrainingPr[data.Intern[theIntern].ProgramID].Prf_d[d];
                        desCons = roster.Sum(desCons, roster.Prod(-x, roster.PresenceOf(discHosp_dh[d][data.General.Hospitals + 1])));

                    }
                }

            }
            // desire Waiting time

            desCons = roster.Sum(desCons, roster.Prod(-data.Intern[theIntern].wieght_w, waitingTime));
            //change and consec
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                desCons = roster.Sum(desCons, roster.Prod(-data.Intern[theIntern].wieght_ch, change_d[d]));

                for (int dd = 0; dd < data.General.Disciplines; dd++)
                {
                    if (data.TrainingPr[data.Intern[theIntern].ProgramID].cns_dD[dd][d] > 0)
                    {
                        desCons = roster.Sum(desCons, roster.Prod(-data.Intern[theIntern].wieght_cns, cns_dD[dd][d]));
                    }
                }
            }

            roster.AddEq(desCons, 0);
            StreamWriter xxxx = new StreamWriter("des.txt");
            xxxx.WriteLine(desCons.ToString());
            xxxx.Close();
        }

        /// <summary>
        /// calculates the coefficient for each discipline and hospital if they start at the specefic time
        /// it also initialze the RCStartTime_tdh
        /// </summary>
        /// <param name="dual">the dual comming from Master problem</param>
        public void setRCStart(double[] dual) 
        {
            try
            {
                int Constraint_Counter = 0;

                // Constraint 2
                for (int i = 0; i < data.General.Interns; i++)
                {
                    Constraint_Counter++;
                }

                // Constraint 3 
                for (int r = 0; r < data.General.Region; r++)
                {
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        if (data.Intern[theIntern].TransferredTo_r[r])
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                for (int h = 0; h < data.General.Hospitals; h++)
                                {
                                    if (data.Hospital[h].InToRegion_r[r])
                                    {
                                        RCDual_tdh[t][d][h] -= dual[Constraint_Counter];
                                    }

                                }
                            }
                        }
                        Constraint_Counter++;
                    }
                }

                // Constraint 4 

                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[theIntern].isProspective)
                                {
                                    RCDual_tdh[t][d][h] -= dual[Constraint_Counter];
                                }


                            }

                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 5

                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        for (int h = 0; h < data.General.Hospitals ; h++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d ][w])
                                {
                                    RCDual_tdh[t][d][h] -= dual[Constraint_Counter];
                                    int theP = data.Intern[theIntern].ProgramID;
                                    for (int tt = t; tt < t + data.Discipline[d].Duration_p[theP]; tt++)
                                    {
                                        
                                    }
                                }


                            }

                            Constraint_Counter++;

                        }
                    }
                }

                //for (int h = 0; h < data.General.Hospitals; h++)
                //{
                //    Console.WriteLine("Hospital " + h );
                //    for (int d = 0; d < data.General.Disciplines; d++)
                //    {
                //        for (int t = 0; t < data.General.TimePriods; t++)
                //        {

                //            Console.Write(RCDual_tdh[t][d][h] + " ");
                //        }

                //        Console.WriteLine();
                //    }
                //}

                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int d = 0; d < data.General.Disciplines; d++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            int theP = data.Intern[theIntern].ProgramID;
                            for (int tt = t; tt < t + data.Discipline[d].Duration_p[theP] && tt < data.General.TimePriods; tt++)
                            {
                                RCStart_tdh[t][d][h] += RCDual_tdh[tt][d][h];
                            }
                        }
                    }
                }
                // RC start time 
                double[] breakPoints = new double[2 * data.General.TimePriods];
                double[] slope = new double[2 * data.General.TimePriods];
                for (int t = 0; t < 2 * data.General.TimePriods -1 ; t++)
                {
                    breakPoints[t] = t/2;
                    breakPoints[t + 1] = t/2;
                }

                startTimeRC_dh = new INumToNumSegmentFunction[data.General.Disciplines][];
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    startTimeRC_dh[d] = new INumToNumSegmentFunction[data.General.Hospitals];
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        for (int t = 0; t < 2 * data.General.TimePriods ;t+=2)
                        {
                            slope[t] = 100;
                            slope[t + 1] = RCStart_tdh[t / 2][d][h];

                        }
                        startTimeRC_dh[d][h] = roster.NumToNumSegmentFunction(breakPoints, slope);
                        
                    }
                }
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        /// <summary>
        /// calculates the reduced costs and add it to the model
        /// </summary>
        /// <param name="dual">dual comming from Master problem</param>
        public void setTheRC(double[] dual)
        {
            try
            {
                
                ReducedCost = roster.Sum(ReducedCost, roster.Prod(data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi, desire));


                int Constraint_Counter = 0;

                // Constraint 2
                for (int i = 0; i < data.General.Interns; i++)
                {
                    if (i == theIntern)
                    {
                        ReducedCost = roster.Sum(ReducedCost, roster.Prod(-dual[Constraint_Counter], internVar));

                    }
                    Constraint_Counter++;
                }

                // Constraint 3 
                for (int r = 0; r < data.General.Region; r++)
                {
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        Constraint_Counter++;
                    }
                }

                // Constraint 4 

                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {

                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 5

                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        for (int h = 0; h < data.General.Hospitals ; h++)
                        {
                            

                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 6

                for (int i = 0; i < data.General.Interns; i++)
                {
                    int theP = data.Intern[theIntern].ProgramID;
                    if (data.Intern[i].ProgramID == theP && theIntern == i)
                    {
                        ReducedCost = roster.Sum(ReducedCost, roster.Prod(dual[Constraint_Counter], desire));
                        
                    }
                    Constraint_Counter++;
                }

                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        ReducedCost = roster.Sum(ReducedCost, roster.StartEval(discHosp_dh[d][h], startTimeRC_dh[d][h], 0));
                    }
                }
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void setTheObjectiveFunction(double[] dual) 
        {
            setRCStart(dual);
            setTheRC(dual);
            // add it to be greater than zero for satisfactory problem
            roster.AddGe(ReducedCost, 2 * data.AlgSettings.RCepsi);
            roster.AddMaximize(ReducedCost);
        }

        public bool KeepGoing(double[] dual, string InsName)
        {
            bool flag = false;
            theColumns = new ArrayList();
            
            roster.SetParameter(CP.IntParam.TimeMode, CP.ParameterValues.ElapsedTime);
            roster.SetParameter(CP.IntParam.LogVerbosity, CP.ParameterValues.Quiet);
            roster.SetParameter(CP.DoubleParam.TimeLimit, data.AlgSettings.SubTime) ;
            roster.StartNewSearch();
            //roster.ExportModel(data.allPath.OutPutGr + "Roster" + InsName + ".cpo");
            // solve it now 
            while (roster.Next())
            {
                //roster.ExportModel(data.allPath.OutPutGr + "Roster" + InsName + ".cpo");
                ColumnInternBasedDecomposition column = new ColumnInternBasedDecomposition(data,theIntern);
                
                column.desire = roster.GetValue(desire);
                Console.WriteLine();
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals + 1; h++)
                    {
                        if (roster.IsPresent(discHosp_dh[d][h]))
                        {
                            int theT = roster.GetStart(discHosp_dh[d][h]);
                            column.S_tdh[theT][d][h] = true;
                            Console.WriteLine("d: " + d + " h: " + h + " t: " + theT);
                        }
                    }
                }
                column.setYdDFromStartTime(data);
                column.setRosterFromStartTime(data);
                column.setReducedCost(dual,data);
                double RCColumn = column.setReducedCost(dual, data);
                int totalChnage = 0;
                
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    if (roster.GetValue(change_d[d]) > 0.5)
                    {
                        Console.WriteLine("change for " + d);
                        totalChnage++;
                    }
                }

                for (int dh = 0; dh < data.General.Disciplines * (data.General.Hospitals + 1); dh++)
                {
                    int dIndex = dh % data.General.Disciplines;
                    int hIndex = dh / data.General.Disciplines;
                    if (roster.IsPresent(discHospOneLine_dh[dh]))
                    {
                        Console.WriteLine(typeHosp[dh]);
                        Console.WriteLine(roster.GetNext(sequenceDisHosp, discHospOneLine_dh[dh]));
                    }
                }

                column.calculteDes(data);
                if (Math.Abs(roster.GetValue(desire) - column.desire) > data.AlgSettings.RCepsi)
                {
                    Console.WriteLine("CP objective {0} != Column objective {1}", roster.GetValue(desire), column.desire);
                    Console.WriteLine("Waiting {0}, totalChanges {1}", roster.GetValue(waitingTime), totalChnage);
                    Console.WriteLine(data.Intern[theIntern].wieght_ch);
                    int x = 0;
                }
                if (Math.Abs( RCColumn - roster.GetObjValue(0)) > data.AlgSettings.RCepsi)
                {
                    Console.WriteLine("CP objective {0} != Column objective {1}", roster.GetObjValue(0), RCColumn);
                }
                if (!column.isColumnFeasible(data))
                {
                    Console.WriteLine();
                }
                if (RCColumn > data.AlgSettings.RCepsi)
                {
                    theColumns.Add(column);
                }
                flag = true;
            }
            roster.End();
            return flag;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;
using System.Collections;
using System.IO;
using System.Diagnostics;
using DataLayer;

namespace BranchAndPriceAlgorithm
{
    public class SubProblem
    {
        public bool SolutionFound;
        public AllData data;
        public long ElapsedTime;
        public Cplex MIPModel;
        public IIntVar[][][] y_dDh;
        public IIntVar[][][] s_dth;
        public INumVar[] w_d;
        public INumVar[] ch_d;
        public INumVar des;
        public INumVar MinPD;
        public INumVar MaxND;
        public IIntVar internVar;   
        public int theIntern;
        public double constantRC;

        public string[][][] Y_dDh;
        public string[][][] S_dth;
        public string[] W_d;
        public string[] Ch_d;
        public string Des;



        public int Interns;
        public int Disciplins;
        public int Hospitals;
        public int Timepriods;
        public int TrainingPr;
        public int Wards;
        public int Regions;
        public int DisciplineGr;

        public ArrayList theColumns;

        public SubProblem(AllData InputData, ArrayList AllBranches, double[] dual, int theIntern, string InsName)
        {
            data = InputData;
            this.theIntern = theIntern;
            MIPalgorithem(AllBranches, dual, InsName);
        }
        public void MIPalgorithem(ArrayList AllBranches, double[] dual, string InsName)
        {
            Initial();
            InitialMIPVar(AllBranches);
            setReducedCost(dual);
            CreateModel();            
        }
        public void SetSol()
        {
            ILinearNumExpr sol = MIPModel.LinearNumExpr();
            //sol.AddTerm(w_d[1][1], 1);
            MIPModel.AddEq(sol, 7);
        }
        public void Initial()
        {

            Interns = data.General.Interns;

            // discipline + 1 for dummy d = 0 is dummy
            Disciplins = data.General.Disciplines + 1;
            Wards = data.General.HospitalWard;
            Regions = data.General.Region;
            DisciplineGr = data.General.DisciplineGr;
            // one hospital oversea H
            Hospitals = data.General.Hospitals + 1;
            Timepriods = data.General.TimePriods;
            TrainingPr = data.General.TrainingPr;
        }
        public void InitialMIPVar(ArrayList AllBranches)
        {

            MIPModel = new Cplex();
            
                Y_dDh = new string[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    Y_dDh[d] = new string[Disciplins][];
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        Y_dDh[d][dd] = new string[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            Y_dDh[d][dd][h] = "Y_dDh[" + theIntern + "][" + d + "][" + dd + "][" + h + "]";
                        }
                    }
                }
            


           
                // discipline + 1 for dummy d = 0 is dummy
                y_dDh = new IIntVar[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    y_dDh[d] = new IIntVar[Disciplins][];
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        y_dDh[d][dd] = new IIntVar[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            y_dDh[d][dd][h] = MIPModel.IntVar(0, 1, Y_dDh[d][dd][h]);

                            // not oversea
                            if (h < Hospitals - 1)
                            {
                                bool overSea = false;
                                for (int t = 0; t < Timepriods && dd > 0; t++)
                                {
                                    if (data.Intern[theIntern].OverSea_dt[dd - 1][t])
                                    {
                                        overSea = true;
                                    }
                                }
                                bool ddinWard = false;
                                for (int w = 0; w < Wards && dd > 0; w++)
                                {
                                    if (data.Hospital[h].Hospital_dw[dd - 1][w])
                                    {
                                        ddinWard = true;
                                    }
                                }
                                if (dd == 0 || dd == d)// dd can not be 0
                                {
                                    y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                }
                                else if (!data.Intern[theIntern].Abi_dh[dd - 1][h]) // ability for dd in Hospital
                                {
                                    y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                }
                                else if (overSea) // if dd should happen in oversea
                                {
                                    y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                }
                                else if (!ddinWard)// availability of dd in hospital
                                {
                                    y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                }
                                else if (d > 0) //when at least one of them not in the the list
                                {
                                    bool dexist = false;
                                    bool ddexist = false;
                                    for (int g = 0; g < DisciplineGr; g++)
                                    {
                                        if (data.Intern[theIntern].DisciplineList_dg[dd - 1][g])
                                        {
                                            ddexist = true;
                                        }
                                        if (d != 0 && data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                                        {
                                            dexist = true;
                                        }
                                    }
                                    if (!dexist || !ddexist)
                                    {
                                        y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                    }
                                }
                                else if (d == 0)
                                {
                                    bool ddexist = false;
                                    for (int g = 0; g < DisciplineGr; g++)
                                    {
                                        if (data.Intern[theIntern].DisciplineList_dg[dd - 1][g])
                                        {
                                            ddexist = true;
                                        }

                                    }
                                    if (!ddexist)
                                    {
                                        y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                    }
                                }
                            }
                            else
                            {
                                bool FHexist = false;
                                if (dd == 0)
                                {
                                    y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                }
                                else
                                {
                                    for (int t = 0; t < Timepriods; t++)
                                    {
                                        if (data.Intern[theIntern].OverSea_dt[dd - 1][t])
                                        {
                                            FHexist = true;
                                        }
                                    }
                                    if (!FHexist)
                                    {
                                        y_dDh[d][dd][h] = MIPModel.IntVar(0, 0, Y_dDh[d][dd][h]);
                                    }
                                }

                            }


                            MIPModel.Add(y_dDh[d][dd][h]);
                        }
                    }
                }


            

            S_dth = new string[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    S_dth[d] = new string[Timepriods][];
                    for (int t = 0; t < Timepriods; t++)
                    {
                        S_dth[d][t] = new string[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            S_dth[d][t][h] = "S_dth[" + theIntern + "][" + d + "][" + t + "][" + h + "]";
                        }
                    }
                }
            

            
                s_dth = new IIntVar[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    s_dth[d] = new IIntVar[Timepriods][];
                    for (int t = 0; t < Timepriods; t++)
                    {
                        s_dth[d][t] = new IIntVar[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            s_dth[d][t][h] = MIPModel.IntVar(0, 1, S_dth[d][t][h]);
                            if (d == 0 && t != 0)
                            {
                                s_dth[d][t][h] = MIPModel.IntVar(0, 0, S_dth[d][t][h]);
                            }
                            else if (d > 0)
                            {
                                bool dexist = false;
                                for (int g = 0; g < DisciplineGr; g++)
                                {
                                    if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                                    {
                                        dexist = true;
                                    }
                                }
                                if (!dexist)
                                {
                                    s_dth[d][t][h] = MIPModel.IntVar(0, 0, S_dth[d][t][h]);
                                }
                            }

                            MIPModel.Add(s_dth[d][t][h]);
                        }
                    }
                }


            // Branches
            foreach (Branch item in AllBranches)
            {
                if (item.BrIntern == theIntern)
                {
                    if (item.branch_status)
                    {
                        if (item.BrTypePrecedence)
                        {
                            y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital] = MIPModel.IntVar(1, 1, Y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital]);
                            MIPModel.Add(y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital]);
                        }
                        if (item.BrTypeStartTime)
                        {
                            s_dth[item.BrDisc][item.BrTime][item.BrHospital] = MIPModel.IntVar(1, 1, S_dth[item.BrDisc][item.BrTime][item.BrHospital]);
                            MIPModel.Add(s_dth[item.BrDisc][item.BrTime][item.BrHospital]);
                        }
                    }
                    else
                    {
                        if (item.BrTypePrecedence)
                        {
                            y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital] = MIPModel.IntVar(0, 0, Y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital]);
                            MIPModel.Add(y_dDh[item.BrPrDisc][item.BrDisc][item.BrHospital]);
                        }
                        if (item.BrTypeStartTime)
                        {
                            s_dth[item.BrDisc][item.BrTime][item.BrHospital] = MIPModel.IntVar(0, 0, S_dth[item.BrDisc][item.BrTime][item.BrHospital]);
                            MIPModel.Add(s_dth[item.BrDisc][item.BrTime][item.BrHospital]);
                        }

                    }
                }

            }


            W_d = new string[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    W_d[d] = "W_d[" + theIntern + "][" + d + "]";
                }
            

                w_d = new INumVar[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    w_d[d] = MIPModel.NumVar(0, Timepriods, W_d[d]);
                    if (d == 0)
                    {
                        w_d[d] = MIPModel.NumVar(0, 0, W_d[d]);
                    }
                    else
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                            {
                                dexist = true;
                            }
                        }
                        if (!dexist)
                        {
                            w_d[d] = MIPModel.NumVar(0, 0, W_d[d]);
                        }

                    }
                    MIPModel.Add(w_d[d]);
                }
            

                Ch_d = new string[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    Ch_d[d] = "Ch_d[" + theIntern + "][" + d + "]";
                }
            

            
                ch_d = new INumVar[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    ch_d[d] = MIPModel.NumVar(0, 1, Ch_d[d]);
                    if (d == 0)
                    {
                        ch_d[d] = MIPModel.NumVar(0, 0, Ch_d[d]);
                    }
                    else
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                            {
                                dexist = true;

                            }
                        }
                        if (!dexist)
                        {
                            ch_d[d] = MIPModel.NumVar(0, 0, Ch_d[d]);
                        }

                    }
                    MIPModel.Add(ch_d[d]);
                }
            

                Des = "Des_[" + theIntern + "]";
            
           
                des = MIPModel.NumVar(-int.MaxValue,
                    int.MaxValue, Des);
            
            internVar = MIPModel.IntVar(1,
                    1, "TheIntern_"+theIntern);



            MinPD = MIPModel.NumVar(-int.MaxValue, int.MaxValue, "MinP");
            MaxND = MIPModel.NumVar(-int.MaxValue, int.MaxValue, "MaxN");
        }

        public void CreateModel()
        {
            // Discipline List  
            for (int i = 0; i < Interns; i++)
            {
                for (int g = 0; g < DisciplineGr; g++)
                {
                    ILinearNumExpr mandatory = MIPModel.LinearNumExpr();
                    int fulfilled = 0;
                    for (int d = 1; d < Disciplins; d++)
                    {
                        if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                        {
                            for (int h = 0; h < Hospitals - 1; h++)
                            {
                                if (data.Intern[theIntern].Fulfilled_dhp[d - 1][h][data.Intern[theIntern].ProgramID])
                                {
                                    fulfilled += data.Discipline[d - 1].CourseCredit_p[data.Intern[theIntern].ProgramID];
                                }
                            }

                            for (int dd = 0; dd < Disciplins; dd++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    mandatory.AddTerm(y_dDh[dd][d][h], data.Discipline[d - 1].CourseCredit_p[data.Intern[theIntern].ProgramID]);
                                }
                            }
                        }
                    }

                    MIPModel.AddGe(mandatory, data.Intern[theIntern].ShouldattendInGr_g[g] - fulfilled, "DisciplineGroupIG_" + i + "_" + g);

                }
            }

            // All disciplines == Sum_g
            for (int i = 0; i < Interns; i++)
            {
                ILinearNumExpr AllDiscipline = MIPModel.LinearNumExpr();
                // check if the training program and involved mandatory discipline
                int totalFulfilled = 0;
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        if (data.Intern[theIntern].Fulfilled_dhp[d - 1][h][data.Intern[theIntern].ProgramID])
                        {
                            totalFulfilled += data.Discipline[d - 1].CourseCredit_p[data.Intern[theIntern].ProgramID];
                        }

                    }
                }
                for (int d = 1; d < Disciplins; d++)
                {
                    bool dexist = false;
                    for (int g = 0; g < DisciplineGr; g++)
                    {
                        if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                        {
                            dexist = true;
                        }
                    }
                    for (int dd = 0; dd < Disciplins && dexist; dd++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            AllDiscipline.AddTerm(y_dDh[dd][d][h], data.Discipline[d - 1].CourseCredit_p[data.Intern[theIntern].ProgramID]);
                        }
                    }
                }
                int totalList = 0;
                for (int g = 0; g < DisciplineGr; g++)
                {
                    totalList += data.Intern[theIntern].ShouldattendInGr_g[g];
                }
                MIPModel.AddEq(AllDiscipline, totalList - totalFulfilled, "AllDisciplineI" + "_" + i);
            }

            // Each discipline Once,
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    ILinearNumExpr DiscAssign = MIPModel.LinearNumExpr();

                    // taking course 
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            DiscAssign.AddTerm(1, y_dDh[dd][d][h]);
                        }
                    }
                    // AKA_dD
                    for (int D = 1; D < Disciplins; D++)
                    {
                        for (int p = 0; p < TrainingPr; p++)
                        {
                            if (data.TrainingPr[p].AKA_dD[d - 1][D - 1])
                            {
                                for (int dd = 0; dd < Disciplins; dd++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        DiscAssign.AddTerm(1, y_dDh[dd][D][h]);
                                    }
                                }
                            }
                        }

                    }

                    int totalFF = 0;


                    // fullfiled d
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        for (int p = 0; p < TrainingPr; p++)
                        {
                            if (data.Intern[theIntern].Fulfilled_dhp[d - 1][h][p])
                            {
                                totalFF++;

                            }
                        }
                    }
                    // fulfilled AKA_dd
                    for (int dd = 1; dd < Disciplins; dd++)
                    {
                        for (int h = 0; h < Hospitals - 1; h++)
                        {
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                if (data.Intern[theIntern].Fulfilled_dhp[dd - 1][h][p] && data.TrainingPr[p].AKA_dD[d - 1][dd - 1])
                                {
                                    totalFF++;

                                }
                            }
                        }
                    }


                    MIPModel.AddLe(DiscAssign, 1 - totalFF, "AssignOnceID_" + i + "_" + d);
                }
            }

            // Foreign Hospital
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    // check if there are oversea demand
                    bool FH = false;
                    for (int t = 0; t < Timepriods; t++)
                    {
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[theIntern].DisciplineList_dg[d - 1][g] && data.Intern[theIntern].OverSea_dt[d - 1][t])
                            {
                                FH = true;
                            }
                        }
                    }
                    if (FH)
                    {
                        ILinearNumExpr OverseaH = MIPModel.LinearNumExpr();
                        for (int dd = 0; dd < Disciplins; dd++)
                        {
                            OverseaH.AddTerm(1, y_dDh[dd][d][Hospitals - 1]);
                        }
                        MIPModel.AddEq(OverseaH, 1, "OverseaID_" + i + "_" + d);
                    }

                }
            }

            // Ability Hospital related
            // we consider it in variable definition 

            // total allowed change in hospital
            for (int i = 0; i < Interns; i++)
            {
                for (int h = 0; h < Hospitals - 1; h++)
                {
                    ILinearNumExpr change = MIPModel.LinearNumExpr();
                    for (int d = 1; d < Disciplins; d++)
                    {
                        for (int dd = 0; dd < Disciplins; dd++)
                        {
                            change.AddTerm(1, y_dDh[dd][d][h]);
                        }
                    }
                    int fulfilled = 0;
                    for (int d = 1; d < Disciplins; d++)
                    {
                        if (data.Intern[theIntern].Fulfilled_dhp[d - 1][h][data.Intern[theIntern].ProgramID])
                        {
                            fulfilled++;
                        }
                    }
                    int RHS = data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp - fulfilled;
                    MIPModel.AddLe(change, RHS, "ChangeIH_" + i + "_" + h);
                }
            }

            // follow each discipline 
            for (int i = 0; i < Interns; i++)
            {
                for (int dd = 1; dd < Disciplins; dd++)
                {
                    ILinearNumExpr follownext = MIPModel.LinearNumExpr();
                    // check if the training program and involved mandatory discipline
                    for (int d = 1; d < Disciplins; d++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            follownext.AddTerm(y_dDh[dd][d][h], 1);
                        }
                    }
                    for (int d = 0; d < Disciplins; d++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            follownext.AddTerm(y_dDh[d][dd][h], -1);
                        }
                    }
                    MIPModel.AddLe(follownext, 0, "follownextID" +
                        "_" + i + "_" + dd);
                }
            }

            // dummy discipline 
            for (int i = 0; i < Interns; i++)
            {
                if (data.Intern[theIntern].K_AllDiscipline > 0)
                {
                    ILinearNumExpr dummy = MIPModel.LinearNumExpr();
                    for (int d = 1; d < Disciplins; d++)
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[theIntern].DisciplineList_dg[d - 1][g])
                            {
                                dexist = true;
                            }
                        }
                        bool hexist = false;
                        for (int h = 0; h < Hospitals && dexist; h++)
                        {

                            for (int w = 0; w < Wards && h < Hospitals - 1; w++)
                            {
                                if (data.Hospital[h].Hospital_dw[d - 1][w])
                                {
                                    hexist = true;
                                }
                            }
                            if (hexist || h == Hospitals - 1)
                            {
                                dummy.AddTerm(y_dDh[0][d][h], 1);
                            }

                        }
                    }

                    MIPModel.AddEq(dummy, 1, "dummyI_" + i);
                }

            }


            // one start time for each discipline 
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int h = 0; h < Hospitals; h++)
                    {
                        ILinearNumExpr onestarttime = MIPModel.LinearNumExpr();
                        // check if the training program and involved mandatory discipline

                        for (int t = 0; t < Timepriods; t++)
                        {
                            onestarttime.AddTerm(s_dth[d][t][h], 1);

                        }
                        for (int dd = 0; dd < Disciplins; dd++)
                        {
                            onestarttime.AddTerm(y_dDh[dd][d][h], -1);
                        }
                        MIPModel.AddEq(onestarttime, 0, "OneStartIDH" +
                            "_" + i + "_" + d + "_" + h);

                    }
                }
            }

            // Start time si <= wi + sj + dj + BM
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        ILinearNumExpr starttime = MIPModel.LinearNumExpr();

                        int dur = 0;
                        if (dd != 0)
                        {
                            dur = data.Discipline[dd - 1].Duration_p[data.Intern[theIntern].ProgramID];
                        }
                        for (int h = 0; h < Hospitals; h++)
                        {
                            starttime.AddTerm(y_dDh[dd][d][h], Timepriods);

                            for (int tt = 0; tt < Timepriods; tt++)
                            {
                                starttime.AddTerm(s_dth[dd][tt][h], -(tt + dur));
                                starttime.AddTerm(s_dth[d][tt][h], tt);
                            }
                        }

                        starttime.AddTerm(w_d[d], -1);

                        MIPModel.AddLe(starttime, Timepriods, "StartWaitIdD_" + i + "_" + d + "_" + dd);

                    }
                }
            }

            // Start time si >= wi + sj + dj - BM
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        ILinearNumExpr starttime = MIPModel.LinearNumExpr();
                        int dur = 0;
                        if (dd != 0)
                        {
                            dur = data.Discipline[dd - 1].Duration_p[data.Intern[theIntern].ProgramID];
                        }
                        for (int h = 0; h < Hospitals; h++)
                        {
                            starttime.AddTerm(y_dDh[dd][d][h], -Timepriods);

                            for (int tt = 0; tt < Timepriods; tt++)
                            {
                                starttime.AddTerm(s_dth[dd][tt][h], -(tt + dur));
                                starttime.AddTerm(s_dth[d][tt][h], tt);
                            }
                        }
                        starttime.AddTerm(w_d[d], -1);

                        MIPModel.AddGe(starttime, -Timepriods, "StartIdD_" + i + "_" + d + "_" + dd);
                    }
                }
            }

            // availability for interns 
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {
                        ILinearNumExpr avail = MIPModel.LinearNumExpr();
                        for (int h = 0; h < Hospitals; h++)
                        {
                            avail.AddTerm(s_dth[d][t][h], data.Discipline[d - 1].Duration_p[data.Intern[theIntern].ProgramID]);
                        }
                        int rhs = 0;
                        for (int tt = t; tt < t + data.Discipline[d - 1].Duration_p[data.Intern[theIntern].ProgramID] && tt < Timepriods; tt++)
                        {
                            rhs += data.Intern[theIntern].Ave_t[tt] ? 1 : 0;
                        }
                        MIPModel.AddLe(avail, rhs, "AvailbilityIDT_" + i + "_" + d + "_" + t);
                    }
                }
            }

            // oversea start time 
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {
                        if (data.Intern[theIntern].OverSea_dt[d - 1][t])
                        {
                            ILinearNumExpr OverseaStart = MIPModel.LinearNumExpr();
                            int RHS = 0;
                            OverseaStart.AddTerm(t, s_dth[d][t][Hospitals - 1]);
                            RHS = t;
                            MIPModel.AddEq(OverseaStart, RHS, "OverSeaStartID_" + i + "_" + d);
                            break;
                        }
                    }


                }
            }

            // oversea Requirement 
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    bool oversea = false;
                    for (int t = 0; t < Timepriods; t++)
                    {
                        if (data.Intern[theIntern].OverSea_dt[d - 1][t])
                        {
                            oversea = true;
                            break;
                        }
                    }
                    for (int dd = 1; dd < Disciplins && oversea; dd++)
                    {
                        ILinearNumExpr fhRequirement = MIPModel.LinearNumExpr();
                        for (int t = 0; t < Timepriods; t++)
                        {
                            fhRequirement.AddTerm(t, s_dth[d][t][Hospitals - 1]);
                        }
                        if (data.Intern[theIntern].FHRequirment_d[dd - 1])
                        {
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    fhRequirement.AddTerm(-t, s_dth[dd][t][h]);
                                }
                            }
                        }
                        MIPModel.AddGe(fhRequirement, 0, "FHrequirementIdD_" + i + "_" + d + "_" + dd);
                    }
                }
            }


            // skill
            for (int i = 0; i < Interns; i++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int dd = 1; dd < Disciplins; dd++)
                    {
                        bool fulfilled = false;
                        for (int h = 0; h < Hospitals - 1; h++)
                        {
                            if (data.Intern[theIntern].Fulfilled_dhp[d - 1][h][data.Intern[theIntern].ProgramID])
                            {
                                fulfilled = true;
                            }
                        }
                        int trPr = data.Intern[theIntern].ProgramID;
                        if (data.Discipline[d - 1].Skill4D_dp[dd - 1][trPr] && !fulfilled)
                        {
                            ILinearNumExpr skill = MIPModel.LinearNumExpr();

                            for (int h = 0; h < Hospitals; h++)
                            {
                                for (int tt = 0; tt < Timepriods; tt++)
                                {
                                    skill.AddTerm(s_dth[d][tt][h], tt);
                                    skill.AddTerm(s_dth[dd][tt][h], -tt);
                                    skill.AddTerm(s_dth[d][tt][h], -Timepriods);
                                    skill.AddTerm(s_dth[dd][tt][h], Timepriods);
                                }
                            }
                            MIPModel.AddGe(skill, 0, "SkillIDd_" + i + "_" + d + "_" + dd);
                        }
                    }
                }
            }

            // change 
            for (int i = 0; i < Interns; i++)
            {
                if (data.Intern[theIntern].wieght_ch != 0)
                {
                    for (int d = 1; d < Disciplins; d++)
                    {
                        for (int dd = 1; dd < Disciplins; dd++)
                        {
                            for (int h = 0; h < Hospitals - 1; h++)
                            {
                                if (d == dd)
                                {
                                    break;
                                }
                                ILinearNumExpr change = MIPModel.LinearNumExpr();

                                change.AddTerm(ch_d[d], 1);
                                change.AddTerm(y_dDh[dd][d][h], -1);
                                for (int ddd = 0; ddd < Disciplins; ddd++)
                                {
                                    change.AddTerm(y_dDh[ddd][dd][h], 1);
                                }


                                MIPModel.AddGe(change, 0, "ChangeIdDH_" + i + "_" + d + "_" + dd + "_" + h);
                            }
                        }
                    }
                }
            }

            // des
            for (int i = 0; i < Interns; i++)
            {
                ILinearNumExpr internDes = MIPModel.LinearNumExpr();
                internDes.AddTerm(des, 1);
                for (int d = 1; d < Disciplins; d++)
                {
                    internDes.AddTerm(ch_d[d], -(double)data.Intern[theIntern].wieght_ch);
                    internDes.AddTerm(w_d[d], -(double)data.Intern[theIntern].wieght_w);
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        for (int h = 0; h < Hospitals - 1; h++)
                        {
                            internDes.AddTerm(y_dDh[dd][d][h], -(double)data.Intern[theIntern].Prf_d[d - 1] * data.Intern[theIntern].wieght_d);
                            internDes.AddTerm(y_dDh[dd][d][h], -(double)data.Intern[theIntern].Prf_h[h] * data.Intern[theIntern].wieght_h);
                            internDes.AddTerm(y_dDh[dd][d][h], -(double)data.TrainingPr[data.Intern[theIntern].ProgramID].weight_p * data.TrainingPr[data.Intern[theIntern].ProgramID].Prf_d[d - 1]);

                            // consecutive 
                            if (dd > 0 && data.TrainingPr[data.Intern[theIntern].ProgramID].cns_dD[dd - 1][d - 1] > 0)
                            {
                                internDes.AddTerm(y_dDh[dd][d][h], -(double)data.TrainingPr[data.Intern[theIntern].ProgramID].cns_dD[dd - 1][d - 1] * data.Intern[theIntern].wieght_cns);
                            }
                        }
                    }
                }
                MIPModel.AddEq(internDes, 0, "DesI" + i);
            }

        }
        public void setReducedCost(double[] dual)
        {
            try
            {
                ILinearNumExpr TmpRC = MIPModel.LinearNumExpr();
                TmpRC.AddTerm(data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi,des);


                int Constraint_Counter = 0;
                
                // Constraint 2
                for (int i = 0; i < Interns; i++)
                {
                    if (i == theIntern)
                    {
                        TmpRC.AddTerm(-dual[Constraint_Counter], internVar);
                        
                    }
                    Constraint_Counter++;
                }

                // Constraint 3 
                for (int r = 0; r < Regions; r++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {

                        if (data.Intern[theIntern].TransferredTo_r[r])
                        {
                            for (int d = 1; d < Disciplins; d++)
                            {
                                for (int h = 0; h < Hospitals-1; h++)
                                {
                                    if (data.Hospital[h].InToRegion_r[r])
                                    {
                                        int theP = data.Intern[theIntern].ProgramID;
                                        for (int tt = Math.Max(0, t - data.Discipline[d - 1].Duration_p[theP] + 1); tt <= t; tt++)
                                        {
                                            TmpRC.AddTerm(-dual[Constraint_Counter], s_dth[d][tt][h]);   
                                        }
                                    }

                                }
                            }
                        }
                        Constraint_Counter++;
                    }
                }

                // Constraint 4 

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals-1; h++)
                        {
                                for (int d = 1; d < Disciplins; d++)
                                {
                                    if (data.Hospital[h].Hospital_dw[d-1][w] && data.Intern[theIntern].isProspective)
                                    {
                                        int theP = data.Intern[theIntern].ProgramID;
                                        for (int tt = Math.Max(0, t - data.Discipline[d-1].Duration_p[theP] + 1); tt <= t; tt++)
                                        {
                                            TmpRC.AddTerm(-dual[Constraint_Counter], s_dth[d][tt][h]);
                                            

                                        }
                                    }


                                }
                            
                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 5

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals-1; h++)
                        {
                                for (int d = 1; d < Disciplins; d++)
                                {
                                    if (data.Hospital[h].Hospital_dw[d-1][w])
                                    {
                                        int theP = data.Intern[theIntern].ProgramID;
                                        for (int tt = Math.Max(0, t - data.Discipline[d-1].Duration_p[theP] + 1); tt <= t; tt++)
                                        {
                                            TmpRC.AddTerm(-dual[Constraint_Counter], s_dth[d][tt][h]);


                                        }
                                    }


                                }
                            
                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 6
                
                    for (int i = 0; i < Interns; i++)
                    {
                        int theP = data.Intern[theIntern].ProgramID;
                        if (data.Intern[i].ProgramID == theP && theIntern == i)
                        {
                            TmpRC.AddTerm(+dual[Constraint_Counter], des);
                        }
                        Constraint_Counter++;
                    }
                

                MIPModel.AddMaximize(TmpRC, "ReducedCost_"+theIntern);
                MIPModel.AddGe(TmpRC, 3 * data.AlgSettings.RCepsi, "RCconstraint_" + theIntern); // feasibility problem
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }

        }



        /// <summary>
        /// this function returns true if there is at least one column
        /// </summary>
        /// <returns>true if there is a suitable list </returns>
        public bool KeepGoing(double[] dual, string InsName)
        {
            bool flag = true;
            theColumns = new ArrayList();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //*************set program
           // MIPModel.ExportModel(data.allPath.OutPutGr + InsName + "SP.lp");
            MIPModel.SetParam(Cplex.DoubleParam.EpRHS, 0.00000001);
            MIPModel.SetParam(Cplex.DoubleParam.EpOpt, data.AlgSettings.RCepsi);
            MIPModel.SetParam(Cplex.IntParam.Threads, 3);
            MIPModel.SetParam(Cplex.DoubleParam.TiLim, data.AlgSettings.SubTime);
            MIPModel.SetParam(Cplex.BooleanParam.MemoryEmphasis, true);
            //MIPModel.SetParam(Cplex.Param.MIP.Pool.RelGap, 1);

            if (!MIPModel.Populate())
            {
                flag = false;
            }
            else

            {
                stopwatch.Stop();
                ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                int numsol = MIPModel.GetSolnPoolNsolns();
                System.Console.WriteLine("The solution pool contains " + numsol +
                                   " solutions.");
                for (int i = 0; i < numsol; i++)
                {

                    ColumnInternBasedDecomposition tmpColumn = setColumn(i);

                    if (tmpColumn.setReducedCost(dual, data) > data.AlgSettings.RCepsi)
                    {
                        theColumns.Add(tmpColumn);
                    }
                }

            }


            MIPModel.End();
            System.GC.Collect();
            return flag;
        }

        public ColumnInternBasedDecomposition setColumn(int solpoolIndex)
        {
            ColumnInternBasedDecomposition theColumn = new ColumnInternBasedDecomposition(data);
            theColumn.theIntern = theIntern;
            theColumn.objectivefunction = MIPModel.GetObjValue(solpoolIndex);
            theColumn.desire = MIPModel.GetValue(des, solpoolIndex);
            int totalChange = 0;
            for (int d = 1; d < Disciplins; d++)
            {
                totalChange += (int)MIPModel.GetValue(ch_d[d], solpoolIndex);
            }
            theColumn.totalChange = totalChange;
            for (int t = 0; t < Timepriods; t++)
            {
                for (int d = 1; d < Disciplins; d++)
                {
                    for (int h = 0; h < Hospitals + 1; h++)
                    {
                        if (MIPModel.GetValue(s_dth[d][t][h], solpoolIndex) > 1 - 0.5)
                        {
                            theColumn.S_tdh[t][d - 1][h] = true;
                            theColumn.theRoster.Add(new RoosterPosition
                            {
                                theDiscipline = d - 1,
                                theHospital = h,
                                theTime = t
                            });


                        }
                    }
                }
            }

            for (int d = 0; d < Disciplins; d++)
            {
                for (int dd = 1; dd < Disciplins; dd++)
                {
                    for (int h = 0; h < Hospitals; h++)
                    {
                        if (MIPModel.GetValue(y_dDh[d][dd][h], solpoolIndex) > 0.5)
                        {
                                theColumn.Y_dDh[d][dd][h] = true;
                                                       
                        }
                    }
                }
            }


            return theColumn;
        }
    }
}

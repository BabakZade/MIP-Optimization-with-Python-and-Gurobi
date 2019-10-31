using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using ILOG.Concert;
using ILOG.CPLEX;
using MultiObjIntInfo = ILOG.CPLEX.Cplex.MultiObjIntInfo;
using MultiObjLongInfo = ILOG.CPLEX.Cplex.MultiObjLongInfo;
using MultiObjNumInfo = ILOG.CPLEX.Cplex.MultiObjNumInfo;
using System.Diagnostics;

namespace GeneralMIPAlgorithm
{
    public class MedicalTraineeSchedulingMIP
    {
        public bool SolutionFound;
        public AllData data;
        public OptimalSolution mipOpt;
        public long ElapsedTime;
        public Cplex MIPModel;
        public IIntVar[][][][] y_idDh;
        public IIntVar[][][][] s_idth;
        public INumVar[][] w_id;
        public INumVar[][] ch_id;
        public INumVar[] des_i;
        public INumVar[] dp_i;
        public INumVar[] dn_i;
        public INumVar[] desMin_p;
        public INumVar MinPD;
        public INumVar MaxND;
        public INumVar[][][] Res_twh;
        public INumVar[][][] Emr_twh;
        public INumVar[][][] sld_twh;
        public INumVar[][] AccSl_tr;
        public INumVar[] s_obj;
        public INumVar[] RBigM_obj;
        public string[][][][] Y_idDh;
        public string[][][][] S_idth;
        public string[][] W_id;
        public string[][] Ch_id;
        public string[] Des_i;
        public string[] dP_i;
        public string[] dN_i;
        public string[][][] RES_twh;
        public string[][][] EMR_twh;
        public string[][][] SLD_twh;
        public double[] multiObjectiveValue;
        public string[][] ACCSL_tr;
        public string[] DESMin_p;
        public bool notFeasible;
        public bool[][][][] warmStartTime_itdh;
        public int Interns;
        public int Disciplins;
        public int Hospitals;
        public int Timepriods;
        public int TrainingPr;
        public int Wards;
        public int Regions;
        public int DisciplineGr;
        public MedicalTraineeSchedulingMIP(AllData InputData, string InsName, bool CPLEXFirstNode, int NewTimeLimit)
        {
            data = InputData;
            MIPalgorithem(InsName, CPLEXFirstNode, NewTimeLimit);
        }
        public MedicalTraineeSchedulingMIP(AllData InputData, AUGMECONS augmentedaddon, bool ifRoot,  string InsName)
        {
            data = InputData;
            SolveAUGMECON(InsName,augmentedaddon,ifRoot);
        }
        public void MIPalgorithem(string InsName, bool CPLEXFirstNode, int NewTimeLimit)
        {
            Initial();
            InitialMIPVar();
            SetObjective();
            CreateModel();
            setDemandConstraint();
            //SetSol();
            solve_MIPmodel(InsName, CPLEXFirstNode, NewTimeLimit);
        }
        public void SetSol()
        {
            ILinearNumExpr sol = MIPModel.LinearNumExpr();



            sol.AddTerm(s_idth[02][12][00][01], 1);
            sol.AddTerm(s_idth[02][10][01][00], 1);
            sol.AddTerm(s_idth[02][11][02][02], 1);
           

            //sol.AddTerm(w_id[1][1], 1);
            MIPModel.AddEq(sol, 3,"setSol");
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
        public void InitialMIPVar()
        {

            MIPModel = new Cplex();
            Y_idDh = new string[Interns][][][];
            for (int i = 0; i < Interns; i++)
            {
                Y_idDh[i] = new string[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    Y_idDh[i][d] = new string[Disciplins][];
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        Y_idDh[i][d][dd] = new string[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            Y_idDh[i][d][dd][h] = "Y_idDh[" + i + "][" + d + "][" + dd + "][" + h + "]";
                        }
                    }
                }
            }


            y_idDh = new IIntVar[Interns][][][];
            for (int i = 0; i < Interns; i++)
            {
                // discipline + 1 for dummy d = 0 is dummy
                y_idDh[i] = new IIntVar[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    y_idDh[i][d] = new IIntVar[Disciplins][];
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        y_idDh[i][d][dd] = new IIntVar[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 1, Y_idDh[i][d][dd][h]);

                            // not oversea
                            if (h < Hospitals - 1)
                            {
                                bool overSea = false;
                                for (int t = 0; t < Timepriods && dd > 0; t++)
                                {
                                    if (data.Intern[i].OverSea_dt[dd - 1][t])
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
                                    y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                }
                                else if (!data.Intern[i].Abi_dh[dd - 1][h]) // ability for dd in Hospital
                                {
                                    y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                }
                                else if (overSea) // if dd should happen in oversea
                                {
                                    y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                }
                                else if (!ddinWard)// availability of dd in hospital
                                {
                                    y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                }
                                else if (d > 0) //when at least one of them not in the the list
                                {
                                    bool dexist = false;
                                    bool ddexist = false;
                                    for (int g = 0; g < DisciplineGr; g++)
                                    {
                                        if (data.Intern[i].DisciplineList_dg[dd - 1][g])
                                        {
                                            ddexist = true;
                                        }
                                        if (d != 0 && data.Intern[i].DisciplineList_dg[d - 1][g])
                                        {
                                            dexist = true;
                                        }
                                    }
                                    if (!dexist || !ddexist)
                                    {
                                        y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                    }
                                }
                                else if (d == 0)
                                {
                                    bool ddexist = false;
                                    for (int g = 0; g < DisciplineGr; g++)
                                    {
                                        if (data.Intern[i].DisciplineList_dg[dd - 1][g])
                                        {
                                            ddexist = true;
                                        }

                                    }
                                    if (!ddexist)
                                    {
                                        y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                    }
                                }
                            }
                            else
                            {
                                bool FHexist = false;
                                if (dd == 0)
                                {
                                    y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                }
                                else
                                {
                                    for (int t = 0; t < Timepriods; t++)
                                    {
                                        if (data.Intern[i].OverSea_dt[dd - 1][t])
                                        {
                                            FHexist = true;
                                        }
                                    }
                                    if (!FHexist)
                                    {
                                        y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
                                    }
                                }

                            }


                            MIPModel.Add(y_idDh[i][d][dd][h]);
                        }
                    }
                }
            }

            S_idth = new string[Interns][][][];
            for (int i = 0; i < Interns; i++)
            {
                S_idth[i] = new string[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    S_idth[i][d] = new string[Timepriods][];
                    for (int t = 0; t < Timepriods; t++)
                    {
                        S_idth[i][d][t] = new string[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            S_idth[i][d][t][h] = "S_idth[" + i + "][" + d + "][" + t + "][" + h + "]";
                        }
                    }
                }
            }

            s_idth = new IIntVar[Interns][][][];
            for (int i = 0; i < Interns; i++)
            {
                s_idth[i] = new IIntVar[Disciplins][][];
                for (int d = 0; d < Disciplins; d++)
                {
                    s_idth[i][d] = new IIntVar[Timepriods][];
                    for (int t = 0; t < Timepriods; t++)
                    {
                        s_idth[i][d][t] = new IIntVar[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            s_idth[i][d][t][h] = MIPModel.IntVar(0, 1, S_idth[i][d][t][h]);
                            if (d == 0 && t != 0)
                            {
                                s_idth[i][d][t][h] = MIPModel.IntVar(0, 0, S_idth[i][d][t][h]);
                            }
                            else if (d > 0)
                            {
                                bool dexist = false;
                                for (int g = 0; g < DisciplineGr; g++)
                                {
                                    if (data.Intern[i].DisciplineList_dg[d - 1][g])
                                    {
                                        dexist = true;
                                    }
                                }
                                if (!dexist)
                                {
                                    s_idth[i][d][t][h] = MIPModel.IntVar(0, 0, S_idth[i][d][t][h]);
                                }
                            }

                            MIPModel.Add(s_idth[i][d][t][h]);
                        }
                    }
                }
            }

            W_id = new string[Interns][];
            for (int i = 0; i < Interns; i++)
            {
                W_id[i] = new string[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    W_id[i][d] = "W_id[" + i + "][" + d + "]";
                }
            }

            w_id = new INumVar[Interns][];
            for (int i = 0; i < Interns; i++)
            {
                w_id[i] = new INumVar[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    w_id[i][d] = MIPModel.NumVar(0, Timepriods, W_id[i][d]);
                    if (d == 0)
                    {
                        w_id[i][d] = MIPModel.NumVar(0, 0, W_id[i][d]);
                    }
                    else
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[i].DisciplineList_dg[d - 1][g])
                            {
                                dexist = true;
                            }
                        }
                        if (!dexist)
                        {
                            w_id[i][d] = MIPModel.NumVar(0, 0, W_id[i][d]);
                        }

                    }
                    MIPModel.Add(w_id[i][d]);
                }
            }

            Ch_id = new string[Interns][];
            for (int i = 0; i < Interns; i++)
            {
                Ch_id[i] = new string[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    Ch_id[i][d] = "Ch_id[" + i + "][" + d + "]";
                }
            }

            ch_id = new INumVar[Interns][];
            for (int i = 0; i < Interns; i++)
            {
                ch_id[i] = new INumVar[Disciplins];
                for (int d = 0; d < Disciplins; d++)
                {
                    ch_id[i][d] = MIPModel.NumVar(0, 1, Ch_id[i][d]);
                    if (d == 0)
                    {
                        ch_id[i][d] = MIPModel.NumVar(0, 0, Ch_id[i][d]);
                    }
                    else
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[i].DisciplineList_dg[d - 1][g])
                            {
                                dexist = true;

                            }
                        }
                        if (!dexist)
                        {
                            ch_id[i][d] = MIPModel.NumVar(0, 0, Ch_id[i][d]);
                        }

                    }
                    MIPModel.Add(ch_id[i][d]);
                }
            }

            Des_i = new string[Interns];
            for (int i = 0; i < Interns; i++)
            {
                Des_i[i] = "Des_i[" + i + "]";
            }
            des_i = new INumVar[Interns];
            double Max = 0;
            for (int i = 0; i < Interns; i++)
            {
                des_i[i] = MIPModel.NumVar(-int.MaxValue,
                    int.MaxValue, Des_i[i]);
            }

            dP_i = new string[Interns];
            for (int i = 0; i < Interns; i++)
            {
                dP_i[i] = "dP_i[" + i + "]";
            }
            dp_i = new INumVar[Interns];

            for (int i = 0; i < Interns; i++)
            {
                dp_i[i] = MIPModel.NumVar(0,
                    int.MaxValue, dP_i[i]);
            }

            dN_i = new string[Interns];
            for (int i = 0; i < Interns; i++)
            {
                dN_i[i] = "dN_i[" + i + "]";
            }
            dn_i = new INumVar[Interns];

            for (int i = 0; i < Interns; i++)
            {
                dn_i[i] = MIPModel.NumVar(0,
                    int.MaxValue, dN_i[i]);
            }

            RES_twh = new string[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                RES_twh[t] = new string[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    RES_twh[t][w] = new string[Hospitals];
                    for (int h = 0; h < Hospitals; h++)
                    {
                        RES_twh[t][w][h] = "RES_twh[" + t + "][" + w + "][" + h + "]";
                    }
                }
            }

            Res_twh = new INumVar[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                Res_twh[t] = new INumVar[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    Res_twh[t][w] = new INumVar[Hospitals];
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        Res_twh[t][w][h] = MIPModel.NumVar(0, data.Hospital[h].ReservedCap_tw[t][w], RES_twh[t][w][h]);
                        //MIPModel.Add(Res_twh[t][w][h]);
                    }
                }
            }

            EMR_twh = new string[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                EMR_twh[t] = new string[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    EMR_twh[t][w] = new string[Hospitals];
                    for (int h = 0; h < Hospitals; h++)
                    {
                        EMR_twh[t][w][h] = "EMR_twh[" + t + "][" + w + "][" + h + "]";

                    }
                }
            }

            Emr_twh = new INumVar[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                Emr_twh[t] = new INumVar[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    Emr_twh[t][w] = new INumVar[Hospitals];
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        Emr_twh[t][w][h] = MIPModel.NumVar(0, data.Hospital[h].EmergencyCap_tw[t][w], EMR_twh[t][w][h]);
                        //MIPModel.Add(Emr_twh[t][w][h]);
                    }
                }
            }

            SLD_twh = new string[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                SLD_twh[t] = new string[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    SLD_twh[t][w] = new string[Hospitals];
                    for (int h = 0; h < Hospitals; h++)
                    {
                        SLD_twh[t][w][h] = "SLD_twh[" + t + "][" + w + "][" + h + "]";
                    }
                }
            }

            sld_twh = new INumVar[Timepriods][][];
            for (int t = 0; t < Timepriods; t++)
            {
                sld_twh[t] = new INumVar[Wards][];
                for (int w = 0; w < Wards; w++)
                {
                    sld_twh[t][w] = new INumVar[Hospitals];
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        sld_twh[t][w][h] = MIPModel.NumVar(0, data.Hospital[h].HospitalMinDem_tw[t][w], SLD_twh[t][w][h]);
                        //MIPModel.Add(sld_twh[t][w][h]);
                    }
                }
            }

            ACCSL_tr = new string[Timepriods][];
            for (int t = 0; t < Timepriods; t++)
            {
                ACCSL_tr[t] = new string[Regions];
                for (int r = 0; r < Regions; r++)
                {
                    ACCSL_tr[t][r] = "ACCSL_tr[" + t + "][" + r + "]";
                }
            }

            AccSl_tr = new INumVar[Timepriods][];
            for (int t = 0; t < Timepriods; t++)
            {
                AccSl_tr[t] = new INumVar[Regions];
                for (int r = 0; r < Regions; r++)
                {
                    AccSl_tr[t][r] = MIPModel.NumVar(0, data.Region[r].AvaAcc_t[t], ACCSL_tr[t][r]);
                    MIPModel.Add(AccSl_tr[t][r]);
                }
            }
            DESMin_p = new string[TrainingPr];
            for (int p = 0; p < TrainingPr; p++)
            {
                DESMin_p[p] = "DESMin_p[" + p + "]";
            }
            desMin_p = new INumVar[TrainingPr];
            for (int p = 0; p < TrainingPr; p++)
            {
                desMin_p[p] = MIPModel.NumVar(0, int.MaxValue, DESMin_p[p]);
            }

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
                        if (data.Intern[i].DisciplineList_dg[d - 1][g])
                        {
                            for (int h = 0; h < Hospitals - 1; h++)
                            {
                                if (data.Intern[i].Fulfilled_dhp[d - 1][h][data.Intern[i].ProgramID])
                                {
                                    fulfilled += data.Discipline[d - 1].CourseCredit_p[data.Intern[i].ProgramID];
                                }
                            }

                            for (int dd = 0; dd < Disciplins; dd++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    mandatory.AddTerm(y_idDh[i][dd][d][h], data.Discipline[d - 1].CourseCredit_p[data.Intern[i].ProgramID]);
                                }
                            }
                        }
                    }

                    MIPModel.AddGe(mandatory, data.Intern[i].ShouldattendInGr_g[g] - fulfilled, "DisciplineGroupIG_" + i + "_" + g);

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
                        if (data.Intern[i].Fulfilled_dhp[d - 1][h][data.Intern[i].ProgramID])
                        {
                            totalFulfilled += data.Discipline[d - 1].CourseCredit_p[data.Intern[i].ProgramID];
                        }

                    }
                }
                for (int d = 1; d < Disciplins; d++)
                {
                    bool dexist = false;
                    for (int g = 0; g < DisciplineGr; g++)
                    {
                        if (data.Intern[i].DisciplineList_dg[d - 1][g])
                        {
                            dexist = true;
                        }
                    }
                    for (int dd = 0; dd < Disciplins && dexist; dd++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            AllDiscipline.AddTerm(y_idDh[i][dd][d][h], data.Discipline[d - 1].CourseCredit_p[data.Intern[i].ProgramID]);
                        }
                    }
                }
                int totalList = 0;
                for (int g = 0; g < DisciplineGr; g++)
                {
                    totalList += data.Intern[i].ShouldattendInGr_g[g];
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
                            DiscAssign.AddTerm(1, y_idDh[i][dd][d][h]);
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
                                        DiscAssign.AddTerm(1, y_idDh[i][dd][D][h]);
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
                            if (data.Intern[i].Fulfilled_dhp[d - 1][h][p])
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
                                if (data.Intern[i].Fulfilled_dhp[dd - 1][h][p] && data.TrainingPr[p].AKA_dD[d - 1][dd - 1])
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
                            if (data.Intern[i].DisciplineList_dg[d - 1][g] && data.Intern[i].OverSea_dt[d - 1][t])
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
                            OverseaH.AddTerm(1, y_idDh[i][dd][d][Hospitals - 1]);
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
                            change.AddTerm(1, y_idDh[i][dd][d][h]);
                        }
                    }
                    int fulfilled = 0;
                    for (int d = 1; d < Disciplins; d++)
                    {
                        if (data.Intern[i].Fulfilled_dhp[d - 1][h][data.Intern[i].ProgramID])
                        {
                            fulfilled++;
                        }
                    }
                    int RHS = data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp - fulfilled;
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
                            follownext.AddTerm(y_idDh[i][dd][d][h], 1);
                        }
                    }
                    for (int d = 0; d < Disciplins; d++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            follownext.AddTerm(y_idDh[i][d][dd][h], -1);
                        }
                    }
                    MIPModel.AddLe(follownext, 0, "follownextID" +
                        "_" + i + "_" + dd);
                }
            }

            // dummy discipline 
            for (int i = 0; i < Interns; i++)
            {
                if (data.Intern[i].K_AllDiscipline > 0)
                {
                    ILinearNumExpr dummy = MIPModel.LinearNumExpr();
                    for (int d = 1; d < Disciplins; d++)
                    {
                        bool dexist = false;
                        for (int g = 0; g < DisciplineGr; g++)
                        {
                            if (data.Intern[i].DisciplineList_dg[d - 1][g])
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
                                dummy.AddTerm(y_idDh[i][0][d][h], 1);
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
                            onestarttime.AddTerm(s_idth[i][d][t][h], 1);

                        }
                        for (int dd = 0; dd < Disciplins; dd++)
                        {
                            onestarttime.AddTerm(y_idDh[i][dd][d][h], -1);
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
                            dur = data.Discipline[dd - 1].Duration_p[data.Intern[i].ProgramID];
                        }
                        for (int h = 0; h < Hospitals; h++)
                        {
                            starttime.AddTerm(y_idDh[i][dd][d][h], Timepriods);

                            for (int tt = 0; tt < Timepriods; tt++)
                            {
                                starttime.AddTerm(s_idth[i][dd][tt][h], -(tt + dur));
                                starttime.AddTerm(s_idth[i][d][tt][h], tt);
                            }
                        }

                        starttime.AddTerm(w_id[i][d], -1);

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
                            dur = data.Discipline[dd - 1].Duration_p[data.Intern[i].ProgramID];
                        }
                        for (int h = 0; h < Hospitals; h++)
                        {
                            starttime.AddTerm(y_idDh[i][dd][d][h], -Timepriods);

                            for (int tt = 0; tt < Timepriods; tt++)
                            {
                                starttime.AddTerm(s_idth[i][dd][tt][h], -(tt + dur));
                                starttime.AddTerm(s_idth[i][d][tt][h], tt);
                            }
                        }
                        starttime.AddTerm(w_id[i][d], -1);

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
                            avail.AddTerm(s_idth[i][d][t][h], data.Discipline[d - 1].Duration_p[data.Intern[i].ProgramID]);
                        }
                        int rhs = 0;
                        for (int tt = t; tt < t + data.Discipline[d - 1].Duration_p[data.Intern[i].ProgramID] && tt < Timepriods; tt++)
                        {
                            rhs += data.Intern[i].Ave_t[tt] ? 1 : 0;
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
                        if (data.Intern[i].OverSea_dt[d - 1][t])
                        {
                            ILinearNumExpr OverseaStart = MIPModel.LinearNumExpr();
                            int RHS = 0;
                            OverseaStart.AddTerm(t, s_idth[i][d][t][Hospitals - 1]);
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
                        if (data.Intern[i].OverSea_dt[d - 1][t])
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
                            fhRequirement.AddTerm(t, s_idth[i][d][t][Hospitals - 1]);
                        }
                        if (data.Intern[i].FHRequirment_d[dd - 1])
                        {
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    fhRequirement.AddTerm(-t, s_idth[i][dd][t][h]);
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
                            if (data.Intern[i].Fulfilled_dhp[d - 1][h][data.Intern[i].ProgramID])
                            {
                                fulfilled = true;
                            }
                        }
                        int trPr = data.Intern[i].ProgramID;
                        if (data.Discipline[d - 1].Skill4D_dp[dd - 1][trPr] && !fulfilled)
                        {
                            ILinearNumExpr skill = MIPModel.LinearNumExpr();

                            for (int h = 0; h < Hospitals; h++)
                            {
                                for (int tt = 0; tt < Timepriods; tt++)
                                {
                                    skill.AddTerm(s_idth[i][d][tt][h], tt);
                                    skill.AddTerm(s_idth[i][dd][tt][h], -tt);
                                    skill.AddTerm(s_idth[i][d][tt][h], -Timepriods);
                                    skill.AddTerm(s_idth[i][dd][tt][h], Timepriods);

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
                if (data.Intern[i].wieght_ch != 0)
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

                                change.AddTerm(ch_id[i][d], 1);
                                change.AddTerm(y_idDh[i][dd][d][h], -1);
                                for (int ddd = 0; ddd < Disciplins; ddd++)
                                {
                                    change.AddTerm(y_idDh[i][ddd][dd][h], 1);
                                }

                                MIPModel.AddGe(change, 0, "ChangeIdDH_" + i + "_" + d + "_" + dd + "_" + h);
                            }
                        }
                    }
                }
            }

            // des_i
            for (int i = 0; i < Interns; i++)
            {
                ILinearNumExpr internDes = MIPModel.LinearNumExpr();
                internDes.AddTerm(des_i[i], 1);
                for (int d = 1; d < Disciplins; d++)
                {
                    internDes.AddTerm(ch_id[i][d], -(double)data.Intern[i].wieght_ch);
                    internDes.AddTerm(w_id[i][d], -(double)data.Intern[i].wieght_w);
                    for (int dd = 0; dd < Disciplins; dd++)
                    {
                        for (int h = 0; h < Hospitals - 1; h++)
                        {
                            internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.Intern[i].Prf_d[d - 1] * data.Intern[i].wieght_d);
                            internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.Intern[i].Prf_h[h] * data.Intern[i].wieght_h);
                            internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.TrainingPr[data.Intern[i].ProgramID].weight_p * data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d - 1]);

                            // consecutive 
                            if (dd > 0 && data.TrainingPr[data.Intern[i].ProgramID].cns_dD[dd - 1][d - 1] > 0)
                            {
                                internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.TrainingPr[data.Intern[i].ProgramID].cns_dD[dd - 1][d - 1] * data.Intern[i].wieght_cns);
                            }
                        }
                    }
                }
                MIPModel.AddEq(internDes, 0, "DesI_" + i);
            }

            // min desire
            for (int i = 0; i < Interns; i++)
            {
                for (int p = 0; p < TrainingPr; p++)
                {
                    if (data.Intern[i].ProgramID == p)
                    {
                        ILinearNumExpr dev = MIPModel.LinearNumExpr();

                        dev.AddTerm(des_i[i], -1);
                        dev.AddTerm(desMin_p[p], 1);
                        MIPModel.AddLe(dev, 0, "MinDesIP_" + i + "_" + p);
                    }
                }

            }
        }

        public void SetObjective()
        {
            // objective function
            ILinearNumExpr Obj = MIPModel.LinearNumExpr();
            for (int i = 0; i < Interns; i++)
            {
                Obj.AddTerm(des_i[i], data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi);
            }
            for (int p = 0; p < TrainingPr; p++)
            {
                Obj.AddTerm(desMin_p[p], data.TrainingPr[p].CoeffObj_MinDesi);
            }

            for (int t = 0; t < Timepriods; t++)
            {
                for (int w = 0; w < Wards; w++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        for (int p = 0; p < TrainingPr; p++)
                        {
                            Obj.AddTerm(Res_twh[t][w][h], -data.TrainingPr[p].CoeffObj_ResCap);
                            Obj.AddTerm(Emr_twh[t][w][h], -data.TrainingPr[p].CoeffObj_EmrCap);
                            Obj.AddTerm(sld_twh[t][w][h], -data.TrainingPr[p].CoeffObj_MINDem);
                        }

                    }
                }
            }



            for (int t = 0; t < Timepriods; t++)
            {
                for (int r = 0; r < Regions; r++)
                {
                    for (int p = 0; p < TrainingPr; p++)
                    {
                        Obj.AddTerm(AccSl_tr[t][r], -data.TrainingPr[p].CoeffObj_NotUsedAcc);
                    }
                }
            }

            //Obj.AddTerm(MaxND, 1);
            MIPModel.AddMaximize(Obj);
        }
        public void setDemandConstraint()
        {
            // minimum demand 
            for (int t = 0; t < Timepriods; t++)
            {
                for (int w = 0; w < Wards; w++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        ILinearNumExpr minDem = MIPModel.LinearNumExpr();

                        for (int i = 0; i < Interns; i++)
                        {
                            int trp = data.Intern[i].ProgramID;
                            for (int d = 1; d < Disciplins && data.Intern[i].isProspective; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d - 1][w])
                                {
                                    for (int tt = Math.Max(0, t - data.Discipline[d - 1].Duration_p[trp] + 1); tt <= t; tt++)
                                    {
                                        minDem.AddTerm(s_idth[i][d][tt][h], 1);
                                    }
                                }

                            }
                        }
                        minDem.AddTerm(sld_twh[t][w][h], 1);
                        MIPModel.AddGe(minDem, data.Hospital[h].HospitalMinDem_tw[t][w], "MinDemTWH_" + t + "_" + w + "_" + h);
                    }
                }
            }

            // max demand 
            for (int t = 0; t < Timepriods; t++)
            {
                for (int w = 0; w < Wards; w++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        ILinearNumExpr maxDem = MIPModel.LinearNumExpr();
                        maxDem.AddTerm(-1, Res_twh[t][w][h]);
                        maxDem.AddTerm(-1, Emr_twh[t][w][h]);
                        for (int i = 0; i < Interns; i++)
                        {
                            int trp = data.Intern[i].ProgramID;
                            for (int d = 1; d < Disciplins; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d - 1][w])
                                {
                                    for (int tt = Math.Max(0, t - data.Discipline[d - 1].Duration_p[trp] + 1); tt <= t; tt++)
                                    {
                                        maxDem.AddTerm(s_idth[i][d][tt][h], 1);
                                    }
                                }

                            }
                        }

                        MIPModel.AddLe(maxDem, data.Hospital[h].HospitalMaxDem_tw[t][w], "MaxDemTWH_" + t + "_" + w + "_" + h);
                    }
                }
            }

            // max Yearly demand 
            for (int h = 0; h < Hospitals - 1; h++)
            {
                for (int w = 0; w < Wards; w++)
                {
                    ILinearNumExpr maxDem = MIPModel.LinearNumExpr();

                    for (int t = 0; t < Timepriods; t++)
                    {
                        for (int i = 0; i < Interns; i++)
                        {
                            int trp = data.Intern[i].ProgramID;
                            for (int d = 1; d < Disciplins; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d - 1][w])
                                {
                                    maxDem.AddTerm(s_idth[i][d][t][h], 1);
                                }
                            }
                        }
                    }
                    MIPModel.AddLe(maxDem, data.Hospital[h].HospitalMaxYearly_w[w], "MaxYearlyDemand_" + w + "_" + h);

                }
            }

            // region demand
            for (int t = 0; t < Timepriods; t++)
            {
                for (int r = 0; r < Regions; r++)
                {
                    if (data.Region[r].AvaAcc_t[t] > 0)
                    {
                        ILinearNumExpr Accommodation = MIPModel.LinearNumExpr();
                        Accommodation.AddTerm(1, AccSl_tr[t][r]);
                        for (int h = 0; h < Hospitals - 1; h++)
                        {
                            for (int i = 0; i < Interns; i++)
                            {
                                if (data.Hospital[h].InToRegion_r[r] && data.Intern[i].TransferredTo_r[r])
                                {
                                    int trp = data.Intern[i].ProgramID;
                                    for (int d = 1; d < Disciplins; d++)
                                    {
                                        for (int tt = Math.Max(0, t - data.Discipline[d - 1].Duration_p[trp] + 1); tt <= t; tt++)
                                        {
                                            Accommodation.AddTerm(s_idth[i][d][tt][h], 1);
                                        }
                                    }
                                }
                            }
                        }

                        MIPModel.AddGe(Accommodation, data.Region[r].AvaAcc_t[t], "AccommodationTR_" + t + "_" + r);
                    }
                }
            }
        }

        public void solve_MIPmodel(string InsName, bool CPLEXFirstNode, int NewTimeLimit)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //*************set program
            MIPModel.ExportModel(data.allPath.OutPutGr + InsName + "LP.lp");
            MIPModel.SetParam(Cplex.DoubleParam.EpRHS, data.AlgSettings.RHSepsi);
            MIPModel.SetParam(Cplex.DoubleParam.EpOpt, data.AlgSettings.RCepsi);
            MIPModel.SetParam(Cplex.IntParam.Threads, 3);
            MIPModel.SetParam(Cplex.DoubleParam.TiLim, data.AlgSettings.MIPTime);
            MIPModel.SetParam(Cplex.BooleanParam.MemoryEmphasis, true);
            try
            {
                SolutionFound = true;
                mipOpt = new OptimalSolution(data);
                if (CPLEXFirstNode)
                {
                    MIPModel.SetParam(Cplex.LongParam.IntSolLim, 1);
                }

                if (MIPModel.Solve())
                {
                    stopwatch.Stop();
                    ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                    stopwatch.Start();
                    if (ElapsedTime < NewTimeLimit && CPLEXFirstNode)
                    {
                        MIPModel.SetParam(Cplex.LongParam.IntSolLim, 9223372036800000000);
                        MIPModel.SetParam(Cplex.DoubleParam.TiLim, NewTimeLimit - ElapsedTime);
                        MIPModel.SetParam(Cplex.Param.Advance, 1); // keep the search tree and resuem
                        MIPModel.Solve();
                    }
                    stopwatch.Stop();
                    ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                    for (int i = 0; i < Interns; i++)
                    {
                        for (int t = 0; t < Timepriods; t++)
                        {
                            for (int d = 1; d < Disciplins; d++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    if (MIPModel.GetValue(s_idth[i][d][t][h]) > 1 - 0.5)
                                    {
                                        mipOpt.Intern_itdh[i][t][d - 1][h] = true;
                                        if (h < Hospitals - 1)
                                        {
                                            Console.WriteLine("intern " + i + " started " + d + " at time " + t + " in hospital " + h + " and changed " + MIPModel.GetValue(ch_id[i][d]));
                                        }
                                        else
                                        {

                                            Console.WriteLine("intern " + i + " started " + d + " at time " + t + " in Oversea hospital " + h + " and changed " + MIPModel.GetValue(ch_id[i][d]));
                                        }

                                    }
                                }
                            }
                        }
                    }


                    // Mandatory discipline 
                    for (int i = 0; i < Interns; i++)
                    {
                        for (int d = 1; d < Disciplins; d++)
                        {
                            // check if the training program and involved mandatory discipline
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                if (data.Intern[i].ProgramID == p)
                                {
                                    for (int dd = 0; dd < Disciplins; dd++)
                                    {
                                        for (int h = 0; h < Hospitals; h++)
                                        {
                                            if (MIPModel.GetValue(y_idDh[i][dd][d][h]) > 0.5)
                                            {
                                                Console.WriteLine("intern " + i + ": " + d + " came after " + dd + " With " + MIPModel.GetValue(w_id[i][d]));
                                            }
                                        }
                                    }

                                }
                            }


                        }
                    }

                }
                for (int i = 0; i < Interns; i++)
                {
                    Console.WriteLine(MIPModel.GetValue(des_i[i]));
                }
                Console.WriteLine(MIPModel.ObjValue);
                mipOpt.WriteSolution(data.allPath.OutPutGr, "MIP" + InsName);


                MIPModel.End();
            }
            catch (ILOG.Concert.Exception e)
            {
                stopwatch.Stop();
                ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                SolutionFound = false;
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void SolveAUGMECON(string InsName, AUGMECONS augmentedaddon, bool ifRoot)
        {
            Initial();
            InitialMIPVar();
            CreateModel();
            setDemandConstraint();
            setEugmentedECons(augmentedaddon, ifRoot, InsName);
            
            
            //SetSol();
            solve_AUGMECON(InsName, augmentedaddon,ifRoot);
           
        }
        public void setEugmentedECons(AUGMECONS augmentedaddon, bool ifRoot, string InsName) 
        {
            setEugmentedSlack(augmentedaddon);
            if (ifRoot)
            {
                setLexicoGraohObjectiveAUGMECON(augmentedaddon);
            }
            else
            {
                setAugmentedObjectiveFunctionAUGMECON(augmentedaddon);
            }
            setMinDesireAUGMECON(augmentedaddon);
            setResDemandAUGMECON(augmentedaddon);
            setEmrDemandAUGMECON(augmentedaddon);
            setMinDemandAUGMECON(augmentedaddon);
            setUnAccDemandAUGMECON(augmentedaddon);
        }

        /// <summary>
        /// it sets the slack variables range and adds it to MIP model
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setEugmentedSlack(AUGMECONS augmentedaddon) 
        {
            s_obj = new INumVar[augmentedaddon.totalObjective];
            RBigM_obj = new INumVar[augmentedaddon.totalObjective];
            for (int o = 0; o < augmentedaddon.totalObjective; o++)
            {
                s_obj[o] = MIPModel.NumVar(augmentedaddon.lowerBound_o[o], augmentedaddon.upperBound_o[o], "slack_[" + o + "]");
                RBigM_obj[o] = MIPModel.NumVar(0, augmentedaddon.upperBound_o[o], "RBigM_[" + o + "]");
            }
        }

        /// <summary>
        /// sets the augmented objective function for MIP model
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setLexicoGraohObjectiveAUGMECON(AUGMECONS augmentedaddon) 
        {
            // objective function
            ILinearNumExpr[] lexExp = new ILinearNumExpr[augmentedaddon.totalObjective];
            IObjective[] lexObj = new IObjective[augmentedaddon.totalObjective];
            for (int o = 0; o < augmentedaddon.totalObjective; o++)
            {
                lexExp[o] = MIPModel.LinearNumExpr();
                string objName = "";
                if (augmentedaddon.activeObj_o[o])
                {
                    switch (o)
                    {
                        case 0:
                            objName = "SumDesire";
                            for (int i = 0; i < Interns; i++)
                            {
                                lexExp[o].AddTerm(des_i[i], 1);
                            }

                            break;
                        case 1:
                            objName = "minDesire";
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                lexExp[o].AddTerm(desMin_p[p], 1);
                            }
                            break;
                        case 2:
                            objName = "resDemand";
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals - 1; h++)
                                    {
                                        for (int p = 0; p < TrainingPr; p++)
                                        {
                                            lexExp[o].AddTerm(Res_twh[t][w][h], -1);
                                        }

                                    }
                                }
                            }
                            break;
                        case 3:
                            objName = "emrDemand";
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals - 1; h++)
                                    {
                                        for (int p = 0; p < TrainingPr; p++)
                                        {
                                            lexExp[o].AddTerm(Emr_twh[t][w][h], -1);
                                        }

                                    }
                                }
                            }
                            break;
                        case 4:
                            objName = "minDemand";
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals - 1; h++)
                                    {
                                        for (int p = 0; p < TrainingPr; p++)
                                        {
                                            lexExp[o].AddTerm(sld_twh[t][w][h], -1);
                                        }

                                    }
                                }
                            }
                            break;
                        case 5:
                            objName = "unUsedAcc";
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int r = 0; r < Regions; r++)
                                {
                                    for (int p = 0; p < TrainingPr; p++)
                                    {
                                        lexExp[o].AddTerm(AccSl_tr[t][r], -1);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    lexObj[o] = MIPModel.Maximize(lexExp[o],objName);

                }                
            }
            INumExpr[] objArray = new INumExpr[augmentedaddon.totalObjective];
            for (int o = 0; o < augmentedaddon.totalObjective; o++)
            {
                objArray[o] = lexObj[o].Expr;
            }

            MIPModel.Add(MIPModel.Maximize(MIPModel.StaticLex(objArray, augmentedaddon.weight_o, augmentedaddon.priority_o,
                augmentedaddon.absTols_o, augmentedaddon.relTols_o, ""), "LexicographObjective"));



        }

        /// <summary>
        /// adds the slack variables to the objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setAugmentedObjectiveFunctionAUGMECON(AUGMECONS augmentedaddon) 
        {
            ILinearNumExpr objectiveFunction = MIPModel.LinearNumExpr();
            for (int i = 0; i < Interns; i++)
            {
                objectiveFunction.AddTerm(des_i[i], 1);
            }
            for (int o = 0; o < augmentedaddon.totalObjective; o++)
            {
                
                double rhsSum = 0.003;
                double rhs_o = augmentedaddon.upperBound_o[o] - augmentedaddon.lowerBound_o[o];
                
                if (rhs_o == 0)
                {
                    objectiveFunction.AddTerm(0, s_obj[o]);
                }
                else
                {
                    objectiveFunction.AddTerm(rhsSum / rhs_o, s_obj[o]);
                    objectiveFunction.AddTerm(-data.AlgSettings.BigM, RBigM_obj[o]);
                }
                
            }

            MIPModel.AddMaximize(objectiveFunction, "AugmentedObjective");
        }

        /// <summary>
        /// adds the constraints related to sum desire objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setSumDesireAUGMECON(AUGMECONS augmentedaddon)
        {
            ILinearNumExpr sumdes = MIPModel.LinearNumExpr();

            for (int i = 0; i < Interns; i++)
            {
                sumdes.AddTerm(1, des_i[i]);
            }
            sumdes.AddTerm(-1, s_obj[0]);
            if (augmentedaddon.activeConst_o[0])
            {
                MIPModel.AddEq(sumdes, augmentedaddon.epsilonCon_o[0], "AugmentedConsSumDes_" + 0);
            }

        }

        /// <summary>
        /// adds the constraints related to min desire objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setMinDesireAUGMECON(AUGMECONS augmentedaddon) 
        {
            ILinearNumExpr mindes = MIPModel.LinearNumExpr();
            
            for (int i = 0; i < TrainingPr; i++)
            {
                mindes.AddTerm(1, desMin_p[i]);
            }
            mindes.AddTerm(-1, s_obj[1]);
            mindes.AddTerm(1, RBigM_obj[1]);
            if (augmentedaddon.activeConst_o[1])
            {
                MIPModel.AddEq(mindes, augmentedaddon.epsilonCon_o[1], "AugmentedConsMinDes_" + 1);
            }
            
        }

        /// <summary>
        /// adds the constraints related to reserved demand objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setResDemandAUGMECON(AUGMECONS augmentedaddon) 
        {
            ILinearNumExpr resdem = MIPModel.LinearNumExpr();
            for (int w = 0; w < Wards; w++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        resdem.AddTerm(1,Res_twh[t][w][h]);
                    }
                }
            }
            resdem.AddTerm(+1, s_obj[2]);
            resdem.AddTerm(-1, RBigM_obj[2]);
            if (augmentedaddon.activeConst_o[2])
            {
                MIPModel.AddEq(resdem, augmentedaddon.epsilonCon_o[2], "AugmentedConsResDem_" + 2);
            }
            
        }

        /// <summary>
        /// adds the constraints related to emergency demand objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setEmrDemandAUGMECON(AUGMECONS augmentedaddon)
        {
            ILinearNumExpr emrdem = MIPModel.LinearNumExpr();
            for (int w = 0; w < Wards; w++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        emrdem.AddTerm(1, Emr_twh[t][w][h]);
                    }
                }
            }
            emrdem.AddTerm(+1, s_obj[3]);
            emrdem.AddTerm(-1, RBigM_obj[3]);
            if (augmentedaddon.activeConst_o[3])
            {
                MIPModel.AddEq(emrdem, augmentedaddon.epsilonCon_o[3], "AugmentedConsEmrDem_" + 3);
            }
            
        }

        /// <summary>
        /// adds the constraints related to min demand objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setMinDemandAUGMECON(AUGMECONS augmentedaddon)
        {
            ILinearNumExpr mindem = MIPModel.LinearNumExpr();
            for (int w = 0; w < Wards; w++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        mindem.AddTerm(1, sld_twh[t][w][h]);
                    }
                }
            }
            mindem.AddTerm(+1, s_obj[4]);
            mindem.AddTerm(-1, RBigM_obj[4]);
            if (augmentedaddon.activeConst_o[4])
            {
                MIPModel.AddEq(mindem, augmentedaddon.epsilonCon_o[4], "AugmentedConsMinDem_" + 4);
            }
            
        }

        /// <summary>
        /// adds the constraints related to unused accomodation in different region objective function
        /// </summary>
        /// <param name="augmentedaddon"></param>
        public void setUnAccDemandAUGMECON(AUGMECONS augmentedaddon)
        {
            ILinearNumExpr mindem = MIPModel.LinearNumExpr();
            for (int r = 0; r < Regions; r++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    mindem.AddTerm(1, AccSl_tr[t][r]);
                }
            }
            mindem.AddTerm(+1, s_obj[5]);
            mindem.AddTerm(-1, RBigM_obj[5]);
            if (augmentedaddon.activeConst_o[5])
            {
                MIPModel.AddEq(mindem, augmentedaddon.epsilonCon_o[5], "AugmentedConsUnAcc_" + 5);
            }
            
        }

        public void setMultiObjValue(AUGMECONS augmentedaddon) 
        {
            multiObjectiveValue = new double[augmentedaddon.totalObjective];
            for (int oo = 0; oo < augmentedaddon.totalObjective; oo++)
            {
                multiObjectiveValue[oo] = 0;
            }
            for (int i = 0; i < Interns; i++)
            {
                multiObjectiveValue[0] += MIPModel.GetValue(des_i[i]);
            }
            for (int p = 0; p < TrainingPr; p++)
            {
                multiObjectiveValue[1] += MIPModel.GetValue(desMin_p[p]);
            }

            for (int w = 0; w < Wards; w++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int h = 0; h < Hospitals - 1; h++)
                    {
                        multiObjectiveValue[2] += MIPModel.GetValue( Res_twh[t][w][h]);
                        multiObjectiveValue[3] += MIPModel.GetValue(Emr_twh[t][w][h]);
                        multiObjectiveValue[4] += MIPModel.GetValue(sld_twh[t][w][h]);
                    }
                }
            }

            for (int r = 0; r < Regions; r++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    multiObjectiveValue[5] += MIPModel.GetValue(AccSl_tr[t][r]);
                }
            }
        }
        public void solve_AUGMECON(string InsName, AUGMECONS augmentedaddon, bool ifRoot)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (augmentedaddon.useWarmStart)
            {
                setWarmStartTime(augmentedaddon.warmStartTime_itdh);
            }
            //*************set program
            MIPModel.ExportModel(data.allPath.OutPutGr + InsName + ".lp");
            
            MIPModel.SetParam(Cplex.DoubleParam.EpRHS, data.AlgSettings.RHSepsi);
            MIPModel.SetParam(Cplex.DoubleParam.EpOpt, data.AlgSettings.RCepsi);
            MIPModel.SetParam(Cplex.IntParam.Threads, 3);
            MIPModel.SetParam(Cplex.DoubleParam.TiLim, data.AlgSettings.MIPTime);
            MIPModel.SetParam(Cplex.BooleanParam.MemoryEmphasis, true);
            if (ifRoot)
            {
                double sumPr = 0;
                for (int o = 0; o < augmentedaddon.totalObjective; o++)
                {
                    if (augmentedaddon.priority_o[o] != augmentedaddon.totalObjective)
                    {
                        sumPr += augmentedaddon.priority_o[o];
                    }
                    
                }
                MIPModel.SetParam(Cplex.Param.MultiObjective.Display, 2);

                // Purely for demonstrative purposes, set global and local limits
                // using parameter sets.

                // First, set the global deterministic time limit.
                MIPModel.SetParam(Cplex.Param.TimeLimit, 2*data.AlgSettings.MIPTime);

                // Second, create a parameter set for each priority.
                Cplex.ParameterSet[] paramsets = new Cplex.ParameterSet[augmentedaddon.totalObjective];
                for (int o = 0; o < augmentedaddon.totalObjective; o++)
                {
                    paramsets[o] = new Cplex.ParameterSet();

                    // Set the local deterministic time limits. Optimization will stop
                    // whenever either the global or local limit is exceeded.
                    if (augmentedaddon.priority_o[o] == augmentedaddon.totalObjective)
                    {
                        paramsets[o].SetParam(Cplex.Param.TimeLimit, data.AlgSettings.MIPTime);
                    }
                    else
                    {
                        paramsets[o].SetParam(Cplex.Param.TimeLimit, data.AlgSettings.MIPTime / sumPr * augmentedaddon.priority_o[o]);
                    }
                    
                    
                }
               
                try
                {
                    SolutionFound = true;
                    mipOpt = new OptimalSolution(data);

                    if (MIPModel.Solve(paramsets))
                    {
                        MIPModel.ExportModel(data.allPath.OutPutGr + InsName + ".lp");
                        notFeasible = false;
                        ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                        setMultiObjValue(augmentedaddon);
                        stopwatch.Stop();
                        ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                        for (int i = 0; i < Interns; i++)
                        {
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int d = 1; d < Disciplins; d++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        if (MIPModel.GetValue(s_idth[i][d][t][h]) > 1 - 0.5)
                                        {
                                            mipOpt.Intern_itdh[i][t][d - 1][h] = true;                           

                                        }
                                    }
                                }
                            }
                        }



                    }
                    else
                    {
                        notFeasible = true;
                    }

                    mipOpt.WriteSolution(data.allPath.OutPutGr, "MIP" + InsName );

                    setsolforNextStart();
                    MIPModel.End();
                }
                catch (ILOG.Concert.Exception e)
                {
                    stopwatch.Stop();
                    ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                    SolutionFound = false;
                    System.Console.WriteLine("Concert exception '" + e + "' caught");
                }
            }
            else
            {
                try
                {
                    SolutionFound = true;
                    mipOpt = new OptimalSolution(data);

                    if (MIPModel.Solve())
                    {
                        notFeasible = false;
                        ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                        setMultiObjValue(augmentedaddon);
                        stopwatch.Stop();
                        ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                        for (int i = 0; i < Interns; i++)
                        {
                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int d = 1; d < Disciplins; d++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        if (MIPModel.GetValue(s_idth[i][d][t][h]) > 1 - 0.5)
                                        {
                                            mipOpt.Intern_itdh[i][t][d - 1][h] = true;
                                            
                                        }
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        notFeasible = true;
                    }

                    mipOpt.WriteSolution(data.allPath.OutPutGr, "MIP" + InsName );

                    setsolforNextStart();
                    MIPModel.End();
                }
                catch (ILOG.Concert.Exception e)
                {
                    stopwatch.Stop();
                    ElapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                    SolutionFound = false;
                    System.Console.WriteLine("Concert exception '" + e + "' caught");
                }
            }

            
        }

        public void setsolforNextStart()
        {
            warmStartTime_itdh = new bool[Interns][][][];
            for (int i = 0; i < Interns; i++)
            {
                warmStartTime_itdh[i] = new bool[Timepriods][][];
                for (int t = 0; t < Timepriods; t++)
                {
                    warmStartTime_itdh[i][t] = new bool[Disciplins][];
                    for (int d = 0; d < Disciplins; d++)
                    {
                        warmStartTime_itdh[i][t][d] = new bool[Hospitals];
                        for (int h = 0; h < Hospitals; h++)
                        {
                            warmStartTime_itdh[i][t][d][h] = false;
                            if (MIPModel.IsPrimalFeasible() && MIPModel.GetValue(s_idth[i][d][t][h]) > 0.5)
                            {
                                warmStartTime_itdh[i][t][d][h] = true ;
                            }
                        }
                    }
                }
            }
        }

        public void setWarmStartTime(bool[][][][] warmStartTime_itdh)
        {
            IIntVar[] warmStart_idthVar = new IIntVar[Interns* Disciplins* Timepriods* Hospitals];
            double[] warmStart_idthVal = new double[Interns * Disciplins * Timepriods * Hospitals];
            int counter = 0;
            for (int i = 0; i < Interns; i++)
            {
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int d = 0; d < Disciplins; d++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            warmStart_idthVar[counter] = s_idth[i][d][t][h];

                            warmStart_idthVal[counter] = warmStartTime_itdh[i][t][d][h] ? 1 : 0;

                            counter++;
                        }
                    }
                }
            }

            MIPModel.AddMIPStart(warmStart_idthVar, warmStart_idthVal, "WarmStart");
            
        }

    }
}

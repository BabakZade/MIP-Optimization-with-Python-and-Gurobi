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
    public class MasterProblem
    {

        public Cplex RMP;
        public IObjective RMPObj;
        public IRange[] RMPRange;
        public double[] pi;       

        public ArrayList X_var;
        public ArrayList X_Data;
        public ArrayList DataColumn;
        public int all_const;
        public int ColumnID;
        public int preIterationColumnID;


        //output
        public double elapsed_time;
        public double Benefit;
        public string model_status;
        public bool is_mip;
        public bool column_is_mip;      
        public bool X_IsMIP;
        


        #region Output Performance
        public int TotalIntVar;
        public int TotalContinuesVar;
        public double AvariageValue;
        public double AverageReducedCost;
        public double AverageAddedReducedCost;
        #endregion


        public int Interns;
        public int Disciplins;
        public int Hospitals;
        public int Timepriods;
        public int TrainingPr;
        public int Wards;
        public int Regions;
        public int DisciplineGr;
        public AllData data;
        public MasterProblem(AllData InputData, ArrayList FathersColumn, ArrayList AllBranches, string InsName)
        {
            data = InputData;
            initial();
            initialCplexVar();
            createModelAndIdividualVar(InsName);
            addAllFatherColumn(FathersColumn, AllBranches);
            //setSol(37);
            solveRMP(InsName);
        }

        public void initial()
        {
            Interns = data.General.Interns;
            DataColumn = new ArrayList();
          
            Disciplins = data.General.Disciplines;
            Wards = data.General.HospitalWard;
            Regions = data.General.Region;
            DisciplineGr = data.General.DisciplineGr;

            Hospitals = data.General.Hospitals;
            Timepriods = data.General.TimePriods;
            TrainingPr = data.General.TrainingPr;
            preIterationColumnID = 0;
            ColumnID = 0;
        }

        public void initialCplexVar()
        {
            all_const = Interns + Timepriods * Regions + 2 * Timepriods * Wards * Hospitals + Interns;
            RMPRange = new IRange[all_const];

            X_var = new ArrayList();
        }

        public void createModelAndIdividualVar( string InsName)
        {
            CreateModel(InsName);
            addDummyColumn();
            addRegionSl();
            addMinDemSl();
            addRes();
            addEmr();
            addMinDesire();
        }


        public void save_dual()
        {
            pi = new double[all_const];
            try
            {
                RMP.GetDuals(RMPRange).CopyTo(pi, 0);
                for (int i = 0; i < pi.Length; i++)
                {
                    pi[i] = Math.Round(pi[i], 6);
                }
            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }

        }

        public void CreateModel(string InsName)
        {
            try
            {
                int Constraint_Counter = 0;
                string Constraint_Name = "";
                int constNumberInTXT = 2;
                RMP = new Cplex();

                // Empty Objective 
                RMPObj = RMP.AddMaximize();

                // Empty Constraint 

                // Constraint 2
                for (int i = 0; i < Interns; i++)
                {
                    Constraint_Name = constNumberInTXT + "_" + "InternAss_" + i;
                    RMPRange[Constraint_Counter] = RMP.AddRange(1, 1, Constraint_Name);
                    Constraint_Counter++;
                }
                constNumberInTXT++;
                // Constraint 3 
                for (int r = 0; r < Regions; r++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {
                        Constraint_Name = constNumberInTXT + "_" + "RegionAssTR_" + r + "_" + t;
                        RMPRange[Constraint_Counter] = RMP.AddRange(data.Region[r].AvaAcc_t[t], double.MaxValue, Constraint_Name);
                        Constraint_Counter++;
                    }
                }
                constNumberInTXT++;
                // Constraint 4 

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            Constraint_Name = constNumberInTXT + "_" + "MinDemtwh_" + t + "_" + w + "_" + h;
                            RMPRange[Constraint_Counter] = RMP.AddRange(data.Hospital[h].HospitalMinDem_tw[t][w], double.MaxValue, Constraint_Name);
                            Constraint_Counter++;
                        }
                    }
                }
                
                constNumberInTXT++;
                // Constraint 5 

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            Constraint_Name = constNumberInTXT + "_" + "MaxDemtwh_" + t + "_" + w + "_" + h;
                            RMPRange[Constraint_Counter] = RMP.AddRange(double.MinValue,data.Hospital[h].HospitalMaxDem_tw[t][w], Constraint_Name);
                            Constraint_Counter++;
                        }
                    }
                }

                constNumberInTXT++;
                // Constraint 6
               
               for (int i = 0; i < Interns; i++)
                    {

                        Constraint_Name = constNumberInTXT + "_" + "MinDesPI_" + data.Intern[i].ProgramID + "_" + i;
                        RMPRange[Constraint_Counter] = RMP.AddRange(double.MinValue, 0, Constraint_Name);
                        Constraint_Counter++;
                   }
                
                
               

                #region Setting
                RMP.ExportModel(data.allPath.OutPutGr + InsName + "RMP.lp");
                RMP.SetParam(Cplex.DoubleParam.EpRHS, 1e-9);
                RMP.SetParam(Cplex.DoubleParam.EpOpt, 1e-9);
                //RMP.SetParam(Cplex.IntParam.Threads, 3);
                RMP.SetParam(Cplex.DoubleParam.TiLim, data.AlgSettings.MasterTime);
                RMP.SetParam(Cplex.BooleanParam.MemoryEmphasis, true);
                //RMP.SetParam(Cplex.IntParam.Conflict.Display, 2);
                //RMP.SetParam(Cplex.IntParam.ConflictDisplay, 2);

                #endregion

            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void addDummyColumn()
        {
            for (int ii = 0; ii < Interns; ii++)
            {

                try
                {
                    //Objective 
                    Column new_col = RMP.Column(RMPObj, -data.AlgSettings.BigM);

                    int Constraint_Counter = 0;
                    // Constraint 2
                    for (int i = 0; i < Interns; i++)
                    {
                        if (i == ii)
                        {
                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                        }
                        Constraint_Counter++;
                    }

                    X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "InternDummyVar_" + ii));

                }
                catch (ILOG.Concert.Exception e)
                {

                    System.Console.WriteLine("Concert exception '" + e + "' caught");
                }

            }
        }

        public void addRegionSl()
        {
            for (int rr = 0; rr < Regions; rr++)
            {
                for (int tt = 0; tt < Timepriods; tt++)
                {
                    try
                    {
                        //Objective 
                        double objCoeff = 0;
                        for (int p = 0; p < TrainingPr; p++)
                        {
                            objCoeff += data.TrainingPr[p].CoeffObj_NotUsedAcc;
                        }
                        Column new_col = RMP.Column(RMPObj, -objCoeff);

                        int Constraint_Counter = Interns;
                        // Constraint 3
                        for (int r = 0; r < Regions; r++)
                        {
                            for (int t = 0; t < Timepriods; t++)
                            {
                                if (r == rr && t == tt)
                                {
                                    new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                                }
                                Constraint_Counter++;
                            }
                        }

                        X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "RegSl_rt_" + rr +"_"+ tt));

                    }
                    catch (ILOG.Concert.Exception e)
                    {

                        System.Console.WriteLine("Concert exception '" + e + "' caught");
                    }
                }
            }
           
        }

        public void addMinDemSl()
        {
            for (int tt = 0; tt < Timepriods; tt++)
            {
                for (int ww = 0; ww < Wards; ww++)
                {
                    for (int hh = 0; hh < Hospitals; hh++)
                    {
                        try
                        {
                            //Objective 
                            double objCoeff = 0;
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                objCoeff += data.TrainingPr[p].CoeffObj_MINDem;
                            }
                            Column new_col = RMP.Column(RMPObj, -objCoeff);

                            int Constraint_Counter = Interns;
                            // Constraint 3
                            for (int r = 0; r < Regions; r++)
                            {
                                for (int t = 0; t < Timepriods; t++)
                                {
                                    Constraint_Counter++;

                                }
                            }
                            // Constraint 4 

                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        if (t == tt && ww == w && h == hh)
                                        {
                                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                                        }
                                        Constraint_Counter++;
                                    }
                                }
                            }

                            X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "MinSl_twh" + tt + "_" + ww + "_" + hh));

                        }
                        catch (ILOG.Concert.Exception e)
                        {

                            System.Console.WriteLine("Concert exception '" + e + "' caught");
                        }
                    }
                }
            }
            
        }

        public void addRes()
        {
            for (int tt = 0; tt < Timepriods; tt++)
            {
                for (int ww = 0; ww < Wards; ww++)
                {
                    for (int hh = 0; hh < Hospitals; hh++)
                    {
                        try
                        {
                            //Objective 
                            double objCoeff = 0;
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                objCoeff += data.TrainingPr[p].CoeffObj_ResCap;
                            }
                            Column new_col = RMP.Column(RMPObj, -objCoeff);

                            int Constraint_Counter = Interns;
                            // Constraint 3
                            for (int r = 0; r < Regions; r++)
                            {
                                for (int t = 0; t < Timepriods; t++)
                                {
                                    Constraint_Counter++;

                                }
                            }

                            // Constraint 4 

                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        Constraint_Counter++;

                                    }
                                }
                            }

                            // Constraint 5

                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        if (t == tt && ww == w && h == hh)
                                        {
                                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], -1));
                                        }
                                        Constraint_Counter++;
                                    }
                                }
                            }

                            X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "Res_twh" + tt + "_" + ww + "_" + hh));

                        }
                        catch (ILOG.Concert.Exception e)
                        {

                            System.Console.WriteLine("Concert exception '" + e + "' caught");
                        }
                    }
                }
            }

        }

        public void addEmr()
        {
            for (int tt = 0; tt < Timepriods; tt++)
            {
                for (int ww = 0; ww < Wards; ww++)
                {
                    for (int hh = 0; hh < Hospitals; hh++)
                    {
                        try
                        {
                            //Objective 
                            double objCoeff = 0;
                            for (int p = 0; p < TrainingPr; p++)
                            {
                                objCoeff += data.TrainingPr[p].CoeffObj_EmrCap;
                            }
                            Column new_col = RMP.Column(RMPObj, -objCoeff);

                            int Constraint_Counter = Interns;
                            // Constraint 3
                            for (int r = 0; r < Regions; r++)
                            {
                                for (int t = 0; t < Timepriods; t++)
                                {
                                    Constraint_Counter++;

                                }
                            }

                            // Constraint 4 

                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        Constraint_Counter++;

                                    }
                                }
                            }

                            // Constraint 5

                            for (int t = 0; t < Timepriods; t++)
                            {
                                for (int w = 0; w < Wards; w++)
                                {
                                    for (int h = 0; h < Hospitals; h++)
                                    {
                                        if (t == tt && ww == w && h == hh)
                                        {
                                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], -1));
                                        }
                                        Constraint_Counter++;
                                    }
                                }
                            }

                            X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "Emr_twh" + tt + "_" + ww + "_" + hh));

                        }
                        catch (ILOG.Concert.Exception e)
                        {

                            System.Console.WriteLine("Concert exception '" + e + "' caught");
                        }
                    }
                }
            }

        }

        public void addMinDesire()
        {
            for (int pp = 0; pp < TrainingPr; pp++)
            {
                try
                {
                    //Objective 
                    double objCoeff = 0;

                    objCoeff = data.TrainingPr[pp].CoeffObj_MinDesi;
                    
                    Column new_col = RMP.Column(RMPObj, objCoeff);

                    int Constraint_Counter = Interns;
                    // Constraint 3
                    for (int r = 0; r < Regions; r++)
                    {
                        for (int t = 0; t < Timepriods; t++)
                        {
                            Constraint_Counter++;

                        }
                    }

                    // Constraint 4 

                    for (int t = 0; t < Timepriods; t++)
                    {
                        for (int w = 0; w < Wards; w++)
                        {
                            for (int h = 0; h < Hospitals; h++)
                            {
                                Constraint_Counter++;

                            }
                        }
                    }

                    // Constraint 5

                    for (int t = 0; t < Timepriods; t++)
                    {
                        for (int w = 0; w < Wards; w++)
                        {
                            for (int h = 0; h < Hospitals; h++)
                            {
                                Constraint_Counter++;
                            }
                        }
                    }

                    // Constraint 6
                   
                        for (int i = 0; i < Interns; i++)
                        {
                            if (data.Intern[i].ProgramID == pp)
                            {
                                new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                            }
                            
                            
                            Constraint_Counter++;
                        }
                    
                    X_var.Add(RMP.NumVar(new_col, double.MinValue, double.MaxValue, "MinDesire_" + pp ));

                }
                catch (ILOG.Concert.Exception e)
                {

                    System.Console.WriteLine("Concert exception '" + e + "' caught");
                }
            }


        }

        public void addColumn(ColumnInternBasedDecomposition theColumn)
        {

            try
            {
                ColumnID++;
                //Objective 
                double objCoeff = 0;
                int theP = data.Intern[theColumn.theIntern].ProgramID;
                objCoeff = data.TrainingPr[theP].CoeffObj_MinDesi;

                Column new_col = RMP.Column(RMPObj, objCoeff * theColumn.desire);

                int Constraint_Counter = 0;
                // Constraint 2
                for (int i = 0; i < Interns; i++)
                {
                    if (i == theColumn.theIntern)
                    {
                        new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                    }

                    Constraint_Counter++;
                }
                // Constraint 3
                for (int r = 0; r < Regions; r++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {
                        if (data.Intern[theColumn.theIntern].TransferredTo_r[r])
                        {
                            for (int d = 0; d < Disciplins; d++)
                            {
                                for (int h = 0; h < Hospitals; h++)
                                {
                                    if (data.Hospital[h].InToRegion_r[r])
                                    {
                                        for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                        {
                                            if (theColumn.S_tdh[tt][d][h])
                                            {
                                                new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                                            }

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
                        for (int h = 0; h < Hospitals; h++)
                        {
                            for (int d = 0; d < Disciplins; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[theColumn.theIntern].isProspective)
                                {
                                    for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                    {
                                        if (theColumn.S_tdh[tt][d][h])
                                        {
                                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                                        }

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
                        for (int h = 0; h < Hospitals; h++)
                        {

                            for (int d = 0; d < Disciplins; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                    {
                                        if (theColumn.S_tdh[tt][d][h])
                                        {
                                            new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], 1));
                                        }

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
                    if (data.Intern[i].ProgramID == theP && theColumn.theIntern == i)
                    {
                        new_col = new_col.And(RMP.Column(RMPRange[Constraint_Counter], -theColumn.desire));
                    }


                    Constraint_Counter++;
                }

                X_var.Add(RMP.NumVar(new_col, 0, double.MaxValue, "Xij_" + theColumn.theIntern + "_" + ColumnID));
                DataColumn.Add(theColumn);
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void solveRMP( string InsName)
        {
            try
            {
                RMP.ExportModel(data.allPath.OutPutGr + InsName + "RMP.lp");
                if (RMP.Solve())
                {
                    Benefit = RMP.ObjValue;
                    Console.WriteLine(RMP.GetStatus());
                    model_status = (RMP.GetStatus()).ToString();                   
                    display(InsName);
                    save_dual();
                   

                }
                model_status = (RMP.GetStatus()).ToString();
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void display( string InsName)
        {
            TotalIntVar = 0;
            TotalContinuesVar = 0;
            AvariageValue = 0;
            AverageReducedCost = 0;
            is_mip = true;


                StreamWriter tw = new StreamWriter(data.allPath.OutPutGr + InsName + "VarStatus.txt");
                X_IsMIP = true;
                Console.WriteLine("####################this is Master##################");
                Console.WriteLine();
                Console.WriteLine("************Cost: {0} , Status: {1}*********", RMP.ObjValue, RMP.GetStatus());
                Console.WriteLine();
                tw.WriteLine("Cost: {0} , Status: {1}", RMP.ObjValue, RMP.GetStatus());
                //Console.WriteLine(masterModel.IsMIP());

                int counter = 0;
                for (int i = 0; i < Interns; i++)
                {
                    tw.WriteLine("Dummy_[" + i + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                    counter++;
                }

                for (int r = 0; r < Regions; r++)
                {
                    for (int t = 0; t < Timepriods; t++)
                    {
                        tw.WriteLine("RegSlrt_[" + r +"]["+ t + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                        counter++;
                    }
                }

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            tw.WriteLine("MinSltwh_[" + t + "][" + w + "][" + h + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                            counter++;
                        }
                    }
                }
                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            tw.WriteLine("Restwh_[" + t + "][" + w + "][" + h + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                            counter++;
                        }
                    }
                }

                for (int t = 0; t < Timepriods; t++)
                {
                    for (int w = 0; w < Wards; w++)
                    {
                        for (int h = 0; h < Hospitals; h++)
                        {
                            tw.WriteLine("Emrtwh_[" + t + "][" + w + "][" + h + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                            counter++;
                        }
                    }
                }

                for (int p = 0; p < TrainingPr; p++)
                {
                    tw.WriteLine("MinDes_[" + p + "]= " + RMP.GetValue((INumVar)X_var[counter]) + " reduced Cost: " + RMP.GetReducedCost((INumVar)X_var[counter]));
                    counter++;
                }
                for (int i = counter; i < X_var.Count; i++)
                {
                    double xval = RMP.GetValue((INumVar)X_var[i]);
                    
                    double xRC = RMP.GetReducedCost((INumVar)X_var[i]);
                   
                    //Console.WriteLine("X_{0}= {1}",i+1,masterModel.GetValue((INumVar)pat_NumVar[i]));
                    tw.WriteLine(X_var[i].ToString() + " = {0} reduced Cost: {1} Intern: {2}", xval, xRC, ((ColumnInternBasedDecomposition)DataColumn[i - counter]).theIntern);
                    ((ColumnInternBasedDecomposition)DataColumn[i - counter]).xRC = xRC;
                    ((ColumnInternBasedDecomposition)DataColumn[i - counter]).xVal = xval;
                    if (i - counter >= preIterationColumnID)
                    {
                        //((ColumnInternBasedDecomposition)DataColumn[i - counter]).WriteXML(data.allPath.ColumnLoc + "X_j" + "[" + (i - counter) + "]");
                    }
                    AverageReducedCost += xRC;
                    if (RMP.GetValue((INumVar)X_var[i]) > 0 + data.AlgSettings.RHSepsi)
                    {
                        AvariageValue += xval;
                        TotalIntVar++;
                    }
                    if (RMP.GetValue((INumVar)X_var[i]) < 1 - data.AlgSettings.RHSepsi && RMP.GetValue((INumVar)X_var[i]) > 0 + data.AlgSettings.RHSepsi)
                    {
                        is_mip = false;
                        X_IsMIP = false;
                        TotalContinuesVar++;
                    }
                }
                TotalIntVar = TotalIntVar - TotalContinuesVar;
                preIterationColumnID = ColumnID;
                tw.Close();
            
        }

        public void addAllFatherColumn(ArrayList FatherColumn, ArrayList AllBranches)
        {
            bool[] columnStatus= new bool[FatherColumn.Count];
            int counter = -1;
            
            foreach (ColumnInternBasedDecomposition item in FatherColumn)
            {
                counter++;
                columnStatus[counter] = true;
                foreach (Branch tmpBR in AllBranches)
                {
                    // if it is right branch 
                    if (tmpBR.branch_status)
                    {
                        if (item.theIntern == tmpBR.BrIntern && !item.Y_dDh[tmpBR.BrPrDisc][tmpBR.BrDisc][tmpBR.BrHospital])
                        {
                            columnStatus[counter] = false;
                            break;
                        }
                    }
                    else
                    {
                        if (item.theIntern == tmpBR.BrIntern && item.Y_dDh[tmpBR.BrPrDisc][tmpBR.BrDisc][tmpBR.BrHospital])
                        {
                            columnStatus[counter] = false;
                            break;
                        }
                    }
                }
            }

            counter = -1;
            foreach (ColumnInternBasedDecomposition item in FatherColumn)
            {
                counter++;
                if (columnStatus[counter])
                {
                    addColumn(item);
                }
            }
        }

        public void setSol(int totalNumberOfColumn)
        {
            for (int i = 0; i < totalNumberOfColumn; i++)
            {
                if (File.Exists(data.allPath.ColumnLoc + "X_j[" + i.ToString() + "]Info.xml"))
                {
                    ColumnInternBasedDecomposition column = new ColumnInternBasedDecomposition().ReadXML(data.allPath.ColumnLoc + "X_j[" + i.ToString() + "]Info.xml");
                    addColumn(column);
                }                
            }
        }


    }
}

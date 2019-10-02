using System;
using System.Collections.Generic;
using System.Text;
using ILOG.Concert;
using ILOG.CPLEX;
using System.Collections;
using System.IO;
using System.Diagnostics;

namespace BranchAndPriceAlgorithm
{
    public class MasterProblem
    {

        public Cplex RMP;
        public IObjective RMPObj;
        public IRange[] RMPRange;
        public int ID;
        public double[] pi;       

        public ArrayList X_var;
        public ArrayList X_Data;
        public ArrayList DataColumn;
        public int all_const;


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


        public void save_dual()
        {
            pi = new double[all_const];
            try
            {
                RMP.GetDuals(RMPRange).CopyTo(pi, 0);
            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }

        }

        public void CreateModel()
        {
            try
            {
                int Constraint_Counter = 0;
                string Constraint_Name = "";
                int constNumberInTXT = 2;
                RMP = new Cplex();

                // Empty Objective 
                RMPObj = RMP.AddMinimize();

                // Empty Constraint 

                // Constraint 2
                for (int d = 0; d < Days; d++)
                {
                    for (int r = 0; r < Rooms; r++)
                    {
                        Constraint_Name = constNumberInTXT + "_" + "OneDayRoom_" + d + "_" + r;
                        RMPRange[Constraint_Counter] = RMP.AddRange(0, 0, Constraint_Name);
                        Constraint_Counter++;
                    }
                }
                constNumberInTXT++;
                // Constraint 3 
                for (int n = 0; n < Nurses; n++)
                {
                    for (int d = 0; d < Days; d++)
                    {
                        for (int t = 0; t < TimeSlot; t++)
                        {
                            Constraint_Name = constNumberInTXT + "_" + "AccupiedNurse_" + n + "_" + d + "_" + t;
                            RMPRange[Constraint_Counter] = RMP.AddRange(double.MinValue, 0, Constraint_Name);
                            Constraint_Counter++;
                        }
                    }
                }
                constNumberInTXT++;
                // constraint 4
                #region Setting
                //RMP.ExportModel("RMPModel.lp");
                RMP.SetParam(Cplex.DoubleParam.EpRHS, 1e-9);
                RMP.SetParam(Cplex.DoubleParam.EpOpt, 1e-9);
                //RMP.SetParam(Cplex.IntParam.Threads, 3);
                RMP.SetParam(Cplex.DoubleParam.TiLim, Program.g_d.MasterTime);
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

    }
}

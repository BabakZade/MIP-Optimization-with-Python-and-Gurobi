using System;
using System.Collections;
using System.Text;
using System.IO;
using DataLayer;
using System.Diagnostics;
using ColumnAndBranchInfo;
using SubProblemMIP;
using SubProblemCP;

namespace BranchAndPriceAlgorithm
{
    public class ColumnGenration
    {

        //output
        public int TotalIntVar;
        public int TotalContinuesVar;
        public double AvariageValue;
        public double AverageImprovement;
        public double AverageReducedCost;
        public double AverageAddedReducedCost;
        public int Iteration;
        public double BestSolution;
        public double elaps_time;
        public string model_status;
        public long master_Time;
        public long subMIP_time;
        public int count_MIP;
        public bool isMIP;
        public long writingX;
        public double[][][][] RCStart_itdh;
        public double[][][][] RCDual_itdh;
        public double[] RCPi2_i;
        public double[] RCDes_i;
        public double[] maxDesire_i;

        public MasterProblem RMP;
        
        public double[] dual;
        public bool isNodeFeasible;


        public int Interns;
        public int Disciplins;
        public int Hospitals;
        public int Timepriods;
        public int TrainingPr;
        public int Wards;
        public int Regions;
        public int DisciplineGr;
        public AllData data;
        public ArrayList allColumns;
        public ColumnGenration(DataLayer.AllData allData, ArrayList FathersColumn, ArrayList AllBranches, string insName, int procedureType)
        {
            data = allData;
            initial();
            SolveCG(AllBranches, FathersColumn ,insName, procedureType);
        }

        public void SolveCG(ArrayList AllBranches, ArrayList FathersColumn, string insName, int procedureType)
        {            
            double lagrangian_LB;
           
            subMIP_time = 0;
          

            RMP = new MasterProblem(data,maxDesire_i, FathersColumn, AllBranches, insName);

            BestSolution = RMP.RMP.ObjValue;


            //rmp solve himself in structor
            dual = RMP.pi;
            
            display_dual(dual, insName);
            Stopwatch cgTime = new Stopwatch();
            cgTime.Start();

            Console.WriteLine("**********this the cost: {0}*********", BestSolution);


            int counter = 0;
            while (findAndAddColumn(AllBranches, dual, insName, procedureType))
            {
                Iteration++;
                Stopwatch sw1 = new Stopwatch();
                sw1.Start();
                counter++;
                RMP.solveRMP(insName);
                sw1.Stop();
                master_Time += sw1.ElapsedMilliseconds/1000;
                sw1.Reset();
               
                dual = RMP.pi;
                display_dual(dual,insName);
                BestSolution = RMP.Benefit;
                Console.WriteLine("**********this the cost: {0}*********", RMP.Benefit);
                ////BandBMSS tmp1 = new BandBMSS(dual);
                Console.WriteLine(cgTime.ElapsedMilliseconds / 1000);
                if (cgTime.ElapsedMilliseconds / 1000 > data.AlgSettings.MasterTime)
                {
                    //break;
                }
            }
            AvariageValue = RMP.AvariageValue / (RMP.TotalContinuesVar + RMP.TotalIntVar);
            TotalContinuesVar = RMP.TotalContinuesVar;
            TotalIntVar = RMP.TotalIntVar;
            AverageImprovement /= Iteration;
            AverageReducedCost = RMP.AverageReducedCost / (RMP.DataColumn.Count - (RMP.TotalContinuesVar + RMP.TotalIntVar));
            AverageAddedReducedCost = RMP.AverageAddedReducedCost / RMP.DataColumn.Count;
            isMIP = RMP.is_mip;

            display_time(insName);
            RMP.RMP.End();
           
        }


        public void initial()
        {
            Interns = data.General.Interns;

            
            Disciplins = data.General.Disciplines;
            Wards = data.General.HospitalWard;
            Regions = data.General.Region;
            DisciplineGr = data.General.DisciplineGr;

            Hospitals = data.General.Hospitals;
            Timepriods = data.General.TimePriods;
            TrainingPr = data.General.TrainingPr;
            maxDesire_i = new double[Interns];
            for (int i = 0; i < Interns; i++)
            {
                maxDesire_i[i] = 0;
            }
            allColumns = new ArrayList();
        }
        public void display_dual(double[] dual, string insName)
        {
            setRCStart(dual);
            int tmp1, tmp2;
            int[] cons = {
                Interns , Timepriods * Regions , Timepriods * Wards * Hospitals,Timepriods * Wards * Hospitals,  Interns
            };

            tmp1 = 0;
            tmp2 = 0;

            StreamWriter tw = new StreamWriter(data.allPath.OutPutGr + insName + "Dual.txt");
                for (int i = 0; i < dual.Length; i++)
                {
                    if (tmp2 == i)
                    {
                        //Console.WriteLine("******{0}th Equation set********", tmp1 + 2);
                        tw.WriteLine("******{0}th Equation set********", tmp1 + 2);
                        tmp1++;

                        tmp2 += cons[tmp1 - 1];
                    }

                    //Console.WriteLine(dual[i]);
                    tw.WriteLine(dual[i]);

                }
                tw.Close();
            
        }

        public void display_time(string insName)
        {

            StreamWriter tw = new StreamWriter(data.allPath.OutPutGr + insName + "timeCG.txt", true);
                tw.WriteLine("{0} \t milisecond for master", master_Time);
                tw.WriteLine("{0} \t milisecond for sub exact", subMIP_time);
                tw.WriteLine("{0} \t number of column by sub exact", count_MIP);

                tw.WriteLine("**************************************************");
                tw.WriteLine("{0} \t benefit", RMP.Benefit);

                tw.Close();
            
        }

        public bool SolveSubProblemMIP(ArrayList AllBranches, double[] dual, string insName)
        {

            int totalColumn = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool firstCol = false;
            for (int i = 0; i < Interns; i++)
            {
                GC.Collect();
                SubProblem sp = new SubProblem(data, AllBranches, dual, i, insName);
                if (sp.KeepGoing(dual,insName))
                {
                    foreach (ColumnInternBasedDecomposition column in sp.theColumns)
                    {
                        totalColumn++;
                        RMP.addColumn(column);
                    }
                                     
                }
            }
            sw.Stop();
            count_MIP+= totalColumn;
            subMIP_time += sw.ElapsedMilliseconds / 1000;
            if (totalColumn>0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool SolveSubProblemCP(ArrayList AllBranches, double[] dual, string insName)
        {

            int totalColumn = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool firstCol = false;
            for (int i = 0; i < Interns; i++)
            {
                GC.Collect();
                preStopProcedure(dual, i);
                CPModelRosterGeneration sp = new CPModelRosterGeneration(dual, i, AllBranches, data);
                if (sp.KeepGoing(dual, insName))
                {
                    foreach (ColumnInternBasedDecomposition column in sp.theColumns)
                    {
                        totalColumn++;
                        if (maxDesire_i[i] < column.desire)
                        {
                            maxDesire_i[i] = column.desire;
                        }
                        RMP.addColumn(column);
                    }

                }
            }
            sw.Stop();
            count_MIP += totalColumn;
            subMIP_time += sw.ElapsedMilliseconds / 1000;
            if (totalColumn > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool SolveSubProblemDP(ArrayList AllBranches, double[] dual, string insName)
        {

            int totalColumn = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool firstCol = false;
            for (int i = 0; i < Interns; i++)
            {
                if (i == 10)
                {
                    Console.WriteLine();
                }
                GC.Collect();
                preStopProcedure(dual, i);
                SubProblemDP.DP sp = new SubProblemDP.DP(dual, AllBranches, data, i);
                if (sp.allColumns.Count >= 0)
                {
                    foreach (ColumnInternBasedDecomposition column in sp.allColumns)
                    {
                        totalColumn++;
                        if (maxDesire_i[i] < column.desire)
                        {
                            maxDesire_i[i] = column.desire;
                        }
                        addTheColumn(column);
                        RMP.addColumn(column);
                    }

                }
            }
            sw.Stop();
            count_MIP += totalColumn;
            subMIP_time += sw.ElapsedMilliseconds / 1000;
            if (totalColumn > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }

        public bool findAndAddColumn(ArrayList AllBranches, double[] dual, string insName, int procedureType) 
        {
            switch (procedureType)
            {
                case 0: // sub problem MIP 
                    return SolveSubProblemMIP(AllBranches, dual, insName);
                case 1: // sub problem CP
                    return SolveSubProblemCP(AllBranches, dual, insName);
                case 2: // sub problem DP
                    return SolveSubProblemDP(AllBranches, dual, insName);
                default:
                    return false;
            }
        }

        /// <summary>
        /// calculates the coefficient for each discipline and hospital if they start at the specefic time
        /// it also initialze the RCStartTime_tdh
        /// </summary>
        /// <param name="dual">the dual comming from Master problem</param>
        public void setRCStart(double[] dual)
        {
            new ArrayInitializer().CreateArray(ref RCDual_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref RCStart_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref RCPi2_i, data.General.Interns, 0);
            new ArrayInitializer().CreateArray(ref RCDes_i, data.General.Interns, 0);
            try
            {
                for (int i = 0; i < data.General.Interns; i++)
                {
                    RCDes_i[i] = data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
                }
                int Constraint_Counter = 0;

                // Constraint 2
                for (int i = 0; i < data.General.Interns; i++)
                {
                    RCPi2_i[i] -= dual[Constraint_Counter];
                    Constraint_Counter++;
                }

                // Constraint 3 
                for (int r = 0; r < data.General.Region; r++)
                {
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        for (int i = 0; i < data.General.Interns; i++)
                        {
                            if (data.Intern[i].TransferredTo_r[r])
                            {
                                for (int d = 0; d < data.General.Disciplines; d++)
                                {
                                    for (int h = 0; h < data.General.Hospitals; h++)
                                    {
                                        if (data.Hospital[h].InToRegion_r[r])
                                        {
                                            RCDual_itdh[i][t][d][h] -= dual[Constraint_Counter];
                                            
                                        }

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
                                for (int i = 0; i < data.General.Interns; i++)
                                {
                                    if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[i].isProspective)
                                    {
                                        RCDual_itdh[i][t][d][h] -= dual[Constraint_Counter];
                                    }
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
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    for (int i = 0; i < data.General.Interns; i++)
                                    {
                                        RCDual_itdh[i][t][d][h] -= dual[Constraint_Counter];
                                    }                                    
                                }
                            }

                            Constraint_Counter++;

                        }
                    }
                }

                // Constraint 6   
                for (int i = 0; i < data.General.Interns; i++)
                {
                    RCDes_i[i] += dual[Constraint_Counter];
                    
                    Constraint_Counter++;
                }


                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int d = 0; d < data.General.Disciplines; d++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            for (int i = 0; i < data.General.Interns; i++)
                            {
                                int theP = data.Intern[i].ProgramID;
                                for (int tt = t; tt < t + data.Discipline[d].Duration_p[theP] && tt < data.General.TimePriods; tt++)
                                {
                                    RCStart_itdh[i][t][d][h] += RCDual_itdh[i][tt][d][h];
                                }
                            }
                            
                        }
                    }
                }
               
            }
            catch (ILOG.Concert.Exception e)
            {

                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
        }

        public void preStopProcedure(double[] dual, int theIntern) 
        {
            
            double RCtmp = 0;
            RCtmp += RCPi2_i[theIntern];
            if (maxDesire_i[theIntern] == 0)
            {
                RCtmp += RCDes_i[theIntern] * data.Intern[theIntern].MaxPrf;
            }
            else
            {
                RCtmp += RCDes_i[theIntern] * maxDesire_i[theIntern];
            }
            
            bool[] disStatus = new bool[data.General.Disciplines];
            int[] ShouldattendInGr_g = new int[data.General.DisciplineGr];
            for (int g = 0; g < data.General.DisciplineGr; g++)
            {
                ShouldattendInGr_g[g] = data.Intern[theIntern].ShouldattendInGr_g[g];
            }
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                disStatus[d] = false;
                bool flag = false;
                for (int g = 0; g < data.General.DisciplineGr; g++)
                {
                    
                    if (data.Intern[theIntern].DisciplineList_dg[d][g] && data.Intern[theIntern].ShouldattendInGr_g[g] > 0)
                    {
                        flag = true;
                    }
                }
                if (!flag)
                {
                    disStatus[d] = true;
                }
            }
            for (int g = 0; g < data.General.DisciplineGr; g++)
            {
                double maxVal = 0;
                int tIndex = -1;
                int hIndex = -1;
                int dIndex = -1;
                for (int d = 0; d < data.General.Disciplines; d++)
                { 
                    if (ShouldattendInGr_g[g] < 1 || !data.Intern[theIntern].DisciplineList_dg[d][g] || disStatus[d])
                    {
                        continue;
                    }
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            bool flag = false;
                            for (int w = 0; w < Wards; w++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag && maxVal < RCStart_itdh[theIntern][t][d][h])
                            {
                               
                                maxVal = RCStart_itdh[theIntern][t][d][h];
                                tIndex = t;
                                hIndex = h;
                                dIndex = d;
                            }

                        }
                    }
                }

                if (maxVal > 0)
                {
                    RCtmp += maxVal;
                    disStatus[dIndex] = true;
                    ShouldattendInGr_g[g]--;
                }

            }

            Console.WriteLine(RCtmp);
            if (RCtmp <= 0)
            {
                Console.WriteLine();
            }

        }

        public void addTheColumn(ColumnAndBranchInfo.ColumnInternBasedDecomposition column)
        {
            foreach (ColumnAndBranchInfo.ColumnInternBasedDecomposition columnIntern in allColumns)
            {
                if (columnIntern.Compare(column, data))
                {
                    return;
                }
            }
            allColumns.Add(column);
        }
    }
}

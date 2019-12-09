using System;
using System.Collections;
using System.Text;
using DataLayer;


namespace SubProblemDP
{
    public struct BestPosition
    {
        public int discIndex;
        public int hospIndex;
        public int timeIndex;
        public double desire;
        
    }
    public class DP
    {
        public StateStage BestSol;
        public DPStage[] dPStages;        
        public bool[] activeDisc_d;
        public int K_totalDiscipline;
        public int[] discGrCounter_g;
        public int MaxProcessedNode;
        public int RealProcessedNode;
        int theIntern;
        AllData data;
        ArrayList allFinalStages;
        ArrayList bestPositions;
        public ArrayList allColumns;
        public double[][][] RCStart_tdh;
        public double[][][] RCDual_tdh;
        public double RCPi2;
        public double RCDes;

        public DP(double[] dual, ArrayList allBranches, AllData alldata, int theI)
        {
            Initial(dual, alldata, theI);
            procedure(dual,allBranches, true);
            setTheColumn(dual);
            if (allColumns.Count == 0)
            {
                Initial(dual, alldata, theI);
                procedure(dual, allBranches, false);
                setTheColumn(dual);
            }
        }
        public void Initial(double[] dual, AllData data, int theI)
        {
            this.data = data;
            theIntern = theI;
            MaxProcessedNode = 0;
            RealProcessedNode = 0;
            BestSol = new StateStage(data);
            dPStages = new DPStage[data.General.TimePriods];
            allFinalStages = new ArrayList();
            allColumns = new ArrayList();
            setRCStart(dual);
        }
        public void procedure(double[] dual, ArrayList allBranches, bool isHeuristic) 
        {
            bool rootIsSet = false;
            for (int t = 0; t < data.General.TimePriods; t++)
            {
                if (!rootIsSet && data.Intern[theIntern].Ave_t[t])
                {
                    dPStages[t] = new DPStage(ref allFinalStages, allBranches, bestPositions, RCStart_tdh, RCPi2, RCDes, data, new DPStage(), theIntern, t, true, isHeuristic);
                    rootIsSet = true;
                }
                else if (rootIsSet && dPStages[t - 1].FutureActiveState.Count == 0)
                {
                    break;
                }
                else if (rootIsSet)
                {
                    dPStages[t] = new DPStage(ref allFinalStages, allBranches, bestPositions, RCStart_tdh, RCPi2, RCDes, data, dPStages[t - 1], theIntern, t, false, isHeuristic);
                }
                MaxProcessedNode += dPStages[t].MaxProcessedNode;
                RealProcessedNode += dPStages[t].RealProcessedNode;
                if (allFinalStages.Count > data.AlgSettings.solutionPoolLimit)
                {
                    break;
                }
            }
        }

        public void setTheColumn(double[] dual) 
        {
            foreach (StateStage state in allFinalStages)
            {
                ColumnAndBranchInfo.ColumnInternBasedDecomposition column = new ColumnAndBranchInfo.ColumnInternBasedDecomposition(data,theIntern);
                int indexPrDis = -1;
                int indexDis = -1;
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    if (state.theSchedule_t[t].theDiscipline >= 0)
                    {
                        indexDis = state.theSchedule_t[t].theDiscipline;
                        if (indexDis != indexPrDis)
                        {
                            indexPrDis = indexDis;
                            column.S_tdh[t][indexDis][state.theSchedule_t[t].theHospital] = true;
                        }
                    }
                }
                column.RCInitial = state.Fx;
                column.setYdDFromStartTime(data);
                column.setRosterFromStartTime(data);
                double rc = column.setReducedCost(dual, data);
                column.RCCalculated = rc;
                
                if (!column.isColumnFeasible(data))
                {
                    Console.WriteLine();
                }
                if (rc > data.AlgSettings.RCepsi)
                {
                    addTheColumn(column);
                }

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

        /// <summary>
        /// calculates the coefficient for each discipline and hospital if they start at the specefic time
        /// it also initialze the RCStartTime_tdh
        /// </summary>
        /// <param name="dual">the dual comming from Master problem</param>
        public void setRCStart(double[] dual)
        {
            new ArrayInitializer().CreateArray(ref RCDual_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref RCStart_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            bestPositions = new ArrayList();
            RCPi2 = 0;
            RCDes = 0;

            RCDes = data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi;

            int Constraint_Counter = 0;

            // Constraint 2
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (i == theIntern)
                {
                    RCPi2 -= dual[Constraint_Counter];
                }

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
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            if (data.Hospital[h].Hospital_dw[d][w])
                            {
                                RCDual_tdh[t][d][h] -= dual[Constraint_Counter];

                            }
                        }

                        Constraint_Counter++;

                    }
                }
            }

            // Constraint 6   
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (theIntern == i)
                {
                    RCDes += dual[Constraint_Counter];
                }


                Constraint_Counter++;
            }


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

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        BestPosition best = new BestPosition();
                        best.hospIndex = h;
                        best.discIndex = d;
                        best.timeIndex = t;
                        best.desire = RCDes * (data.Intern[theIntern].wieght_d * data.Intern[theIntern].Prf_d[d]
                               + data.Intern[theIntern].wieght_h * data.Intern[theIntern].Prf_h[h]
                               + data.TrainingPr[data.Intern[theIntern].ProgramID].weight_p * data.TrainingPr[data.Intern[theIntern].ProgramID].Prf_d[d]);
                        best.desire += RCStart_tdh[t][d][h];
                        int pos = 0;
                        foreach (BestPosition item in bestPositions)
                        {
                            if (item.desire > best.desire)
                            {
                                pos++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        bestPositions.Insert(pos, best);
                    }
                }
            }

        }
    }
}

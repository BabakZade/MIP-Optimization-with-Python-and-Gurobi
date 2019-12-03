using System;
using System.Collections;
using System.Text;
using DataLayer;


namespace SubProblemDP
{
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
        public ArrayList allColumns;

        public DP(double[] dual, ArrayList allBranches, AllData alldata, int theI)
        {
            Initial(alldata, theI);
            procedure(dual,allBranches, true);
            setTheColumn(dual);
            if (allColumns.Count == 0)
            {
                Initial(alldata, theI);
                procedure(dual, allBranches, false);
                setTheColumn(dual);
            }
        }
        public void Initial(AllData data, int theI)
        {
            this.data = data;
            theIntern = theI;
            MaxProcessedNode = 0;
            RealProcessedNode = 0;
            BestSol = new StateStage(data);
            dPStages = new DPStage[data.General.TimePriods];
            allFinalStages = new ArrayList();
            allColumns = new ArrayList();

        }
        public void procedure(double[] dual, ArrayList allBranches, bool isHeuristic) 
        {
            bool rootIsSet = false;
            for (int t = 0; t < data.General.TimePriods; t++)
            {
                if (!rootIsSet && data.Intern[theIntern].Ave_t[t])
                {
                    dPStages[t] = new DPStage(ref allFinalStages, allBranches, dual, data, new DPStage(), theIntern, t, true, isHeuristic);
                    rootIsSet = true;
                }
                else if (rootIsSet && dPStages[t - 1].FutureActiveState.Count == 0)
                {
                    break;
                }
                else if (rootIsSet)
                {
                    dPStages[t] = new DPStage(ref allFinalStages, allBranches, dual, data, dPStages[t - 1], theIntern, t, false, isHeuristic);
                }
                MaxProcessedNode += dPStages[t].MaxProcessedNode;
                RealProcessedNode += dPStages[t].RealProcessedNode;
                
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

                column.setYdDFromStartTime(data);
                column.setRosterFromStartTime(data);
                double rc = column.setReducedCost(dual, data);
                if (!column.isColumnFeasible(data))
                {
                    Console.WriteLine();
                }
                if (rc > data.AlgSettings.RCepsi)
                {
                    allColumns.Add(column);
                }

            }
        }
    }
}

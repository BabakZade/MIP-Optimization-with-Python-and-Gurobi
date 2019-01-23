using System;
using System.Collections;
using System.Text;
using DataLayer;



namespace NestedDynamicProgrammingAlgorithm
{
	public class DP
	{
		public StateStage BestSol;
		public DPStage[] dPStages;
		public DP(AllData alldata, int theI, OptimalSolution incumbentSol)
		{
			Initial(alldata, incumbentSol);
			dPStages = new DPStage[alldata.General.TimePriods];

			bool rootIsSet = false;
			for (int t = 0; t < alldata.General.TimePriods; t++)
			{
				if (!rootIsSet && alldata.Intern[theI].Ave_t[t])
				{
					dPStages[t] = new DPStage(ref BestSol, alldata, new DPStage(), theI, t, true, incumbentSol);
					rootIsSet = true;
				}
				else if(rootIsSet && dPStages[t - 1].FutureActiveState.Count == 0)
				{
					break;
				}
				else if (rootIsSet)
				{
					dPStages[t] = new DPStage(ref BestSol, alldata, dPStages[t - 1], theI, t,false, incumbentSol);
				}
			}
			
		}
		public void Initial(AllData allData, OptimalSolution incumbentSol)
		{
			BestSol = new StateStage(allData);
		}
	}
}

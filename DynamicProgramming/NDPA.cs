using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace NestedDynamicProgrammingAlgorithm
{
	public class NDPA
	{
		public OptimalSolution optimalSolution;
		public NDPA(AllData allData)
		{
			optimalSolution = new OptimalSolution(allData);
			for (int i = 0; i < allData.General.Interns; i++)
			{
				DP tmp = new DP(allData, i);
				for (int d = 0; d < allData.General.Disciplines; d++)
				{
					for (int t = 0; t < allData.General.TimePriods; t++)
					{
						if (tmp.BestSol.theSchedule_t[t].theDiscipline == d)
						{
							optimalSolution.Intern_itdh[i][t][d][tmp.BestSol.theSchedule_t[t].theHospital] = true;
							break;
						}
					}
				}
				
			}
			optimalSolution.WriteSolution(allData.allPath.OutPutLocation, "NDPA");
		}
		
	}
}

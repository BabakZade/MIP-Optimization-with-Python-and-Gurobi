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
						if (((StateStage)tmp.Allsol[0]).theSchedule_t[t] == d)
						{
							optimalSolution.Intern_itdh[i][t][d][((StateStage)tmp.Allsol[0]).x_Hosp
] = true;
						}
					}
				}
				
			}
			optimalSolution.WriteSolution(allData.allPath.OutPutLocation, "NDPA");
		}
	}
}

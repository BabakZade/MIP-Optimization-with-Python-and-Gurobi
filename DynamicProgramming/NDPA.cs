using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace NestedDynamicProgrammingAlgorithm
{
	public class NDPA
	{
		public OptimalSolution optimalSolution;
		public double UpperBound;
		public NDPA(AllData allData)
		{
			optimalSolution = new OptimalSolution(allData);
			UpperBound = 0;
			for (int i = 0; i < allData.General.Interns; i++)
			{
				DP tmp = new DP(allData, i, new OptimalSolution(allData));
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
			for (int i = 0; i < allData.General.Interns; i++)
			{
				UpperBound += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * optimalSolution.Des_i[i];
			}
			for (int p = 0; p < allData.General.TrainingPr; p++)
			{
				UpperBound += allData.TrainingPr[p].CoeffObj_MinDesi * optimalSolution.MinDis[p];
			}
		}
		
	}
}

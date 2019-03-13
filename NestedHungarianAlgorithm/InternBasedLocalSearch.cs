using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using NestedDynamicProgrammingAlgorithm;

namespace NestedHungarianAlgorithm
{
	public class InternBasedLocalSearch
	{
		public AllData data;
		public int InternDesMin;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public OptimalSolution finalSol;
		public bool[] InternStatus;
		public InternBasedLocalSearch(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			data = allData;
			Initial(incumbentSol);
			reScheduleIntern(incumbentSol, data.AlgSettings.internBasedImpPercentage, Name);
		}
		public void reScheduleIntern(OptimalSolution incumbentSol, double ChangePercentage, string Name)
		{
			finalSol = new OptimalSolution(data);
			OptimalSolution solI = new OptimalSolution(data);
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			solI.copyRosters(incumbentSol.Intern_itdh);
			solI.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved" + Name);
			finalSol.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved" + Name);
			for (int i = 0; i < Interns * ChangePercentage; i++)
			{				
				int theI = findCandidateForDP(solI);
				if (theI < 0)
				{
					break;
				}
				DP neighbourhoodSol = new DP(data, theI, solI);
				solI.CleanInternRoster(theI);
				for (int t = 0; t < Timepriods; t++)
				{
					if (neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline < 0 || neighbourhoodSol.BestSol.theSchedule_t[t].theHospital < 0)
					{
						continue;
					}
					solI.Intern_itdh[theI][t][neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline][neighbourhoodSol.BestSol.theSchedule_t[t].theHospital] = true;
					t = t + data.Discipline[neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline].Duration_p[data.Intern[theI].ProgramID] - 1;
				}
				finalSol.copyRosters(solI.Intern_itdh);
				solI = new OptimalSolution(data);
				solI.copyRosters(finalSol.Intern_itdh);
				solI.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved_" + i + Name);
			}
			finalSol.copyRosters(solI.Intern_itdh);
			finalSol.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved" + Name);
		}
		public void Initial(OptimalSolution incumbentSol)
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			InternStatus = new bool[Interns];
			for (int i = 0; i < Interns; i++)
			{
				InternStatus[i] = false;
			}
		}

		public int findCandidateForDP(OptimalSolution incumbentSol)
		{
			int candidate = -1;
			double MinObj = data.AlgSettings.BigM;

			for (int i = 0; i < Interns; i++)
			{
				if (!InternStatus[i])
				{
					double tmpObj = incumbentSol.Des_i[i] * data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
					if (tmpObj < MinObj)
					{
						MinObj = tmpObj;
						candidate = i;
					}
				}
			}
			if (candidate >= 0)
			{
				InternStatus[candidate] = true;
			}
			return candidate;
		}
	}
}

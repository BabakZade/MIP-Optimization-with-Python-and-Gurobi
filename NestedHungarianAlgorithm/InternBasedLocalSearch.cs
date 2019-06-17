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
			
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			finalSol.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved" + Name);
			double MaxChange = Math.Ceiling(Interns * ChangePercentage);
			if (MaxChange == 0) // to check the infeasibility
			{
				MaxChange = 1;
			}
			for (int i = 0; i < MaxChange; i++)
			{
				OptimalSolution solI = new OptimalSolution(data);
				solI.copyRosters(finalSol.Intern_itdh);
				solI.WriteSolution(data.allPath.InsGroupLocation, "tmpImprovedSol" + Name);
				int theI = findCandidateForDP(ref i, solI);
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
				solI.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved_" + theI + "_" + Name);
				if (solI.Obj > finalSol.Obj || (!solI.infeasibleIntern_i[theI] && finalSol.infeasibleIntern_i[theI]))
				{
					finalSol = new OptimalSolution(data);
					finalSol.copyRosters(solI.Intern_itdh);
					finalSol.WriteSolution(data.allPath.InsGroupLocation, "InternBasedImproved" + Name);					
				}
			}
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

		public int findCandidateForDP(ref int Counter, OptimalSolution incumbentSol)
		{
			int candidate = -1;
			double MaxDif = 0;

			for (int i = 0; i < Interns; i++)
			{				
				if (!InternStatus[i])
				{
					if (incumbentSol.infeasibleIntern_i[i])
					{
						candidate = i;
						Counter--;
						break;
					}
					double tmpObj = data.Intern[i].MaxPrf - incumbentSol.Des_i[i];
					if (tmpObj > MaxDif)
					{
						MaxDif = tmpObj;
						candidate = i;
					}
				}
			}
			if (candidate >= 0)
			{
				InternStatus[candidate] = true;
				Console.WriteLine("Intern " + candidate + " MaxPrf: "+ data.Intern[candidate].MaxPrf + " Recieved Prf: " + incumbentSol.Des_i[candidate]);
			}
			return candidate;
		}
	}
}

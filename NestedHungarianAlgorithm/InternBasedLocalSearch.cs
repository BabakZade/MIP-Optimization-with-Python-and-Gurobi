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
		public int[] InternDesMin_p;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		OptimalSolution finalSol;
		public InternBasedLocalSearch(AllData allData ,OptimalSolution incumbentSol)
		{
			data = allData;
			Initial(incumbentSol);
			reScheduleIntern(incumbentSol);
		}
		public void reScheduleIntern(OptimalSolution incumbentSol)
		{
			finalSol = new OptimalSolution(data);
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			for (int p = 0; p < TrainingPr; p++)
			{
				DP neighbourhoodSol = new DP(data, InternDesMin_p[p], incumbentSol);
				finalSol.CleanInternRoster(InternDesMin_p[p]);				

				for (int t = 0; t < Timepriods; t++)
				{
					if (neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline < 0 || neighbourhoodSol.BestSol.theSchedule_t[t].theHospital < 0)
					{
						continue;
					}
					finalSol.Intern_itdh[InternDesMin_p[p]][t][neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline][neighbourhoodSol.BestSol.theSchedule_t[t].theHospital] = true;
					t = t + data.Discipline[neighbourhoodSol.BestSol.theSchedule_t[t].theDiscipline].Duration_p[data.Intern[InternDesMin_p[p]].ProgramID] - 1;
				}
			}
			finalSol.WriteSolution(data.allPath.OutPutLocation, "NHAIntern");
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
			
			InternDesMin_p = new int[TrainingPr];
			for (int p = 0; p < TrainingPr; p++)
			{
				InternDesMin_p[p] = -1;
			}
			for (int p = 0; p < TrainingPr; p++)
			{
				for (int i = 0; i < Interns; i++)
				{
					if (data.Intern[i].ProgramID == p && InternDesMin_p[p] < 0)
					{
						InternDesMin_p[p] = i;
					}
					if (data.Intern[i].ProgramID == p && InternDesMin_p[p] >= 0
						&& incumbentSol.Des_i[i] < incumbentSol.Des_i[InternDesMin_p[p]])
					{
						InternDesMin_p[p] = i;
					}
				}
			}
		}
	}
}

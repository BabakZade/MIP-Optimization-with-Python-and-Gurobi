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
		public int MaxProcessedNode;
		public int RealProcessedNode;
		public InternBasedLocalSearch(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			data = allData;
			MaxProcessedNode = 0;
		    RealProcessedNode = 0;
		    Initial(incumbentSol);
			reScheduleIntern(incumbentSol, data.AlgSettings.internBasedImpPercentage, Name);
		}
		public void reScheduleIntern(OptimalSolution incumbentSol, double ChangePercentage, string Name)
		{
			finalSol = new OptimalSolution(data);
			
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			finalSol.WriteSolution(data.allPath.OutPutGr, "InternBasedImproved" + Name);
			double MaxChange = Math.Ceiling(Interns * ChangePercentage);
			if (MaxChange == 0) // to check the infeasibility
			{
				MaxChange = 1;
			}
			int counter = 0;
			for (int i = 0; i < MaxChange; i++)
			{
				OptimalSolution solI = new OptimalSolution(data);
				solI.copyRosters(finalSol.Intern_itdh);
				solI.WriteSolution(data.allPath.OutPutGr, "tmpImprovedSol" + Name);
				int theI = findCandidateForDP(ref i, solI);
                if (theI < 0)
                {
                    return;
                }
                double OLD = solI.Des_i[theI];
				if (theI < 0)
				{
					break;
				}
				counter++;
				DP neighbourhoodSol = new DP(data, theI, solI);
				MaxProcessedNode += neighbourhoodSol.MaxProcessedNode;
				RealProcessedNode += neighbourhoodSol.RealProcessedNode;
                
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
				solI.WriteSolution(data.allPath.OutPutGr, "InternBasedImproved_" + theI + "_" + Name);
                Console.WriteLine(OLD + " ==> " +solI.Des_i[theI]);
				if (solI.Obj > finalSol.Obj || (!solI.infeasibleIntern_i[theI] && finalSol.infeasibleIntern_i[theI]))
				{
					finalSol = new OptimalSolution(data);
					finalSol.copyRosters(solI.Intern_itdh);
					finalSol.WriteSolution(data.allPath.OutPutGr, "InternBasedImproved" + Name);					
				}
			}
			if (counter > 0)
			{
				MaxProcessedNode /= counter;
				RealProcessedNode /= counter;
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
			double MaxDif = -1;
            bool thereIsInf = false ;
            for (int i = 0; i < Interns; i++)
            {
                if (!InternStatus[i] && incumbentSol.infeasibleIntern_i[i])
                {
                    thereIsInf = true;
                    double tmpObj = data.Intern[i].MaxPrf * data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_MinDesi;
                    if (tmpObj > MaxDif)
                    {
                        MaxDif = tmpObj;
                        candidate = i;
                    }
                }
            }
            if (thereIsInf)
            {
                Counter--;
            }

            for (int i = 0; i < Interns && !thereIsInf; i++)
			{				
				if (!InternStatus[i])
				{
                    if (data.Intern[i].MaxPrf < incumbentSol.Des_i[i])
                    {
                        data.Intern[i].MaxPrf = incumbentSol.Des_i[i];
                    }
					double tmpObj = (data.Intern[i].MaxPrf - incumbentSol.Des_i[i]) / data.Intern[i].MaxPrf ;
					if (tmpObj > MaxDif)
					{
						MaxDif = tmpObj;
						candidate = i;
                        if (incumbentSol.MinDis[data.Intern[i].ProgramID] == incumbentSol.Des_i[i])
                        {
                            //break;
                        }
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

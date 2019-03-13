using System;
using System.Collections;
using System.Text;
using DataLayer;
using NestedDynamicProgrammingAlgorithm;
using System.Diagnostics;

namespace NestedHungarianAlgorithm
{
	public class NHA
	{
		public ArrayList ActiveList;
		public AllData data;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public int TimeForNHA;
		public bool[][][][] motivationList_itdh;
		public bool[][] NotRequiredSkill_id;
		HungarianNode Root;
		public OptimalSolution nhaResult;
		public ImprovementStep improvementStep;
		public NHA(AllData data, string Name)
		{
			this.data = data;
			Initialization();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			TimelineBasedHungarianAlgorithm();
			stopwatch.Stop();
			TimeForNHA = (int)stopwatch.ElapsedMilliseconds / 1000;
			setSolution(Name);
			improvementStep = new ImprovementStep(data, nhaResult, ActiveList, Name);
		}
		public void Initialization()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			ActiveList = new ArrayList();
			new ArrayInitializer().CreateArray(ref motivationList_itdh, Interns, Timepriods, Disciplins, Hospitals, false);
			new ArrayInitializer().CreateArray(ref NotRequiredSkill_id, Interns, Disciplins, false);
		}

		public void TimelineBasedHungarianAlgorithm()
		{
			Root = new HungarianNode(0, data, new HungarianNode(),motivationList_itdh, NotRequiredSkill_id);
			ActiveList.Add(Root);
			for (int t = 1; t < Timepriods; t++)
			{
				HungarianNode nestedHungrian = new HungarianNode(t, data, (HungarianNode)ActiveList[t-1],motivationList_itdh,NotRequiredSkill_id);
				ActiveList.Add(nestedHungrian);
			}
		}
		
		public void setSolution(string Name)
		{
			nhaResult = new OptimalSolution(data);
			for (int i = 0; i < Interns; i++)
			{				
				for (int d = 0; d < Disciplins ; d++)
				{
					bool assignedDisc = false;
					for (int t = 0; t < Timepriods && !assignedDisc; t++)
					{
						for (int h = 0; h < Hospitals + 1 && !assignedDisc; h++)
						{
							if (((HungarianNode)ActiveList[ActiveList.Count - 1]).ResidentSchedule_it[i][t].dIndex == d
								&& ((HungarianNode)ActiveList[ActiveList.Count - 1]).ResidentSchedule_it[i][t].HIndex == h)
							{
								assignedDisc = true;
								nhaResult.Intern_itdh[i][t][d][h] = true;
								break;
							}
						}
					}
				}
			}
			nhaResult.WriteSolution(data.allPath.InsGroupLocation, "NHA"+ Name);
		}
	}
}

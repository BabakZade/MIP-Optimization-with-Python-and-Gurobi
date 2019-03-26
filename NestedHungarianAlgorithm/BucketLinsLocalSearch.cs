using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedHungarianAlgorithm
{
	public struct Bucket
	{
		int theHosp;
		int theDisc;
		int theIntern;
		int theTime;
		public Bucket(int theI, int theT, int theD, int theH)
		{
			theIntern = theI;
			theTime = theT;
			theDisc = theD;
			theHosp = theH;
		}
		public void copyBucket(Bucket copyable)
		{
			theIntern = copyable.theIntern;
			theDisc = copyable.theDisc;
			theHosp = copyable.theHosp;
			theTime = copyable.theTime;
		}
	}
	public class BucketLinsLocalSearch
	{
		

		ArrayList bucketlist;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public int[][][] MaxDem_twh;
		public int[][][] MinDem_twh;
		public int[][][] ResDem_twh;
		public int[][][] EmrDem_twh;
		public AllData data;
		public OptimalSolution improvedSolution;
		public bool[][][][] bucketArray_itdh;
		public int theTimeOfImprove;
		public bool[][] NotRequiredSkill_id;
		int BLcounter;
		int BLLimit;
		public BucketLinsLocalSearch(AllData alldata, OptimalSolution incumbentSol, ArrayList HungarianActiveList, string Name)
		{
			data = alldata;
			Initial(incumbentSol);
			InitialBucketChange(data.AlgSettings.bucketBasedImpPercentage, incumbentSol);
			InitialBucketSkill(incumbentSol);
			ImproveTheSchedule(HungarianActiveList);
			setSolution(HungarianActiveList, Name);
		}
		public void Initial(OptimalSolution incumbentSol)
		{
			BLLimit = 0;
			for (int i = 0; i < data.General.Interns; i++)
			{
				BLLimit += data.Intern[i].K_AllDiscipline;
			}
			BLLimit = (int)Math.Ceiling(BLLimit * data.AlgSettings.bucketBasedImpPercentage);
			BLcounter = 0;
			
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			theTimeOfImprove = Timepriods;
			new ArrayInitializer().CreateArray(ref bucketArray_itdh, Interns, Timepriods, Disciplins, Hospitals, false);
			new ArrayInitializer().CreateArray(ref NotRequiredSkill_id, Interns, Disciplins, false);
		}
		public void InitialBucketChange(double neighbourhoodSize, OptimalSolution incumbentSol)
		{
			bucketlist = new ArrayList();
			for (int i = 0; i < Interns; i++)
			{
				bool redundantCh = true;
				// if the intern must change the hospital it is not redundant otherwise we check the schedule 
				if (data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp > 1)
				{
					redundantCh = false;
				}
				if (BLcounter >= BLLimit)
				{
					break;
				}
				int thet1 = 0;
				for (int tt = thet1; tt < Timepriods; tt++)
				{					
					int theh2 = -1;
					int theh1 = -1;
					for (int t = tt; t < Timepriods && !redundantCh; t++)
					{
						for (int d = 0; d < Disciplins && !redundantCh; d++)
						{
							for (int h = 0; h < Hospitals && !redundantCh; h++)
							{
								if (incumbentSol.Intern_itdh[i][t][d][h])
								{
									if (theh1 < 0)
									{
										theh1 = h;

									}
									else if (theh2 < 0 && h != theh1)
									{
										theh2 = h;
										// the time that h1 is changed
										thet1 = t;
										
									}
									if (theh1 >= 0 && theh2 >= 0 && h == theh1)
									{
										if (theTimeOfImprove > thet1)
										{
											theTimeOfImprove = thet1;
										}
										redundantCh = true;
										// we only need the time it is changed to insert the discipline there
										bucketArray_itdh[i][thet1][d][h] = true;
										BLcounter++;
										bucketlist.Add(new Bucket(i, thet1, d, h) { });
									}
								}
							}
						}
					}
				}

			}

		}
		public void InitialBucketSkill(OptimalSolution incumbentSol)
		{
			for (int i = 0; i < Interns; i++)
			{				
				for (int t = 0; t < Timepriods; t++)
				{
					for (int d = 0; d < Disciplins ; d++)
					{
						for (int h = 0; h < Hospitals ; h++)
						{
							if (BLcounter >= BLLimit)
							{
								break;
							}
							if (incumbentSol.Intern_itdh[i][t][d][h] && data.Discipline[d].requiredLater_p[data.Intern[i].ProgramID])
							{
								bool redundantSkill = true;
								for (int tt = t; tt < Timepriods && redundantSkill ; tt++)
								{
									for (int dd = 0; dd < Disciplins && redundantSkill; dd++)
									{
										for (int hh = 0; hh < Hospitals && redundantSkill; hh++)
										{
											if (incumbentSol.Intern_itdh[i][tt][dd][hh] && data.Discipline[dd].Skill4D_dp[d][data.Intern[i].ProgramID])
											{
												redundantSkill = false;
											}
										}
									}
								}
								if (redundantSkill)
								{
									BLcounter++;
									NotRequiredSkill_id[i][d] = true;
									if (theTimeOfImprove > t)
									{
										theTimeOfImprove = t;
									}
								}
							}
						}
					}

				}

			}

		}
		public void InitialDemand(OptimalSolution incumbentSol)
		{
			new ArrayInitializer().CreateArray(ref MaxDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref MinDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref ResDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref EmrDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					for (int h = 0; h < data.General.Hospitals; h++)
					{
						MaxDem_twh[t][w][h] = data.Hospital[h].HospitalMaxDem_tw[t][w];
						MinDem_twh[t][w][h] = data.Hospital[h].HospitalMinDem_tw[t][w];
						ResDem_twh[t][w][h] = data.Hospital[h].ReservedCap_tw[t][w];
						EmrDem_twh[t][w][h] = data.Hospital[h].EmergencyCap_tw[t][w];
					}
				}
			}

			// use incumbent solution
			bool improved = false;
			for (int i = 0; i < data.General.Interns; i++)
			{
				if (i != 0)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							for (int t = 0; t < data.General.TimePriods; t++)
							{
								if (incumbentSol.Intern_itdh[i][t][d][h])
								{
									for (int w = 0; w < data.General.HospitalWard; w++)
									{
										if (data.Hospital[h].Hospital_dw[d][w])
										{
											if (MaxDem_twh[t][w][h] > 0)
											{
												MaxDem_twh[t][w][h]--;
												MinDem_twh[t][w][h]--;
												improved = true;
											}
											else if (ResDem_twh[t][w][h] > 0)
											{
												ResDem_twh[t][w][h]--;
											}
											else if (EmrDem_twh[t][w][h] > 0)
											{
												EmrDem_twh[t][w][h]--;
											}
										}
									}
								}
							}
						}
					}
				}
			}

		}

		public void ImproveTheSchedule(ArrayList HungarianActiveList)
		{

			HungarianActiveList.RemoveRange(theTimeOfImprove, HungarianActiveList.Count - theTimeOfImprove);
			if (theTimeOfImprove == 0)
			{
				theTimeOfImprove = 1;
				HungarianNode Root = new HungarianNode(0, data, new HungarianNode(), bucketArray_itdh, NotRequiredSkill_id);
				HungarianActiveList.Add(Root);
			}
			
			for (int t = theTimeOfImprove; t < Timepriods; t++)
			{
				HungarianNode nestedHungrian = new HungarianNode(t, data, (HungarianNode)HungarianActiveList[t - 1],bucketArray_itdh, NotRequiredSkill_id);
				HungarianActiveList.Add(nestedHungrian);
			}
		}

		public void setSolution(ArrayList HungarianActiveList, string Name)
		{
			improvedSolution = new OptimalSolution(data);
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					bool assignedDisc = false;
					for (int t = 0; t < Timepriods && !assignedDisc; t++)
					{
						for (int h = 0; h < Hospitals + 1 && !assignedDisc; h++)
						{
							if (((HungarianNode)HungarianActiveList[HungarianActiveList.Count - 1]).ResidentSchedule_it[i][t].dIndex == d
								&& ((HungarianNode)HungarianActiveList[HungarianActiveList.Count - 1]).ResidentSchedule_it[i][t].HIndex == h)
							{
								assignedDisc = true;
								improvedSolution.Intern_itdh[i][t][d][h] = true;
								break;
							}
						}
					}
				}
			}
			improvedSolution.WriteSolution(data.allPath.InsGroupLocation, "BucketListImproved" + Name);
		}
	}
}

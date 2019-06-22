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
		public int[][][] MaxDem_twh;
		public int[][][] MinDem_twh;
		public int[][][] ResDem_twh;
		public int[][][] EmrDem_twh;
		public int[][] AccDem_tr;
		public bool[][][] activatedDisc_tdh; // from sol
		public bool[] activeDisc_d;
		public bool incombentExist;
		public int K_totalDiscipline;
		public int[] discGrCounter_g;
		public DP(AllData alldata, int theI, OptimalSolution incumbentSol)
		{
			Initial(alldata, incumbentSol, theI);
			dPStages = new DPStage[alldata.General.TimePriods];

			bool rootIsSet = false;
			for (int t = 0; t < alldata.General.TimePriods; t++)
			{
				if (!rootIsSet && alldata.Intern[theI].Ave_t[t])
				{
					dPStages[t] = new DPStage(ref BestSol, alldata, new DPStage(), theI, t, true, MaxDem_twh, MinDem_twh, ResDem_twh, EmrDem_twh, AccDem_tr, incombentExist);
					rootIsSet = true;
				}
				else if(rootIsSet && dPStages[t - 1].FutureActiveState.Count == 0)
				{
					break;
				}
				else if (rootIsSet)
				{
					dPStages[t] = new DPStage(ref BestSol, alldata, dPStages[t - 1], theI, t,false, MaxDem_twh, MinDem_twh, ResDem_twh, EmrDem_twh, AccDem_tr, incombentExist);
				}
			}
			
		}
		public void Initial(AllData data, OptimalSolution incumbentSol, int theIntern)
		{
			BestSol = new StateStage(data);
			incombentExist = false;
			//
			new ArrayInitializer().CreateArray(ref MaxDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref MinDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref ResDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref EmrDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref AccDem_tr, data.General.TimePriods, data.General.Region, 0);
			new ArrayInitializer().CreateArray(ref activatedDisc_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, false);
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int r = 0; r < data.General.Region; r++)
				{
					AccDem_tr[t][r] = data.Region[r].AvaAcc_t[t];
				}
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
				if (i != theIntern)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							for (int t = 0; t < data.General.TimePriods; t++)
							{
								if (incumbentSol.Intern_itdh[i][t][d][h])
								{
									incombentExist = true;
									for (int r = 0; r < data.General.Region; r++)
									{
										if (data.Intern[i].TransferredTo_r[r] && data.Hospital[h].InToRegion_r[r])
										{
											if (AccDem_tr[t][r] > 0)
											{
												AccDem_tr[t][r]--;
											}
										}
									}
									for (int w = 0; w < data.General.HospitalWard; w++)
									{
										if (data.Hospital[h].Hospital_dw[d][w])
										{
											for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; tt++)
											{
												activatedDisc_tdh[t][d][h] = true;
												if (MaxDem_twh[tt][w][h] > 0)
												{
													MaxDem_twh[tt][w][h]--;
													MinDem_twh[tt][w][h]--;
													if (MinDem_twh[tt][w][h] < 0)
													{
														MinDem_twh[tt][w][h] = 0;
													}
													improved = true;
												}
												else if (ResDem_twh[tt][w][h] > 0)
												{
													ResDem_twh[tt][w][h]--;
												}
												else if (EmrDem_twh[tt][w][h] > 0)
												{
													EmrDem_twh[tt][w][h]--;
												}
											}
											
											break;
										}
									}
								}
							}
						}
					}
				}

			}

			for (int i = 0; i < data.General.Interns; i++)
			{
				if (i == theIntern)
				{
					for (int h = 0; h < data.General.Hospitals; h++)
					{
						for (int t = 0; t < data.General.TimePriods; t++)
						{
							for (int w = 0; w < data.General.HospitalWard; w++)
							{
								bool heGot = false;
								for (int d = 0; d < data.General.Disciplines; d++)
								{
									if (incumbentSol.Intern_itdh[i][t][d][h])
									{

										if (data.Hospital[h].Hospital_dw[d][w])
										{
											heGot = true;
										}
									}
								}
								if (heGot)
								{
									//nothing
								}
								else if(!incumbentSol.infeasibleIntern_i[i] )
								{
									ResDem_twh[t][w][h] = 0;
									EmrDem_twh[t][w][h] = 0;
								}
							}
						}
					}
				}

			}

			//for (int w = 0; w < data.General.HospitalWard; w++)
			//{
			//	Console.WriteLine("Resource TimeLine for ward " + w);
			//	for (int t = 0; t < data.General.TimePriods; t++)
			//	{
			//		int x = 0;
			//		for (int h = 0; h < data.General.Hospitals; h++)
			//		{
			//			x += MaxDem_twh[t][w][h];
			//			x += ResDem_twh[t][w][h];
			//			x += EmrDem_twh[t][w][h];
			//		}
			//		Console.Write(x.ToString("00") + " ");
			//	}
			//	Console.WriteLine();
			//}
		}

		public void FastScheduleEveryTimeChange(AllData data, OptimalSolution incumbentSol, int theIntern)
		{
			K_totalDiscipline = data.Intern[theIntern].K_AllDiscipline;
			discGrCounter_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				discGrCounter_g[g] = data.Intern[theIntern].ShouldattendInGr_g[g];				
			}
			
			
		}
	}
}

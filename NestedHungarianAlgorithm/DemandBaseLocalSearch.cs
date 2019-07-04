using System;
using System.Collections;
using System.Text;
using DataLayer;
using NestedDynamicProgrammingAlgorithm;
using System.Linq;

namespace NestedHungarianAlgorithm
{
	public struct PairWiseChange
	{
		int Disc1;
		int Disc2;
		int Intern;
		int TimeDisc1;
		int TimeDisc2;
	}
	public class DemandBaseLocalSearch
	{
		public AllData data;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public int[][][] MaxResEmrDemand_tdh;
		public int[][][] ResEmrDemand_tdh;
		public bool[] InternStatus;
		public bool[][] isONDisc_id;
		public int[][] Roster_it;
		public OptimalSolution Global;
		public OptimalSolution Local;
		int BLcounter;
		int BLLimit;
		public ArrayList HungarianActiveList;
		public DemandBaseLocalSearch(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			data = allData;
			HungarianActiveList = new ArrayList();
			Initial(incumbentSol, Name);

			AssignUnAssignedDiscipline(allData, Name);
			UpdateGlobal(Name);


			PairwiseChangeForward(allData, Name);
			UpdateGlobal(Name);

			PairwiseChangeBackward(allData, Name);
			UpdateGlobal(Name);

			LoopPairChanges(allData, Name);
		
			//UseHungarianAgain();
			//setSolution(Name);

		}

		public void Initial(OptimalSolution incumbentSol, string Name)
		{
			BLLimit = 0;
			for (int i = 0; i < data.General.Interns; i++)
			{
				BLLimit += data.Intern[i].K_AllDiscipline;
			}
			BLLimit = (int)Math.Ceiling(BLLimit * data.AlgSettings.bucketBasedImpPercentage);
			BLcounter = 0;

			Global = new OptimalSolution(data);
			Local = new OptimalSolution(data);
			Global.copyRosters(incumbentSol.Intern_itdh);
			Global.WriteSolution(data.allPath.InsGroupLocation, "ImprovedDemand" + Name);

			Local.copyRosters(incumbentSol.Intern_itdh);


			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;

			new ArrayInitializer().CreateArray(ref MaxResEmrDemand_tdh, Timepriods, Disciplins, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref ResEmrDemand_tdh, Timepriods, Disciplins, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref isONDisc_id, Interns, Disciplins, false);
			new ArrayInitializer().CreateArray(ref Roster_it, Interns, Timepriods, -1);

			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int h = 0; h < data.General.Hospitals; h++)
				{
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						for (int d = 0; d < data.General.Disciplines; d++)
						{
							if (data.Hospital[h].Hospital_dw[d][w])
							{
								for (int i = 0; i < data.General.Interns; i++)
								{
									if (incumbentSol.Intern_itdh[i][t][d][h])
									{
										for (int dd = 0; dd < Disciplins; dd++) // for the shared recouces 
										{
											if (data.Hospital[h].Hospital_dw[dd][w])
											{
												// only during the d1 we used the resources 
												for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID] && t < Timepriods; tt++)
												{
													MaxResEmrDemand_tdh[tt][dd][h]++;
													ResEmrDemand_tdh[tt][dd][h]++;
													isONDisc_id[i][d] = true;
													Roster_it[i][tt] = d;
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

			for (int t = 0; t < Timepriods; t++)
			{
				for (int h = 0; h < Hospitals; h++)
				{
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						for (int d = 0; d < Disciplins; d++)
						{
							if (data.Hospital[h].Hospital_dw[d][w])
							{
								MaxResEmrDemand_tdh[t][d][h] -= data.Hospital[h].HospitalMaxDem_tw[t][w];
								ResEmrDemand_tdh[t][d][h] -= (data.Hospital[h].HospitalMaxDem_tw[t][w] + data.Hospital[h].ReservedCap_tw[t][w] + data.Hospital[h].EmergencyCap_tw[t][w]);
							}
						}
					}
				}
			}
		}

		public void InitialSolUpdate(OptimalSolution incumbentSol) {

			Local = new OptimalSolution(data);
			Local.copyRosters(incumbentSol.Intern_itdh);
			new ArrayInitializer().CreateArray(ref MaxResEmrDemand_tdh, Timepriods, Disciplins, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref ResEmrDemand_tdh, Timepriods, Disciplins, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref isONDisc_id, Interns, Disciplins, false);
			new ArrayInitializer().CreateArray(ref Roster_it, Interns, Timepriods, -1);

			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int h = 0; h < data.General.Hospitals; h++)
				{
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						for (int d = 0; d < data.General.Disciplines; d++)
						{
							if (data.Hospital[h].Hospital_dw[d][w])
							{
								for (int i = 0; i < data.General.Interns; i++)
								{
									if (incumbentSol.Intern_itdh[i][t][d][h])
									{
										Roster_it[i][t] = d;
										for (int dd = 0; dd < Disciplins; dd++) // for the shared recouces 
										{
											if (data.Hospital[h].Hospital_dw[dd][w])
											{
												// only during the d1 we used the resources 
												for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID] && t < Timepriods; tt++)
												{
													MaxResEmrDemand_tdh[tt][dd][h]++;
													ResEmrDemand_tdh[tt][dd][h]++;
													isONDisc_id[i][d] = true;
													
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

			for (int t = 0; t < Timepriods; t++)
			{
				for (int h = 0; h < Hospitals; h++)
				{
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						for (int d = 0; d < Disciplins; d++)
						{
							if (data.Hospital[h].Hospital_dw[d][w])
							{
								MaxResEmrDemand_tdh[t][d][h] -= data.Hospital[h].HospitalMaxDem_tw[t][w];
								ResEmrDemand_tdh[t][d][h] -= (data.Hospital[h].HospitalMaxDem_tw[t][w] + data.Hospital[h].ReservedCap_tw[t][w] + data.Hospital[h].EmergencyCap_tw[t][w]);
							}
						}
					}
				}
			}
		}
		public void AssignUnAssignedDiscipline(AllData allData, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (MaxResEmrDemand_tdh[t][d][h] > 0)
						{
							for (int i = 0; i < Interns; i++)
							{
								if (Roster_it[i][t] == d)
								{
									bool isthereBetterChoice;
									double MincurrentObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[d]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[d])
													- allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
									for (int dd = 0; dd < Disciplins; dd++)
									{
										if (BLcounter >= BLLimit)
										{
											break;
										}
										int durDD = allData.Discipline[dd].Duration_p[allData.Intern[i].ProgramID];
										int durD = allData.Discipline[d].Duration_p[allData.Intern[i].ProgramID];
										if (!isONDisc_id[i][dd] && durDD <= durD
											&& allData.Discipline[d].requiredLater_p[allData.Intern[i].ProgramID]
											&& allData.Discipline[dd].requiresSkill_p[allData.Intern[i].ProgramID])
										{
											bool sameGroup = true;
											for (int g = 0; g < allData.General.DisciplineGr; g++)
											{
												if (data.Intern[i].DisciplineList_dg[d][g] != data.Intern[i].DisciplineList_dg[dd][g])
												{
													sameGroup = false;
												}
											}
											if (!sameGroup)
											{
												continue;
											}
											bool hospitalHas = false;
											for (int w = 0; w < Wards; w++)
											{
												if (allData.Hospital[h].Hospital_dw[d][w])
												{
													hospitalHas = true;
													break;
												}
											}
											double MinImpObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[dd]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[dd]);
											if (MaxResEmrDemand_tdh[t][dd][h] < 0 && hospitalHas && MincurrentObj <= MinImpObj)
											{
												Local.Intern_itdh[i][t][d][h] = false;
												Local.Intern_itdh[i][t][dd][h] = true;
												MaxResEmrDemand_tdh[t][dd][h]++;
												MaxResEmrDemand_tdh[t][d][h]--;
												BLcounter++;
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

		public void PairwiseChangeForward(AllData allData, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (MaxResEmrDemand_tdh[t][d][h] > 0)
						{
							for (int i = 0; i < Interns; i++)
							{
								if (BLcounter >= BLLimit)
								{
									break;
								}
								if (Local.Intern_itdh[i][t][d][h] && data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp > 1)
								{
									int dd = -1;
									int SecondHospital = -1;
									int secontTime = -1;
									// find the next discipline
									for (int tt = t; tt < Timepriods; tt++)
									{
										if (Roster_it[i][tt] >= 0 && Roster_it[i][tt] != d)
										{
											dd = Roster_it[i][tt];
											secontTime = tt;
											for (int hh = 0; hh < Hospitals; hh++)
											{
												if (Local.Intern_itdh[i][tt][dd][hh])
												{
													SecondHospital = hh;

													break;
												}
											}
											break;
										}
									}

									if (dd >= 0)
									{
										//check skill
										bool goOn = true;
										if (allData.Discipline[dd].requiresSkill_p[allData.Intern[i].ProgramID] && allData.Discipline[dd].Skill4D_dp[d][allData.Intern[i].ProgramID])
										{
											goOn = false;
										}

										if (goOn)
										{
											double MinImpObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[dd]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[dd]);


											double MincurrentObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[d]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[d]);


											if (h == SecondHospital
												&& ResEmrDemand_tdh[t][dd][h] < 0
												&& ResEmrDemand_tdh[secontTime][d][h] < 0
												)
											{
												if (MaxResEmrDemand_tdh[t][dd][h] > 0)
												{
													MinImpObj -= TrainingPr * allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												}
												// if change happens we recieve it
												// it should not be counted in min obj
												//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
												//{
												//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												//}
												if (MaxResEmrDemand_tdh[secontTime][d][SecondHospital] > 0)
												{
													MincurrentObj -= TrainingPr * allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												}
												//if (MaxResEmrDemand_tdh[t][d][h] > 0)
												//{
												//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												//}
												if (MincurrentObj <= MinImpObj)
												{
													ChangePair(i, t, d, h, h, secontTime, dd, SecondHospital, h);

												}
											}
											else if (h != SecondHospital)
											{
												// if first hospital has the second discipline
												if (ResEmrDemand_tdh[t][dd][h] < 0
												&& ResEmrDemand_tdh[secontTime][d][h] < 0)
												{
													MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[h]);
													MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[h]);
													if (MaxResEmrDemand_tdh[t][dd][h] > 0)
													{
														MinImpObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
													//{
													//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}
													if (MaxResEmrDemand_tdh[secontTime][d][h] > 0)
													{
														MincurrentObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[t][d][h] > 0)
													//{
													//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}

													if (MincurrentObj <= MinImpObj)
													{
														ChangePair(i, t, d, h, h, secontTime, dd, SecondHospital, h);

													}
												}
												else if (ResEmrDemand_tdh[secontTime][d][SecondHospital] < 0
												&& ResEmrDemand_tdh[t][dd][SecondHospital] < 0)
												{
													// 
													MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[SecondHospital]);
													MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[SecondHospital]);
													if (MaxResEmrDemand_tdh[t][dd][SecondHospital] > 0)
													{
														MinImpObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
													//{
													//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}
													if (MaxResEmrDemand_tdh[secontTime][d][SecondHospital] > 0)
													{
														MincurrentObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[t][d][h] > 0)
													//{
													//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}

													if (MincurrentObj <= MinImpObj)
													{
														ChangePair(i, t, d, h, SecondHospital, secontTime, dd, SecondHospital, SecondHospital);

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
			}
		}

		public void PairwiseChangeBackward(AllData allData, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (MaxResEmrDemand_tdh[t][d][h] > 0)
						{
							for (int i = 0; i < Interns; i++)
							{
								if (BLcounter >= BLLimit)
								{
									continue;
								}
								if (Local.Intern_itdh[i][t][d][h] && data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp > 1)
								{
									int dd = -1;
									int SecondHospital = -1;
									int secontTime = -1;
									// find the Pr discipline
									for (int tt = t; tt < 0; tt--)
									{
										if (Roster_it[i][tt] >= 0 && Roster_it[i][tt] != d)
										{
											dd = Roster_it[i][tt];
											secontTime = tt;
											for (int hh = 0; hh < Hospitals; hh++)
											{
												if (Local.Intern_itdh[i][tt][dd][hh])
												{
													SecondHospital = hh;

													break;
												}
											}
											break;
										}
									}

									if (dd >= 0)
									{
										//check skill
										bool goOn = true;
										if (allData.Discipline[dd].requiresSkill_p[allData.Intern[i].ProgramID] && allData.Discipline[dd].Skill4D_dp[d][allData.Intern[i].ProgramID])
										{
											goOn = false;
										}

										if (goOn)
										{
											double MinImpObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[dd]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[dd]);


											double MincurrentObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[d]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[d]);


											if (h == SecondHospital
												&& ResEmrDemand_tdh[t][dd][h] < 0
												&& ResEmrDemand_tdh[secontTime][d][h] < 0
												)
											{
												if (MaxResEmrDemand_tdh[t][dd][h] > 0)
												{
													MinImpObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												}
												// if change happens we recieve it
												// it should not be counted in min obj
												//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
												//{
												//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												//}
												if (MaxResEmrDemand_tdh[secontTime][d][SecondHospital] > 0)
												{
													MincurrentObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												}
												//if (MaxResEmrDemand_tdh[t][d][h] > 0)
												//{
												//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
												//}
												if (MincurrentObj <= MinImpObj)
												{
													ChangePair(i, t, d, h, h, secontTime, dd, SecondHospital, h);

												}
											}
											else if (h != SecondHospital)
											{
												// if first hospital has the second discipline
												if (ResEmrDemand_tdh[t][dd][h] < 0
												&& ResEmrDemand_tdh[secontTime][d][h] < 0)
												{
													MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[h]);
													MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[h]);
													if (MaxResEmrDemand_tdh[t][dd][h] > 0)
													{
														MinImpObj -= TrainingPr * allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
													//{
													//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}
													if (MaxResEmrDemand_tdh[secontTime][d][h] > 0)
													{
														MincurrentObj -= TrainingPr * allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[t][d][h] > 0)
													//{
													//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}

													if (MincurrentObj <= MinImpObj)
													{
														ChangePair(i, t, d, h, h, secontTime, dd, SecondHospital, h);

													}
												}
												else if (ResEmrDemand_tdh[secontTime][d][SecondHospital] < 0
												&& ResEmrDemand_tdh[t][dd][t] < 0)
												{
													// 
													MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[SecondHospital]);
													MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_h * allData.Intern[i].Prf_h[SecondHospital]);
													if (MaxResEmrDemand_tdh[t][dd][SecondHospital] > 0)
													{
														MinImpObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[secontTime][dd][SecondHospital] > 0)
													//{
													//	MinImpObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}
													if (MaxResEmrDemand_tdh[secontTime][d][SecondHospital] > 0)
													{
														MincurrentObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													}
													//if (MaxResEmrDemand_tdh[t][d][h] > 0)
													//{
													//	MincurrentObj += allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
													//}

													if (MincurrentObj <= MinImpObj)
													{
														ChangePair(i, t, d, h, SecondHospital, secontTime, dd, SecondHospital, SecondHospital);

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
			}
		}

		public void UpdateGlobal(string Name)
		{
			Local.WriteSolution(data.allPath.InsGroupLocation, "ImprovedLocal");
			if (Local.Obj >= Global.Obj)
			{
				Global = new OptimalSolution(data);
				Global.copyRosters(Local.Intern_itdh);
				Global.WriteSolution(data.allPath.InsGroupLocation, "ImprovedDemand" + Name);
			}
			InitialSolUpdate(Global);
		}

		/// <summary>
		/// d1h1t1 d2h2t2 => d1HCh1t1 d2HCh2t2
		/// d1 and d2 equal length 
		/// </summary>
		/// <param name="theI"></param>
		/// <param name="t1"></param>
		/// <param name="d1"></param>
		/// <param name="h1"></param>
		/// <param name="changeH1"></param>
		/// <param name="t2"></param>
		/// <param name="d2"></param>
		/// <param name="h2"></param>
		/// <param name="changeH2"></param>
		public void ChangePair(int theI, int t1, int d1, int h1, int changeH1, int t2, int d2, int h2, int changeH2)
		{
			if (data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID] != data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID])
			{
				return;
			}
			
			// check timeline 
			bool timelineCap = true;
			if (t1 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID] > Timepriods
				|| t2 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID] > Timepriods)
			{
				return; 
			}

			// check from t1 till t1 + du_2, hospital h1 has capcity for h1change
			for (int t = t1	; t < t1 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID] && timelineCap; t++)
			{
				if (ResEmrDemand_tdh[t][d2][changeH1] >= 0)
				{
					timelineCap = false;
				}
			}

			// chech from t2 till t2 + du_1, hospital h2 has capcity for d1
			for (int t = t2; t < t2 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID] && timelineCap; t++)
			{
				if (ResEmrDemand_tdh[t][d1][changeH2] >= 0)
				{
					timelineCap = false;
				}
			}

			if (!timelineCap)
			{
				return;
			}

			BLcounter++;



			Local.Intern_itdh[theI][t1][d1][h1] = false;
			Local.Intern_itdh[theI][t2][d2][h2] = false;
			Local.Intern_itdh[theI][t1][d2][changeH2] = true;
			Local.Intern_itdh[theI][t2][d1][changeH1] = true;

			// lets update when d1 goes to changeH1


			for (int w = 0; w < Wards; w++)
			{
				if (data.Hospital[h1].Hospital_dw[d1][w]) // lets chech d1
				{
					for (int dd = 0; dd < Disciplins; dd++) // shared resource
					{
						for (int t = t1; t < t1 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID]; t++)
						{
							if (data.Hospital[h1].Hospital_dw[dd][w])
							{
								MaxResEmrDemand_tdh[t][dd][h1]--;
								ResEmrDemand_tdh[t][dd][h1]--;
							}
						}
					}
				}
			}
			for (int w = 0; w < Wards; w++)
			{
				if (data.Hospital[changeH1].Hospital_dw[d1][w]) // lets chech d1
				{
					for (int dd = 0; dd < Disciplins; dd++) // shared resource
					{
						for (int t = t2; t < t2 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID]; t++)
						{

							if (data.Hospital[h1].Hospital_dw[dd][w])
							{
								MaxResEmrDemand_tdh[t][dd][changeH1]++;
								ResEmrDemand_tdh[t][dd][changeH1]++;
							}

						}
					}
				}
			}


			// lets update when d2 goes to changeH2

			for (int w = 0; w < Wards; w++)
			{
				if (data.Hospital[h2].Hospital_dw[d2][w]) // lets chech d1
				{
					for (int dd = 0; dd < Disciplins; dd++) // shared resource
					{
						for (int t = t2; t < t2 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID]; t++)
						{
							if (data.Hospital[h2].Hospital_dw[dd][w])
							{
								MaxResEmrDemand_tdh[t][dd][h2]--;
								ResEmrDemand_tdh[t][dd][h2]--;
							}

						}
					}

				}
			}


			for (int w = 0; w < Wards; w++)
			{
				if (data.Hospital[changeH2].Hospital_dw[d2][w]) // lets chech d1
				{
					for (int dd = 0; dd < Disciplins; dd++) // shared resource
					{
						for (int t = t1; t < t1 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID]; t++)
						{
							if (data.Hospital[changeH2].Hospital_dw[dd][w])
							{
								MaxResEmrDemand_tdh[t][d2][changeH2]++;
								ResEmrDemand_tdh[t][d2][changeH2]++;
							}

						}
					}
				}
			}
			

			Roster_it[theI][t1] = d2;
			Roster_it[theI][t2] = d1;
			
		}


		/// <summary>
		/// d1h1 d2h2 => d2h1 d1h2
		/// d1 and d2 can have different duration 
		/// </summary>
		/// <param name="theI"></param>
		/// <param name="t1"></param>
		/// <param name="d1"></param>
		/// <param name="h1"></param>
		/// <param name="t2"></param>
		/// <param name="d2"></param>
		/// <param name="h2"></param>
		public void ChangePairWardPairHospital(int theI, int t1, int d1, int h1, int t2, int d2, int h2)
		{

			// check timeline 
			bool timelineCap = true;
			if (t1 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID] > Timepriods
				|| t2 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID] > Timepriods)
			{
				return;
			}

			// chech from t1 till t1 + du_2, hospital h1 has capcity for d2
			for (int t = t1; t < t1 + data.Discipline[d2].Duration_p[data.Intern[theI].ProgramID] && timelineCap; t++)
			{
				if (ResEmrDemand_tdh[t][d2][h1] >= 0)
				{
					timelineCap = false;
				}
			}

			// chech from t2 till t2 + du_1, hospital h1 has capcity for d2
			for (int t = t2; t < t2 + data.Discipline[d1].Duration_p[data.Intern[theI].ProgramID] && timelineCap; t++)
			{
				if (ResEmrDemand_tdh[t][d2][h1] >= 0)
				{
					timelineCap = false;
				}
			}

			if (!timelineCap)
			{
				return;
			}

			BLcounter++;



			//Local.Intern_itdh[theI][t1][d1][h1] = false;
			//Local.Intern_itdh[theI][t2][d2][h2] = false;
			//Local.Intern_itdh[theI][t1][d2][changeH2] = true;
			//Local.Intern_itdh[theI][t2][d1][changeH1] = true;

			//// lets update when d1 goes to changeH1

			//for (int w = 0; w < Wards; w++)
			//{
			//	if (data.Hospital[h1].Hospital_dw[d1][w]) // lets chech d1
			//	{
			//		for (int dd = 0; dd < Disciplins; dd++) // shared resource
			//		{
			//			MaxResEmrDemand_tdh[t1][dd][h1]--;
			//			ResEmrDemand_tdh[t1][dd][h1]--;
			//		}
			//	}
			//	if (data.Hospital[changeH1].Hospital_dw[d1][w]) // lets chech d1
			//	{
			//		for (int dd = 0; dd < Disciplins; dd++) // shared resource
			//		{
			//			MaxResEmrDemand_tdh[t2][dd][changeH1]++;
			//			ResEmrDemand_tdh[t2][dd][changeH1]++;
			//		}
			//	}
			//}

			//// lets update when d2 goes to changeH2

			//for (int w = 0; w < Wards; w++)
			//{
			//	if (data.Hospital[h2].Hospital_dw[d2][w]) // lets chech d1
			//	{
			//		for (int dd = 0; dd < Disciplins; dd++) // shared resource
			//		{
			//			MaxResEmrDemand_tdh[t2][dd][h2]--;
			//			ResEmrDemand_tdh[t2][dd][h2]--;
			//		}
			//	}
			//	if (data.Hospital[changeH2].Hospital_dw[d2][w]) // lets chech d1
			//	{
			//		for (int dd = 0; dd < Disciplins; dd++) // shared resource
			//		{
			//			MaxResEmrDemand_tdh[t1][d2][changeH2]++;
			//			ResEmrDemand_tdh[t1][d2][changeH2]++;
			//		}
			//	}
			//}

			//Roster_it[theI][t1] = d2;
			//Roster_it[theI][t2] = d1;

		}

		// w1 - h1, w2 - h2 =====> w1 - h2, w2 - h1 
		public void SwapPairWardsInPairHospital(AllData allData, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (MaxResEmrDemand_tdh[t][d][h] > 0)
						{
							for (int i = 0; i < Interns; i++)
							{
								if (BLcounter >= BLLimit)
								{
									break;
								}
								if (Local.Intern_itdh[i][t][d][h])
								{
									int dd = -1;
									int SecondHospital = -1;
									int secontTime = -1;

									// range for time: max time for oversea and skill
									int maxTime = Timepriods;

									//oversea
									if (data.Intern[i].FHRequirment_d[d])
									{
										for (int tt = 0; tt < maxTime; tt++)
										{
											for (int fh = 0; fh < Disciplins; fh++)
											{
												if (data.Intern[i].OverSea_dt[fh][tt])
												{
													if (maxTime > tt)
													{
														maxTime = tt;
														break;
													}
												}
											}
										}
									}

									// skill 
									if (data.Discipline[d].requiredLater_p[data.Intern[i].ProgramID])
									{
										for (int sk = 0; sk < Disciplins; sk++)
										{
											if (data.Discipline[sk].Skill4D_dp[d][data.Intern[i].ProgramID])
											{
												for (int tt = 0; tt < maxTime; tt++)
												{
													for (int hh = 0; hh < Hospitals; hh++)
													{
														if (Local.Intern_itdh[i][tt][sk][hh])
														{
															maxTime = tt;
															break;
														}
													}
												}
											}
										}
									}

									// required skill
									if (data.Discipline[d].requiresSkill_p[data.Intern[i].ProgramID])
									{
										bool swapd = false;
										for (int sk = 0; sk < Disciplins; sk++)
										{
											if (data.Discipline[d].Skill4D_dp[sk][data.Intern[i].ProgramID])
											{
												swapd = false;
												for (int tt = 0; tt < maxTime; tt++)
												{
													if (Roster_it[i][tt] == sk)
													{
														swapd = true;
														break;
													}

												}
											}
										}
										// if until max t we do not have the required skill it means that we can not swap this one
										if (swapd)
										{
											continue;
										}
									}

									// find the next discipline
									// random tt
									int[] time = new int[maxTime];
								
									for (int rt = 0; rt < maxTime; rt++)
									{
										time[rt] = rt;
									}
									Random rnd = new Random();
									int[] rtime = time.OrderBy(x => rnd.Next()).ToArray();
									for (int rt = 0; rt < maxTime; rt++)
									{
										dd = -1;
										SecondHospital = -1;
										int tt = rtime[rt];
										if (Roster_it[i][tt] >= 0 && Roster_it[i][tt] != d)
										{

											dd = Roster_it[i][tt];
											secontTime = tt;

											// check the duration 
											if (data.Discipline[d].Duration_p[data.Intern[i].ProgramID] != data.Discipline[dd].Duration_p[data.Intern[i].ProgramID])
											{
												continue;
											}

											// check if it requires skills and you can not change it (tt > t) otherwise we will send it furthor so definetly it has its required skill
											if (data.Discipline[dd].requiresSkill_p[data.Intern[i].ProgramID] && tt > t)
											{
												// we sant to schedule dd at t
												bool swapInd = false;
												for (int sk = 0; sk < Disciplins; sk++)
												{
													if (data.Discipline[dd].Skill4D_dp[sk][data.Intern[i].ProgramID])
													{
														swapInd = false;
														for (int ts = 0; ts < t; ts++)
														{
															if (Roster_it[i][ts] == sk)
															{
																swapInd = true;
															}
														}
													}
												}

												if (!swapInd)
												{
													continue;
												}
											}

											// check if it is skill and you can not change it (tt < t) otherwise we will send it even behind where it is now 
											if (data.Discipline[dd].requiredLater_p[data.Intern[i].ProgramID] && tt < t)
											{
												// we sant to schedule dd at t
												bool swapInd = true;
												for (int sk = 0; sk < Disciplins; sk++)
												{
													if (data.Discipline[sk].Skill4D_dp[dd][data.Intern[i].ProgramID])
													{
														swapInd = true;
														for (int ts = tt; ts < t; ts++)
														{
															if (Roster_it[i][ts] == sk)
															{
																swapInd = false;
															}
														}
													}
												}

												if (!swapInd)
												{
													continue;
												}
											}

											// check oversea requirment 
											// oversea must be after t (we want to send it to t)
											if (data.Intern[i].FHRequirment_d[dd])
											{
												bool swapInd = true;
												for (int tf = 0; tf < t; tf++)
												{
													for (int fh = 0; fh < Disciplins; fh++)
													{
														if (data.Intern[i].OverSea_dt[fh][tf])
														{
															swapInd = false;
														}
													}
												}
												if (!swapInd)
												{
													continue;
												}
											}


											for (int hh = 0; hh < Hospitals; hh++)
											{
												if (Local.Intern_itdh[i][tt][dd][hh])
												{
													SecondHospital = hh;

													break;
												}
											}

											// we send dd to t  and hospital h
											// we send d  to tt and hospital hh
											// desire will not change 
											// but demand will change 

											//if the first hospital has dd at tt
											bool possibleChange = false;

											if (ResEmrDemand_tdh[tt][dd][h] < 0)
											{
												possibleChange = true;
											}

											if (!possibleChange)
											{
												continue;
											}
											// if the second hospital has d at t
											possibleChange = false;

											if (ResEmrDemand_tdh[t][d][SecondHospital] < 0)
											{
												possibleChange = true;
											}

											if (!possibleChange)
											{
												continue;
											}

											
											break;
										}
									}

									if (dd >= 0 && SecondHospital >=0)
									{
										ChangePair(i, t, d, h, SecondHospital, secontTime, dd, SecondHospital, h);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LoopPairChanges(AllData allData, string Name)
		{
			int iterWithuotChange = 0;
			while (BLcounter < BLLimit && iterWithuotChange < 5)
			{
				int  currentBL= BLcounter;
				SwapPairWardsInPairHospital(allData, Name);

				
				UpdateGlobal(Name);
				if (currentBL == BLcounter)
				{
					iterWithuotChange++;
				}
				else
				{
					iterWithuotChange = 0;
				}
			}
		}

		public void UseHungarianAgain()
		{
			bool[][] NotRequiredSkill_id = new bool[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				NotRequiredSkill_id[i] = new bool[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					NotRequiredSkill_id[i][d] = false;
				}
			}
			HungarianNode Root = new HungarianNode(0, data, new HungarianNode(), Global.Intern_itdh, NotRequiredSkill_id);
			HungarianActiveList.Add(Root);


			for (int t = 1; t < Timepriods; t++)
			{
				HungarianNode nestedHungrian = new HungarianNode(t, data, (HungarianNode)HungarianActiveList[t - 1], Global.Intern_itdh, NotRequiredSkill_id);
				HungarianActiveList.Add(nestedHungrian);
			}
		}


		public void setSolution(string Name)
		{
			Global = new OptimalSolution(data);
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
								Global.Intern_itdh[i][t][d][h] = true;
								break;
							}
						}
					}
				}
			}
			Global.WriteSolution(data.allPath.InsGroupLocation, "secondHungarian" + Name);
			
		}
	}
}

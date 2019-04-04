using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using NestedDynamicProgrammingAlgorithm;

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
		public DemandBaseLocalSearch(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			data = allData;
			Initial(incumbentSol, Name);
			
			AssignUnAssignedDiscipline(allData, Name);
			UpdateGlobal(Name, incumbentSol);
			

			PairwiseChangeForward(allData,  Name);
			UpdateGlobal(Name, incumbentSol);

			PairwiseChangeBackward(allData, Name);
			UpdateGlobal(Name, incumbentSol);
			
		}

		public void Initial(OptimalSolution incumbentSol, string Name)
		{

			Global = new OptimalSolution(data);
			Global.copyRosters(incumbentSol.Intern_itdh);
			Global.WriteSolution(data.allPath.InsGroupLocation, "ImprovedDemand" + Name);
			Local = new OptimalSolution(data);
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
										for (int dd = 0; dd < Disciplins; dd++)
										{
											if (data.Hospital[h].Hospital_dw[dd][w])
											{
												MaxResEmrDemand_tdh[t][dd][h]++;
												ResEmrDemand_tdh[t][dd][h]++;
												isONDisc_id[i][d] = true;
												Roster_it[i][t] = d;
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
									double MincurrentObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi*(allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[d] 
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[d])
													- allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
									for (int dd = 0; dd < Disciplins; dd++)
									{
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
											else if(h != SecondHospital)
											{
												// if first hospital has the second discipline
												if ( ResEmrDemand_tdh[t][dd][h] < 0
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

		public void UpdateGlobal(string Name, OptimalSolution incumbentSol)
		{
			Local.WriteSolution(data.allPath.InsGroupLocation, "ImprovedLocal");
			if (Local.Obj > Global.Obj)
			{
				Global = new OptimalSolution(data);
				Global.copyRosters(Local.Intern_itdh);
				Global.WriteSolution(data.allPath.InsGroupLocation, "ImprovedDemand" + Name);
			}
			else
			{
				Initial(incumbentSol, Name);

			}
		}

		public void ChangePair(int theI, int t1, int d1, int h1, int changeH1, int t2, int d2, int h2, int changeH2)
		{
			Local.Intern_itdh[theI][t1][d1][h1] = false;
			Local.Intern_itdh[theI][t2][d2][h2] = false;
			Local.Intern_itdh[theI][t1][d2][changeH2] = true;
			Local.Intern_itdh[theI][t2][d1][changeH1] = true;

			MaxResEmrDemand_tdh[t1][d2][changeH2]++;
			MaxResEmrDemand_tdh[t1][d1][h1]--;
			MaxResEmrDemand_tdh[t2][d1][changeH1]++;
			MaxResEmrDemand_tdh[t2][d2][h2]--;
			Roster_it[theI][t1] = d2;
			Roster_it[theI][t2] = d1;
		}

	
	}
}

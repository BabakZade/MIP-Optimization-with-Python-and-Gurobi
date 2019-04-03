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
		public int[][][] ResEmrDemand_tdh;
		public OptimalSolution finalSol;
		public bool[] InternStatus;
		public bool[][] isONDisc_id;
		public int[][] Roster_it;
		public DemandBaseLocalSearch(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			data = allData;
			Initial(incumbentSol);
			AssignUnAssignedDiscipline(allData, incumbentSol, Name);
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
										ResEmrDemand_tdh[t][d][h]++;
										isONDisc_id[i][d] = true;
										Roster_it[i][t] = d;
									}
								}
							}
						}
					}
				}

			}

			for (int t = 0; t < Timepriods; t++)
			{
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						for (int d = 0; d < Disciplins; d++)
						{
							if (data.Hospital[h].Hospital_dw[d][w])
							{
								ResEmrDemand_tdh[t][d][h] = ResEmrDemand_tdh[t][d][h] - data.Hospital[h].HospitalMaxDem_tw[t][w];
							}
						}
					}
				}
			}
		}

		public void AssignUnAssignedDiscipline(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (ResEmrDemand_tdh[t][d][h] > 0)
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
											if (ResEmrDemand_tdh[t][dd][h] < 0 && hospitalHas && MincurrentObj <= MinImpObj)
											{
												incumbentSol.Intern_itdh[i][t][d][h] = false;
												incumbentSol.Intern_itdh[i][t][dd][h] = true;
												ResEmrDemand_tdh[t][dd][h]++;
												ResEmrDemand_tdh[t][d][h]--;

											}
										}
									}
								}
							}
						}
					}
				}
			}
			finalSol = new OptimalSolution(allData);
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			finalSol.WriteSolution(data.allPath.InsGroupLocation, "UnusedCapImproved" + Name);
		}

		public void PairwiseChange(AllData allData, OptimalSolution incumbentSol, string Name)
		{
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						for (int i = 0; i < Interns; i++)
							{
								if (Roster_it[i][t] == d)
								{
									bool isthereBetterChoice;
									double MincurrentObj = allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_SumDesi * (allData.Intern[i].wieght_d * allData.Intern[i].Prf_d[d]
													+ allData.TrainingPr[allData.Intern[i].ProgramID].weight_p * allData.TrainingPr[allData.Intern[i].ProgramID].Prf_d[d]);
								if (ResEmrDemand_tdh[t][d][h] > 0)
								{
									MincurrentObj -= allData.TrainingPr[allData.Intern[i].ProgramID].CoeffObj_ResCap;
								}
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
											if (ResEmrDemand_tdh[t][dd][h] < 0 && hospitalHas && MincurrentObj <= MinImpObj)
											{
												incumbentSol.Intern_itdh[i][t][d][h] = false;
												incumbentSol.Intern_itdh[i][t][dd][h] = true;
												ResEmrDemand_tdh[t][dd][h]++;
												ResEmrDemand_tdh[t][d][h]--;

											}
										}
									}
								}
							}
					
					}
				}
			}
			finalSol = new OptimalSolution(allData);
			finalSol.copyRosters(incumbentSol.Intern_itdh);
			finalSol.WriteSolution(data.allPath.InsGroupLocation, "UnusedCapImproved" + Name);
		}
	}
}

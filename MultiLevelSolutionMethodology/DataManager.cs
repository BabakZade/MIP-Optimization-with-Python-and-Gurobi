using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace MultiLevelSolutionMethodology
{
	public class DataManager
	{
		public AllData[] data_p;
		

		public DataManager(AllData allData)
		{
			setData(allData);
		}
		public void setData(AllData allData)
		{
			data_p = new AllData[allData.General.TrainingPr];
			for (int p = 0; p < allData.General.TrainingPr; p++)
			{
				data_p[p] = new AllData();
				data_p[p].allPath = allData.allPath;
			}
			for (int p = 0; p < allData.General.TrainingPr; p++)
			{
				// general Info
				data_p[p].General = new GeneralInfo();

				data_p[p].General.Disciplines = allData.General.Disciplines;
				data_p[p].General.Interns = allData.General.Interns;
				data_p[p].General.HospitalWard = allData.General.HospitalWard;
				
				data_p[p].General.Hospitals = allData.General.Hospitals;
				
				data_p[p].General.Region = allData.General.Region;
				data_p[p].General.TrainingPr = 1;
				data_p[p].General.DisciplineGr = allData.General.DisciplineGr;
				
				data_p[p].General.TimePriods = allData.General.TimePriods;
				//Create training program info
				data_p[p].TrainingPr = new TrainingProgramInfo[data_p[p].General.TrainingPr];
				for (int pp = 0; pp < data_p[p].General.TrainingPr; pp++)
				{
					data_p[p].TrainingPr[pp] = new TrainingProgramInfo(data_p[p].General.Disciplines, data_p[p].General.DisciplineGr);
					data_p[p].TrainingPr[pp].Name = (p + 1).ToString() + "_Year";
					data_p[p].TrainingPr[pp].AcademicYear = p + 1;
					
					for (int g = 0; g < data_p[p].General.DisciplineGr; g++)
					{
						for (int d = 0; d < data_p[p].General.Disciplines; d++)
						{
							if (allData.TrainingPr[p].InvolvedDiscipline_gd[g][d])
							{
								data_p[p].TrainingPr[pp].InvolvedDiscipline_gd[g][d] = true;
								data_p[p].TrainingPr[pp].Prf_d[d] = allData.TrainingPr[p].Prf_d[d];
							}
						}
					}

					// assign the weight for preference
					data_p[p].TrainingPr[pp].weight_p = allData.TrainingPr[p].weight_p;
					// assign the coefficient 
					data_p[p].TrainingPr[pp].CoeffObj_SumDesi = allData.TrainingPr[p].CoeffObj_SumDesi;
					data_p[p].TrainingPr[pp].CoeffObj_MinDesi = allData.TrainingPr[p].CoeffObj_MinDesi;
					data_p[p].TrainingPr[pp].CoeffObj_ResCap = allData.TrainingPr[p].CoeffObj_ResCap;
					data_p[p].TrainingPr[pp].CoeffObj_EmrCap = allData.TrainingPr[p].CoeffObj_EmrCap;
					data_p[p].TrainingPr[pp].CoeffObj_NotUsedAcc = allData.TrainingPr[p].CoeffObj_NotUsedAcc;
					data_p[p].TrainingPr[pp].CoeffObj_MINDem = allData.TrainingPr[p].CoeffObj_MINDem;

					data_p[p].TrainingPr[pp].DiscChangeInOneHosp = allData.TrainingPr[p].DiscChangeInOneHosp;
				}

				// Create Hospital Info 
				data_p[p].Hospital = new HospitalInfo[data_p[p].General.Hospitals];
				for (int h = 0; h < data_p[p].General.Hospitals; h++)
				{
					data_p[p].Hospital[h] = new HospitalInfo(data_p[p].General.Disciplines, data_p[p].General.TimePriods, data_p[p].General.HospitalWard, data_p[p].General.Region);
					data_p[p].Hospital[h].Name = h.ToString();
					for (int w = 0; w < data_p[p].General.HospitalWard; w++)
					{
						for (int d = 0; d < data_p[p].General.Disciplines; d++)
						{
							data_p[p].Hospital[h].Hospital_dw[d][w] = allData.Hospital[h].Hospital_dw[d][w];
							
						}
						for (int t = 0; t < data_p[p].General.TimePriods; t++)
						{
							data_p[p].Hospital[h].HospitalMinDem_tw[t][w] = 0;
							data_p[p].Hospital[h].HospitalMaxDem_tw[t][w] = 0;
							data_p[p].Hospital[h].EmergencyCap_tw[t][w] = 0;
							data_p[p].Hospital[h].ReservedCap_tw[t][w] = 0;
						}
					}
					// if the hospital is in the region
					for (int r = 0; r < data_p[p].General.Region; r++)
					{
						data_p[p].Hospital[h].InToRegion_r[r] = allData.Hospital[h].InToRegion_r[r];
					}
				}

				// Create Discipline Info
				data_p[p].Discipline = new DisciplineInfo[data_p[p].General.Disciplines];
				for (int d = 0; d < data_p[p].General.Disciplines; d++)
				{
					data_p[p].Discipline[d] = new DisciplineInfo(data_p[p].General.Disciplines, data_p[p].General.TrainingPr);
				}
				for (int d = 0; d < data_p[p].General.Disciplines; d++)
				{
					data_p[p].Discipline[d].Name = d.ToString();
					for (int pp = 0; pp < data_p[p].General.TrainingPr; pp++)
					{
						data_p[p].Discipline[d].Duration_p[pp] = allData.Discipline[d].Duration_p[p];
						for (int dd = 0; dd < data_p[p].General.Disciplines; dd++)
						{
							data_p[p].Discipline[d].Skill4D_dp[dd][pp] = allData.Discipline[d].Skill4D_dp[dd][p];
							data_p[p].Discipline[d].requiresSkill_p[pp] = allData.Discipline[d].requiresSkill_p[p];
							data_p[p].Discipline[dd].requiredLater_p[pp] = allData.Discipline[dd].requiredLater_p[p];
						}
					}
				}

				// create Intern info
				InternInfo[] xIntern = new InternInfo[data_p[p].General.Interns];
				int totalInternforP = 0;
				for (int i = 0; i < data_p[p].General.Interns; i++)
				{
					xIntern[i] = new InternInfo(data_p[p].General.Hospitals, data_p[p].General.Disciplines, data_p[p].General.TimePriods, data_p[p].General.DisciplineGr, allData.General.TrainingPr, data_p[p].General.Region);
					xIntern[i].ProgramID = allData.Intern[i].ProgramID;
					if (xIntern[i].ProgramID == p)
					{
						totalInternforP++;
					}
					xIntern[i].isProspective = allData.Intern[i].isProspective;
					// hand out group and discipline


					for (int d = 0; d < data_p[p].General.Disciplines; d++)
					{
						for (int h = 0; h < data_p[p].General.Hospitals; h++)
						{
							xIntern[i].Abi_dh[d][h] = allData.Intern[i].Abi_dh[d][h];
							xIntern[i].Fulfilled_dhp[d][h][p] = allData.Intern[i].Fulfilled_dhp[d][h][p];
							xIntern[i].FHRequirment_d[d] = allData.Intern[i].FHRequirment_d[d];
							xIntern[i].overseaReq = allData.Intern[i].overseaReq;
							for (int t = 0; t < data_p[p].General.TimePriods; t++)
							{
								xIntern[i].OverSea_dt[d][t] = allData.Intern[i].OverSea_dt[d][t];
								xIntern[i].Ave_t[t] = allData.Intern[i].Ave_t[t];
							}
							for (int g = 0; g < data_p[p].General.DisciplineGr; g++)
							{
								xIntern[i].DisciplineList_dg[d][g] = allData.Intern[i].DisciplineList_dg[d][g];
								xIntern[i].K_AllDiscipline = allData.Intern[i].K_AllDiscipline;
								xIntern[i].ShouldattendInGr_g[g] = allData.Intern[i].ShouldattendInGr_g[g];
								
							}
						}
						xIntern[i].Prf_d[d] = allData.Intern[i].Prf_d[d];
					}
					// region 
					for (int r = 0; r < data_p[p].General.Region; r++)
					{
						xIntern[i].TransferredTo_r[r] = allData.Intern[i].TransferredTo_r[r];
					}
					
					for (int h = 0; h < data_p[p].General.Hospitals; h++)
					{
						xIntern[i].Prf_h[h] = allData.Intern[i].Prf_h[h] ;
					}
					xIntern[i].wieght_ch = allData.Intern[i].wieght_ch;
					xIntern[i].wieght_d = allData.Intern[i].wieght_d;
					xIntern[i].wieght_h = allData.Intern[i].wieght_h;
					xIntern[i].wieght_w = allData.Intern[i].wieght_w;
				}


				// split the interns
				data_p[p].General.Interns = totalInternforP;
				data_p[p].Intern = new InternInfo[data_p[p].General.Interns];
				totalInternforP = -1;
				for (int i = 0; i < xIntern.Length; i++)
				{					
					if (xIntern[i].ProgramID == p)
					{
						totalInternforP++;
					}
					else
					{
						continue;
					}
					data_p[p].Intern[totalInternforP] = new InternInfo(data_p[p].General.Hospitals, data_p[p].General.Disciplines, data_p[p].General.TimePriods, data_p[p].General.DisciplineGr, allData.General.TrainingPr, data_p[p].General.Region);
					data_p[p].Intern[totalInternforP].ProgramID = 0;
					
					data_p[p].Intern[totalInternforP].isProspective = xIntern[i].isProspective;
					// hand out group and discipline


					for (int d = 0; d < data_p[p].General.Disciplines; d++)
					{
						for (int h = 0; h < data_p[p].General.Hospitals; h++)
						{

							data_p[p].Intern[totalInternforP].Abi_dh[d][h] = xIntern[i].Abi_dh[d][h];
							data_p[p].Intern[totalInternforP].Fulfilled_dhp[d][h][p] = xIntern[i].Fulfilled_dhp[d][h][p];
							data_p[p].Intern[totalInternforP].FHRequirment_d[d] = xIntern[i].FHRequirment_d[d];
							data_p[p].Intern[totalInternforP].overseaReq = xIntern[i].overseaReq;
							for (int t = 0; t < data_p[p].General.TimePriods; t++)
							{
								data_p[p].Intern[totalInternforP].OverSea_dt[d][t] = xIntern[i].OverSea_dt[d][t];
								data_p[p].Intern[totalInternforP].Ave_t[t] = xIntern[i].Ave_t[t];
							}
							for (int g = 0; g < data_p[p].General.DisciplineGr; g++)
							{
								data_p[p].Intern[totalInternforP].DisciplineList_dg[d][g] = xIntern[i].DisciplineList_dg[d][g];
								data_p[p].Intern[totalInternforP].K_AllDiscipline = xIntern[i].K_AllDiscipline;
								data_p[p].Intern[totalInternforP].ShouldattendInGr_g[g] = xIntern[i].ShouldattendInGr_g[g];
							}

							// demand correction
							bool onceChecked = false;
							for (int w = 0; w < allData.General.HospitalWard && !onceChecked; w++)
							{
								for (int g = 0; g < data_p[p].General.DisciplineGr && !onceChecked; g++)
								{
									if (!allData.Hospital[h].Hospital_dw[d][w])
									{
										continue;
									}
									if (!data_p[p].Intern[totalInternforP].DisciplineList_dg[d][g])
									{
										for (int t = 0; t < data_p[p].General.TimePriods; t++)
										{
											if (data_p[p].Hospital[h].HospitalMaxDem_tw[t][w] > 0)
											{
												onceChecked = true;
												break;
											}
											data_p[p].Hospital[h].HospitalMinDem_tw[t][w] = 0;
											data_p[p].Hospital[h].HospitalMaxDem_tw[t][w] = 0;
											data_p[p].Hospital[h].EmergencyCap_tw[t][w] = 0;
											data_p[p].Hospital[h].ReservedCap_tw[t][w] = 0;
										}
									}
									else
									{
										for (int t = 0; t < data_p[p].General.TimePriods; t++)
										{
											onceChecked = true;
											data_p[p].Hospital[h].HospitalMinDem_tw[t][w] = allData.Hospital[h].HospitalMinDem_tw[t][w];
											data_p[p].Hospital[h].HospitalMaxDem_tw[t][w] = allData.Hospital[h].HospitalMaxDem_tw[t][w];
											data_p[p].Hospital[h].EmergencyCap_tw[t][w] = allData.Hospital[h].EmergencyCap_tw[t][w];
											data_p[p].Hospital[h].ReservedCap_tw[t][w] = allData.Hospital[h].ReservedCap_tw[t][w];
										}
									}
								}

							}
						}
						data_p[p].Intern[totalInternforP].Prf_d[d] = xIntern[i].Prf_d[d];
					}
					// region 
					for (int r = 0; r < data_p[p].General.Region; r++)
					{
						data_p[p].Intern[totalInternforP].TransferredTo_r[r] = xIntern[i].TransferredTo_r[r];
					}

					for (int h = 0; h < data_p[p].General.Hospitals; h++)
					{
						data_p[p].Intern[totalInternforP].Prf_h[h] = xIntern[i].Prf_h[h];
					}
					data_p[p].Intern[totalInternforP].wieght_ch = xIntern[i].wieght_ch;
					data_p[p].Intern[totalInternforP].wieght_d = xIntern[i].wieght_d;
					data_p[p].Intern[totalInternforP].wieght_h = xIntern[i].wieght_h;
					data_p[p].Intern[totalInternforP].wieght_w = xIntern[i].wieght_w;
				}
				// create Region info
				data_p[p].Region = new RegionInfo[data_p[p].General.Region];
				for (int r = 0; r < data_p[p].General.Region; r++)
				{
					data_p[p].Region[r] = new RegionInfo(data_p[p].General.TimePriods);
					data_p[p].Region[r].Name = "Reg_" + r;
					data_p[p].Region[r].SQLID = r;
					
					for (int t = 0; t < data_p[p].General.TimePriods; t++)
					{
						data_p[p].Region[r].AvaAcc_t[t] = allData.Region[r].AvaAcc_t[t];
					}
				}

				//Algorithm Settings
				data_p[p].AlgSettings = new AlgorithmSettings();
				data_p[p].AlgSettings.BPTime = allData.AlgSettings.BPTime;
				data_p[p].AlgSettings.MasterTime = allData.AlgSettings.MasterTime;
				data_p[p].AlgSettings.MIPTime = allData.AlgSettings.MIPTime/allData.General.TrainingPr;
				data_p[p].AlgSettings.RCepsi = allData.AlgSettings.RCepsi;
				data_p[p].AlgSettings.RHSepsi = allData.AlgSettings.RHSepsi;
				data_p[p].AlgSettings.SubTime = allData.AlgSettings.SubTime;
				data_p[p].AlgSettings.NodeTime = allData.AlgSettings.NodeTime;
				data_p[p].AlgSettings.BigM = allData.AlgSettings.BigM;
				data_p[p].AlgSettings.MotivationBM = allData.AlgSettings.MotivationBM;
				data_p[p].AlgSettings.bucketBasedImpPercentage = allData.AlgSettings.bucketBasedImpPercentage;
				data_p[p].AlgSettings.internBasedImpPercentage = allData.AlgSettings.internBasedImpPercentage;
			}

			
		}
	}
}

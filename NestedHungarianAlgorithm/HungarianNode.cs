using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedHungarianAlgorithm
{
	public struct PositionMap
	{
		public int HIndex;
		public int WIndex;
		public int dIndex;
		public int DemandIndex;
		public bool EmrDem;
		public bool ResDem;
		public bool Mindemand;
		public double positionDesire;
		public PositionMap(int x)
		{
			HIndex = x;
			WIndex = x;
			dIndex = x;
			DemandIndex = x;
			EmrDem = false;
			ResDem = false;
			positionDesire = 0;
			Mindemand = false;
		}
		public void CopyPosition(PositionMap copyable)
		{
			HIndex = copyable.HIndex;
			WIndex = copyable.WIndex;
			dIndex = copyable.dIndex;
			DemandIndex = copyable.DemandIndex;
			EmrDem = copyable.EmrDem;
			ResDem = copyable.ResDem;
			positionDesire = copyable.positionDesire;
			Mindemand = copyable.Mindemand;
		}
	}
	public class HungarianNode
	{
		
		public AllData data;
		public bool isRoot;
		public int TimeID;
		public double[][] CostMatrix_i_whDem;
		public int[] BestSchedule;
		public bool[][][] Schedule_idh;
		public HungarianNode parentNode;
		public int[][] TimeLine_it;
		public int[] requiredTimeForRemainedDisc;
		public int[][][] DemMax_wth;
		public int[][][] DemMin_wth;
		public int[][][] Disc_iwh;
		public int[][] AvAcc_rt;
		public PositionMap[][] ResidentSchedule_it; // it shows the discipline 
		public ArrayList MappingTable;
		public int TotalAvailablePosition;
		public PositionMap[] LastPosition_i;
		public int[] K_totalDiscipline;
		public int[][] discGrCounter_ig;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public int DisciplineGr;
		
		public int[][] Change_ih;

		public bool[][] sHeOccupies_ir;

		public double[] discAvailbilityHosp_d;
		public void Initial()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			DisciplineGr = data.General.DisciplineGr;
			TimeLine_it = new int[Interns][];
			discAvailbilityHosp_d = new double[Disciplins];
			for (int d = 0; d < Disciplins; d++)
			{
				discAvailbilityHosp_d[d] = 0;
			}
			for (int h = 0; h < Hospitals; h++)
			{
				for (int w = 0; w < Wards; w++)
				{
					for (int d = 0; d < Disciplins; d++)
					{
						if (data.Hospital[h].Hospital_dw[d][w])
						{
							discAvailbilityHosp_d[d]++;
						}
					}
				}
			}
			for (int d = 0; d < Disciplins; d++)
			{
				discAvailbilityHosp_d[d] /= Hospitals;
			}
			for (int i = 0; i < Interns; i++)
			{
				TimeLine_it[i] = new int[Timepriods];
				for (int t = 0; t < Timepriods; t++)
				{
					//  0 available
					// -1 not available 
					//  1 engaged with a discipline
					if (data.Intern[i].Ave_t[t])
					{
						if (isRoot)
						{
							TimeLine_it[i][t] = 0;
						}
						else
						{
							TimeLine_it[i][t] = parentNode.TimeLine_it[i][t];
						}
					}
					else
					{
						TimeLine_it[i][t] = -1;
					}
				}
			}

			Change_ih = new int[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				Change_ih[i] = new int[Hospitals];
				for (int h = 0; h < Hospitals; h++)
				{
					if (isRoot)
					{
						Change_ih[i][h] = data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp;
					}
					else
					{
						Change_ih[i][h] = parentNode.Change_ih[i][h];
					}
					
				}
			}

			TotalAvailablePosition = 0;
			ResidentSchedule_it = new PositionMap[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				ResidentSchedule_it[i] = new PositionMap[Timepriods];
				for (int t = 0; t < Timepriods; t++)
				{
					ResidentSchedule_it[i][t] = new PositionMap(-1);
					if (isRoot)
					{


					}
					else
					{
						ResidentSchedule_it[i][t].CopyPosition(parentNode.ResidentSchedule_it[i][t]);
					}

				}
			}
			K_totalDiscipline = new int[Interns];
			for (int i = 0; i < Interns; i++)
			{
				if (isRoot)
				{
					K_totalDiscipline[i] = data.Intern[i].K_AllDiscipline;
				}
				else
				{
					K_totalDiscipline[i] = parentNode.K_totalDiscipline[i];
				}
				
			}
			discGrCounter_ig = new int[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				
				bool[] d_status = new bool[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					d_status[d] = false;
				}
				discGrCounter_ig[i] = new int[DisciplineGr];
				for (int g = 0; g < DisciplineGr; g++)
				{
					if (isRoot)
					{
						discGrCounter_ig[i][g] = data.Intern[i].ShouldattendInGr_g[g];
						for (int d = 0; d < Disciplins; d++)
						{
							if (!data.Intern[i].DisciplineList_dg[d][g])
							{
								continue;
							}
							for (int t = 0; t < Timepriods; t++)
							{
								if (data.Intern[i].OverSea_dt[d][t] && !d_status[d])
								{
									discGrCounter_ig[i][g]--;
									K_totalDiscipline[i]--;
									d_status[d] = true;
									break;
								}
								else if(data.Intern[i].OverSea_dt[d][t] && d_status[d])
								{
									// the oversea discipline is a mutual discipline and might be and issue 
								}
							}
							
						}
					}
					else
					{
						discGrCounter_ig[i][g] = parentNode.discGrCounter_ig[i][g];
					}
				}
			}
			setDisc_iwh();
			MappingTable = new ArrayList();

			// max demand has reserved and emergency inside
			DemMax_wth = new int[Wards][][];
			for (int d = 0; d < Wards; d++)
			{
				DemMax_wth[d] = new int[Timepriods][];
				for (int t = 0; t < Timepriods; t++)
				{
					DemMax_wth[d][t] = new int[Hospitals];
					for (int h = 0; h < Hospitals; h++)
					{
						if (isRoot)
						{
							DemMax_wth[d][t][h] = data.Hospital[h].HospitalMaxDem_tw[t][d] + data.Hospital[h].EmergencyCap_tw[t][d] + data.Hospital[h].ReservedCap_tw[t][d];
						}
						else
						{
							DemMax_wth[d][t][h] = parentNode.DemMax_wth[d][t][h];							
						}						
					}
				}
			}

			// we need map for current time
			for (int w = 0; w < Wards; w++)
			{
				for (int h = 0; h < Hospitals; h++)
				{
					TotalAvailablePosition += DemMax_wth[w][TimeID][h];
					int totalMinDem = data.Hospital[h].HospitalMinDem_tw[TimeID][w];
					for (int dd = 0; dd < DemMax_wth[w][TimeID][h] - (data.Hospital[h].EmergencyCap_tw[TimeID][w] + data.Hospital[h].ReservedCap_tw[TimeID][w]); dd++)
					{
						MappingTable.Add(new PositionMap()
						{
							DemandIndex = dd,
							WIndex = w,
							HIndex = h,
							ResDem = false,
							EmrDem = false,
							Mindemand = totalMinDem > 0 ? true : false,
						});
						totalMinDem--;
					}
					int start = DemMax_wth[w][TimeID][h] - (data.Hospital[h].EmergencyCap_tw[TimeID][w] + data.Hospital[h].ReservedCap_tw[TimeID][w]);
					for (int dd = start; dd < DemMax_wth[w][TimeID][h] - data.Hospital[h].EmergencyCap_tw[TimeID][w]; dd++)
					{
						MappingTable.Add(new PositionMap()

						{
							DemandIndex = dd,
							WIndex = w,
							HIndex = h,
							ResDem = true,
							EmrDem = false,
						});
					}
					start = DemMax_wth[w][TimeID][h] - data.Hospital[h].EmergencyCap_tw[TimeID][w];
					for (int dd = start; dd < DemMax_wth[w][TimeID][h]; dd++)
					{
						MappingTable.Add(new PositionMap()
						{
							DemandIndex = dd,
							WIndex = w,
							HIndex = h,
							ResDem = false,
							EmrDem = true,
						});
					}
					
					
				}
			}
			// oversea
			for (int dd = TotalAvailablePosition; dd < TotalAvailablePosition + Interns; dd++)
			{
				MappingTable.Add(new PositionMap(-1));
			}

			DemMin_wth = new int[Wards][][];
			for (int d = 0; d < Wards; d++)
			{
				DemMin_wth[d] = new int[Timepriods][];
				for (int t = 0; t < Timepriods; t++)
				{
					DemMin_wth[d][t] = new int[Hospitals];
					for (int h = 0; h < Hospitals; h++)
					{
						if (isRoot)
						{
							DemMin_wth[d][t][h] = data.Hospital[h].HospitalMinDem_tw[t][d];
						}
						else
						{
							DemMin_wth[d][t][h] = parentNode.DemMin_wth[d][t][h];
						}
					}
				}
			}
			AvAcc_rt = new int[Region][];
			for (int r = 0; r < Region; r++)
			{
				AvAcc_rt[r] = new int[Timepriods];
				for (int t = 0; t < Timepriods; t++)
				{
					if (isRoot)
					{
						AvAcc_rt[r][t] = data.Region[r].AvaAcc_t[t];
					}
					else
					{
						AvAcc_rt[r][t] = parentNode.AvAcc_rt[r][t];
					}

				}
			}
			LastPosition_i = new PositionMap[Interns];
			for (int i = 0; i < Interns; i++)
			{
				LastPosition_i[i] = new PositionMap();
				if (isRoot)
				{

				}
				else
				{
					LastPosition_i[i].CopyPosition(parentNode.LastPosition_i[i]);
				}
			}

			requiredTimeForRemainedDisc = new int[Interns];
			for (int i = 0; i < Interns; i++)
			{
				if (isRoot)
				{
					requiredTimeForRemainedDisc[i] = (int)data.Intern[i].requieredTimeForRemianed[data.Intern[i].K_AllDiscipline - 1];
				}
				else
				{
					requiredTimeForRemainedDisc[i] = parentNode.requiredTimeForRemainedDisc[i];
				}
				
			}
		}
		public void setCostMatrix(bool[][][][] MotivationList_itdh, bool[][] NotRequiredSkill_id)
		{
			CostMatrix_i_whDem = new double[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				// total available position 
				// intern = if we want to assign oversea
				// intern = if intern wants to wait		

				CostMatrix_i_whDem[i] = new double[TotalAvailablePosition + Interns + Interns];
				for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
				{
					CostMatrix_i_whDem[i][j] = 0;
				}
			}
			// just desire (discipline + hospital + training program + change) + wait + oversea
			setDesireCost(MotivationList_itdh);
			// if intern is not available 
			setAvailibilityCost();
			// if interns can not
			setAbilityCost();
			// set emergency and reserved demand in cost
			setResEmrDemandCost();
			// set unoccupied accommodation 
			setUnaccupiedAccCost();
			// set motivation to get oversea
			setOverseaReq();
			// set motivation for demand min
			setMinDemMotivationCost();
			// set motivation for rare discipline and vital
			setTakingPercentageMotivation();
			// set motivation for need skill in the future
			setSkillbasedCost(NotRequiredSkill_id);
		}

		public void setDesireCost(bool[][][][] MotivationList_itdh)
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
				{					
					// just desire 
					if (j < TotalAvailablePosition)
					{
						int discIn = Disc_iwh[i][((PositionMap)MappingTable[j]).WIndex][((PositionMap)MappingTable[j]).HIndex];
						// if the intern is already assigned to this discipline
						if (discIn < 0)
						{
							CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
						}
						else
						{
							double coeff = data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi + data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_MinDesi;
				
							//double coeff = data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
							if (Change_ih[i][((PositionMap)MappingTable[j]).HIndex] > 0)
							{
								// if the person can be assigned to this hospital
								// it is based on total changes
								CostMatrix_i_whDem[i][j] -=  coeff * data.Intern[i].wieght_h * data.Intern[i].Prf_h[((PositionMap)MappingTable[j]).HIndex];
							}
							else
							{
								CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
							}

							// discipline prf
							if (false && TimeID <= 1 && data.Intern[i].takingDiscPercentage[discIn] > 0.95)// for the discipline which will be added anyway I will not add preferences 
							{
								CostMatrix_i_whDem[i][j] -= coeff * data.Intern[i].wieght_d * data.Intern[i].MaxPrfDiscipline;
							}
							else
							{
								CostMatrix_i_whDem[i][j] -= coeff * data.Intern[i].wieght_d * data.Intern[i].Prf_d[discIn];
							}
							
							// Training Program prf
							CostMatrix_i_whDem[i][j] -= coeff * data.TrainingPr[data.Intern[i].ProgramID].weight_p * data.TrainingPr[data.Intern[i].ProgramID].Prf_d[discIn];

							// motivationList
							if (MotivationList_itdh[i][TimeID][discIn][((PositionMap)MappingTable[j]).HIndex])
							{
								CostMatrix_i_whDem[i][j] -= data.AlgSettings.MotivationBM;
							}
							
							//Change
							if (!isRoot)
							{
								int prHosp = parentNode.LastPosition_i[i].HIndex;
								if (prHosp != ((PositionMap)MappingTable[j]).HIndex)
								{
									CostMatrix_i_whDem[i][j] -= coeff * data.Intern[i].wieght_ch;
								}
							}
							// if interns list is longer than availble time the we have to devide the preferences to duration of the discipline
							if (requiredTimeForRemainedDisc[i] > data.General.TimePriods - TimeID) 
							{
								CostMatrix_i_whDem[i][j] /= data.Discipline[discIn].Duration_p[data.Intern[i].ProgramID];
							}

						}						
					}
					else if(j < TotalAvailablePosition + Interns) // oversea intern
					{
						bool overseaExist = false;
						for (int d = 0; d < Disciplins; d++)
						{
							if (data.Intern[i].OverSea_dt[d][TimeID])
							{
								overseaExist = true;
								break;
							}
						}
						if (overseaExist) // he or she has to go oversea
						{
							CostMatrix_i_whDem[i][j] -= data.AlgSettings.BigM;
						}
						else
						{
							CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
						}						
					}
					else 
					{
						if (requiredTimeForRemainedDisc[i] > data.General.TimePriods - TimeID ) // if interns must take a course 
						{
							//CostMatrix_i_whDem[i][j] += data.AlgSettings.MotivationBM;
						}
						else // if the intern wants to wait(weight_w<0)
						{
							CostMatrix_i_whDem[i][j] -= data.Intern[i].wieght_w;
						}
						
					}
				}
			}
		}

		public void setAvailibilityCost()
		{
			for (int i = 0; i < Interns; i++)
			{
				// if it is not available j must be less than TotalAvailablePosition
				for (int j = 0; j < TotalAvailablePosition; j++)
				{
					if (j < TotalAvailablePosition + Interns)
					{
						// if intern is not available or engaged with other disciplines
						if (!data.Intern[i].Ave_t[TimeID])
						{
							CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
						} // if intern is engaged with other disciplines
						else if (!isRoot && parentNode.TimeLine_it[i][TimeID] > 0)
						{
							CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
						}
					}
					else
					{
						// anyway he has to wait :D
					}
					
				}
			}
		}

		public void setAbilityCost()
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition ; j++)
				{
					int discIn = Disc_iwh[i][((PositionMap)MappingTable[j]).WIndex][((PositionMap)MappingTable[j]).HIndex];
					// if the intern is already assigned to this discipline
					if (discIn < 0)
					{
						CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
					}
					// if intern is not available 
					else if (j < TotalAvailablePosition && !data.Intern[i].Abi_dh[discIn][((PositionMap)MappingTable[j]).HIndex])
					{
						CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
					}
				}
			}
		}

		public void setResEmrDemandCost()
		{
			// this function considers if at the timeID the intern assigned to emergency or reserved capacity 
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition; j++)
				{
					// like MIP
					for (int p = 0; p < data.General.TrainingPr; p++)
					{
						if (((PositionMap)MappingTable[j]).ResDem)
						{
							CostMatrix_i_whDem[i][j] += data.TrainingPr[p].CoeffObj_ResCap;
						}
						if (((PositionMap)MappingTable[j]).EmrDem)
						{
							CostMatrix_i_whDem[i][j] += data.TrainingPr[p].CoeffObj_EmrCap;
						}

					}

				}
			}

		}


		public void setMinDemMotivationCost()
		{
			// this function considers if at the timeID the intern assigned to emergency or reserved capacity 
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition; j++)
				{
					// like MIP
					for (int p = 0; p < data.General.TrainingPr; p++)
					{
						if (((PositionMap)MappingTable[j]).Mindemand && data.Intern[i].isProspective)
						{
							CostMatrix_i_whDem[i][j] -= data.TrainingPr[p].CoeffObj_MINDem;
						}
					}

				}
			}

		}

		public void setTakingPercentageMotivation()
		{
			// we need to motivate the intern to take the discipline which is not in all the hospitals
			// i.e. intern must take disc 1, but the only hospital he can go is hospital 1 which does not have disc1
			// therefore we motivate the intern to take disc 1 at the time that he can go all the hospital
			// this can happen to the interns that must change the hospital for each discpline 
			for (int i = 0; i < Interns; i++)
			{
				if (data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp < 2 || true) // we consider 1 // we cosnider all
				{
					for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
					{
						// just desire 
						if (j < TotalAvailablePosition && TimeID > 1)
						{
							int discIn = Disc_iwh[i][((PositionMap)MappingTable[j]).WIndex][((PositionMap)MappingTable[j]).HIndex];
							// if the intern is already assigned to this discipline
							// the chance that the intern takes this discipline must be above 70% then we motivate him
							// the discpline must be in less than 50% of the hospital
							if (discIn >= 0 && ((data.Intern[i].takingDiscPercentage[discIn] > 0.7 && discAvailbilityHosp_d[discIn] < 0.5) || data.Intern[i].takingDiscPercentage[discIn] > 0.95)) 
							{
								// see if there is an hospital that has not have this discpline
								double motivation = 1 + (data.Intern[i].takingDiscPercentage[discIn] + (1 - discAvailbilityHosp_d[discIn]));
								if (data.Hospital[((PositionMap)MappingTable[j]).HIndex].Hospital_dw[discIn][((PositionMap)MappingTable[j]).WIndex])
								{
									CostMatrix_i_whDem[i][j] *= motivation;
								}

							}
						}
					}
				}
				
			}

		}

		public void setUnaccupiedAccCost()
		{
			// set the sorting based algorithm
			WhoWillOccupySingleRegionSortBased();
			// this function considers if at the timeID the intern assigned to emergency or reserved capacity 
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition; j++)
				{
					for (int r = 0; r < Region; r++)
					{
						if (sHeOccupies_ir[i][r] && data.Intern[i].TransferredTo_r[r] && data.Hospital[((PositionMap)MappingTable[j]).HIndex].InToRegion_r[r] && AvAcc_rt[r][TimeID] > 0)
						{
							CostMatrix_i_whDem[i][j] -= data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_NotUsedAcc;
						}
					}
				}
			}

		}

		public void setOverseaReq()
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
				{
					// just desire 
					if (j < TotalAvailablePosition && data.Intern[i].overseaReq)
					{
						int discIn = Disc_iwh[i][((PositionMap)MappingTable[j]).WIndex][((PositionMap)MappingTable[j]).HIndex];
						// if the intern is already assigned to this discipline
						if (discIn >= 0)
						{
							if (data.Intern[i].FHRequirment_d[discIn])
							{
								CostMatrix_i_whDem[i][j] -= data.AlgSettings.MotivationBM;
							}
						}
					}
				}
			}
		}

		public void setSkillbasedCost(bool[][] NotRequiredSkill_id)
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
				{
					// just desire 
					if (j < TotalAvailablePosition )
					{
						int discIn = Disc_iwh[i][((PositionMap)MappingTable[j]).WIndex][((PositionMap)MappingTable[j]).HIndex];
						// if the intern is already assigned to this discipline
						if (discIn >= 0)
						{
							if (data.Discipline[discIn].requiredLater_p[data.Intern[i].ProgramID] && !NotRequiredSkill_id[i][discIn])
							{
								CostMatrix_i_whDem[i][j] -= data.AlgSettings.MotivationBM;
							}
						}
					}
				}
			}
		}

		public HungarianNode() { }

		public HungarianNode(int startTime, AllData allData, HungarianNode parent, bool[][][][] MotivationList_itdh, bool[][] NotRequiredSkill_id)
		{
			TimeID = startTime;
			parentNode = parent;
			if (startTime == 0)
			{				
				isRoot = true;
			}
			data = allData;
			Initial();
			setCostMatrix(MotivationList_itdh, NotRequiredSkill_id);
			setBestSchedule();
			updateLastPos();
			updateDemand();
			updateTimeLine();
			
		}

		public void setBestSchedule()
		{
			int[,] tmpCost = new int[Interns, TotalAvailablePosition + Interns + Interns];
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition + Interns + Interns; j++)
				{
					tmpCost[i, j] = (int)Math.Round(CostMatrix_i_whDem[i][j]);
				}
			}
			BestSchedule = HungarianAlgorithm.FindAssignments(tmpCost);
		}

		public void updateDemand()
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int w = 0; w < Wards; w++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						for (int d = 0; d < Disciplins; d++)
						{
							if (Schedule_idh[i][d][h] && data.Hospital[h].Hospital_dw[d][w])
							{
								for (int t = TimeID; t < TimeID + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; t++)
								{
									DemMax_wth[w][t][h]--;
									if (DemMin_wth[w][t][h] > 0)
									{
										DemMin_wth[w][t][h]--;
									}
									else
									{
										DemMin_wth[w][t][h] = 0;
									}
									for (int r = 0; r < Region; r++)
									{
										if (data.Intern[i].TransferredTo_r[r] && data.Hospital[h].InToRegion_r[r])
										{
											AvAcc_rt[r][t]--;
										}
									}
									
								}
							}
						}
						
					}
				}
			}
		}

		public void updateTimeLine()
		{

			for (int i = 0; i < Interns; i++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals + 1; h++)
					{
						if (Schedule_idh[i][d][h])
						{
							for (int t = TimeID; t < TimeID + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; t++)
							{
								TimeLine_it[i][t] = 1;
								
							}
						}
					}
				}
			}
		}


		/// <summary>
		/// It sets the last position of the interns
		/// It also changes the ward to discipline
		/// </summary>
		public void updateLastPos()
		{
			new ArrayInitializer().CreateArray(ref Schedule_idh, Interns, Disciplins, Hospitals+1, false);
			for (int i = 0; i < Interns; i++)
			{
				int Index = BestSchedule[i] < TotalAvailablePosition + Interns ? BestSchedule[i] : -1;
				int discIn = -1;
				// if intern waits 
				if (Index < 0)
				{
					// waited 
					// it is already copied from parents
					continue;
				}
				if (BestSchedule[i] < TotalAvailablePosition)
				{
					discIn = Disc_iwh[i][((PositionMap)MappingTable[Index]).WIndex][((PositionMap)MappingTable[Index]).HIndex];

					// Hospital changed
					LastPosition_i[i].CopyPosition((PositionMap)MappingTable[Index]);
					LastPosition_i[i].dIndex = discIn;
					Change_ih[i][((PositionMap)MappingTable[Index]).HIndex]--;
					// we need to assign intern to the discipline not to ward
					Schedule_idh[i][discIn][((PositionMap)MappingTable[Index]).HIndex] = true;
					for (int t = TimeID; t < TimeID + data.Discipline[discIn].Duration_p[data.Intern[i].ProgramID]; t++)
					{
						ResidentSchedule_it[i][t] = new PositionMap
						{
							dIndex = discIn,
							WIndex = ((PositionMap)MappingTable[Index]).WIndex,
							HIndex = ((PositionMap)MappingTable[Index]).HIndex,
						};
					}
					K_totalDiscipline[i]--;
					for (int g = 0; g < DisciplineGr; g++)
					{
						if (data.Intern[i].DisciplineList_dg[discIn][g] && discGrCounter_ig[i][g]>0)
						{
							discGrCounter_ig[i][g]--;
							break;
						}
					}

					requiredTimeForRemainedDisc[i] -= data.Discipline[discIn].Duration_p[data.Intern[i].ProgramID];
				}
				else if(BestSchedule[i] < TotalAvailablePosition + Interns) // oversea
				{
					for (int d = 0; d < Disciplins; d++)
					{
						if (data.Intern[i].OverSea_dt[d][TimeID])
						{
							discIn = d;
							// Hospital changed
							LastPosition_i[i].CopyPosition((PositionMap)MappingTable[Index]);
							LastPosition_i[i].dIndex = discIn;
							// we need to assign intern to the discipline not to ward
							Schedule_idh[i][discIn][Hospitals] = true; // oversea hospital
							for (int t = TimeID; t < TimeID + data.Discipline[discIn].Duration_p[data.Intern[i].ProgramID]; t++)
							{
								ResidentSchedule_it[i][t] = new PositionMap
								{
									dIndex = discIn,
									WIndex = ((PositionMap)MappingTable[Index]).WIndex,
									HIndex = Hospitals,
								};

							}
							//K_totalDiscipline[i]--; dont need it
							for (int g = 0; g < DisciplineGr; g++)
							{
								if (data.Intern[i].DisciplineList_dg[discIn][g] && discGrCounter_ig[i][g] > 0)
								{
									//discGrCounter_ig[i][g]--; dont need it
									break;
								}
							}
							requiredTimeForRemainedDisc[i] -= data.Discipline[discIn].Duration_p[data.Intern[i].ProgramID];
						}
					}
					
				}
				
			}
		}

		public int get_DisciplineFromWard(int theI, int theW, int theH)
		{
			

			int result = -1;			
			int MaxObj = -1;
			int discInd = -1;
			for (int d = 0; d < Disciplins ; d++)
			{
				
				if (data.Intern[theI].OverSea_dt[d][TimeID]) // if the interns must go oversea now
				{
					discInd = -1;
					break;
				}
				bool overseaLater = false;
				for (int t = TimeID; t < Timepriods; t++)
				{
					if (data.Intern[theI].OverSea_dt[d][t]) // if the interns must go oversea later
					{
						overseaLater = true;
						break;
					}
				}
				if (overseaLater)
				{
					continue;
				}
				// the duration of discipline
				if (TimeID + data.Discipline[d].Duration_p[data.Intern[theI].ProgramID] > Timepriods)
				{
					continue;
				}
				int thep = -1;
				
				for (int p = 0; p < TrainingPr && thep < 0; p++)
				{
					for (int h = 0; h < Hospitals && thep < 0; h++)
					{
						if (data.Intern[theI].Fulfilled_dhp[d][h][p])
						{
							thep = p;							
						}
					}					
				}

				if (thep >= 0)
				{
					if (thep == data.Intern[theI].ProgramID && TimeID == 0)
					{
						K_totalDiscipline[theI]--;
					}
					continue;
				}

				if (K_totalDiscipline[theI] <= 0) // no more assignment 
				{
					discInd = -1;
					break;
				}
				// if all the discipline in the group is finished
				bool necessaryToAttend = false;
				for (int g = 0; g < DisciplineGr; g++)
				{
					if (data.Intern[theI].DisciplineList_dg[d][g] && discGrCounter_ig[theI][g]>0)
					{
						necessaryToAttend = true;
						break;
					}
				}
				if (!necessaryToAttend)
				{
					continue;
				}

				if (data.Hospital[theH].Hospital_dw[d][theW])
				{
					// if already assigned
					bool assigned = false;
					for (int t = 0; t < TimeID; t++)
					{
						if (ResidentSchedule_it[theI][t].dIndex == d)
						{
							assigned = true;
							break;
						}						
					}
					bool requiresDis = false;
					if (!assigned && !checkCompetence(theI,d)) // requires other discipline
					{
						requiresDis = true;						
					}

					// availibity during the disciplien
					bool durAve = true;
					for (int t = 0; t < TimeID + data.Discipline[d].Duration_p[data.Intern[theI].ProgramID]; t++)
					{
						if (!data.Intern[theI].Ave_t[t])
						{
							durAve = false;
						}
					}
					if (durAve && !assigned && !requiresDis && 
						MaxObj <= (data.Intern[theI].Prf_d[d] * data.Intern[theI].wieght_d + data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[d] * data.TrainingPr[data.Intern[theI].ProgramID].weight_p ))
					{
						MaxObj = data.Intern[theI].Prf_d[d] + data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[d];
						discInd = d;
					}
					if (durAve && !assigned && !requiresDis && data.Intern[theI].overseaReq && data.Intern[theI].FHRequirment_d[d]) // spacial request for required discipline oversea
					{
						discInd = d;
						break;
					}
					if (durAve && !assigned && !requiresDis && data.Discipline[d].requiredLater_p[data.Intern[theI].ProgramID] ) // the skill of this discipline is important
					{
						discInd = d;
						break;
					}
				}
			}
			result = discInd;
			return result;
		}

		public void setDisc_iwh()
		{
			Disc_iwh = new int[Interns][][];
			for (int i = 0; i < Interns; i++)
			{
				Disc_iwh[i] = new int[Wards][];
				for (int w = 0; w < Wards; w++)
				{
					Disc_iwh[i][w] = new int[Hospitals];
					for (int h = 0; h < Hospitals; h++)
					{
						Disc_iwh[i][w][h] = get_DisciplineFromWard(i, w, h);
					}
				}
			}
		}
		/// <summary>
		/// Checks if the intern needs other discipline in advance to the following discipline
		/// </summary>
		/// <param name="theI">The intern</param>
		/// <param name="theD">The following discipline </param>
		/// <returns>True if (s)he does not need any discipline, false otherwise</returns>
		public bool checkCompetence(int theI, int theD)
		{
			bool result = true;
			if (data.Discipline[theD].requiresSkill_p[data.Intern[theI].ProgramID])
			{
				result = false;
				for (int d = 0; d < Disciplins; d++)
				{
					if (data.Discipline[theD].Skill4D_dp[d][data.Intern[theI].ProgramID])
					{
						bool alreadyDone = false;
						for (int t = 0; t < TimeID && !alreadyDone; t++)
						{
							if (ResidentSchedule_it[theI][t].dIndex == d)
							{
								alreadyDone = true;
							}							
						}
						if (!alreadyDone)
						{
							result = false;
							break;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			else
			{
				result = true;
			}
			
			return result;
		}

		public void WhoWillOccupySingleRegionSortBased()
		{
			new ArrayInitializer().CreateArray(ref sHeOccupies_ir, Interns, Region, false);
			double[][] costForOccupying_ir = new double[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				costForOccupying_ir[i] = new double[Region];
				for (int r = 0; r < Region; r++)
				{
					costForOccupying_ir[i][r] = 0;
				}
			}
			for (int i = 0; i < Interns; i++)
			{
				for (int r = 0; r < Region; r++)
				{
					for (int h = 0; h < Hospitals && data.Intern[i].TransferredTo_r[r]; h++)
					{
						double theObj = 0;
						theObj += data.Intern[i].wieght_h * data.Intern[i].Prf_h[h];
						double MaxDiscPrf = 0;
						for (int w = 0; w < Wards; w++)
						{
							for (int d = 0; d < Disciplins && data.Hospital[h].InToRegion_r[r]; d++)
							{
								double tmpDiscPrf = 0;
								if (data.Hospital[h].Hospital_dw[d][w])
								{
									tmpDiscPrf += data.Intern[i].wieght_d * data.Intern[i].Prf_d[d];
									tmpDiscPrf += data.TrainingPr[data.Intern[i].ProgramID].weight_p * data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d];
								}
								if (tmpDiscPrf > MaxDiscPrf)
								{
									MaxDiscPrf = tmpDiscPrf;
								}
							}
						}

						theObj += MaxDiscPrf;
						if (costForOccupying_ir[i][r] < theObj)
						{
							costForOccupying_ir[i][r] = theObj;  
						}
					}
					
				}
			}

			int[][] indexInternInR_ri = new int[Region][];
			for (int r = 0; r < Region; r++)
			{
				indexInternInR_ri[r] = new int[Interns];
				for (int i = 0; i < Interns; i++)
				{
					indexInternInR_ri[r][i] = i;
				}
			}
			for (int r = 0; r < Region; r++)
			{
				for (int i = 0; i < Interns; i++)
				{
					for (int j = i + 1; j < Interns; j++)
					{
						if (costForOccupying_ir[indexInternInR_ri[r][i]][r] < costForOccupying_ir[indexInternInR_ri[r][j]][r])
						{
							int tmp = indexInternInR_ri[r][i];
							indexInternInR_ri[r][i] = indexInternInR_ri[r][j];
							indexInternInR_ri[r][j] = tmp;
						}
					}
				}
			}
			bool[] InternStatus = new bool[Interns];
			for (int i = 0; i < Interns; i++)
			{
				InternStatus[i] = false;
			}
			for (int r = 0; r < Region; r++)
			{
				for (int cap = 0; cap < data.Region[r].AvaAcc_t[TimeID]; cap++)
				{
					for (int i = 0; i < Interns; i++)
					{
						// he is busy in the other hospital
						if (!isRoot && parentNode.TimeLine_it[indexInternInR_ri[r][i]][TimeID] > 0)
						{
							continue;
						}

						// if he is not alreaady assigned to other region and available
						if (!InternStatus[indexInternInR_ri[r][i]] && data.Intern[indexInternInR_ri[r][i]].Ave_t[TimeID] )
						{
							sHeOccupies_ir[indexInternInR_ri[r][i]][r] = true;
							InternStatus[indexInternInR_ri[r][i]] = true;							
						}
					}					
				}
			}
		}
	}
}

using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedHungarianAlgorithm
{
	public class HungarianNode
	{
		public struct PositionMap
		{
			public int HIndex;
			public int WIndex;
			public int dIndex;
			public int DemandIndex;
			public bool EmrDem;
			public bool ResDem;
			public PositionMap(int x)
			{
				HIndex = x;
				WIndex = x ;
				dIndex = x;
				DemandIndex = x;
				EmrDem = false;
				ResDem = false;
			}
			public void CopyPosition(PositionMap copyable)
			{
				HIndex = copyable.HIndex;
				WIndex = copyable.WIndex;
				dIndex = copyable.dIndex;
				DemandIndex = copyable.DemandIndex;
				EmrDem = copyable.EmrDem;
				ResDem = copyable.ResDem;
			}
		}
		public AllData data;
		public bool isRoot;
		public int TimeID;
		public double[][] CostMatrix_i_whDem;
		public int[] BestSchedule;
		public bool[][][] Schedule_idh;
		public HungarianNode parentNode;
		public int[][] TimeLine_it;
		public int[][][] DemMax_wth;
		public int[][][] DemMin_wth;
		public int[][][] Disc_iwh;
		public int[][] AvAcc_rt;
		public PositionMap[][] ResidentSchedule_it; // it shows the disciplne 
		public ArrayList MappingTable;
		public int TotalAvailablePosition;
		public PositionMap[] LastPosition_i;
		public int[] K_totalDiscipline;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		

		public void Initial()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			TimeLine_it = new int[Interns][];
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
					for (int dd = 0; dd < DemMax_wth[w][TimeID][h] - (data.Hospital[h].EmergencyCap_tw[TimeID][w] + data.Hospital[h].ReservedCap_tw[TimeID][w]); dd++)
					{
						MappingTable.Add(new PositionMap()
						{
							DemandIndex = dd,
							WIndex = w,
							HIndex = h,
							ResDem = false,
							EmrDem = false,
						});
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
			
		}

		public void setCostMatrix()
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
			setDesireCost();
			// if intern is not available 
			setAvailibilityCost();
			// if interns can not
			setAbilityCost();
			// set emergency and reserved demand in cost
			setResEmrDemandCost();
			// set unoccupied accommodation 
			setUnaccupiedAccCost();
		}

		public void setDesireCost()
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
							// hospital prf
							CostMatrix_i_whDem[i][j] -= (double)data.Intern[i].wieght_h * data.Intern[i].Prf_h[((PositionMap)MappingTable[j]).HIndex];
							// discipline prf
							CostMatrix_i_whDem[i][j] -= (double)data.Intern[i].wieght_d * data.Intern[i].Prf_d[discIn];
							// Training Program prf
							CostMatrix_i_whDem[i][j] -= (double)data.TrainingPr[data.Intern[i].ProgramID].weight_p * data.TrainingPr[data.Intern[i].ProgramID].Prf_d[discIn];
							//Change
							if (!isRoot)
							{
								int prHosp = parentNode.LastPosition_i[i].HIndex;
								if (prHosp != ((PositionMap)MappingTable[j]).HIndex)
								{
									CostMatrix_i_whDem[i][j] -= (double)data.Intern[i].wieght_ch * data.TrainingPr[data.Intern[i].ProgramID].Prf_d[discIn];
								}
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
					else // if the intern wants to wait(weight_w<0)
					{
						CostMatrix_i_whDem[i][j] -= (double)data.Intern[i].wieght_w;
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
					// if intern is not available 
					if (!data.Intern[i].Ave_t[TimeID])
					{
						CostMatrix_i_whDem[i][j] += data.AlgSettings.BigM;
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
					if (((PositionMap)MappingTable[j]).ResDem )
					{
						CostMatrix_i_whDem[i][j] += data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_ResCap;
					}
					if (((PositionMap)MappingTable[j]).EmrDem)
					{
						CostMatrix_i_whDem[i][j] += data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_EmrCap;
					}
				}
			}

		}

		public void setUnaccupiedAccCost()
		{
			// this function considers if at the timeID the intern assigned to emergency or reserved capacity 
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition; j++)
				{
					for (int r = 0; r < Region; r++)
					{
						if (data.Intern[i].TransferredTo_r[r] && data.Hospital[((PositionMap)MappingTable[j]).HIndex].InToRegion_r[r] && AvAcc_rt[r][TimeID] > 0)
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

		public HungarianNode() { }

		public HungarianNode(int startTime, AllData allData, HungarianNode parent)
		{
			TimeID = startTime;
			parentNode = parent;
			if (startTime == 0)
			{				
				isRoot = true;
			}
			data = allData;
			Initial();
			setCostMatrix();
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
					for (int h = 0; h < Hospitals; h++)
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
						if (TimeID == 0)
						{
							K_totalDiscipline[theI]--;
						}
						break;
					}
				}
				if (overseaLater)
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

				if (K_totalDiscipline[theI] < 0) // no more assignment 
				{
					discInd = -1;
					break;
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
						if (!checkCompetence(d,theI)) // requires other discipline
						{
							assigned = true;
							break;
						}
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
					if (durAve && !assigned && MaxObj <= (data.Intern[theI].Prf_d[d] + data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[d]))
					{
						MaxObj = data.Intern[theI].Prf_d[d] + data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[d];
						discInd = d;
					}
					if (data.Intern[theI].overseaReq && data.Intern[theI].FHRequirment_d[d]) // spacial request for required discipline oversea
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

		public bool checkCompetence(int theI, int theD)
		{
			bool result = true;
			if (data.Discipline[theD].requiresSkill)
			{
				result = false;
				for (int d = 0; d < Disciplins; d++)
				{
					if (data.Discipline[theD].Skill4D_dp[d][data.Intern[theI].ProgramID])
					{
						bool alreadyDone = false;
						for (int t = 0; t < TimeID && !alreadyDone; t++)
						{
							for (int h = 0; h < Hospitals && !alreadyDone; h++)
							{
								if (Schedule_idh[theI][d][h])
								{
									alreadyDone = true;

								}
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


	}
}

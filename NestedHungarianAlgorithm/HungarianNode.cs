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
			public int DIndex;
			public int DemandIndex;

			public void CopyPosition(PositionMap copyable)
			{
				HIndex = copyable.HIndex;
				DIndex = copyable.DIndex;
				DemandIndex = copyable.DemandIndex;
			}
		}
		public AllData data;
		public bool isRoot;
		public int TimeID;
		public double[][][] CostMatrix_i_tdhDem;
		public int[] BestSchedule;
		public bool[][][] Schedule_idh;
		public HungarianNode parentNode;
		public int[][] TimeLine_it;
		public int[][][] DemMax_dth;
		public int[][][] DemMin_dth;
		public ArrayList MappingTable;
		public int TotalAvailablePosition;
		public PositionMap[] LastPosition_i;

		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;

		public void Initial()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
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
			MappingTable = new ArrayList();
			DemMax_dth = new int[Disciplins][][];
			for (int d = 0; d < Disciplins; d++)
			{
				DemMax_dth[d] = new int[Timepriods][];
				for (int t = 0; t < Timepriods; t++)
				{
					DemMax_dth[d][t] = new int[Hospitals];
					for (int h = 0; h < Hospitals; h++)
					{
						if (t == TimeID)
						{
							TotalAvailablePosition += data.Hospital[h].HospitalMaxDem_td[t][d];
							for (int dd = 0; dd < data.Hospital[h].HospitalMaxDem_td[t][d]; dd++)
							{
								MappingTable.Add(new PositionMap()
								{
									DemandIndex = dd,
									DIndex = d,
									HIndex = h,
								});
							}
						}
						
						if (isRoot)
						{
							DemMax_dth[d][t][h] = data.Hospital[h].HospitalMaxDem_td[t][d];
						}
						else
						{
							DemMax_dth[d][t][h] = parentNode.DemMax_dth[d][t][h];
						}
					}
				}
			}

			DemMin_dth = new int[Disciplins][][];
			for (int d = 0; d < Disciplins; d++)
			{
				DemMin_dth[d] = new int[Timepriods][];
				for (int t = 0; t < Timepriods; t++)
				{
					DemMin_dth[d][t] = new int[Hospitals];
					for (int h = 0; h < Hospitals; h++)
					{
						if (isRoot)
						{
							DemMin_dth[d][t][h] = data.Hospital[h].HospitalMinDem_td[t][d];
						}
						else
						{
							DemMin_dth[d][t][h] = parentNode.DemMin_dth[d][t][h];
						}
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
			CostMatrix_i_tdhDem = new double[Timepriods][][];
			for (int t = 0; t < Timepriods; t++)
			{
				CostMatrix_i_tdhDem[t] = new double[Interns][];
				for (int i = 0; i < Interns; i++)
				{
					// total available position + in case to not assign the student to position 
					CostMatrix_i_tdhDem[t][i] = new double[TotalAvailablePosition + Interns];
					for (int j = 0; j < TotalAvailablePosition + Interns; j++)
					{
						CostMatrix_i_tdhDem[t][i][j] = 0;
						if (isRoot)
						{
							// just desire 
							if (j < TotalAvailablePosition)
							{
								CostMatrix_i_tdhDem[t][i][j] -= (double)data.Intern[i].wieght_h * data.Intern[i].Prf_h[((PositionMap)MappingTable[j]).HIndex] / data.PrHosp_i[i];
								CostMatrix_i_tdhDem[t][i][j] -= (double)data.Intern[i].wieght_d * data.Intern[i].Prf_d[((PositionMap)MappingTable[j]).DIndex] / data.PrDisc_i[i];
							}
							else // if the intern wants to wait(weight_w<0)
							{
								CostMatrix_i_tdhDem[t][i][j] -= (double)data.Intern[i].wieght_w / data.PrWait_i[i];
							}
							// if intern is not available 
							if (!data.Intern[i].Ave_t[0])
							{
								CostMatrix_i_tdhDem[t][i][j] += data.AlgSettings.BigM;
							}
							// if interns can not
							if (j < TotalAvailablePosition && !data.Intern[i].Abi_d[((PositionMap)MappingTable[j]).DIndex])
							{
								CostMatrix_i_tdhDem[t][i][j] += data.AlgSettings.BigM;
							}
						}
						else
						{
							CostMatrix_i_tdhDem[t][i][j] = parentNode.CostMatrix_i_tdhDem[t][i][j];
						}
					}
				}
			}
			
		}

		public HungarianNode(int t, AllData allData)
		{
			if (t == 0)
			{
				isRoot = true;
			}
			data = allData;
			Initial();
			setCostMatrix();
			setBestSchedule();
			updateDemand();
			updateTimeLine();
			updateLastPos();
			updateCost();
		}

		public void setBestSchedule()
		{
			int[,] tmpCost = new int[Interns, TotalAvailablePosition + Interns];
			for (int i = 0; i < Interns; i++)
			{
				for (int j = 0; j < TotalAvailablePosition + Interns; j++)
				{
					tmpCost[i, j] = (int)Math.Round(CostMatrix_i_tdhDem[i][TimeID][j] * 100);
				}
			}
			BestSchedule = HungarianAlgorithm.FindAssignments(tmpCost);
		}


		public void updateDemand()
		{
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 0; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						if (Schedule_idh[i][d][h])
						{
							for (int t = TimeID; t < data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; t++)
							{
								DemMax_dth[d][t][h]--;
								if (DemMin_dth[d][t][h] > 0)
								{
									DemMin_dth[d][t][h]--;
								}
								else
								{
									DemMin_dth[d][t][h] = 0;
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
							for (int t = TimeID; t < data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; t++)
							{
								TimeLine_it[i][t] = 1;

							}
						}
					}
				}
			}
		}

		public void updateLastPos()
		{
			for (int i = 0; i < Interns; i++)
			{
				int Index = BestSchedule[i] < TotalAvailablePosition ? BestSchedule[i] : -1;

				// if intern waits 
				if (Index < 0)
				{
					// waited 
					// it is already copied from parents
					continue;
				}
				// Hospital changed
				LastPosition_i[i].CopyPosition((PositionMap)MappingTable[Index]);
				
			}
		}

		public void updateCost()
		{
			for (int i = 0; i < Interns; i++)
			{
				int Index = BestSchedule[i] < TotalAvailablePosition ? BestSchedule[i] : -1;
				int HIndex = -1;
				int DIndex = -1;
				int DemIndex = -1;
				
				if (Index < 0) // if intern waits 
				{
					// waited 
					// Last position 
					HIndex = LastPosition_i[i].HIndex;
					DIndex = LastPosition_i[i].DIndex;
					DemIndex = LastPosition_i[i].DemandIndex - 1;
				}
				else // if not
				{
					// Hospital change 
					HIndex = ((PositionMap)MappingTable[Index]).HIndex;
					DIndex = ((PositionMap)MappingTable[Index]).DIndex;
					DemIndex = ((PositionMap)MappingTable[Index]).DemandIndex - 1;
				}
				
				for (int j = 0; j < TotalAvailablePosition + Interns; j++)
				{
					
					if (((PositionMap)MappingTable[j]).DIndex == DIndex)
					{
						// If it is assigned you should not assign it any more
						for (int t = TimeID+1; t < Timepriods; t++)
						{
							CostMatrix_i_tdhDem[i][t][j] = data.AlgSettings.BigM;
						}						
					}
					// alias
					if (j < TotalAvailablePosition && data.TrainingPr[data.Intern[i].ProgramID].Alias_d_d[((PositionMap)MappingTable[j]).DIndex][DIndex])
					{
						for (int t = TimeID + 1; t < Timepriods; t++)
						{
							CostMatrix_i_tdhDem[i][t][j] = data.AlgSettings.BigM;
						}
					}
					
					// Duration
					for (int h = 0; h < Hospitals; h++)
					{
						for (int t = TimeID + 1; t < t + data.Discipline[DIndex].Duration_p[data.Intern[i].ProgramID]; t++)
						{
							CostMatrix_i_tdhDem[i][t][j] = data.AlgSettings.BigM;
						}
					}

					// Hospital change 
					if (((PositionMap)MappingTable[j]).HIndex != HIndex)
					{
						// The interns assigned to the hospital
						CostMatrix_i_tdhDem[i][TimeID + 1][j] += data.Intern[i].wieght_ch / data.PrChan_i[i];
					}
				}
			}
		}

	}
}

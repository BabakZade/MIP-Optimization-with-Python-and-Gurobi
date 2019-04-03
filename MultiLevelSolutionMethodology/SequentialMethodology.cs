using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using GeneralMIPAlgorithm;
using NestedHungarianAlgorithm;
using System.Diagnostics;


namespace MultiLevelSolutionMethodology
{
	public class SequentialMethodology
	{
		public DataManager dataManager;
		public OptimalSolution[] finalSol_p;
		public OptimalSolution finalSol;
		public int[] ElappesedTime_p;
		public double objFunction;
		int[] TrPrOrder;
		public SequentialMethodology(AllData data, string InsName)
		{
			Initial(data);
			Methodology(TrPrOrder, InsName);
		}

		public void Initial(AllData data)
		{
			dataManager = new DataManager(data);
			ElappesedTime_p = new int[data.General.TrainingPr];
			TrPrOrder = new int[data.General.TrainingPr];
			finalSol_p = new OptimalSolution[data.General.TrainingPr];
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				finalSol_p[p] = new OptimalSolution(dataManager.data_p[p]);
				TrPrOrder[p] = p;
				ElappesedTime_p[p] = 0;
			}
			objFunction = 0;
		}

		public void Methodology(int[] trProgram, string InsName)
		{
			
			for (int p = 0; p < trProgram.Length; p++)
			{
				setMIPMethodology(dataManager.data_p[trProgram[p]], trProgram[p], InsName);
				if (p < trProgram.Length - 1)
				{
					setData(trProgram[p], trProgram[p + 1]);
				}
			}
		}
		
		public void setMIPMethodology(AllData data_p, int theP, string InsName)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			MedicalTraineeSchedulingMIP mip = new MedicalTraineeSchedulingMIP(data_p, InsName + "PrTr_" + theP);
			stopwatch.Stop();
			ElappesedTime_p[theP] = (int)stopwatch.ElapsedMilliseconds / 1000;
			finalSol_p[theP].copyRosters(mip.mipOpt.Intern_itdh);
			finalSol_p[theP].WriteSolution(data_p.allPath.OutPutLocation, InsName + "PrTr_" + theP);
			objFunction += finalSol_p[theP].Obj;
		}

		public void setData(int ExP, int FuP)
		{
			for (int i = 0; i < dataManager.data_p[ExP].General.Interns; i++)
			{
				for (int t = 0; t < dataManager.data_p[ExP].General.TimePriods; t++)
				{
					for (int d = 0; d < dataManager.data_p[ExP].General.Disciplines; d++)
					{
						for (int h = 0; h < dataManager.data_p[ExP].General.Hospitals; h++)
						{
							if (finalSol_p[ExP].Intern_itdh[i][t][d][h])
							{
								for (int w = 0; w < dataManager.data_p[ExP].General.HospitalWard ; w++)
								{
									if (dataManager.data_p[ExP].Hospital[h].Hospital_dw[d][w])
									{
										// demand	
										if (dataManager.data_p[FuP].Hospital[h].HospitalMaxDem_tw[t][w] > 0)
										{
											dataManager.data_p[FuP].Hospital[h].HospitalMaxDem_tw[t][w]--;
											if (dataManager.data_p[FuP].Hospital[h].HospitalMinDem_tw[t][w] > 0)
											{
												dataManager.data_p[FuP].Hospital[h].ReservedCap_tw[t][w]--;
											}
										}
										else
										{
											if (dataManager.data_p[FuP].Hospital[h].ReservedCap_tw[t][w] > 0)
											{
												dataManager.data_p[FuP].Hospital[h].ReservedCap_tw[t][w]--;
											}
											else if (dataManager.data_p[FuP].Hospital[h].EmergencyCap_tw[t][w] > 0)
											{
												dataManager.data_p[FuP].Hospital[h].EmergencyCap_tw[t][w]--;
											}
										}
									}
								}
								for (int r = 0; r < dataManager.data_p[ExP].General.Region ; r++)
								{
									if (dataManager.data_p[ExP].Hospital[h].InToRegion_r[r] && dataManager.data_p[ExP].Intern[i].TransferredTo_r[r])
									{
										if (dataManager.data_p[FuP].Region[r].AvaAcc_t[t] > 0)
										{
											dataManager.data_p[FuP].Region[r].AvaAcc_t[t]--;
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

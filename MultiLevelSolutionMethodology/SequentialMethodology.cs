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
		bool[] setTheTrPR;
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
			setTheTrPR = new bool[data.General.TrainingPr];
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				finalSol_p[p] = new OptimalSolution(data);
				TrPrOrder[p] = p;
				ElappesedTime_p[p] = 0;
				setTheTrPR[p] = false;
			}
			objFunction = 0;
			finalSol = new OptimalSolution(data);
		}

		public void Methodology(int[] trProgram, string InsName)
		{
		
			for (int p = 0; p < trProgram.Length; p++)
			{
				setTheTrPR[trProgram[p]] = true;
				dataManager.SetDataPrTr(setTheTrPR);
				setMIPMethodology(dataManager.localData, trProgram[p], InsName);
				if (p < trProgram.Length - 1)
				{
					setData(trProgram[p]);
				}
				setTheTrPR[trProgram[p]] = false;
			}
			finalSol.WriteSolution(dataManager.localData.allPath.OutPutLocation, InsName + "SeqFinal");
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
			finalSol.addRosters(mip.mipOpt.Intern_itdh);
			objFunction += finalSol_p[theP].Obj;
		}

		public void setData(int ExP)
		{
			for (int i = 0; i < dataManager.localData.General.Interns; i++)
			{
				for (int t = 0; t < dataManager.localData.General.TimePriods; t++)
				{
					for (int d = 0; d < dataManager.localData.General.Disciplines; d++)
					{
						for (int h = 0; h < dataManager.localData.General.Hospitals; h++)
						{
							if (finalSol_p[ExP].Intern_itdh[i][t][d][h])
							{
								for (int w = 0; w < dataManager.localData.General.HospitalWard ; w++)
								{
									if (dataManager.localData.Hospital[h].Hospital_dw[d][w])
									{
										// demand	
										if (dataManager.localData.Hospital[h].HospitalMaxDem_tw[t][w] > 0)
										{
											dataManager.localData.Hospital[h].HospitalMaxDem_tw[t][w]--;
											if (dataManager.localData.Hospital[h].HospitalMinDem_tw[t][w] > 0)
											{
												dataManager.localData.Hospital[h].ReservedCap_tw[t][w]--;
											}
										}
										else
										{
											if (dataManager.localData.Hospital[h].ReservedCap_tw[t][w] > 0)
											{
												dataManager.localData.Hospital[h].ReservedCap_tw[t][w]--;
											}
											else if (dataManager.localData.Hospital[h].EmergencyCap_tw[t][w] > 0)
											{
												dataManager.localData.Hospital[h].EmergencyCap_tw[t][w]--;
											}
										}
									}
								}
								for (int r = 0; r < dataManager.localData.General.Region ; r++)
								{
									if (dataManager.localData.Hospital[h].InToRegion_r[r] && dataManager.localData.Intern[i].TransferredTo_r[r])
									{
										if (dataManager.localData.Region[r].AvaAcc_t[t] > 0)
										{
											dataManager.localData.Region[r].AvaAcc_t[t]--;
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

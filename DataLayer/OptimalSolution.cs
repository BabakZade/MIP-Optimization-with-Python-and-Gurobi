using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataLayer
{
	public class OptimalSolution
	{
		AllData data;
		public bool[][][][] Intern_itdh;
		public double[] PrDisp_i;
		public double[] PrHosp_i;
		public double[] PrWait_i;
		public double[] PrChang_i;
		public double[] TrPrPrf;
		public double[] Des_i;
		public double AveDes;
		public double[] dp_i;
		public double[] dn_i;
		public int[][][] Assigned_twh;
		public double minimizedDev;
		public double[] MinDis;
		public double ResDemand;
		public double EmrDemand;
		public double[][] NotUsesAcc;
		public double[] TotalDis;
		public double NotUsedAccTotal;
		public double Obj;
		public OptimalSolution(AllData data)
		{
			Initial(data);
			this.data = data;
		}
		public void Initial(AllData data)
		{
			new ArrayInitializer().CreateArray(ref Intern_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, false);
			new ArrayInitializer().CreateArray(ref PrDisp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrHosp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrWait_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrChang_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref TrPrPrf, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref Des_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref dp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref dn_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref MinDis, data.General.TrainingPr, data.AlgSettings.BigM);
			new ArrayInitializer().CreateArray(ref TotalDis, data.General.TrainingPr, 0);
			new ArrayInitializer().CreateArray(ref Assigned_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
			new ArrayInitializer().CreateArray(ref NotUsesAcc, data.General.Region, data.General.TimePriods, 0);
			AveDes = 0;
			minimizedDev = 0;
			ResDemand = 0;
			EmrDemand = 0;
			NotUsedAccTotal = 0;
			Obj = 0;

		}

		public void WriteSolution(string Path, string Name)
		{
			StreamWriter tw = new StreamWriter(Path + Name + "OptSol.txt");


			tw.WriteLine("PP | II | GG | DD | TT | HH | K_G | FH (Schedule)");
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int i = 0; i < data.General.Interns; i++)
				{
					for (int g = 0; g < data.General.DisciplineGr; g++)
					{
						if (data.Intern[i].ProgramID == p)
						{
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								if (data.Intern[i].DisciplineList_dg[d][g])
								{
									for (int t = 0; t < data.General.TimePriods; t++)
									{
										for (int h = 0; h < data.General.Hospitals; h++)
										{
											if (data.Intern[i].OverSea_dt[d][t])
											{
												tw.WriteLine(p.ToString("00") + " | " + i.ToString("00") + " | " + g.ToString("00") + " | " + d.ToString("00") + " | " + t.ToString("00") + " | " + h.ToString("00") + " | " + data.Intern[i].ShouldattendInGr_g[g].ToString("000") + " | " + "**");
											}
											else if (Intern_itdh[i][t][d][h])
											{
												tw.WriteLine(p.ToString("00") + " | " + i.ToString("00") + " | " + g.ToString("00") + " | " + d.ToString("00") + " | " + t.ToString("00") + " | " + h.ToString("00") + " | " + data.Intern[i].ShouldattendInGr_g[g].ToString("000") + " | ");

												for (int r = 0; r < data.General.Region; r++)
												{
													if (data.Intern[i].TransferredTo_r[r] && data.Hospital[h].InToRegion_r[r])
													{
														NotUsesAcc[r][t]++;
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
			for (int r = 0; r < data.General.Region; r++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					NotUsesAcc[r][t] -= data.Region[r].AvaAcc_t[t];
				}
			}
			for (int r = 0; r < data.General.Region; r++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					if (NotUsesAcc[r][t] < 0)
					{
						NotUsedAccTotal += -NotUsesAcc[r][t];
					}

				}
			}
			tw.WriteLine("PP II GG DD HH  (Fulfilled already and exist in the list)");
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int i = 0; i < data.General.Interns; i++)
				{
					for (int g = 0; g < data.General.DisciplineGr; g++)
					{
						if (data.Intern[i].ProgramID == p)
						{
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								if (data.Intern[i].DisciplineList_dg[d][g])
								{
									for (int h = 0; h < data.General.Hospitals; h++)
									{
										if (data.Intern[i].Fulfilled_dhp[d][h][p])
										{
											tw.WriteLine(p.ToString("00") + " | " + i.ToString("00") + " | " + g.ToString("00") + " | " + d.ToString("00") + " | " + h.ToString("00"));
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
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int t = 0; t < data.General.TimePriods; t++)
					{
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							if (Intern_itdh[i][t][d][h])
							{
								PrDisp_i[i] += data.Intern[i].Prf_d[d];
								PrHosp_i[i] += data.Intern[i].Prf_h[h];
								TrPrPrf[i] += data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d];
							}
						}
					}
				}
			}
			for (int i = 0; i < data.General.Interns; i++)
			{
				TotalDis[data.Intern[i].ProgramID] = PrDisp_i[i] + PrHosp_i[i];
			}
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int p = 0; p < data.General.TrainingPr; p++)
				{
					if (data.Intern[i].ProgramID == p)
					{
						int indext = 0;
						int sum = 0;
						int lastJob = 0;
						for (int t = 0; t < data.General.TimePriods; t++)
						{
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								for (int h = 0; h < data.General.Hospitals; h++)
								{
									if (Intern_itdh[i][t][d][h])
									{
										indext = t;
										sum += data.Discipline[d].Duration_p[p];
										lastJob = data.Discipline[d].Duration_p[p];
									}
								}
							}


						}

						PrWait_i[i] = indext + lastJob - sum;
					}
				}

			}

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
									if (Intern_itdh[i][t][d][h])
									{
										Assigned_twh[t][w][h]++;
									}
								}
							}
						}
					}
				}

			}
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					for (int h = 0; h < data.General.Hospitals; h++)
					{
						if (Assigned_twh[t][w][h] > data.Hospital[h].HospitalMaxDem_tw[t][w])
						{
							int diff = Assigned_twh[t][w][h] - data.Hospital[h].HospitalMaxDem_tw[t][w];
							if (diff <= data.Hospital[h].ReservedCap_tw[t][w])
							{
								ResDemand += diff;
							}
							else
							{
								ResDemand += data.Hospital[h].ReservedCap_tw[t][w];
								diff -= data.Hospital[h].ReservedCap_tw[t][w];
								EmrDemand += diff;
							}
						}
					}
				}
			}

			for (int i = 0; i < data.General.Interns; i++)
			{
				int IndH1 = -1;
				int IndH2 = -1;
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							if (Intern_itdh[i][t][d][h])
							{
								if (IndH1 < 0)
								{
									IndH1 = h;
								}
								else
								{
									IndH2 = h;
									if (IndH1 != IndH2)
									{
										PrChang_i[i]++;
									}
									IndH1 = IndH2;
									IndH2 = -1;
								}
							}
						}
					}
				}
			}

			for (int i = 0; i < data.General.Interns; i++)
			{
				Des_i[i] = data.Intern[i].wieght_ch * PrChang_i[i]
						 + data.Intern[i].wieght_w * PrWait_i[i]
						 + data.Intern[i].wieght_d * PrDisp_i[i]
						 + data.Intern[i].wieght_h * PrHosp_i[i]
						 + data.TrainingPr[data.Intern[i].ProgramID].weight_p * TrPrPrf[i];
				AveDes += Des_i[i];
				if (MinDis[data.Intern[i].ProgramID] == data.AlgSettings.BigM)
				{
					MinDis[data.Intern[i].ProgramID] = Des_i[i];
				}
				else
				{
					MinDis[data.Intern[i].ProgramID] = Des_i[i] < MinDis[data.Intern[i].ProgramID] ? Des_i[i] : MinDis[data.Intern[i].ProgramID];
				}
			}

			AveDes = AveDes / data.General.Interns;
			for (int i = 0; i < data.General.Interns; i++)
			{
				if (AveDes > Des_i[i])
				{
					dn_i[i] = AveDes - Des_i[i];
				}
				else
				{
					dp_i[i] = Des_i[i] - AveDes;
				}
				minimizedDev += (dn_i[i] - dp_i[i]);
			}
			tw.WriteLine("III PrD PrH PrW PrC PrP W_d W_h W_w W_c W_p DesI DNi DPi");
			for (int i = 0; i < data.General.Interns; i++)
			{
				tw.WriteLine(i.ToString("000") + " " + PrDisp_i[i].ToString("000")
					+ " " + PrHosp_i[i].ToString("000") + " " + PrWait_i[i].ToString("000")
					+ " " + PrChang_i[i].ToString("000") + " " + TrPrPrf[i].ToString("000") + " " + data.Intern[i].wieght_d.ToString("000")
					+ " " + data.Intern[i].wieght_h.ToString("000") + " " + data.Intern[i].wieght_w.ToString("00")
					+ " " + data.Intern[i].wieght_ch.ToString("00") + " " + data.TrainingPr[data.Intern[i].ProgramID].weight_p.ToString("000")
					+ " " + Des_i[i].ToString("0.000") + " " + dn_i[i].ToString("0.00") + " " + dp_i[i].ToString("0.00"));
			}

			tw.WriteLine("III DesI DesP");
			for (int i = 0; i < data.General.Interns; i++)
			{
				Obj += Des_i[i] * data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
				
				tw.WriteLine(i.ToString("000") + " " + Des_i[i].ToString("000")
				 + " " + MinDis[data.Intern[i].ProgramID].ToString("000"));
			}
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				Obj += MinDis[p] * data.TrainingPr[p].CoeffObj_MinDesi;
				Obj -= ResDemand * data.TrainingPr[p].CoeffObj_ResCap;
				Obj -= EmrDemand * data.TrainingPr[p].CoeffObj_EmrCap;
				Obj -= NotUsedAccTotal * data.TrainingPr[p].CoeffObj_NotUsedAcc;
			}
			tw.WriteLine("Res: " + ResDemand.ToString("000.0"));
			tw.WriteLine("Emr: " + EmrDemand.ToString("000.0"));
			tw.WriteLine("Acc: " + NotUsedAccTotal.ToString("000.0"));
			tw.WriteLine("Obj: " + Obj.ToString("000.0"));
			tw.Close();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataLayer
{
	public class OptimalSolution
	{
		public bool[][][][] Intern_itdh;
		public double[] PrDisp_i;
		public double[] PrHosp_i;
		public double[] PrWait_i;
		public double[] PrChang_i;
		public double[] Des_i;
		public double AveDes;
		public double[] dp_i;
		public double[] dn_i;
		public double minimizedDev;
		public double MinDis;

		public OptimalSolution(AllData data)
		{
			Initial(data);
		}
		public void Initial(AllData data)
		{
			new ArrayInitializer().CreateArray(ref Intern_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, false);
			new ArrayInitializer().CreateArray(ref PrDisp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrHosp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrWait_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref PrChang_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref Des_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref dp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref dn_i, data.General.Interns, 0);
			MinDis = 0;
			AveDes = 0;
			minimizedDev = 0;

		}

		public void WriteSolution(string Path, string Name, AllData data)
		{
			StreamWriter tw = new StreamWriter(Path + Name + "OptSol.txt");

			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				tw.WriteLine("Program " + p);
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					if (data.TrainingPr[p].PrManDis_d[d])
					{
						for (int i = 0; i < data.General.Interns; i++)
						{
							if (data.Intern[i].ProgramID == p)
							{
								bool Performed = false;
								for (int t = 0; t < data.General.TimePriods; t++)
								{
									for (int h = 0; h < data.General.Hospitals; h++)
									{
										if (Intern_itdh[i][t][d][h])
										{
											Performed = true;
											tw.WriteLine("Intern " + i.ToString("00") + " fulfilled mandatory discipline " + d.ToString("00") + " in hospital " + h.ToString("00") + " at time " + t.ToString("00"));
										}
									}
								}

								if (!Performed)
								{
									if (data.Intern[i].Abi_d[d])
									{
										tw.WriteLine("Intern " + i.ToString("00") + " did not fulfill mandatory discipline " + d.ToString("00"));
									}
									else
									{
										tw.WriteLine("Intern " + i.ToString("00") + " can not fulfill mandatory discipline " + d.ToString("00"));
									}

								}
							}
						}
					}
				}
			}

			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				tw.WriteLine("Program " + p + " total Arbitrary discipline " + data.TrainingPr[p].ArbitraryD);
				for (int i = 0; i < data.General.Interns; i++)
				{
					if (data.Intern[i].ProgramID == p)
					{
						for (int d = 0; d < data.General.Disciplines; d++)
						{
							if (!data.TrainingPr[p].PrManDis_d[d] && data.TrainingPr[p].Program_d[d])
							{
								bool Performed = false;
								for (int t = 0; t < data.General.TimePriods; t++)
								{
									for (int h = 0; h < data.General.Hospitals; h++)
									{
										if (Intern_itdh[i][t][d][h])
										{
											Performed = true;
											tw.WriteLine("Intern " + i.ToString("00") + " fulfilled arbitrary discipline " + d.ToString("00") + " in hospital " + h.ToString("00") + " at time " + t.ToString("00"));
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
							}
						}
					}
				}
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
				Des_i[i] = data.Intern[i].wieght_ch * PrChang_i[i] / data.PrChan_i[i]
					     + data.Intern[i].wieght_w * PrWait_i[i] / data.PrWait_i[i]
						 + data.Intern[i].wieght_d * PrDisp_i[i] / data.PrDisc_i[i]
						 + data.Intern[i].wieght_h * PrHosp_i[i] / data.PrHosp_i[i];
				AveDes += Des_i[i];
				if (i == 0)
				{
					MinDis = Des_i[i];
				}
				else
				{
					MinDis = Des_i[i] < MinDis ? Des_i[i] : MinDis;
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
					dp_i[i] =  Des_i[i] - AveDes;
				}
				minimizedDev += (dn_i[i] - dp_i[i]);
			}
			tw.WriteLine("III PrD PrH PrW PrC W_d W_h W_w W_c DesI DNi DPi");
			for (int i = 0; i < data.General.Interns; i++)
			{
				tw.WriteLine(i.ToString("000") + " " + PrDisp_i[i].ToString("000") 
					+ " " + PrHosp_i[i].ToString("000") + " " + PrWait_i[i].ToString("000")
					+ " " + PrChang_i[i].ToString("000") + " " + data.Intern[i].wieght_d.ToString("000")
					+ " " + data.Intern[i].wieght_h.ToString("000") + " " + data.Intern[i].wieght_w.ToString("00")
					+ " " + data.Intern[i].wieght_ch.ToString("00") + " " + Des_i[i].ToString("0.000") 
					+" " + dn_i[i].ToString("0.00") + " " + dp_i[i].ToString("0.00"));
			}
			tw.WriteLine("III PrD PrH PrW PrC MxD MxH MxW MxC WdWh MxWdWh");
			for (int i = 0; i < data.General.Interns; i++)
			{
				tw.WriteLine(i.ToString("000") + " " + PrDisp_i[i].ToString("000")
					+ " " + PrHosp_i[i].ToString("000") + " " + PrWait_i[i].ToString("000")
					+ " " + PrChang_i[i].ToString("000") + " " + data.Intern[i].MaxDesireDis.ToString("000")
					+ " " + data.Intern[i].MaxDesireHos.ToString("000") + " " + data.PrWait_i[i].ToString("000")
					+ " " + data.PrChan_i[i].ToString("000")
					+ " " + (PrDisp_i[i] * data.Intern[i].wieght_d + PrHosp_i[i] * data.Intern[i].wieght_h).ToString("0000")
					+ " " + (data.Intern[i].MaxDesireDis * data.Intern[i].wieght_d + data.Intern[i].MaxDesireHos * data.Intern[i].wieght_h).ToString("0000"));
			}
			tw.WriteLine("MinDes: " + MinDis.ToString("0.000"));
			tw.WriteLine("MinDev: " + minimizedDev.ToString("0.000"));
			tw.WriteLine("DsAvrg: " + AveDes.ToString("0.000"));
			tw.Close();
		}
	}
}

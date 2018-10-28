 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace DataLayer
{
	public class WriteInformation
	{
		public WriteInformation(string location, string name)
		{
			WriteInstances(location, name);
		}


		/// <summary>
		/// this function creates the instances and writes it in the given location
		/// </summary>
		public void WriteInstances(string location, string name)
		{
			Random random_ = new Random();

			// general Info
			GeneralInfo tmpGeneral = new GeneralInfo();
			tmpGeneral.Disciplines = random_.Next(3, 5);
			tmpGeneral.Hospitals = random_.Next(1, 3);
			tmpGeneral.Interns = random_.Next(3, 4);
			tmpGeneral.TrainingPr = random_.Next(1, 2);
			//time period 
			// 12 => 4week
			// 24 => 2week
			int[] tp_ = { 12, 24 };
			tmpGeneral.TimePriods = tp_[(int)random_.Next(0, tp_.Length)];

			string strline;
			int rnd, temp;

			TextWriter tw = new StreamWriter(location + name );

			strline = "// Interns Disciplines Hospitals TimePriods Programs";

			//write the general Data
			tw.WriteLine(strline);
			tw.WriteLine(tmpGeneral.Interns +" "+ tmpGeneral.Disciplines + " " + tmpGeneral.Hospitals + " " + tmpGeneral.TimePriods + " " + tmpGeneral.TrainingPr);
			tw.WriteLine();

			//Create and write training program info
			TrainingProgramInfo[] tmpTrainingPr = new TrainingProgramInfo[tmpGeneral.TrainingPr];
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				tmpTrainingPr[p] = new TrainingProgramInfo(tmpGeneral.Disciplines);
				tmpTrainingPr[p].MandatoryD = random_.Next(1, tmpGeneral.Disciplines);
				tmpTrainingPr[p].ArbitraryD = random_.Next(0, tmpGeneral.Disciplines - tmpTrainingPr[p].MandatoryD + 1);

				// assign the disciplines
				int maxdisp = Math.Min(tmpTrainingPr[p].MandatoryD + 2 * tmpTrainingPr[p].ArbitraryD, tmpGeneral.Disciplines);
				int[] xx = new int[maxdisp];
				for (int ii = 0; ii < maxdisp; ii++)
				{
					xx[ii] = ii;
				}
				int[] yy = new int[maxdisp];
				yy = xx.OrderBy(x => random_.Next()).ToArray();
				for (int d = 0; d < maxdisp; d++)
				{
					tmpTrainingPr[p].Program_d[yy[d]] = true;
				}
				// find mandatory disciplines
				for (int mn = 0; mn < tmpTrainingPr[p].MandatoryD ; mn++)
				{
					int disp = random_.Next(0,tmpGeneral.Disciplines);
					while (tmpTrainingPr[p].PrManDis_d[disp] || !tmpTrainingPr[p].Program_d[disp])
					{
						disp = random_.Next(0, tmpGeneral.Disciplines);
					}
					tmpTrainingPr[p].PrManDis_d[disp] = true;

				}

				tmpTrainingPr[p].Name = p.ToString();			

			}
			strline = "// PrName Mandatory Arbitrary [p]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				tw.WriteLine(tmpTrainingPr[p].Name + " " + tmpTrainingPr[p].MandatoryD + " " + tmpTrainingPr[p].ArbitraryD);
			}
			
			tw.WriteLine();

			strline = "// Involved disciplines [p][d]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					if (tmpTrainingPr[p].Program_d[d])
					{
						tw.Write(1 + " ");
					}
					else
					{
						tw.Write(0 + " ");
					}
				}
				tw.WriteLine();
			}
			
			tw.WriteLine();

			strline = "// Involved Mandatory disciplines [p][d]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					if (tmpTrainingPr[p].PrManDis_d[d])
					{
						tw.Write(1 + " ");
					}
					else
					{
						tw.Write(0 + " ");
					}
				}
				tw.WriteLine();
			}
			
			tw.WriteLine();

			// Create and write Hospital Info 
			HospitalInfo[] tmphospitalInfos = new HospitalInfo[tmpGeneral.Hospitals];
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				tmphospitalInfos[h] = new HospitalInfo(tmpGeneral.Disciplines, tmpGeneral.TimePriods);
				tmphospitalInfos[h].Name = h.ToString();
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					tmphospitalInfos[h].Hospital_d[d] = random_.Next(0, 3) > 0;
				}
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					if (tmphospitalInfos[h].Hospital_d[d])
					{
						int rate = tmpGeneral.Interns / tmpGeneral.Hospitals;
						int min = random_.Next(0, 4) > 2 ? 1 : 0;
						int max = random_.Next(1, rate+1);

						for (int t = 0; t < tmpGeneral.TimePriods; t++)
						{
							tmphospitalInfos[h].HospitalMinDem_td[t][d] = 0;
							tmphospitalInfos[h].HospitalMaxDem_td[t][d] = 3;
						}
						
					}
					else
					{
						int min = 0;
						int max = 0;
						for (int t = 0; t < tmpGeneral.TimePriods; t++)
						{
							tmphospitalInfos[h].HospitalMinDem_td[t][d] = min;
							tmphospitalInfos[h].HospitalMaxDem_td[t][d] = max;
						}
					}

				}
			}

			strline = "// Involved discipline Hospital [h][d]";
			tw.WriteLine(strline);
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					if (tmphospitalInfos[h].Hospital_d[d])
					{
						tw.Write(1 + " ");
					}
					else
					{
						tw.Write(0 + " ");
					}
				}
				tw.WriteLine();
			}
			
			tw.WriteLine();
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				strline = "// Max Demand for discipline in each time period [t][d] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmphospitalInfos[h].HospitalMaxDem_td[t][d] > 0)
						{
							tw.Write(tmphospitalInfos[h].HospitalMaxDem_td[t][d] + " ");
						}
						else
						{
							tw.Write(0 + " ");
						}
					}
					tw.WriteLine();
				}
			}
			
			tw.WriteLine();
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				strline = "// Min Demand for discipline in each time period [t][d] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmphospitalInfos[h].HospitalMinDem_td[t][d] > 0)
						{
							tw.Write(tmphospitalInfos[h].HospitalMinDem_td[t][d] + " ");
						}
						else
						{
							tw.Write(0 + " ");
						}
					}
					tw.WriteLine();
				}
			}
			
			tw.WriteLine();

			// Create and write Discipline Info
			DisciplineInfo[] tmpdisciplineInfos = new DisciplineInfo[tmpGeneral.Disciplines];
			for (int d = 0; d < tmpGeneral.Disciplines; d++)
			{
				tmpdisciplineInfos[d] = new DisciplineInfo(tmpGeneral.Disciplines,tmpGeneral.TrainingPr);
				tmpdisciplineInfos[d].Name = d.ToString();
				for (int p = 0; p < tmpGeneral.TrainingPr; p++)
				{
					if (tmpTrainingPr[p].Program_d[d])
					{
						tmpdisciplineInfos[d].Duration_p[p] = 1;
					}					
				}
				for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
				{
					for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
					{
						if (tmpTrainingPr[pp].Program_d[dd] && tmpTrainingPr[pp].Program_d[d])
						{
							tmpdisciplineInfos[d].Skill4D_dp[dd][pp] = random_.Next(1, 4) > 3; 
						}
					}
				}
			}
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				double totalLeft = (double)tmpGeneral.TimePriods /( tmpTrainingPr[p].MandatoryD + tmpTrainingPr[p].ArbitraryD);

				for (int ii = 0; ii < totalLeft; ii++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						tmpdisciplineInfos[d].Duration_p[p] += random_.Next(0,2);
					}					
				}
			}
			strline = "// Discipline duration different in Program [d][p]";
			tw.WriteLine(strline);
			for (int d = 0; d < tmpGeneral.Disciplines; d++)
			{
				for (int p = 0; p < tmpGeneral.TrainingPr; p++)
				{
					tw.Write(tmpdisciplineInfos[d].Duration_p[p] + " ");
				}
				tw.WriteLine();
			}
			tw.WriteLine();
			
			
			for (int d = 0; d < tmpGeneral.Disciplines; d++)
			{
				strline = "// required skill[d'][p] for discipline " + d;
			    tw.WriteLine(strline);
				for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
				{
					for (int p = 0; p < tmpGeneral.TrainingPr; p++)
					{
						if (tmpdisciplineInfos[d].Skill4D_dp[dd][p])
						{
							tw.Write(1+" ");
						}
						else
						{
							tw.Write(0+" ");
						}
					}
					tw.WriteLine(); 
				}
				
				tw.WriteLine();
			}
			tw.WriteLine();

			// create and write Intern info
			InternInfo[] tmpinternInfos = new InternInfo[tmpGeneral.Interns]; 
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				tmpinternInfos[i] = new InternInfo(tmpGeneral.Hospitals,tmpGeneral.Disciplines,tmpGeneral.TimePriods);
				tmpinternInfos[i].ProgramID = random_.Next(0,tmpGeneral.TrainingPr-1);
				int totalTime = 0;
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					tmpinternInfos[i].Abi_d[d] = true;
					tmpinternInfos[i].Prf_d[d] = random_.Next(1, 3);
					totalTime += tmpdisciplineInfos[d].Duration_p[tmpinternInfos[i].ProgramID];
				}
				totalTime = tmpGeneral.TimePriods - totalTime;
				for (int t = 0; t < totalTime; t++)
				{
					int tIndx = random_.Next(0,tmpGeneral.TimePriods-1);
					tmpinternInfos[i].Ave_t[tIndx] = random_.Next(0, 2) > 1;
				}
				for (int h = 0; h < tmpGeneral.Hospitals; h++)
				{
					tmpinternInfos[i].Prf_h[h] = random_.Next(1,3);
				}
				tmpinternInfos[i].wieght_ch = -random_.Next(1,4);
				tmpinternInfos[i].wieght_d = random_.Next(1, 4);
				tmpinternInfos[i].wieght_h = random_.Next(1, 4);
				tmpinternInfos[i].wieght_w = -random_.Next(1, 4);
				}
			strline = "// InternName Program W_disc W_hosp W_change W_wait  [i][w]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				tw.WriteLine(i + " " + tmpinternInfos[i].ProgramID + " " + tmpinternInfos[i].wieght_d + " " + tmpinternInfos[i].wieght_h + " " + tmpinternInfos[i].wieght_ch + " " + tmpinternInfos[i].wieght_w);

			}
			
			tw.WriteLine();

			strline = "// Ability for discipline [i][d]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					if (tmpinternInfos[i].Abi_d[d])
					{
						tw.Write(1+" ");
					}
					else
					{
						tw.Write(0+" ");
					}
				}
				tw.WriteLine();
			}
			tw.WriteLine();

			strline = "// Preference for discipline [i][d]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
						tw.Write(tmpinternInfos[i].Prf_d[d]+ " ");
				
					
				}
				tw.WriteLine();
			}
			tw.WriteLine();

			strline = "// Preference for hospital [i][h]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Hospitals; d++)
				{
					tw.Write(tmpinternInfos[i].Prf_h[d] + " ");

				}
				tw.WriteLine();
			}
			tw.WriteLine();

			strline = "// Availability [i][t]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.TimePriods; d++)
				{
					if (tmpinternInfos[i].Ave_t[d])
					{
						tw.Write(1+" ");
					}
					else
					{
						tw.Write(0 + " ");
					}

				}
				tw.WriteLine();
			}
			tw.WriteLine();

			//Algorithm Settings
			AlgorithmSettings tmpalgorithmSettings = new AlgorithmSettings();
			tmpalgorithmSettings.BPTime = 7200;
			tmpalgorithmSettings.MasterTime = 100;
			tmpalgorithmSettings.MIPTime = 7200;
			tmpalgorithmSettings.RCepsi = 0.000001;
			tmpalgorithmSettings.RHSepsi = 0.000001;
			tmpalgorithmSettings.SubTime = 600;
			tmpalgorithmSettings.NodeTime = 3600;
			strline = "// Algorithm Settings MIPtime BPtime Nodetime Mastertime Subtime RCepsi RCepsi";
			tw.WriteLine(strline);
			tw.WriteLine(tmpalgorithmSettings.MIPTime +" "+ tmpalgorithmSettings.BPTime + " " + tmpalgorithmSettings.NodeTime + " " + tmpalgorithmSettings.MasterTime + " " + tmpalgorithmSettings.SubTime + " " + tmpalgorithmSettings.RHSepsi + " " + tmpalgorithmSettings.RCepsi);

			tw.Close();
		}
	}
}

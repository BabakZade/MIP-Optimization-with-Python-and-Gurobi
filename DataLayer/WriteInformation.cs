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
			
			tmpGeneral.Hospitals = random_.Next(5, 5);
			tmpGeneral.Interns = random_.Next(10, 10);
			tmpGeneral.TrainingPr = random_.Next(1, 3);
			tmpGeneral.Disciplines = tmpGeneral.TrainingPr * random_.Next(5, 10);
			tmpGeneral.DisciplineGr = random_.Next(2, 5) ;
			tmpGeneral.HospitalWard = random_.Next(5, tmpGeneral.Disciplines + 1);
			tmpGeneral.Region = 1;
			tmpGeneral.TrainingPr = 2;
			tmpGeneral.DisciplineGr = 4;
			//[groupName pr1 pr2 0 0]
			double[,] prValue = new double[5, 5] {{ 0, 0 , 0 , 0 , 0},
												  { 1, 1 , 0 , 0 , 0},
												  { 2, 0.5 , 0.5 , 0 , 0},
												  { 3, 0.2 , 0.3 , 0.5 , 0},
												  { 4, 0.1 , 0.2 , 0.3 , 0.4}};
			double[] prValueG = new double[tmpGeneral.DisciplineGr];
			for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
			{
				prValueG[g] = prValue[tmpGeneral.DisciplineGr,g+1];
			}
			//time period 
			// 12 => 4week
			// 24 => 2week
			int[] tp_ = { 12, 24 };
			tmpGeneral.TimePriods = tp_[(int)random_.Next(0, tp_.Length)];

			string strline;
			int rnd, temp;

			TextWriter tw = new StreamWriter(location + name );

			strline = "// Interns Disciplines Hospitals TimePriods TrPrograms HospitalWards DisciplineGr Region";

			//write the general Data
			tw.WriteLine(strline);
			tw.WriteLine(tmpGeneral.Interns +" "+ tmpGeneral.Disciplines 
						+ " " + tmpGeneral.Hospitals + " " + tmpGeneral.TimePriods 
						+ " " + tmpGeneral.TrainingPr + " " + tmpGeneral.HospitalWard
						+ " " + tmpGeneral.DisciplineGr + " " + tmpGeneral.Region);
			tw.WriteLine();

			//Create and write training program info
			TrainingProgramInfo[] tmpTrainingPr = new TrainingProgramInfo[tmpGeneral.TrainingPr];
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				tmpTrainingPr[p] = new TrainingProgramInfo(tmpGeneral.Disciplines, tmpGeneral.DisciplineGr);
				tmpTrainingPr[p].Name = (p+1).ToString()+"_Year";
				tmpTrainingPr[p].AcademicYear = p + 1;

				// assign the preference for disciplines
				int maxdisp = tmpGeneral.Disciplines;
				int[] xx = new int[maxdisp];
				for (int ii = 0; ii < maxdisp; ii++)
				{
					xx[ii] = ii;
				}
				int[] yy = new int[maxdisp];
				yy = xx.OrderBy(x => random_.Next()).ToArray();
				
				int init = 0;
				double cumolative = p;
				for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
				{	
					init = (int)Math.Ceiling( cumolative * tmpGeneral.Disciplines / tmpGeneral.TrainingPr);
					cumolative += prValueG[g];
					for (int d = init ;  d < Math.Ceiling( cumolative * tmpGeneral.Disciplines / tmpGeneral.TrainingPr); d++)
					{
						tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
						tmpTrainingPr[p].Prf_d[d] = random_.Next(1,4);
					}
				}
				
				// assign the weight for preference
				tmpTrainingPr[p].weight_p = random_.Next(1,4);
				// assign the coefficient 
				tmpTrainingPr[p].CoeffObj_SumDesi = random_.Next(1, 5);
				tmpTrainingPr[p].CoeffObj_MinDesi = random_.Next(1, 5);
				tmpTrainingPr[p].CoeffObj_ResCap = random_.Next(1, 5);
				tmpTrainingPr[p].CoeffObj_EmrCap = random_.Next(1, 5);
				tmpTrainingPr[p].CoeffObj_NotUsedAcc = random_.Next(1, 5);

				tmpTrainingPr[p].DiscChangeInOneHosp = random_.Next(1, tmpGeneral.Disciplines);

			}


			strline = "// PrName Year WeightPrf Obj_SumDesi Obj_MinDesi Obj_ResCap Obj_EmrCap Obj_NUsedAcc AllowedChange [p]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				tw.WriteLine(tmpTrainingPr[p].Name + " " + tmpTrainingPr[p].AcademicYear 
					         + " " + tmpTrainingPr[p].weight_p + " " + tmpTrainingPr[p].CoeffObj_SumDesi
							 + " " + tmpTrainingPr[p].CoeffObj_MinDesi + " " + tmpTrainingPr[p].CoeffObj_ResCap
							 + " " + tmpTrainingPr[p].CoeffObj_EmrCap + " " + tmpTrainingPr[p].CoeffObj_NotUsedAcc + " " + tmpTrainingPr[p].DiscChangeInOneHosp);
			}
			
			tw.WriteLine();

			strline = "// Preference for discipline [p][d]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					tw.Write(tmpTrainingPr[p].Prf_d[d] + " ");
				}
				tw.WriteLine();
			}
			tw.WriteLine();

			strline = "// Involved discipline  [p][g][d]";
			tw.WriteLine(strline);
			for (int p = 0; p < tmpGeneral.TrainingPr; p++)
			{
				strline = "// Program  " + p;
				tw.WriteLine(strline);
				for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						tw.Write( (tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] == true ? 1 : 0).ToString() + " ");
					}
					tw.WriteLine();
				}
				
			}
			tw.WriteLine();


			// Create and write Hospital Info 
			HospitalInfo[] tmphospitalInfos = new HospitalInfo[tmpGeneral.Hospitals];
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				tmphospitalInfos[h] = new HospitalInfo(tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.HospitalWard, tmpGeneral.Region);
				tmphospitalInfos[h].Name = h.ToString();

				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					int w = d;
					if (w > tmpGeneral.HospitalWard - 1)
					{
						w -= tmpGeneral.HospitalWard;
					}
					tmphospitalInfos[h].Hospital_dw[d][w] = random_.Next(0, 4) > 0;
				}

				for (int w = 0; w < tmpGeneral.HospitalWard; w++)
				{
					bool haveW = false;
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmphospitalInfos[h].Hospital_dw[d][w])
						{
							haveW = true;
							break;
						}
					}
					// if the hospital has the ward has demand + reserved and emergency, otherwise 0
					if (haveW)
					{
						int rate = tmpGeneral.Interns / tmpGeneral.Hospitals;
						int min = random_.Next(0, 5) > 3 ? 1 : 0;
						int max = random_.Next(min + 1, rate + 1);
						for (int t = 0; t < tmpGeneral.TimePriods; t++)
						{
							tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
							tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
							tmphospitalInfos[h].EmergencyCap_tw[t][w] = 1;
							tmphospitalInfos[h].ReservedCap_tw[t][w] = 2;
						}
						
					}
					else
					{
						int min = 0;
						int max = 0;
						for (int t = 0; t < tmpGeneral.TimePriods; t++)
						{
							tmphospitalInfos[h].HospitalMinDem_tw[t][w] = min;
							tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = max;
						}
						
					}
				}
				// if the hospital is in the region
				for (int r = 0; r < tmpGeneral.Region; r++)
				{
					tmphospitalInfos[h].InToRegion_r[r] = random_.Next(0, 4) > 2;
				}
			}

			strline = "// Involved discipline in ward in hospital [h]  =>[w][d]";
			tw.WriteLine(strline);
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				tw.WriteLine("Hospital" + h);
				for (int w = 0; w < tmpGeneral.HospitalWard; w++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmphospitalInfos[h].Hospital_dw[d][w])
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
				
				
			}
			
			tw.WriteLine();
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				strline = "// Max Demand for ward in each time period [t][w] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.HospitalWard; d++)
					{
						tw.Write(tmphospitalInfos[h].HospitalMaxDem_tw[t][d] + " ");
					}
					tw.WriteLine();
				}
			}

			tw.WriteLine();
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				strline = "// Min Demand for ward in each time period [t][w] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.HospitalWard; d++)
					{
						if (tmphospitalInfos[h].HospitalMinDem_tw[t][d] > 0)
						{
							tw.Write(tmphospitalInfos[h].HospitalMinDem_tw[t][d] + " ");
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
				strline = "// Reserved Demand for ward in each time period [t][w] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.HospitalWard; d++)
					{
						if (tmphospitalInfos[h].ReservedCap_tw[t][d] > 0)
						{
							tw.Write(tmphospitalInfos[h].ReservedCap_tw[t][d] + " ");
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
				strline = "// Emergency Demand for ward in each time period [t][w] in Hospital" + h;
				tw.WriteLine(strline);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					for (int d = 0; d < tmpGeneral.HospitalWard; d++)
					{
						if (tmphospitalInfos[h].EmergencyCap_tw[t][d] > 0)
						{
							tw.Write(tmphospitalInfos[h].EmergencyCap_tw[t][d] + " ");
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
			strline = "// located in regions [h][r]";
			tw.WriteLine(strline);
			for (int h = 0; h < tmpGeneral.Hospitals; h++)
			{
				for (int t = 0; t < tmpGeneral.Region; t++)
				{
						if (tmphospitalInfos[h].InToRegion_r[t])
						{
							tw.Write(1 + " ");
						}
						else
						{
							tw.Write(0 + " ");
						}
					
					tw.WriteLine();
				}
			}

			tw.WriteLine();

			// Create and write Discipline Info
			DisciplineInfo[] tmpdisciplineInfos = new DisciplineInfo[tmpGeneral.Disciplines];
			for (int d = 0; d < tmpGeneral.Disciplines; d++)
			{
				tmpdisciplineInfos[d] = new DisciplineInfo(tmpGeneral.Disciplines, tmpGeneral.TrainingPr);
				tmpdisciplineInfos[d].Name = d.ToString();
				for (int p = 0; p < tmpGeneral.TrainingPr; p++)
				{
						tmpdisciplineInfos[d].Duration_p[p] = random_.Next(0,4) > 2 ? 2 : 1;
					
				}

				
			}
			for (int d = 0; d < tmpGeneral.Disciplines; d++)
			{
				for (int pp = 0; pp < tmpGeneral.TrainingPr; pp++)
				{
					for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
					{
						bool dInv = false;
						bool ddInv = false;
						for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
						{
							if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][d])
							{
								dInv = true;
							}
							if (tmpTrainingPr[pp].InvolvedDiscipline_gd[g][dd])
							{
								ddInv = true;
							}
						}
						if (random_.Next(0, 8) == 7 && !tmpdisciplineInfos[dd].Skill4D_dp[d][pp] && dd != d && dInv && ddInv)
						{
							tmpdisciplineInfos[d].Skill4D_dp[dd][pp] = true;
							// if you only want the discipline d connects to only one other discipline 
							// otherwise clean the break
							break;
						}

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

				tmpinternInfos[i] = new InternInfo(tmpGeneral.Hospitals, tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.DisciplineGr, tmpGeneral.TrainingPr, tmpGeneral.Region);
				tmpinternInfos[i].ProgramID = random_.Next(0, tmpGeneral.TrainingPr);
				int totalTime = 0;
				int totalDisc = 0;
				// hand out group and discipline
				for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
				{
					int total = 0;
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						tmpinternInfos[i].DisciplineList_dg[d][g] = tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d];
						if (tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d])
						{
							total++;
						}
						
					}
					tmpinternInfos[i].ShouldattendInGr_g[g] = random_.Next(1, total + 1);
					totalDisc += tmpinternInfos[i].ShouldattendInGr_g[g];
				}

				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					for (int h = 0; h < tmpGeneral.Hospitals; h++)
					{
						tmpinternInfos[i].Abi_dh[d][h] = true;
					}
					tmpinternInfos[i].Prf_d[d] = random_.Next(1, 3);
					totalTime += tmpdisciplineInfos[d].Duration_p[tmpinternInfos[i].ProgramID];
				}
				// fulfilled disciplines happens for 10-20 percent of students
				double ffPrcntStudent = 0.2;
				if (random_.NextDouble() < ffPrcntStudent)
				{
					// 10 percent of discipline may fulfilled before
					double ffPrcntDiscipline = 0.1;
					int totalFulfilled = random_.Next(0, (int)Math.Round(ffPrcntDiscipline * totalDisc + 1, 0));

					for (int ff = 0; ff < totalFulfilled; ff++)
					{
						int dIndex = random_.Next(0, tmpGeneral.Disciplines);
						int Gr = -1;
						int Pr = random_.Next(0, 5) < 4 ? tmpinternInfos[i].ProgramID : random_.Next(0, tmpGeneral.TrainingPr);
						int Hosp = -1;
						while (true)
						{
							for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
							{
								if (tmpinternInfos[i].DisciplineList_dg[dIndex][g])
								{
									Gr = g;
									for (int h = 0; h < tmpGeneral.Hospitals; h++)
									{
										for (int d = 0; d < tmpGeneral.HospitalWard; d++)
										{
											if (tmphospitalInfos[h].Hospital_dw[dIndex][d])
											{
												Hosp = h;
											}
										}

									}
								}
							}
							if (Gr >= 0 && Hosp >= 0)
							{
								break;
							}
							else
							{
								dIndex = random_.Next(0, tmpGeneral.Disciplines);
							}

						}

						tmpinternInfos[i].Fulfilled_dhp[dIndex][Hosp][Pr] = true;

					}
				}

				// oversea disciplines happens for 10-20 percent of students
				double fhPrcntStudent = 0.1;
				if (random_.NextDouble() < fhPrcntStudent)
				{
					// only one discipline out side of country					
					int totalFulfilled = 1;

					for (int ff = 0; ff < totalFulfilled; ff++)
					{
						int dIndex = random_.Next(0, tmpGeneral.Disciplines);
						int Gr = -1;
						int Pr = random_.Next(0, 5) < 4 ? tmpinternInfos[i].ProgramID : random_.Next(0, tmpGeneral.TrainingPr);
						// it comes after half time
						int timeInd = random_.Next((int)(0.4 * tmpGeneral.TimePriods), (int)(0.7 * tmpGeneral.TimePriods));
						while (true)
						{
							for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
							{
								if (tmpinternInfos[i].DisciplineList_dg[dIndex][g])
								{
									Gr = g;
								}
							}
							if (Gr >= 0)
							{
								break;
							}
							else
							{
								dIndex = random_.Next(0, tmpGeneral.Disciplines);
							}

						}
						// Requirement for Foreign Hospital
						int disReq = random_.Next(0, tmpGeneral.Disciplines);
						while (true)
						{
							for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
							{
								if (tmpinternInfos[i].DisciplineList_dg[disReq][g])
								{
									Gr = g;
								}
							}
							if (Gr >= 0 && disReq != dIndex)
							{
								break;
							}
							else
							{
								disReq = random_.Next(0, tmpGeneral.Disciplines);
							}
						}
						tmpinternInfos[i].FHRequirment_d[disReq] = true;
						tmpinternInfos[i].OverSea_dt[dIndex][timeInd] = true;

					}
				}

				// region 
				for (int r = 0; r < tmpGeneral.Region; r++)
				{
					tmpinternInfos[i].TransferredTo_r[r] = random_.NextDouble() < 0.4;
				}
				totalTime = tmpGeneral.TimePriods - totalTime;
				if (totalTime < 0)
				{
					totalTime = 0;
				}
				totalTime = random_.Next(0, totalTime);
				int startInd = random_.Next(0, tmpGeneral.TimePriods);
				for (int t = startInd; t < totalTime && t < tmpGeneral.TimePriods; t++)
				{
					tmpinternInfos[i].Ave_t[t] = false;
				}
				for (int h = 0; h < tmpGeneral.Hospitals; h++)
				{
					tmpinternInfos[i].Prf_h[h] = random_.Next(1, 3);
				}
				tmpinternInfos[i].wieght_ch = -random_.Next(1, 4);
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

			strline = "// Discipline list";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				strline = "// Intern "+i+": [g][d]";
				tw.WriteLine(strline);
				for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmpinternInfos[i].DisciplineList_dg[d][g])
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

			}
			tw.WriteLine();

			strline = "// total discipline should be fulfilled from each group [i][g]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.DisciplineGr; d++)
				{
					tw.Write(tmpinternInfos[i].ShouldattendInGr_g[d] + " ");
				}
				tw.WriteLine();
			}
			tw.WriteLine();

			strline = "// Oversea request i - d - t - RequiredDiscipline";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					for (int t = 0; t < tmpGeneral.TimePriods; t++)
					{
						if (tmpinternInfos[i].OverSea_dt[d][t])
						{
							tw.WriteLine( i + " " + d+ " " + t);
							
						}
						
					}					
				}
				
			}
			tw.WriteLine("---");

			strline = "// Oversea Discipline requirement  [i oversea] [d1 d2 d3]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				bool OVERSEA = false;
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					for (int t = 0; t < tmpGeneral.TimePriods; t++)
					{
						if (tmpinternInfos[i].OverSea_dt[d][t])
						{
							OVERSEA = true;							
						}

					}
				}
				for (int d = 0; d < tmpGeneral.Disciplines && OVERSEA; d++)
				{
					if (tmpinternInfos[i].FHRequirment_d[d])
					{
						tw.Write(1 + " ");
					}
					else
					{
						tw.Write(0 + " ");
					}
				}
				if (OVERSEA)
				{
					tw.WriteLine();
				}
			}
			tw.WriteLine();

			strline = "// Transfered to region [i][r]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Region; d++)
				{
					if (tmpinternInfos[i].TransferredTo_r[d])
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


			strline = "// Ability";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				strline = "// Ability "+i+" [d][h]";
				tw.WriteLine(strline);
				for (int h = 0; h < tmpGeneral.Hospitals; h++)
				{
					for (int d = 0; d < tmpGeneral.Disciplines; d++)
					{
						if (tmpinternInfos[i].Abi_dh[d][h])
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

			}
			tw.WriteLine();

			strline = "// Fulfilled disciplines i-d-h-p";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Interns; i++)
			{
				for (int d = 0; d < tmpGeneral.Disciplines; d++)
				{
					for (int h = 0; h < tmpGeneral.Hospitals; h++)
					{
						for (int p = 0; p < tmpGeneral.TrainingPr; p++)
						{
							if (tmpinternInfos[i].Fulfilled_dhp[d][h][p])
							{
								tw.WriteLine(i + " " + d + " " + h + " " + p);
							}
						}
						
					}
				}
			}
			tw.WriteLine("---");

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


			// create and write Intern info
			RegionInfo[] tmpRegionInfos = new RegionInfo[tmpGeneral.Region];
			for (int r = 0; r < tmpGeneral.Region; r++)
			{
				tmpRegionInfos[r] = new RegionInfo(tmpGeneral.TimePriods);
				tmpRegionInfos[r].Name = "Reg_" + r;
				tmpRegionInfos[r].SQLID = r;
				int MIN = random_.Next(0, 2);
				for (int t = 0; t < tmpGeneral.TimePriods; t++)
				{
					tmpRegionInfos[r].AvaAcc_t[t] = MIN;
				}
			}

			strline = "// RegionNme SqlID";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Region; i++)
			{
				tw.WriteLine(tmpRegionInfos[i].Name + " " + tmpRegionInfos[i].SQLID);
			}
			tw.WriteLine();

			strline = "// Region Accommodation [r][t]";
			tw.WriteLine(strline);
			for (int i = 0; i < tmpGeneral.Region; i++)
			{
				for (int d = 0; d < tmpGeneral.TimePriods; d++)
				{
					tw.Write(tmpRegionInfos[i].AvaAcc_t[d] + " ");
					

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
			tmpalgorithmSettings.BigM = 4000;
			strline = "// Algorithm Settings MIPtime BPtime Nodetime Mastertime Subtime RCepsi RCepsi BigM";
			tw.WriteLine(strline);
			tw.WriteLine(tmpalgorithmSettings.MIPTime +" "+ tmpalgorithmSettings.BPTime + " " + tmpalgorithmSettings.NodeTime + " " + tmpalgorithmSettings.MasterTime + " " + tmpalgorithmSettings.SubTime + " " + tmpalgorithmSettings.RHSepsi + " " + tmpalgorithmSettings.RCepsi+ " " + tmpalgorithmSettings.BigM);
			tw.WriteLine();
			

			tw.Close();
		}
	}
}

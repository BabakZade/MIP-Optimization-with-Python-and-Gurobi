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
		public int SlackDem;
		public double[][] NotUsesAcc;
		public double[] TotalDis;
		public double NotUsedAccTotal;
		public double Obj;
		public double[] Obj_i;
		public double wieghtedSumInDisPrf;
		public double wieghtedSumPrDisPrf;
		public double wieghtedSumInHosPrf;
		public double wieghtedSumInChnPrf;
		public double wieghtedSumInWaiPrf;
		public double MaxDisier;
		public double MaxEmr;
		public double MaxRes;
		public double MaxMin;
		public double MaxAcc;


		// in-feasibility signs 
		bool infeasibilityK_Assigned;
        bool infeasibilityYearlyDemand;
		bool[] infeasibilityK_Assigned_g;
		bool infeasibilityChangesInHospital;
		bool infeasibilityOverseaAbilityAve;
		bool infeasibilitySkill;
		bool infeasibilityKdiscOneHosp;
		public bool IsFeasible;
		public bool[] infeasibleIntern_i;
		public bool[][] overLap_dd;
		public int[][][] overused_hwt;
        public int[][] YearlyCap_wh;
		public bool infeasibilityOverused;
		public bool infeasibilityOverlap;
        public int[] totalK_g;
        public bool[][] internDiscStatus_id;
        public bool[][] internAvaStatus_it;
        public int[][] internGrK_ig;

        public OptimalSolution(AllData data)
		{
			Initial(data);
			this.data = data;
		}
		public void CleanInternRoster(int theI)
		{
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int h = 0; h < data.General.Hospitals + 1; h++)
					{
							Intern_itdh[theI][t][d][h] = false;					
					}
				}
			}

			new ArrayInitializer().CreateArray(ref PrDisp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref Obj_i, data.General.Interns, 0);
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
			SlackDem = 0;

		}

		public void copyRosters(bool[][][][] Copy_itdh)
		{
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals+1; h++)
						{
							Intern_itdh[i][t][d][h] = Copy_itdh[i][t][d][h];
						}
					}
				}
			}
		}

		public void addRosters(bool[][][][] Copy_itdh)
		{
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals +1; h++)
						{
							if (Copy_itdh[i][t][d][h])
							{
								Intern_itdh[i][t][d][h] = Copy_itdh[i][t][d][h];
							}							
						}
					}
				}
			}

			new ArrayInitializer().CreateArray(ref PrDisp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref Obj_i, data.General.Interns, 0);
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
			SlackDem = 0;

			
		}

		public void Initial(AllData data)
		{
			new ArrayInitializer().CreateArray(ref Intern_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals + 1, false); // one for oversea hospital
			new ArrayInitializer().CreateArray(ref PrDisp_i, data.General.Interns, 0);
			new ArrayInitializer().CreateArray(ref Obj_i, data.General.Interns, 0);
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
			SlackDem = 0;

			
		}
		public void resetInf(AllData data)
		{
			infeasibilityK_Assigned = false;
			new ArrayInitializer().CreateArray(ref infeasibilityK_Assigned_g, data.General.DisciplineGr, false);
			new ArrayInitializer().CreateArray(ref overLap_dd, data.General.Disciplines, data.General.Disciplines, false);
            new ArrayInitializer().CreateArray(ref internDiscStatus_id, data.General.Interns, data.General.Disciplines, false);
            new ArrayInitializer().CreateArray(ref internAvaStatus_it, data.General.Interns, data.General.TimePriods, false);
            new ArrayInitializer().CreateArray(ref internGrK_ig, data.General.Interns, data.General.DisciplineGr, 0);
            new ArrayInitializer().CreateArray(ref YearlyCap_wh, data.General.HospitalWard, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref overused_hwt, data.General.Hospitals, data.General.Disciplines, data.General.TimePriods, 0);
			infeasibilityChangesInHospital = false;
			infeasibilityOverseaAbilityAve = false;
			infeasibilitySkill = false;
            infeasibilityYearlyDemand = false;
		}
		public string setInfSetting()
		{
			string  Result = "";
			resetInf(data);
			infeasibleIntern_i = new bool[data.General.Interns];

			for (int i = 0; i < data.General.Interns; i++)
			{
				infeasibleIntern_i[i] = false;
				int totalK = 0;
				totalK_g = new int[data.General.DisciplineGr];
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					totalK_g[g] = 0;
				}
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					bool CheckedDis = false;
					for (int t = 0; t < data.General.TimePriods && !CheckedDis; t++)
					{
						for (int g = 0; g < data.General.DisciplineGr && !CheckedDis; g++)
						{

							if (data.Intern[i].DisciplineList_dg[d][g])
							{
								for (int h = 0; h < data.General.Hospitals + 1 && !CheckedDis; h++)
								{
									if (Intern_itdh[i][t][d][h])
									{
										totalK += data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
										totalK_g[g] += data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
										if (h < data.General.Hospitals)
										{
											wieghtedSumInHosPrf += data.Intern[i].wieght_d * data.Intern[i].Prf_h[h];
										}
										wieghtedSumInDisPrf += data.Intern[i].wieght_d * data.Intern[i].Prf_d[d];
										wieghtedSumPrDisPrf += data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d];										
										wieghtedSumInChnPrf = 0;
										wieghtedSumInWaiPrf = 0;
										if (data.Intern[i].OverSea_dt[d][t] && h != data.General.Hospitals)
										{
											infeasibilityOverseaAbilityAve = true;
											infeasibleIntern_i[i] = true;
											Result += "The intern " + i + " requested oversea at time " + t + " for discipline " + d + " but (s)he didnot get it \n";
											if (data.Intern[i].Abi_dh[d][h])
											{
												infeasibilityOverseaAbilityAve = true;
												Result += "The intern " + i + " does not has ability for hospital " + h + " for discipline " + d + " \n";
											}
											if (!data.Intern[i].Ave_t[t])
											{
												infeasibilityOverseaAbilityAve = true;
												Result += "The intern " + i + " is not available at time " + t + " \n";
											}
										}
										if (data.Intern[i].OverSea_dt[d][t] && h == data.General.Hospitals)
										{
											for (int dd = 0; dd < data.General.Disciplines; dd++)
											{
												if (data.Intern[i].FHRequirment_d[dd])
												{
													bool thereis = false;
													for (int tt = 0; tt < t && !thereis; tt++)
													{
														for (int hh = 0; hh < data.General.Hospitals && !thereis; hh++)
														{
															if (Intern_itdh[i][tt][dd][hh])
															{
																thereis = true;
															}
														}
													}
													if (!thereis)
													{
														infeasibilitySkill = true;
														Result += "The intern " + i + " must fulfill discipline  " + dd + " before going abroad for discipline " + d + " \n";
														infeasibleIntern_i[i] = true;
													}
												}

											}
										}
										if (data.Discipline[d].requiresSkill_p[data.Intern[i].ProgramID])
										{
											for (int dd = 0; dd < data.General.Disciplines; dd++)
											{
												if (data.Discipline[d].Skill4D_dp[dd][data.Intern[i].ProgramID])
												{
													bool thereis = false;
													for (int tt = 0; tt < t && !thereis; tt++)
													{
														for (int hh = 0; hh < data.General.Hospitals + 1 && !thereis; hh++)
														{
															if (Intern_itdh[i][tt][dd][hh])
															{
																thereis = true;
															}
														}
													}
													if (!thereis)
													{
														infeasibilitySkill = true;
														Result += "The intern " + i + " must fulfill discipline  " + dd + " before discipline " + d + " \n";
														infeasibleIntern_i[i] = true;
													}
												}

											}
										}

										CheckedDis = true;
									}
								}
							}
						}


					}
				}
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int h = 0; h < data.General.Hospitals; h++)
					{
						if (data.Intern[i].Fulfilled_dhp[d][h][data.Intern[i].ProgramID])
						{
							totalK -= data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
							for (int g = 0; g < data.General.DisciplineGr; g++)
							{
								if (data.Intern[i].DisciplineList_dg[d][g])
								{
									totalK_g[g] -= data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
									break;
								}
							}

							break;
						}
					}
				}
				if (totalK != data.Intern[i].K_AllDiscipline)
				{
					Result += "The intern " + i + " should fulfill " + data.Intern[i].K_AllDiscipline + " but (s)he did  " + totalK + " \n";
					infeasibilityK_Assigned = true;
					infeasibleIntern_i[i] = true;
				}
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					if (totalK_g[g] != data.Intern[i].ShouldattendInGr_g[g])
					{
						Result += "The intern " + i + " should fulfill " + data.Intern[i].ShouldattendInGr_g[g] + " but (s)he did  " + totalK_g[g] + " in Group " + g + " \n";

					}
                    internGrK_ig[i][g] = totalK_g[g];
                }
                

            }

			for (int i = 0; i < data.General.Interns; i++)
			{
				int IndH1 = -1;
				int IndH2 = -1;
				PrChang_i[i] = 0;
				int totalDis = 0;
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						for (int h = 0; h < data.General.Hospitals + 1; h++)
						{
							if (Intern_itdh[i][t][d][h])
							{
								int totalDiscInthisHosp = 0;
								for (int dd = 0; dd < data.General.Disciplines; dd++)
								{
									for (int tt = 0; tt < data.General.TimePriods; tt++)
									{
										if (Intern_itdh[i][t][dd][h])
										{
											totalDiscInthisHosp++;
											break;
										}
									}
									
								}
								if (totalDiscInthisHosp > data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp)
								{
									infeasibleIntern_i[i] = true;
									infeasibilityKdiscOneHosp = true;
									Result += "The intern " + i + " performed " + totalDiscInthisHosp + " in hospital  " + h + " instead of  " + data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp + " \n";

								}

								totalDis++;
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
				if (PrChang_i[i] > data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp * totalDis)
				{
					infeasibilityChangesInHospital = true;
					infeasibleIntern_i[i] = true;
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
                                internDiscStatus_id[i][d] = true;
                                for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; tt++)
                                {
                                    internAvaStatus_it[i][tt] = true;
                                }
                                for (int w = 0; w < data.General.HospitalWard; w++)
								{
									if (data.Hospital[h].Hospital_dw[d][w])
									{
                                        YearlyCap_wh[w][h]++;
										for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; tt++)
										{
											overused_hwt[h][w][tt]++;
										}
										
									}
								}
							}
						}
					}
				}
			}

			for (int t = 0; t < data.General.TimePriods; t++)
			{
				for (int h = 0; h < data.General.Hospitals; h++)
				{
					for (int w = 0; w < data.General.HospitalWard; w++)
					{

                        overused_hwt[h][w][t] -= data.Hospital[h].HospitalMaxDem_tw[t][w];
						overused_hwt[h][w][t] -= data.Hospital[h].EmergencyCap_tw[t][w];
						overused_hwt[h][w][t] -= data.Hospital[h].ReservedCap_tw[t][w];
						if (overused_hwt[h][w][t] > 0)
						{
							infeasibilityOverused = true;
							Result += "The hospital " + h + " in ward " + w + " at time  " + t + " overassigned " + overused_hwt[h][w][t] + " position \n";
							for (int i = 0; i < data.General.Interns; i++)
							{
								for (int d = 0; d < data.General.Disciplines; d++)
								{
									if (data.Hospital[h].Hospital_dw[d][w])
									{
										// if we are at time 3 and the duration is 2
										// we have to chack from 3 - 2 + 1 till 3 + 2 -1
										int start = t - (data.Discipline[d].Duration_p[data.Intern[i].ProgramID] - 1);
										start = Math.Max(start, 0);
										int finish = t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; // (we are not counting the last one in the loop)
										finish = Math.Min(finish, data.General.TimePriods);
										for (int tt = start; tt < finish; tt++)
										{
											if (Intern_itdh[i][tt][d][h])
											{
												infeasibleIntern_i[i] = true;
												Result += "Intern " + i + " used the capacity at time " + tt + " till  " + (tt + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]).ToString() + "\n";
											}
										}
									}
								}
								
							}
						}
					}
				}
			}

            for (int h = 0; h < data.General.Hospitals; h++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    YearlyCap_wh[w][h] -= data.Hospital[h].HospitalMaxYearly_w[w];
                    
                    if (YearlyCap_wh[w][h] > 0)
                    {
                        infeasibilityYearlyDemand = true;
                        Result += "The hospital " + h + " in ward " + w + " overuesed yearly capacity by " + YearlyCap_wh[w][h] + " position \n";
                        for (int i = 0; i < data.General.Interns; i++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    // if we are at time 3 and the duration is 2
                                    // we have to chack from 3 - 2 + 1 till 3 + 2 -1
                                    for (int tt = 0; tt < data.General.TimePriods; tt++)
                                    {
                                        if (Intern_itdh[i][tt][d][h])
                                        {
                                            infeasibleIntern_i[i] = true;
                                            Result += "Intern " + i + " used the capacity at time " + tt + " till  " + (tt + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]).ToString() + "\n";
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
								for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; tt++)
								{
									for (int dd = 0; dd < data.General.Disciplines; dd++)
									{
										if (d == dd)
										{
											continue;
										}
										for (int hh = 0; hh < data.General.Hospitals + 1; hh++)
										{
											if (Intern_itdh[i][tt][dd][hh])
											{
												overLap_dd[d][dd] = true;
												infeasibilityOverused = true;
												infeasibleIntern_i[i] = true;
												Result += "The intern " + i + " has overlap for discipline " + d + " and  " + dd + " \n";
												
											}
										}
									}
								}
							}
						}
					}
				}
			}


			if (Result == "")
			{
				Result += "The solution is feasible";
			}
			return Result;
		}

		public void WriteSolution(string Path, string Name)
		{
			wieghtedSumInDisPrf = 0;
			wieghtedSumPrDisPrf = 0;
			wieghtedSumInHosPrf = 0;
			wieghtedSumInChnPrf = 0;
			wieghtedSumInWaiPrf = 0;
			MaxDisier = 0;
			MaxEmr = 0;
			MaxRes = 0;
			MaxMin = 0;
			MaxAcc = 0;
			StreamWriter tw = new StreamWriter(Path + Name + "OptSol.txt");
			tw.WriteLine("PP | II | GG | DD | TT | Du | HH | PrD | PrH | PrP | K_G | C_C | FH (Schedule)");
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int i = 0; i < data.General.Interns; i++)
				{
					MaxDisier += data.Intern[i].MaxPrf;
					for (int t = 0; t < data.General.TimePriods; t++)
					{
						for (int g = 0; g < data.General.DisciplineGr; g++)
						{
							if (data.Intern[i].ProgramID == p)
							{
								for (int d = 0; d < data.General.Disciplines; d++)
								{
									if (data.Intern[i].DisciplineList_dg[d][g])
									{

										for (int h = 0; h < data.General.Hospitals + 1; h++)
										{
											if (Intern_itdh[i][t][d][h])
											{
												wieghtedSumInDisPrf += data.Intern[i].wieght_d * data.Intern[i].Prf_d[d];
												wieghtedSumPrDisPrf += data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d];
												if (h < data.General.Hospitals)
												{
													wieghtedSumInHosPrf += data.Intern[i].wieght_d * data.Intern[i].Prf_h[h];
												}
												wieghtedSumInChnPrf = 0;
												wieghtedSumInWaiPrf = 0;
												if (data.Intern[i].OverSea_dt[d][t])
												{
													tw.WriteLine(p.ToString("00") + " | " + i.ToString("00") + " | " + g.ToString("00") + " | " + d.ToString("00") + " | " + t.ToString("00") + " | " + (data.Discipline[d].Duration_p[p]).ToString("00") + " | " + h.ToString("00") + " | " + data.Intern[i].Prf_d[d].ToString("000") + " | " + "***" + " | " + data.TrainingPr[p].Prf_d[d].ToString("000") + " | " + data.Intern[i].ShouldattendInGr_g[g].ToString("000") + " | " + data.Discipline[d].CourseCredit_p[p].ToString("000") + " | " + "**");
												}
												else
												{													
													tw.WriteLine(p.ToString("00") + " | " + i.ToString("00") + " | " + g.ToString("00") + " | " + d.ToString("00") + " | " + t.ToString("00") + " | " + (data.Discipline[d].Duration_p[p]).ToString("00") + " | " + h.ToString("00") + " | " + data.Intern[i].Prf_d[d].ToString("000") + " | " + data.Intern[i].Prf_h[h].ToString("000") + " | " + data.TrainingPr[p].Prf_d[d].ToString("000") + " | " + data.Intern[i].ShouldattendInGr_g[g].ToString("000") + " | " + data.Discipline[d].CourseCredit_p[p].ToString("000") + " | ");
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
			}
			MaxDisier /= data.General.Interns;
			for (int r = 0; r < data.General.Region; r++)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					NotUsesAcc[r][t] -= data.Region[r].AvaAcc_t[t];
					MaxAcc += data.Region[r].AvaAcc_t[t]; 
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
						for (int h = 0; h < data.General.Hospitals + 1; h++)
						{
							if (Intern_itdh[i][t][d][h] && h < data.General.Hospitals)
							{
								PrDisp_i[i] += data.Intern[i].Prf_d[d];
								PrHosp_i[i] += data.Intern[i].Prf_h[h];
								TrPrPrf[i] += data.TrainingPr[data.Intern[i].ProgramID].Prf_d[d];
							}
							else if (Intern_itdh[i][t][d][h])
							{
								PrDisp_i[i] += data.Intern[i].Prf_d[d];
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
								for (int h = 0; h < data.General.Hospitals + 1; h++)
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
						MaxMin += data.Hospital[h].HospitalMinDem_tw[t][w];
						MaxEmr += data.Hospital[h].EmergencyCap_tw[t][w];
						MaxRes += data.Hospital[h].ReservedCap_tw[t][w];
						int filledDem = 0;
						for (int d = 0; d < data.General.Disciplines; d++)
						{

							if (data.Hospital[h].Hospital_dw[d][w])
							{
								for (int i = 0; i < data.General.Interns; i++)
								{
									if (Intern_itdh[i][t][d][h])
									{
										Assigned_twh[t][w][h]++;
										filledDem++;
									}
								}
							}
						}
						if (filledDem < data.Hospital[h].HospitalMinDem_tw[t][w])
						{
							SlackDem += data.Hospital[h].HospitalMinDem_tw[t][w] - filledDem;
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
						for (int h = 0; h < data.General.Hospitals + 1; h++)
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
				wieghtedSumInChnPrf += data.Intern[i].wieght_ch * PrChang_i[i];
				wieghtedSumInWaiPrf += data.Intern[i].wieght_w * PrWait_i[i];
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

			tw.WriteLine("III DesI DesP MaxP");
			for (int i = 0; i < data.General.Interns; i++)
			{
				Obj += Des_i[i] * data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
				Obj_i[i] += Des_i[i] * data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi;
				tw.WriteLine(i.ToString("000") + " " + Des_i[i].ToString("000")
				 + " " + MinDis[data.Intern[i].ProgramID].ToString("000")
				 + " " + data.Intern[i].MaxPrf.ToString("000"));
		}
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				Obj += MinDis[p] * data.TrainingPr[p].CoeffObj_MinDesi;
				Obj -= ResDemand * data.TrainingPr[p].CoeffObj_ResCap;
				Obj -= EmrDemand * data.TrainingPr[p].CoeffObj_EmrCap;
				Obj -= NotUsedAccTotal * data.TrainingPr[p].CoeffObj_NotUsedAcc;
				Obj -= SlackDem * data.TrainingPr[p].CoeffObj_MINDem;
			}
			tw.WriteLine("Res: " + ResDemand.ToString("000.0"));
			tw.WriteLine("Emr: " + EmrDemand.ToString("000.0"));
			tw.WriteLine("Acc: " + NotUsedAccTotal.ToString("000.0"));
			tw.WriteLine("SlD: " + SlackDem.ToString("000.0"));
			tw.WriteLine("Obj: " + Obj.ToString("000.0"));
			tw.WriteLine(setInfSetting());
			IsFeasible = !infeasibilityChangesInHospital 
				&& !infeasibilityK_Assigned && !infeasibilityOverseaAbilityAve 
				&& !infeasibilitySkill && !infeasibilityOverused && !infeasibilityOverlap
				&& !infeasibilityKdiscOneHosp;
			if (!IsFeasible)
			{
				Obj = -data.AlgSettings.BigM;
			}
			
			tw.Close();
            DemonstraitSolution(Path, Name);

        }

        public void DemonstraitSolution(string Path, string Name)
        {
            // infeasibility part
            StreamWriter demonstration = new StreamWriter(Path + Name + "Demonstration.txt");
            demonstration.WriteLine("Inforamtion related to the Infeasibility");
            for (int i = 0; i < data.General.Interns; i++) {
                for (int g = 0; g < data.General.DisciplineGr; g++)
                {
                    if (internGrK_ig[i][g] != data.Intern[i].ShouldattendInGr_g[g])
                    {
                        demonstration.WriteLine("The intern " + i + " should fulfill " + data.Intern[i].ShouldattendInGr_g[g] + " but (s)he did  " + internGrK_ig[i][g] + " in Group " + g );
                        demonstration.Write("Intern Availability: ");
                        for (int t = 0; t < data.General.TimePriods; t++)
                        {
                            if (internAvaStatus_it[i][t])
                            {
                                demonstration.Write("-- ");
                            }
                            else
                            {
                                demonstration.Write("++ ");
                            }
                        }
                        demonstration.WriteLine();
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            if (data.Intern[i].DisciplineList_dg[d][g] && !internDiscStatus_id[i][d])
                            {
                                

                                for (int h = 0; h < data.General.Hospitals; h++)
                                {
                                    for (int w = 0; w < data.General.HospitalWard; w++)
                                    {
                                       
                                        if (data.Hospital[h].Hospital_dw[d][w])
                                        {
                                           
                                            demonstration.Write("Capcity_hd    " + h.ToString("00") + " " + d.ToString("00") + ": ");
                                            for (int t = 0; t < data.General.TimePriods; t++)
                                            {
                                               
                                                demonstration.Write((-overused_hwt[h][w][t]).ToString("00") + " ");
                                            }
                                            demonstration.Write( " || "+(-YearlyCap_wh[w][h]).ToString("00") + " ");
                                            demonstration.WriteLine();
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
            }


            demonstration.Close();
        }
	}
}

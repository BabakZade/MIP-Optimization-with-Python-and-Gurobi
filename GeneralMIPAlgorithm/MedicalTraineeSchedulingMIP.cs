using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;
using ILOG.Concert;
using ILOG.CPLEX;

namespace GeneralMIPAlgorithm
{
	public class MedicalTraineeSchedulingMIP
	{		
		public AllData data;

		public Cplex MIPModel;
		public IIntVar[][][][] y_idDh;
		public IIntVar[][][][] s_idth;
		public INumVar[][] w_id;
		public INumVar[][] ch_id;
		public INumVar[] des_i;
		public INumVar desmax;

		public string[][][][] Y_idDh;
		public string[][][][] S_idth;
		public string[][] W_id;
		public string[][] Ch_id;
		public string[] Des_i;

		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;

		public MedicalTraineeSchedulingMIP(AllData InputData, string Path, string InsName)
		{
			data = InputData;
			Initial();
			InitialMIPVar();
			CreateModel();
			solve_MIPmodel(Path, InsName);
		}

		public void Initial() {

			Interns = data.General.Interns;

			// discipline + 1 for dummy d = 0 is dummy
			Disciplins = data.General.Disciplines + 1;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
		}
		public void InitialMIPVar() {

			MIPModel = new Cplex();
			Y_idDh = new string[Interns][][][];
			for (int i = 0; i < Interns; i++)
			{
				Y_idDh[i] = new string[Disciplins][][];
				for (int d = 0; d < Disciplins ; d++)
				{
					Y_idDh[i][d] = new string[Disciplins][];
					for (int dd = 0; dd < Disciplins ; dd++)
					{
						Y_idDh[i][d][dd] = new string[Hospitals];
						for (int h = 0; h < Hospitals; h++)
						{
							Y_idDh[i][d][dd][h] = "Y_idDh[" + i + "][" + d + "][" + dd + "][" + h + "]";
						}
					}
				}
			}


			y_idDh = new IIntVar[Interns][][][];
			for (int i = 0; i < Interns; i++)
			{
				// discipline + 1 for dummy d = 0 is dummy
				y_idDh[i] = new IIntVar[Disciplins][][];
				for (int d = 0; d < Disciplins; d++)
				{
					y_idDh[i][d] = new IIntVar[Disciplins][];
					for (int dd = 0; dd < Disciplins; dd++)
					{
						y_idDh[i][d][dd] = new IIntVar[Hospitals];
						for (int h = 0; h < Hospitals; h++)
						{
							y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 1, Y_idDh[i][d][dd][h]);
							if (dd == 0 || dd == d)
							{
								y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 0, Y_idDh[i][d][dd][h]);
							}
							MIPModel.Add(y_idDh[i][d][dd][h]);
						}
					}
				}
			}

			S_idth = new string[Interns][][][];
			for (int i = 0; i < Interns; i++)
			{
				S_idth[i] = new string[Disciplins][][];
				for (int d = 0; d < Disciplins; d++)
				{
					S_idth[i][d] = new string[Timepriods][];
					for (int t = 0; t < Timepriods; t++)
					{
						S_idth[i][d][t] = new string[Hospitals];
						for (int h = 0; h < Hospitals; h++)
						{
							S_idth[i][d][t][h] = "S_idth[" + i + "][" + d + "][" + t + "][" + h + "]";
						}
					}
				}
			}

			s_idth = new IIntVar[Interns][][][];
			for (int i = 0; i < Interns; i++)
			{
				s_idth[i] = new IIntVar[Disciplins][][];
				for (int d = 0; d < Disciplins; d++)
				{
					s_idth[i][d] = new IIntVar[Timepriods][];
					for (int t = 0; t < Timepriods; t++)
					{
						s_idth[i][d][t] = new IIntVar[Hospitals];
						for (int h = 0; h < Hospitals; h++)
						{
							s_idth[i][d][t][h] = MIPModel.IntVar(0, 1, S_idth[i][d][t][h]);
							// dummy discipline 
							if (d == 0)
							{
								s_idth[i][d][t][h] = MIPModel.IntVar(0, 0, S_idth[i][d][t][h]);
							}
							MIPModel.Add(s_idth[i][d][t][h]);
						}
					}
				}
			}

			W_id = new string[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				W_id[i] = new string[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					W_id[i][d] = "W_id[" + i + "][" + d + "]";
				}
			}

			w_id = new INumVar[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				w_id[i] = new INumVar[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					w_id[i][d] = MIPModel.NumVar(0, Timepriods, W_id[i][d]);
					MIPModel.Add(w_id[i][d]);
				}
			}

			Ch_id = new string[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				Ch_id[i] = new string[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					Ch_id[i][d] = "Ch_id[" + i + "][" + d + "]";
				}
			}

			ch_id = new INumVar[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				ch_id[i] = new INumVar[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					ch_id[i][d] = MIPModel.NumVar(0, 1, Ch_id[i][d]);
					MIPModel.Add(ch_id[i][d]);
				}
			}

			Des_i = new string[Interns];
			for (int i = 0; i < Interns; i++)
			{
				Des_i[i] = "Des_i[" + i + "]";
			}
			des_i = new INumVar[Interns];
			double Max = 0;
			for (int i = 0; i < Interns; i++)
			{
				des_i[i] = MIPModel.NumVar(data.Intern[i].wieght_ch
					+ data.Intern[i].wieght_w,
					+ data.Intern[i].wieght_d
					+ data.Intern[i].wieght_h, Des_i[i]);
				double xx = - data.Intern[i].wieght_ch
					- data.Intern[i].wieght_w
					+ data.Intern[i].wieght_d
					+ data.Intern[i].wieght_h;
				if (Max < xx)
				{
					Max = xx;
				}
			}
			desmax = MIPModel.NumVar(0, Max, "DesMax");
		}

		public void CreateModel()
		{
			// objective function

			MIPModel.AddMaximize(desmax);

			// Mandatory discipline 
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					ILinearNumExpr mandatory = MIPModel.LinearNumExpr();
					// check if the training program and involved mandatory discipline
					for (int p = 0; p < TrainingPr; p++)
					{
						if (data.TrainingPr[p].PrManDis_d[d-1] && data.Intern[i].ProgramID == p )
						{
							for (int dd = 0; dd < Disciplins; dd++)
							{
								for (int h = 0; h < Hospitals; h++)
								{
									mandatory.AddTerm(y_idDh[i][dd][d][h],1);
								}
							}
							MIPModel.AddEq(mandatory, (data.Intern[i].Abi_d[d - 1] ? 1 : 0), "Mandatory_" + i + "_" + d);
						}
					}
				}
			}

			// Arbitrary discipline 
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					ILinearNumExpr arbitrary = MIPModel.LinearNumExpr();
					// check if the training program and involved mandatory discipline
					for (int p = 0; p < TrainingPr; p++)
					{
						if (!data.TrainingPr[p].PrManDis_d[d - 1] && data.TrainingPr[p].Program_d[d - 1] && data.Intern[i].ProgramID == p)
						{
							for (int dd = 0; dd < Disciplins; dd++)
							{
								for (int h = 0; h < Hospitals; h++)
								{
									arbitrary.AddTerm(y_idDh[i][dd][d][h], 1);
								}
							}
							MIPModel.AddLe(arbitrary, (data.Intern[i].Abi_d[d - 1] ? 1 : 0), "Arbitrary" +
								"_" + i + "_" + d);
						}
					}
				}
			}

			// Total Discipline 
			for (int i = 0; i < Interns; i++)
			{
				ILinearNumExpr allDisc = MIPModel.LinearNumExpr();
				for (int p = 0; p < TrainingPr; p++)
				{
					if (data.Intern[i].ProgramID == p)
					{
						for (int d = 1; d < Disciplins; d++)
						{
							for (int dd = 0; dd < Disciplins; dd++)
							{
								for (int h = 0; h < Hospitals; h++)
								{
									allDisc.AddTerm(y_idDh[i][dd][d][h],1);
								}
							}
						}

						MIPModel.AddEq(allDisc, data.TrainingPr[p].MandatoryD + data.TrainingPr[p].ArbitraryD, "TotalDiscipline_" + i);
					}
				}
			}

			// dummy discipline 
			for (int i = 0; i < Interns; i++)
			{
				ILinearNumExpr dummy = MIPModel.LinearNumExpr();
				for (int d = 1; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						dummy.AddTerm(y_idDh[i][0][d][h],1);
					}
				}

				MIPModel.AddEq(dummy, 1, "dummy_" + i);
			}

			// Start time
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					for (int dd = 0; dd < Disciplins; dd++)
					{
						for (int t = 0; t < Timepriods; t++)
						{
							ILinearNumExpr starttime = MIPModel.LinearNumExpr();
							for (int p = 0; p < TrainingPr; p++)
							{
								if (data.Intern[i].ProgramID == p && data.TrainingPr[p].Program_d[d-1])
								{
									if ( dd == 0 || (dd > 0 && data.TrainingPr[p].Program_d[dd - 1]))
									{
										for (int h = 0; h < Hospitals; h++)
										{
											starttime.AddTerm(y_idDh[i][dd][d][h],-Timepriods);
											starttime.AddTerm(s_idth[i][d][t][h],t);
											starttime.AddTerm(s_idth[i][dd][t][h],-t);
										}
										
									}

									starttime.AddTerm(w_id[i][d] , -1);
									int dur = 0;
									if (dd != 0)
									{
										dur = data.Discipline[dd - 1].Duration_p[p];
									}
									MIPModel.AddGe(starttime, dur - Timepriods, "Start_" + i + "_" + d + "_" + dd + "_" + t);
								}
							}
						}
					}
				}
			}

			// availability for interns 
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 1; d <  Disciplins; d++)
				{
					for (int t = 0; t < Timepriods; t++)
					{
						ILinearNumExpr avail = MIPModel.LinearNumExpr();

						for (int p = 0; p < TrainingPr; p++)
						{
							if (data.Intern[i].ProgramID == p && data.TrainingPr[p].Program_d[d-1])
							{
								for (int h = 0; h < Hospitals; h++)
								{
									avail.AddTerm(s_idth[i][d][t][h],data.Discipline[d-1].Duration_p[p]);
								}
								int rhs = 0;
								for (int tt = t; tt < t+ data.Discipline[d-1].Duration_p[p] && tt < Timepriods; tt++)
								{
									rhs += data.Intern[i].Ave_t[tt] ? 1 : 0;
								}
								MIPModel.AddLe(avail, rhs, "Availbility_" + i + "_" + d + "_" + t);
							}
						}
					}
				}
			}

			// minimum demand 
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						ILinearNumExpr minDem = MIPModel.LinearNumExpr();
						for (int i = 0; i < Interns; i++)
						{
							for (int p = 0; p < TrainingPr; p++)
							{
								for (int tt = 0; tt < t; tt++)
								{
									minDem.AddTerm(s_idth[i][d][tt][h], 1);
								}
								for (int tt = 0; tt < t - data.Discipline[d-1].Duration_p[p] ; tt++)
								{
									minDem.AddTerm(s_idth[i][d][tt][h], -1);
								}
								minDem.AddTerm(s_idth[i][d][t][h], 1);
							}
							
						}
						MIPModel.AddGe(minDem, data.Hospital[h].HospitalMinDem_td[t][d-1], "MinDem_" + t + "_" + d + "_" + h);
					}
				}
			}

			// max demand 
			for (int t = 0; t < Timepriods; t++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					for (int h = 0; h < Hospitals; h++)
					{
						ILinearNumExpr maxDem = MIPModel.LinearNumExpr();
						for (int i = 0; i < Interns; i++)
						{
							for (int p = 0; p < TrainingPr; p++)
							{
								for (int tt = 0; tt < t; tt++)
								{
									maxDem.AddTerm(s_idth[i][d][tt][h], 1);
								}
								for (int tt = 0; tt < t - data.Discipline[d-1].Duration_p[p]; tt++)
								{
									maxDem.AddTerm(s_idth[i][d][tt][h], -1);
								}
								maxDem.AddTerm(s_idth[i][d][t][h], 1);
							}

						}
						MIPModel.AddLe(maxDem, data.Hospital[h].HospitalMaxDem_td[t][d-1], "MaxDem_" + t + "_" + d + "_" + h);
					}
				}
			}

			// skill
			for (int i = 0; i < Interns; i++)
			{
				for (int t = 0; t < Timepriods; t++)
				{
					for (int d = 1; d < Disciplins; d++)
					{
						ILinearNumExpr skill = MIPModel.LinearNumExpr();
						for (int p = 0; p < TrainingPr; p++)
						{
							if (data.Intern[i].ProgramID == p && data.TrainingPr[p].Program_d[d-1])
							{
								for (int h = 0; h < Hospitals; h++)
								{
									skill.AddTerm(s_idth[i][d][t][h],t);
									for (int dd = 1; dd < Disciplins; dd++)
									{
										if (data.Discipline[d-1].Skill4D_dp[dd-1][p] && data.TrainingPr[p].Program_d[dd - 1])
										{
											skill.AddTerm(s_idth[i][dd][t][h],-t);
										}
									}
								}								
							}

							MIPModel.AddGe(skill, 0, "Skill_" + i + "_" + t + "_" + d);
						}
					}
				}
			}

			// change 
			for (int i = 0; i < Interns; i++)
			{
				for (int d = 1; d < Disciplins; d++)
				{
					for (int dd = 0; dd < Disciplins; dd++)
					{
						for (int h = 0; h < Hospitals; h++)
						{
							ILinearNumExpr change = MIPModel.LinearNumExpr();
							for (int p = 0; p < TrainingPr; p++)
							{
								if (data.Intern[i].ProgramID == p && data.TrainingPr[p].Program_d[d - 1])
								{
									change.AddTerm(ch_id[i][d],1);
									change.AddTerm(y_idDh[i][dd][d][h], -1);
									for (int ddd = 0; ddd < Disciplins; ddd++)
									{
										change.AddTerm(y_idDh[i][d][ddd][h],1);
									}
								}
								
							}
							
						}
					}
				}
			}

			// des_i
			for (int i = 0; i < Interns; i++)
			{
				ILinearNumExpr internDes = MIPModel.LinearNumExpr();
				internDes.AddTerm(des_i[i],1);
				for (int d = 1; d < Disciplins; d++)
				{
					internDes.AddTerm(ch_id[i][d], -(double)data.Intern[i].wieght_ch / data.PrChan_i[i]);
					internDes.AddTerm(w_id[i][d], -(double)data.Intern[i].wieght_w / data.PrWait_i[i]);
					for (int dd = 0; dd < Disciplins; dd++)
					{
						for (int h = 0; h < Hospitals; h++)
						{
							for (int p = 0; p < TrainingPr; p++)
							{
								if (data.Intern[i].ProgramID == p && data.TrainingPr[p].Program_d[d - 1])
								{
									internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.Intern[i].Prf_d[d - 1] * data.Intern[i].wieght_d / data.PrDisc_i[i]);
									internDes.AddTerm(y_idDh[i][dd][d][h], -(double)data.Intern[i].Prf_h[h] * data.Intern[i].wieght_h / data.PrHosp_i[i]);
								}

							}
							
						}
					}


				}

				MIPModel.AddLe(internDes, 0, "Des_" + i);
			}

			// desire  Max
			for (int i = 0; i < Interns; i++)
			{
				ILinearNumExpr demax = MIPModel.LinearNumExpr();
				demax.AddTerm(desmax,1);
				demax.AddTerm(des_i[i],-1);
				MIPModel.AddLe(demax, 0, "DesMax");
			}




		}

		public void solve_MIPmodel(string Path, string InsName)
		{
			//*************set program
			MIPModel.ExportModel(Path+InsName+"MIPModel.lp");
			MIPModel.SetParam(Cplex.DoubleParam.EpRHS, data.AlgSettings.RHSepsi);
			MIPModel.SetParam(Cplex.DoubleParam.EpOpt, data.AlgSettings.RCepsi);
			MIPModel.SetParam(Cplex.IntParam.Threads, 3);
			MIPModel.SetParam(Cplex.DoubleParam.TiLim, data.AlgSettings.MIPTime);
			MIPModel.SetParam(Cplex.BooleanParam.MemoryEmphasis, true);
			try
			{
				OptimalSolution mipOpt = new OptimalSolution(data);
				if (MIPModel.Solve())
				{
					for (int i = 0; i < Interns; i++)
					{
						for (int t = 0; t < Timepriods; t++)
						{
							for (int d = 1; d < Disciplins; d++)
							{
								for (int h = 0; h < Hospitals; h++)
								{
									if (MIPModel.GetValue(s_idth[i][d][t][h]) > 1 - 0.5)
									{
										mipOpt.Intern_itdh[i][t][d-1][h] = true;
									}
								}
							}
						}
					}
				}
				mipOpt.WriteSolution( Path, InsName,data);
				MIPModel.End();
			}
			catch (ILOG.Concert.Exception e)
			{
				System.Console.WriteLine("Concert exception '" + e + "' caught");
			}
		}
	}
}

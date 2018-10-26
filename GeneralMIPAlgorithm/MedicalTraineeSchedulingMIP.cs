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
		public IIntVar[][] ch_id;
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

		public void Initial() {

			Interns = data.General.Interns;

			// discipline + 1 for dummy d = 0 is dummy
			Disciplins = data.General.Disciplines + 1;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
		}
		public void InitialMIPVar() {

			MIPModel = new Cplex();
			Y_idDh = new string[data.General.Interns][][][];
			for (int i = 0; i < data.General.Interns; i++)
			{
				Y_idDh[i] = new string[data.General.Disciplines][][];
				for (int d = 0; d < data.General.Disciplines ; d++)
				{
					Y_idDh[i][d] = new string[data.General.Disciplines][];
					for (int dd = 0; dd < data.General.Disciplines ; dd++)
					{
						Y_idDh[i][d][dd] = new string[data.General.Hospitals];
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							Y_idDh[i][d][dd][h] = "Y_idDh[" + i + "][" + d + "][" + dd + "][" + h + "]";
						}
					}
				}
			}


			y_idDh = new IIntVar[data.General.Interns][][][];
			for (int i = 0; i < data.General.Interns; i++)
			{
				// discipline + 1 for dummy d = 0 is dummy
				y_idDh[i] = new IIntVar[data.General.Disciplines][][];
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					y_idDh[i][d] = new IIntVar[data.General.Disciplines][];
					for (int dd = 0; dd < data.General.Disciplines; dd++)
					{
						y_idDh[i][d][dd] = new IIntVar[data.General.Hospitals];
						for (int h = 0; h < data.General.Hospitals; h++)
						{
							y_idDh[i][d][dd][h] = MIPModel.IntVar(0, 1, Y_idDh[i][d][dd][h]);
							if (dd == 0)
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

			ch_id = new IIntVar[Interns][];
			for (int i = 0; i < Interns; i++)
			{
				ch_id[i] = new IIntVar[Disciplins];
				for (int d = 0; d < Disciplins; d++)
				{
					ch_id[i][d] = MIPModel.IntVar(0, 1, Ch_id[i][d]);
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
				des_i[i] = MIPModel.NumVar( - data.Intern[i].wieght_ch
					 - data.Intern[i].wieght_w,
					+data.Intern[i].wieght_d
					+data.Intern[i].wieght_h , Des_i[i]);
			}
			desmax = MIPModel.NumVar(0, data.Intern[i].wieght_ch
					+ data.Intern[i].wieght_w
					+ data.Intern[i].wieght_d
					+ data.Intern[i].wieght_h, Des_i[i]);
		}

		public void CreateModel()
		{

		}
	}
}

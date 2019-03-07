using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class InstanceSetting
	{
		public ArrayList AllinstanceSettings;
		// ratio of ward to discipline
		public double R_wd;

		// ratio of discipline group size to attended course from each group
		// this ratio is same in each training program
		public double[] R_gk_g;

		// ratio of ward in hospital
		public double R_wh;

		// ratio of required skills between total relationship
		public double R_Trel;

		// ratio of hospital to intern
		public double R_hi;

		// total number of intern
		public int TotalIntern;

		// total number of discipline 
		public int TotalDiscipline;

		
		// min Demand Rate
		public double R_dMin;

		// min demand 
		public int MinDem;

		// max demand
		public int MaxDem;

		// emr demand
		public int EmrDem;

		// res demand
		public int ResDem;

		// ratio of mutual discipline in training program
		public double R_muDp;

		// ratio of mutual discipline in discipline groups
		public double R_muDg;

		// preference max value 
		public int PrfMaxValue;

		// Coefficient Max value 
		public int CoefficientMaxValue;

		// is prespective
		public double Prespective;

		// oversea hospital
		public double overseaHosp;

		// fulfilled %
		public double fulfilled;

		// total Region
		public int TRegion;

		// total training Program
		public int TTrainingP;

		// total Groups
		public int TDGroup;

		// total TimePriod
		public int TTime;



		// the percentage of discipline in each discipline groups
		// 2 group, 2 traing program
		// discipline distribution per training program (sum = 1)
		// 0.6 => training program 1
		// 0.4 => training program 2
		public double[] DisciplineDistribution_p;
		// discipline distribution per group (from all assigned discipline to the training program) 
		// sum per row => 1
		// 0.2 0.8 
		// 0.5 0.5
		// it is considired equal for all training program
		public double[] DisciplineDistribution_g;
	

		public InstanceSetting(double R_wd, double[] R_gk_g, double R_wh, 
							   double R_Trel, double R_hi, int TotalIntern, 
							   int TotalDiscipline, double R_dMin, int MinDem, int MaxDem,
							   int EmrDem, int ResDem, double R_muDp, double R_muDg, int PrfMaxValue, int CoefficientMaxValue,
							   double Prespective, double overseaHosp, double fulfilled, int TRegion, int TTrainingP, int TDGroup, int TTime,
							   double[] DisciplineDistribution_p, double[] DisciplineDistribution_g)
		{
			this.R_wd = R_wd;
			this.R_gk_g = R_gk_g;
			this.R_wh = R_wh;
			this.R_Trel = R_Trel;
			this.R_hi = R_hi;
			this.TotalIntern = TotalIntern;
			this.TotalDiscipline = TotalDiscipline;
			this.R_dMin = R_dMin;
			this.MinDem = MinDem;
			this.MaxDem = MaxDem;
			this.EmrDem = EmrDem;
			this.ResDem = ResDem;
			this.R_muDp = R_muDp;
			this.R_muDg = R_muDg;
			this.PrfMaxValue = PrfMaxValue;
			this.CoefficientMaxValue = CoefficientMaxValue;
			this.Prespective = Prespective;
			this.overseaHosp = overseaHosp;
			this.fulfilled = fulfilled;
			this.TRegion = TRegion;
			this.TTrainingP = TTrainingP;
			this.TDGroup = TDGroup;
			this.TTime = TTime;
			this.DisciplineDistribution_p = DisciplineDistribution_p;
			this.DisciplineDistribution_g = DisciplineDistribution_g;
			
		}
		public InstanceSetting() { setInstanceSetting(); }
		public void setInstanceSetting()
		{
			// ratio of ward to discipline
			AllinstanceSettings = new ArrayList();
			double[] R_wd = new double[3] { 1, 0.75, 0.5 };

			// ratio of discipline group size to attended course from that group
			double[][] R_gk_g = new double[3][]{ new double[2]{ 1, 0.2 }, new double[2]{ 0.8, 0.4 }, new double[2] { 0.6, 0.6 } };

			// ratio of ward in hospital
			double[] R_wh = new double[3] { 1, 0.75, 0.5 };

			// ratio of required skills between total relationship
			double[] R_Trel = new double[3] { 0, 0.1, 0.25 };

			// ratio of hospital to intern
			double[] R_hi = new double[2] { 2, 4 };

			// total number of intern
			int[] Totalintern = new int[2] { 20, 40 };

			// total number of discipline 
			int[] TotalDiscipline = new int[2] { 12, 24 };

			// min Demand Rate
			double[] R_dMin = new double[1] { 0.05 };

			// min demand 
			int[] MinDem = new int[1] { 1 };

			// max demand
			int[] MaxDem = new int[1] { 4 };

			// emr demand
			int[] EmrDem = new int[1] { 1 };

			// res demand
			int[] ResDem = new int[1] { 1 };

			// ratio of mutual discipline in training program
			double[] R_muDp = new double[3] { 0, 0.1, 0.25 };

			// ratio of mutual discipline in discipline groups
			double[] R_muDg = new double[3] { 0, 0.1, 0.25 };

			// preference max value 
			int[] PrfMaxValue = new int[1] { 4 };

			// Coefficient Max value 
			int[] CoefficientMaxValue = new int[1] { 1 };

			// is prespective
			double[] Prespective = new double[1] { 0.95 };

			// oversea hospital
			double[] overseaHosp = new double[3] { 0, 0.1, 0.25 };

			// fulfilled %
			double[] fulfilled = new double[1] { 0.1 };

			// total Region
			int[] TRegion = new int[1] { 1 };

			// total training Program
			int[] TTrainingP = new int[1] { 2 };

			// total Groups
			int[] TDGroup = new int[1] { 2 };

			// total TimePriod
			int[] TTime = new int[1] { 24 };

			// the percentage of discipline in each discipline groups
			// 2 group, 2 traing program
			// discipline distribution per training program (sum = 1)
			// 0.6 => training program 1
			// 0.4 => training program 2
			double[][] DisciplineDistribution_p = new double[3][] { new double[2] {0.3 , 0.7 }, new double[2] { 0.5 , 0.5 }, new double[2] { 0.7, 0.3 } };
			// discipline distribution per group (from all assigned discipline to the training program) 
			// sum per row => 1
			// 0.2 0.8 
			// 0.5 0.5
			// it is considired equal for all training program
		    double[][] DisciplineDistribution_g = new double[3][] { new double[2] { 0.3, 0.7 }, new double[2] { 0.5, 0.5 }, new double[2] { 0.7, 0.3 } };

			for (int wd = 0; wd < R_wd.Length; wd++)
			{
				for (int gk = 0; gk < R_gk_g.GetLength(0); gk++)
				{
					for (int wh = 0; wh < R_wh.Length; wh++)
					{
						for (int trel = 0; trel < R_Trel.Length; trel++)
						{
							for (int hi = 0; hi < R_hi.Length; hi++)
							{
								for (int i = 0; i < Totalintern.Length; i++)
								{
									for (int d = 0; d < TotalDiscipline.Length; d++)
									{
										for (int rdmin = 0; rdmin < R_dMin.Length; rdmin++)
										{
											for (int mind = 0; mind < MinDem.Length; mind++)
											{
												for (int maxd = 0; maxd < MaxDem.Length; maxd++)
												{
													for (int emrd = 0; emrd < EmrDem.Length; emrd++)
													{
														for (int resd = 0; resd < ResDem.Length; resd++)
														{
															for (int mup = 0; mup < R_muDp.Length; mup++)
															{
																for (int mug = 0; mug < R_muDg.Length; mug++)
																{
																	for (int prfMax = 0; prfMax < PrfMaxValue.Length; prfMax++)
																	{
																		for (int coef = 0; coef < CoefficientMaxValue.Length; coef++)
																		{
																			for (int prs = 0; prs < Prespective.Length; prs++)
																			{
																				for (int overs = 0; overs < overseaHosp.Length; overs++)
																				{
																					for (int ff = 0; ff < fulfilled.Length; ff++)
																					{
																						for (int r = 0; r < TRegion.Length; r++)
																						{
																							for (int p = 0; p < TTrainingP.Length; p++)
																							{
																								for (int g = 0; g < TDGroup.Length; g++)
																								{
																									for (int t = 0; t < TTime.Length; t++)
																									{
																										for (int ddistp = 0; ddistp < DisciplineDistribution_p.GetLength(0); ddistp++)
																										{
																											for (int ddistg = 0; ddistg < DisciplineDistribution_g.GetLength(0); ddistg++)
																											{
																												AllinstanceSettings.Add(new InstanceSetting(R_wd[wd], R_gk_g[gk], R_wh[wh], R_Trel[trel], R_hi[hi], Totalintern[i], TotalDiscipline[d], R_dMin[rdmin], MinDem[mind],
																													MaxDem[maxd], EmrDem[emrd], ResDem[resd], R_muDp[mup], R_muDg[mug], PrfMaxValue[prfMax], CoefficientMaxValue[coef], Prespective[prs],
																													overseaHosp[overs], fulfilled[ff], TRegion[r], TTrainingP[p], TDGroup[g], TTime[t], DisciplineDistribution_p[ddistp],DisciplineDistribution_g[ddistg]));
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
			}

		}
	}
}

using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public struct DesirePos
	{
		public int theD;
		public int theH;
		public double Desire;
		DesirePos(int theD, int theH, double Desire)
		{
			this.theD = theD;
			this.theH = theH;
			this.Desire = Desire;
		}
		public void Copy(DesirePos copyable)
		{
			theD = copyable.theD;
			theH = copyable.theH;
			Desire = copyable.Desire;
		}
	}
	public class InternInfo
	{
		public string Name;
		public int SQLID;
		public int[] Prf_d;
		public int[] Prf_h;
		public bool[] Ave_t;
		public bool[][] Abi_dh;
		public int wieght_d;
		public int wieght_h;
		public int wieght_w;
		public int wieght_ch;
		public int ProgramID;
		public bool[] TransferredTo_r;
		public bool[][][] Fulfilled_dhp;
		public bool[][] OverSea_dt;
		public bool[] FHRequirment_d;
		public bool overseaReq;
		public int[] SortedPrfD_pos;
		public int[] SortedPrfH_pos;
		public int[][] AllDes_dh;
		//public int MaxDesireHos;
		//public int MaxDesireDis;
		//public double MaxDesir;
		public ArrayList sortedPrf;
		public double MaxPrf;
		public bool[][] DisciplineList_dg;
		public int[] ShouldattendInGr_g;
		public bool isProspective;
		public int K_AllDiscipline;
		public double[] takingDiscPercentage;
		public double AveDur;

		public double[] requieredTimeForRemianed;
		public InternInfo(int Hospitals, int Disciplines, int TimePeriods, int DisciplineGr, int TrainingPr, int Region)
		{
			Name = "";
			SQLID = 0;
			new ArrayInitializer().CreateArray(ref Prf_d, Disciplines, 0);
			new ArrayInitializer().CreateArray(ref FHRequirment_d, Disciplines, false);
			new ArrayInitializer().CreateArray(ref TransferredTo_r, Region, false);
			new ArrayInitializer().CreateArray(ref ShouldattendInGr_g, DisciplineGr, 0);
			new ArrayInitializer().CreateArray(ref Prf_h, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref SortedPrfD_pos, Disciplines, 0);
			new ArrayInitializer().CreateArray(ref SortedPrfH_pos, Hospitals, 0);
			new ArrayInitializer().CreateArray(ref Ave_t, TimePeriods, true);
			new ArrayInitializer().CreateArray(ref Abi_dh, Disciplines, Hospitals, true);
			new ArrayInitializer().CreateArray(ref Fulfilled_dhp, Disciplines, Hospitals, TrainingPr, false);
			new ArrayInitializer().CreateArray(ref DisciplineList_dg, Disciplines, DisciplineGr, false);
			new ArrayInitializer().CreateArray(ref OverSea_dt, Disciplines, TimePeriods, false);
			new ArrayInitializer().CreateArray(ref AllDes_dh,Disciplines, Hospitals, -1);
			new ArrayInitializer().CreateArray(ref takingDiscPercentage, Disciplines, 0);

			wieght_d = 0;
			wieght_h = 0;
			wieght_w = 0;
			wieght_ch = 0;
			ProgramID = -1;
			isProspective = true;
			K_AllDiscipline = 0;
			overseaReq = false;
			//MaxDesir = 0;
			//MaxDesireDis = 0;
			//MaxDesireHos = 0;
		}

		public void sortPrf(int Hospitals, int Disciplines, int DisciplineGr , int HospitalWard, AllData allData)
		{
			sortedPrf = new ArrayList();
			
			K_AllDiscipline = 0;
			for (int g = 0; g < DisciplineGr; g++)
			{
				K_AllDiscipline += ShouldattendInGr_g[g];
			}
			for (int d = 0; d < Disciplines ; d++)
			{
				for (int h = 0; h < Hospitals; h++)
				{
					
					double MaxDesire = -1;
					for (int w = 0; w < HospitalWard ; w++)
					{
						if (allData.Hospital[h].Hospital_dw[d][w])
						{
							MaxDesire = (Prf_d[d] * wieght_d 
										+ Prf_h[h] * wieght_h 
										+ allData.TrainingPr[ProgramID].weight_p * allData.TrainingPr[ProgramID].Prf_d[d]) * takingDiscPercentage[d] ;
						}
					}
					if (MaxDesire!=-1)
					{
						int counter = 0;
						foreach (DesirePos item in sortedPrf)
						{
							if (item.Desire > MaxDesire)
							{
								counter++;
							}
							else
							{
								break;
							}
						}
						sortedPrf.Insert(counter, new DesirePos() {
							theD = d,
							theH = h,
							Desire = MaxDesire
						});
					}
					
				}
			}

			MaxPrf = 0;
			for (int c = 0; c < K_AllDiscipline; c++)
			{
				MaxPrf += ((DesirePos)sortedPrf[c]).Desire * allData.TrainingPr[ProgramID].CoeffObj_SumDesi;
			}
	
		}
		public void setKAllDiscipline(AllData allData)
		{
			for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				K_AllDiscipline += this.ShouldattendInGr_g[g];
			}
		}

		public void setAveInfo(AllData allData)
		{
			int counter = 0;
			AveDur = 0;
			ArrayList sequence = new ArrayList();
			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[d][g])
					{
						sequence.Add(allData.Discipline[d].Duration_p[ProgramID]);
						counter++;
						break;
					}
				}
			}
			double[] seq = new double[sequence.Count];
			for (int s = 0; s < sequence.Count; s++)
			{
				// to sort increasing 
				seq[s] = -((int)sequence[s]);
			}

			AveDur = Percentile(seq, 0.5);
		}

		public double Percentile(double[] sequence, double excelPercentile)
		{
			Array.Sort(sequence);

			//first set this data
			requieredTimeForRemianed = new double[K_AllDiscipline];
			
			for (int i = 0; i < K_AllDiscipline; i++)
			{
				for (int j = i; j < K_AllDiscipline; j++)
				{
					requieredTimeForRemianed[i] += (-sequence[j]);
				}
			}
			Array.Sort(requieredTimeForRemianed);
			double sum = 0;
			int N = sequence.Length;
			double n = (N - 1) * excelPercentile + 1;
			// Another method: double n = (N + 1) * excelPercentile;
			for (int i = 0; i < n && i < sequence.Count(); i++)
			{
				sum += sequence[i];
			}
			if (n > 1)
			{
				n = (int)n;
			}
			else
			{
				n = 1;
			}
			return -sum/n;
		}

		public void setThePercetage(AllData allData)
		{
			int totalDiscList = 0;
			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				for (int g = 0; g < allData.General.DisciplineGr; g++)
				{
					if (DisciplineList_dg[d][g])
					{
						totalDiscList++;
						break; // no mutual discipline
					}
				}
			}

			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				takingDiscPercentage[d] = K_AllDiscipline / totalDiscList;
			}

			for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				int totalGr = 0;
				for (int d = 0; d < allData.General.Disciplines; d++)
				{
					if (DisciplineList_dg[d][g])
					{
						totalGr++;
					}
				}
				for (int d = 0; d < allData.General.Disciplines; d++)
				{
					if (DisciplineList_dg[d][g])
					{
						takingDiscPercentage[d] = ShouldattendInGr_g[g] / totalGr;
					}
				}
			}

			for (int d = 0; d < allData.General.Disciplines; d++)
			{
				if (takingDiscPercentage[d] >= 0.95 && allData.Discipline[d].requiresSkill_p[ProgramID])
				{
					for (int dd = 0; dd < allData.General.Disciplines; dd++)
					{
						if (allData.Discipline[d].Skill4D_dp[dd][ProgramID])
						{
							takingDiscPercentage[dd] = 1;
						}
					}
				}
				for (int t = 0; t < allData.General.TimePriods; t++)
				{
					if (OverSea_dt[d][t])
					{
						takingDiscPercentage[d] = 1;
						for (int dd = 0; dd < allData.General.Disciplines; dd++)
						{
							if (FHRequirment_d[dd])
							{
								takingDiscPercentage[dd] = 1;
							}
						}
					}
				}
			}
		}
	}
}

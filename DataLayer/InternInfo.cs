using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
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
		public bool[][] DisciplineList_dg;
		public int[] ShouldattendInGr_g;
		public bool isProspective;
		public int K_AllDiscipline;
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
			for (int h = 0; h < Hospitals; h++)
			{
				SortedPrfH_pos[h] = h;
			}

			for (int d = 0; d < Disciplines; d++)
			{
				SortedPrfD_pos[d] = d;
			}

			for (int h = 0; h < Hospitals; h++)
			{
				for (int hh = h + 1; hh < Hospitals; hh++)
				{
					if (Prf_h[SortedPrfH_pos[hh]] > Prf_h[SortedPrfH_pos[h]])
					{
						int x = SortedPrfH_pos[hh];
						SortedPrfH_pos[hh] = SortedPrfH_pos[h];
						SortedPrfH_pos[h] = x;
					}
				}
			}

			for (int d = 0; d < Disciplines; d++)
			{
				for (int dd = 0; dd < Disciplines; dd++)
				{
					if (Prf_d[SortedPrfD_pos[dd]] < Prf_d[SortedPrfD_pos[d]])
					{
						int x = SortedPrfD_pos[dd];
						SortedPrfD_pos[dd] = SortedPrfD_pos[d];
						SortedPrfD_pos[d] = x;
					}
				}
			}

			for (int d = 0; d < Disciplines; d++)
			{
				for (int h = 0; h < Hospitals; h++)
				{
					AllDes_dh[d][h] = Prf_d[d] * wieght_d + Prf_h[h] * wieght_h + allData.TrainingPr[ProgramID].weight_p * allData.TrainingPr[ProgramID].Prf_d[d];
				}
			}
		}
		public void setKAllDiscipline(AllData allData)
		{
			for (int g = 0; g < allData.General.DisciplineGr; g++)
			{
				K_AllDiscipline += this.ShouldattendInGr_g[g];
			}
		}
	}
}

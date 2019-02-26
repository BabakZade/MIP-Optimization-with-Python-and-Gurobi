using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class InstanceSetting
	{
		// ratio of ward to discipline
		public double R_wd;

		// ratio of discipline group size to attended course from that group
		public double R_gk;

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

		// ratio of mutual discipline
		public double R_muDp;
		
		// preference max value 
		public int PrfMaxValue;

		// Coefficient Max value 
		public int CoefficientMaxValue;

		// isprespective
		public double Prespective;

		public InstanceSetting(double R_wd, double R_gk, double R_wh, 
							   double R_Trel, double R_hi, int TotalIntern, 
							   int TotalDiscipline, double R_dMin, int MinDem, int MaxDem,
							   int EmrDem, int ResDem, double R_muDp, int PrfMaxValue, int CoefficientMaxValue,
							   double Prespective)
		{
			this.R_wd = R_wd;
			this.R_gk = R_gk;
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
			this.PrfMaxValue = PrfMaxValue;
			this.CoefficientMaxValue = CoefficientMaxValue;
			this.Prespective = Prespective;

		}

	}
}

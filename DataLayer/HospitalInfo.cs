using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class HospitalInfo
	{
		public string Name;
		public int SQLID;
		public bool[][] Hospital_dw;
		public int[][] HospitalMaxDem_tw;
		public int[][] HospitalMinDem_tw;
        public int[] HospitalMaxYearly_w;
        public int[] HospitalMinYearly_w;
        public int[][] ReservedCap_tw;
		public int[][] EmergencyCap_tw;
		public bool[] InToRegion_r;
		public HospitalInfo(int Disciplins, int TimePeriods, int HospitalWard, int Region)
		{
			Name = "Program";
			SQLID = 0;			
			new ArrayInitializer().CreateArray(ref Hospital_dw, Disciplins, HospitalWard, false);
			new ArrayInitializer().CreateArray(ref HospitalMaxDem_tw,TimePeriods, HospitalWard, 0);
			new ArrayInitializer().CreateArray(ref HospitalMinDem_tw, TimePeriods, HospitalWard, 0);
			new ArrayInitializer().CreateArray(ref ReservedCap_tw, TimePeriods, HospitalWard, 0);
			new ArrayInitializer().CreateArray(ref EmergencyCap_tw, TimePeriods, HospitalWard, 0);
			new ArrayInitializer().CreateArray(ref InToRegion_r, Region, false);
            new ArrayInitializer().CreateArray(ref HospitalMaxYearly_w, HospitalWard, 0);
            new ArrayInitializer().CreateArray(ref HospitalMinYearly_w, HospitalWard, 0);
        }
	}
}

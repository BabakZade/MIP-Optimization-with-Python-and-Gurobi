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
		public bool[] Hospital_d;
		public int[][] HospitalMaxDem_td;
		public int[][] HospitalMinDem_td;
		public HospitalInfo(int Disciplins, int TimePeriods)
		{
			Name = "Program";
			SQLID = 0;			
			new ArrayInitializer().CreateArray(ref Hospital_d, Disciplins, false);
			new ArrayInitializer().CreateArray(ref HospitalMaxDem_td,TimePeriods, Disciplins, 0);
			new ArrayInitializer().CreateArray(ref HospitalMinDem_td, TimePeriods, Disciplins, 0);
		}
	}
}

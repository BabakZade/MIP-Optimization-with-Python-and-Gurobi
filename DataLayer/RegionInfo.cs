using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class RegionInfo
	{
		public string Name;
		public int SQLID;
		public int[] AvaAcc_t;
		public RegionInfo( int TimePeriods)
		{
			Name = "Region";
			SQLID = 0;
			new ArrayInitializer().CreateArray(ref AvaAcc_t, TimePeriods, 0);
			
		}
	}
}

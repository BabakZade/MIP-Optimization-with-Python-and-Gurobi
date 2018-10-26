using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class TrainingProgramInfo
	{
		public string Name;
		public int SQLID;
		public int AcademicYear;
		public bool[] Program_d;
		public int MandatoryD;
		public int ArbitraryD;
		public bool[] PrManDis_d;

		public TrainingProgramInfo(int Disciplins)
		{
			Name = "Program";
			SQLID = 0;
			AcademicYear = 0;
			new ArrayInitializer().CreateArray(ref Program_d,Disciplins,false);
			MandatoryD = 0;
			ArbitraryD = 0;
			new ArrayInitializer().CreateArray(ref PrManDis_d, Disciplins, false);
		}
	}
}

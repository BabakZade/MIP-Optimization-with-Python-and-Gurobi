using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class AllData
	{
		public InternInfo[] Intern;
		public RegionInfo[] Region;
		public GeneralInfo General;
		public DisciplineInfo[] Discipline;
		public HospitalInfo[] Hospital;
		public TrainingProgramInfo[] TrainingPr;
		public AlgorithmSettings AlgSettings;
	

		public int[] PrDisc_i;
		public int[] PrHosp_i;
		public int[] PrChan_i;
		public int[] PrWait_i;
	}
}

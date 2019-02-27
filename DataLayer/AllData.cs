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
		public SetAllPathForResult allPath;
		public AllData() { }
		public AllData(string Expriment, string Methodology, string InsGroup)
		{
			allPath = new SetAllPathForResult(Expriment, Methodology, InsGroup);
		}
	}
}

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
		public int[] Prf_d;
		public bool[][] InvolvedDiscipline_gd;
		public int weight_p;
		public double CoeffObj_SumDesi;
		public double CoeffObj_MinDesi;
		public double CoeffObj_ResCap;
		public double CoeffObj_EmrCap;
		public double CoeffObj_NotUsedAcc;
		public int DiscChangeInOneHosp;
		public TrainingProgramInfo(int Disciplins, int DisciplineGr)
		{
			Name = "Program";
			SQLID = 0;
			AcademicYear = 0;
			new ArrayInitializer().CreateArray(ref Prf_d, Disciplins, 0);
			new ArrayInitializer().CreateArray(ref InvolvedDiscipline_gd, DisciplineGr, Disciplins, false);
			weight_p = 0;
			CoeffObj_SumDesi = 0;
			CoeffObj_MinDesi = 0;
			CoeffObj_ResCap = 0;
			CoeffObj_EmrCap = 0;
			CoeffObj_NotUsedAcc = 0;
			DiscChangeInOneHosp = Disciplins;
		}
	}
}

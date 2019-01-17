using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class DisciplineInfo
	{
		public string Name;
		public int SQLID;
		public bool[][] Skill4D_dp;
		public bool[] requiresSkill_p;
		public bool[] requiredLater_p;
		public int[] Duration_p;

		public DisciplineInfo(int Disciplines, int Programs)
		{
			Name = "";
			SQLID = 0;
			new ArrayInitializer().CreateArray(ref Skill4D_dp, Disciplines,Programs, false);
			new ArrayInitializer().CreateArray(ref Duration_p, Programs, 0);
			new ArrayInitializer().CreateArray(ref requiresSkill_p, Programs, false);
			new ArrayInitializer().CreateArray(ref requiredLater_p, Programs, false);
		}


	}
}

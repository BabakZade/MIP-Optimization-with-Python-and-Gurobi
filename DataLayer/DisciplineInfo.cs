using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class DisciplineInfo
	{
		public string Name;
		public int ID;
		public int SQLID;
		public bool[][] Skill4D_dp;
		public bool[] requiresSkill_p;
		public bool[] requiredLater_p;
		public int[] Duration_p;
		public int[] CourseCredit_p;
		public bool isRare;
		public double howRare;
		public DisciplineInfo(int Disciplines, int Programs)
		{
			Name = "";
			SQLID = 0;
			new ArrayInitializer().CreateArray(ref Skill4D_dp, Disciplines,Programs, false);
			new ArrayInitializer().CreateArray(ref Duration_p, Programs, 0);
			new ArrayInitializer().CreateArray(ref requiresSkill_p, Programs, false);
			new ArrayInitializer().CreateArray(ref requiredLater_p, Programs, false);
			new ArrayInitializer().CreateArray(ref CourseCredit_p, Programs, 1);
		}

		public void setIsRare(AllData data)
		{
			isRare = false;
			int couter = 0;
			howRare = 0;
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				bool exist = false;
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					if (data.Hospital[h].Hospital_dw[this.ID][w])
					{
						exist = true;
						couter++;
						break;
					}
				}
				if (exist && !isRare)
				{
					isRare = false;
				}
				else
				{
					isRare = true;					
				}
			}
			howRare = couter / data.General.Hospitals;
		}
	}
}

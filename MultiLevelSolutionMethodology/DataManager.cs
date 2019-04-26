using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace MultiLevelSolutionMethodology
{
	public class DataManager
	{
		public int[] KallDisc_i;
		public int[][] KgroupDisc_ig;
		public int timeWhole;
		public AllData localData;
		public DataManager(AllData allData)
		{
			localData = allData;
			setData();
		}
		public void setData()
		{
			KallDisc_i = new int[localData.General.Interns];
			KgroupDisc_ig = new int[localData.General.Interns][];
			for (int i = 0; i < localData.General.Interns; i++)
			{
				KallDisc_i[i] = localData.Intern[i].K_AllDiscipline;
				KgroupDisc_ig[i] = new int[localData.General.DisciplineGr];
				for (int g = 0; g < localData.General.DisciplineGr; g++)
				{
					KgroupDisc_ig[i][g] = localData.Intern[i].ShouldattendInGr_g[g];
				}
			}
			timeWhole = localData.AlgSettings.MIPTime;
		}
		public void SetDataPrTr(bool[] trPr)
		{
			for (int i = 0; i < localData.General.Interns; i++)
			{
				localData.Intern[i].K_AllDiscipline = 0;
				
				for (int g = 0; g < localData.General.DisciplineGr; g++)
				{
					localData.Intern[i].ShouldattendInGr_g[g] = 0;
				}
			}
			int counter = 0;
			for (int p = 0; p < localData.General.TrainingPr; p++)
			{
				if (trPr[p])
				{
					counter++;
					for (int i = 0; i < localData.General.Interns; i++)
					{
						if (localData.Intern[i].ProgramID == p)
						{
							localData.Intern[i].K_AllDiscipline = KallDisc_i[i];

							for (int g = 0; g < localData.General.DisciplineGr; g++)
							{
								localData.Intern[i].ShouldattendInGr_g[g] = KgroupDisc_ig[i][g];
							}
						}
						
					}
				}
			}
			localData.AlgSettings.MIPTime = timeWhole ;
		}

		public void ResetToDefaultData()
		{
			bool[] setAll = new bool[localData.General.TrainingPr];
			for (int p = 0; p < localData.General.TrainingPr; p++)
			{
				setAll[p] = true;
			}
			SetDataPrTr(setAll);
			
		}
	}
}

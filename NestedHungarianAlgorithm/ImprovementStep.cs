using System;
using System.Collections.Generic;
using System.Text;
using NestedDynamicProgrammingAlgorithm;
using DataLayer;
namespace NestedHungarianAlgorithm
{
	public class ImprovementStep
	{
		public int[] InternDesMin_p;
		public NHA incumbentSol;
		public AllData data;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public ImprovementStep(AllData alldata)
		{
			data = alldata;
			Initial();
			new InternBasedLocalSearch(data, incumbentSol.nhaResult);

		}
		public void Initial()
		{
		}
	}
}
                                                                                                               
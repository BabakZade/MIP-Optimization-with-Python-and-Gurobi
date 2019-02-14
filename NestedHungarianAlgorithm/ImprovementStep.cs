using System;
using System.Collections;
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
		public ImprovementStep(AllData alldata, OptimalSolution incumbentSol, ArrayList HungarianActiveList)
		{
			data = alldata;
			Initial();
			BucketLinsLocalSearch bucketLinsLocal= new BucketLinsLocalSearch(0.5, data, incumbentSol, HungarianActiveList);
			new InternBasedLocalSearch(data, bucketLinsLocal.improvedSolution);
			

		}
		public void Initial()
		{
		}
	}
}
                                                                                                               
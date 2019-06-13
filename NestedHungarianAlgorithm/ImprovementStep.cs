using System;
using System.Collections;
using System.Text;
using NestedDynamicProgrammingAlgorithm;
using DataLayer;
using System.Diagnostics;
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
		public int TimeForbucketListImp;
		public int TimeForInternBaseImp;
		public int TimeForResEmrImp;
		public BucketLinsLocalSearch bucketLinsLocal;
		public InternBasedLocalSearch internBasedLocalSearch;
		public DemandBaseLocalSearch demandBaseLocalSearch;
		
		public ImprovementStep(AllData alldata, OptimalSolution incumbentSol, ArrayList HungarianActiveList, string Name)
		{
			data = alldata;
			HungarianActiveList = new ArrayList();
			Initial();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			//bucketLinsLocal = new BucketLinsLocalSearch(data, incumbentSol, HungarianActiveList,Name);
			stopwatch.Stop();
			TimeForbucketListImp = (int)stopwatch.ElapsedMilliseconds / 1000;
			stopwatch.Reset();
			stopwatch.Restart();
			
			stopwatch.Reset();
			stopwatch.Restart();
			internBasedLocalSearch = new InternBasedLocalSearch(data, incumbentSol, Name);
			stopwatch.Stop();
			TimeForInternBaseImp = (int)stopwatch.ElapsedMilliseconds / 1000;
			demandBaseLocalSearch = new DemandBaseLocalSearch(data, internBasedLocalSearch.finalSol , Name);
			stopwatch.Stop();
			TimeForResEmrImp = (int)stopwatch.ElapsedMilliseconds / 1000;
			TimeForbucketListImp += TimeForResEmrImp;

		}
		public void Initial()
		{
		}
		
	}
}
                                                                                                               
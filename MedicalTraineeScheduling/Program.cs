using System;

namespace MedicalTraineeScheduling
{	
	public class Program
	{
		public int xx;
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			//new DataLayer.WriteInformation("","inst.txt");

			new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(new DataLayer.ReadInformation("", "MIP", "inst.txt").data, "", "inst");

			new NestedHungarianAlgorithm.NHA(new DataLayer.ReadInformation("","NHA","inst.txt").data);

			
		}
	}
}

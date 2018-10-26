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
			new DataLayer.ReadInformation("inst.txt");
		}
	}
}

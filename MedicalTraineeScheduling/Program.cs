using System;
using System.Collections;
using DataLayer;
using System.IO;
using System.Diagnostics;


namespace MedicalTraineeScheduling
{
	public class Program
	{
		public static int InstanceSize;
		static void Main(string[] args)
		{
            //new SolveMe().createInstanceObjCoeff(5);
            new SolveMe().solveThisDataSetBP(1, 1, "ResourcePool", "BPCP");
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            //new SolveMe().solveAddaptiveWeight();
            double elapsedTime = stopwatch.ElapsedMilliseconds / 1000;
            Console.WriteLine(elapsedTime);
            
		}

	}
}

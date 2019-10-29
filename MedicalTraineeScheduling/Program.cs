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
            // new SolveMe().solveObjCoeffGammaDelta(24,5,8);
            //new SolveMe().solveThisDataSet(9, 5, "ResourcePool", "NHA");
            new SolveMe().solveEConstriant();
            
		}

	}
}

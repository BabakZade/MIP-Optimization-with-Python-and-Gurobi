using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataLayer
{
	public class AlgorithmSettings
	{
		// times
		public int MIPTime;
		public int MasterTime;
		public int SubTime;
		public int NodeTime;
		public int BPTime;
		public int BigM;

		// Epsilon
		public double RHSepsi;
		public double RCepsi;
		public double TAILepsi;
	}
}

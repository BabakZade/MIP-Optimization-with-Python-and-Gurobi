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
		public int MotivationBM;

        // evolutionary algorithm for addapted weight setting
        public int sizeOfArchivedSet;
        public int sizeOfEvolutionaySet;
        public int totalIteration;

        public AlgorithmSettings() {
            sizeOfArchivedSet = 100;
            sizeOfEvolutionaySet = 100;
            totalIteration = 20;
        }

		// Epsilon
		public double RHSepsi;
		public double RCepsi;
		public double TAILepsi;

		//imporvement step
		public double bucketBasedImpPercentage;
		public double internBasedImpPercentage;
	}
}

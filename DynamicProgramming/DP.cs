using System;
using System.Collections;
using System.Text;
using DataLayer;


namespace NestedDynamicProgrammingAlgorithm
{
	public class DP
	{
		public ArrayList Allsol;
		public DPStage root;
		public DP(AllData alldata, int theI)
		{
			root = new DPStage(ref Allsol, alldata, new DPStage(),theI, 0);
			for (int t = 0; t < alldata.General.TimePriods; t++)
			{

			}
		}
	}
}

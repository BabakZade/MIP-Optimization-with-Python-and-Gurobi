using System;
using System.Collections;
using DataLayer;
namespace MedicalTraineeScheduling
{
	public class Program
	{
		public static int InstanceSize;
		static void Main(string[] args)
		{

			InstanceSize = 5;
			DataLayer.InstanceSetting inssetting = new InstanceSetting();
			int groupCounter = 0;
			foreach (InstanceSetting insset in inssetting.AllinstanceSettings)
			{
				groupCounter++;
				for (int i = 0; i < InstanceSize; i++)
				{
					SetAllPathForResult allpath = new DataLayer.SetAllPathForResult("Test", "", "G_"+groupCounter);
					new WriteInformation(insset, allpath.InstanceLocation + "G_" + groupCounter + "\\", "Instance_" + i);
				}
			}
			

			//new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(new DataLayer.ReadInformation("", "MIP", "inst.txt").data, "", "inst");



			//new NestedHungarianAlgorithm.NHA(new DataLayer.ReadInformation("","NHA","inst.txt").data);

			//new NestedDynamicProgrammingAlgorithm.NDPA(new DataLayer.ReadInformation("", "NDPA", "inst.txt").data);
		}
		
	}
}

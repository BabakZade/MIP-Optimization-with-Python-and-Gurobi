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

			InstanceSize = 5;
			DataLayer.InstanceSetting inssetting = new InstanceSetting();
			int groupCounter = 0;
			//foreach (InstanceSetting insset in inssetting.AllinstanceSettings)
			//{
			//	groupCounter++;
			//	for (int i = 0; i < InstanceSize; i++)
			//	{
			//		SetAllPathForResult allpath = new DataLayer.SetAllPathForResult("Integration", "", "G_" + groupCounter);
			//		WriteInformation TMPinstance = new WriteInformation(insset, allpath.InstanceLocation + "G_" + groupCounter + "\\", "Instance_" + i);
			//		using (StreamWriter file = new StreamWriter(allpath.InstanceLocation + "\\FeasibleResult.txt", true))
			//		{
			//			string xx = String.Join(" \t ", TMPinstance.FeasibleSolution.MinDis);
			//			file.WriteLine(i + "\t" + TMPinstance.FeasibleSolution.Obj + "\t" + TMPinstance.FeasibleSolution.AveDes + "\t" + xx
			//				+ "\t" + TMPinstance.FeasibleSolution.EmrDemand + "\t" + TMPinstance.FeasibleSolution.ResDemand
			//				+ "\t" + TMPinstance.FeasibleSolution.SlackDem + "\t" + TMPinstance.FeasibleSolution.NotUsedAccTotal);
			//		}

			//	}
			//}
			SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("Complexity", "NHA", "");
			for (int g = 0; g < 9; g++)
			{
				for (int i = 0; i < InstanceSize; i++)
				{
					if (g < 3)
					{
						continue;
					}
					ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "Complexity", "MIP", "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					MultiLevelSolutionMethodology.SequentialMethodology xy = new MultiLevelSolutionMethodology.SequentialMethodology(read.data, i.ToString());
					GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
					//NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
					stopwatch.Stop();
					int time = (int)stopwatch.ElapsedMilliseconds / 1000;
					using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
					{
						string output = i + "\t" + time + "\t" + xy.objFunction; 
						for (int p = 0; p < read.data.General.TrainingPr; p++)
						{
							output += "\t" + xy.ElappesedTime_p[p]
							// NHA first Sol
							+ "\t" + xy.finalSol_p[p].Obj + "\t" + xy.finalSol_p[p].AveDes + "\t" + String.Join(" \t ", xy.finalSol_p[p].MinDis)
							+ "\t" + xy.finalSol_p[p].EmrDemand + "\t" + xy.finalSol_p[p].ResDemand
							+ "\t" + xy.finalSol_p[p].SlackDem + "\t" + xy.finalSol_p[p].NotUsedAccTotal;
						}
						file.WriteLine(output);
					}
				}

			}


			//new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(new DataLayer.ReadInformation("", "MIP", "inst.txt").data, "", "inst");



			//new NestedHungarianAlgorithm.NHA(new DataLayer.ReadInformation("","NHA","inst.txt").data);

			//new NestedDynamicProgrammingAlgorithm.NDPA(new DataLayer.ReadInformation("", "NDPA", "inst.txt").data);
		}

	}
}

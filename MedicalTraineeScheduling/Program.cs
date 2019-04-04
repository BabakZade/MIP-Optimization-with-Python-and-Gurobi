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
			//		SetAllPathForResult allpath = new DataLayer.SetAllPathForResult("DesCoeff", "", "");
			//		WriteInformation TMPinstance = new WriteInformation(insset, allpath.InstanceLocation + "\\", "Instance_" + i);
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

			//string[] nameCoeff = new string[] { "W_h", "W_d", "W_p", "W_c", "W_w" };
			//string[] level = new string[4] { "00", "01", "05", "10" };
			//for (int g = 0; g < 5; g++)
			//{
			//	for (int l = 0; l < 1; l++)
			//	{

			//		for (int i = 0; i < InstanceSize; i++)
			//		{
			//			groupCounter++;
			//			if (groupCounter < 15)
			//			{
			//				continue;
			//			}

			//			ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ResourcePool", "NHA", nameCoeff[g] + level[l], "Instance_" + i + ".txt");
			//			Stopwatch stopwatch = new Stopwatch();
			//			read.data.AlgSettings.internBasedImpPercentage = 0.2;
			//			//GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
			//			stopwatch.Start();
			//			NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
			//			stopwatch.Stop();
			//			int time = (int)stopwatch.ElapsedMilliseconds / 1000;
						
			//			using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
			//			{
			//				file.WriteLine(i + "\t" + time
			//					// NHA first Sol
			//					+ "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
			//					+ "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
			//					+ "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
			//					+ "\t" + nha.nhaResult.wieghterSumInHosPrf + "\t" + nha.nhaResult.wieghterSumInDisPrf
			//					 + "\t" + nha.nhaResult.wieghterSumPrDisPrf + "\t" + nha.nhaResult.wieghterSumInChnPrf
			//					 + "\t" + nha.nhaResult.wieghterSumInWaiPrf

			//					// NHA bucket list improvement
			//					+ "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.Obj + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.bucketLinsLocal.improvedSolution.MinDis)
			//					+ "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.EmrDemand + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.ResDemand
			//					+ "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.SlackDem + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.NotUsedAccTotal
			//					+ "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.wieghterSumInHosPrf + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.wieghterSumInDisPrf
			//					 + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.wieghterSumPrDisPrf + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.wieghterSumInChnPrf
			//					 + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.wieghterSumInWaiPrf

			//					// NHA intern based improvement 
			//					+ "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghterSumInHosPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghterSumInDisPrf
			//					 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghterSumPrDisPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghterSumInChnPrf
			//					 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghterSumInWaiPrf

			//					);
			//			}
			//		}
			//	}


			//}

			for (int g = 0; g < 9; g++)
				{
					for (int i = 0; i < InstanceSize; i++)
					{
					groupCounter++;
					if (groupCounter< 13)
					{
						continue;
					}
						ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ResourcePool", "NHA" , "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
						
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						//MultiLevelSolutionMethodology.SequentialMethodology xy = new MultiLevelSolutionMethodology.SequentialMethodology(read.data, i.ToString());
						//GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
						NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
						stopwatch.Stop();
						int time = (int)stopwatch.ElapsedMilliseconds / 1000;
						using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
						{

							file.WriteLine(i + "\t" + time
								// NHA first Sol
								+ "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
								+ "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
								+ "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
								// NHA bucket list improvement
								+ "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.Obj + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.bucketLinsLocal.improvedSolution.MinDis)
								+ "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.EmrDemand + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.ResDemand
								+ "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.SlackDem + "\t" + nha.improvementStep.bucketLinsLocal.improvedSolution.NotUsedAccTotal
								// NHA intern based improvement 
								+ "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
								+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
								+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal

								);
						}
						//using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
						//{
						//	string output = i + "\t" + time + "\t" + xy.objFunction; 
						//	for (int p = 0; p < read.data.General.TrainingPr; p++)
						//	{
						//		output += "\t" + xy.ElappesedTime_p[p]
						//		// NHA first Sol
						//		+ "\t" + xy.finalSol_p[p].Obj + "\t" + xy.finalSol_p[p].AveDes + "\t" + String.Join(" \t ", xy.finalSol_p[p].MinDis)
						//		+ "\t" + xy.finalSol_p[p].EmrDemand + "\t" + xy.finalSol_p[p].ResDemand
						//		+ "\t" + xy.finalSol_p[p].SlackDem + "\t" + xy.finalSol_p[p].NotUsedAccTotal;
						//	}
						//	file.WriteLine(output);
						//}
					}

				}
			
			


			//new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(new DataLayer.ReadInformation("", "MIP", "inst.txt").data, "", "inst");



			//new NestedHungarianAlgorithm.NHA(new DataLayer.ReadInformation("","NHA","inst.txt").data);

			//new NestedDynamicProgrammingAlgorithm.NDPA(new DataLayer.ReadInformation("", "NDPA", "inst.txt").data);
		}

	}
}

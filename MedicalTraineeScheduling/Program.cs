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
            //    groupCounter++;
            //    for (int i = 0; i < InstanceSize; i++)
            //    {
            //        SetAllPathForResult allpath = new DataLayer.SetAllPathForResult("TestCCYD", "", "G_" + groupCounter);
            //        WriteInformation TMPinstance = new WriteInformation(insset, allpath.InsGroupLocation + "\\", "Instance_" + i, false);
            //        using (StreamWriter file = new StreamWriter(allpath.InstanceLocation + "\\FeasibleResult.txt", true))
            //        {
            //            string xx = String.Join(" \t ", TMPinstance.FeasibleSolution.MinDis);
            //            file.WriteLine(i + "\t" + TMPinstance.FeasibleSolution.Obj + "\t" + TMPinstance.FeasibleSolution.AveDes + "\t" + xx
            //                + "\t" + TMPinstance.FeasibleSolution.EmrDemand + "\t" + TMPinstance.FeasibleSolution.ResDemand
            //                + "\t" + TMPinstance.FeasibleSolution.SlackDem + "\t" + TMPinstance.FeasibleSolution.NotUsedAccTotal);
            //        }

            //    }
            //}
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeff", "NHA", "");
			string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
			string[] level = new string[4] { "00", "01", "05", "10" };
			//for (int g = 0; g < 6; g++)
			//{
			//	for (int l = 0; l < 4; l++)
			//	{

			//		for (int i = 0; i < InstanceSize; i++)
			//		{
			//			groupCounter++;

			//			ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeff", "NHA", nameCoeff[g] + level[l], "Instance_" + i + ".txt");
			//			Stopwatch stopwatch = new Stopwatch();
			//			read.data.AlgSettings.internBasedImpPercentage = 0.5;
			//			read.data.AlgSettings.bucketBasedImpPercentage = 1;
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
			//					+ "\t" + nha.nhaResult.wieghtedSumInHosPrf + "\t" + nha.nhaResult.wieghtedSumInDisPrf
			//					 + "\t" + nha.nhaResult.wieghtedSumPrDisPrf + "\t" + nha.nhaResult.wieghtedSumInChnPrf
			//					 + "\t" + nha.nhaResult.wieghtedSumInWaiPrf

			//					// NHA bucket list improvement
			//					+ "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
			//					+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
			//					+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
			//					+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInHosPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInDisPrf
			//					 + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInChnPrf
			//					 + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInWaiPrf

			//					// NHA intern based improvement 
			//					+ "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
			//					+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInHosPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInDisPrf
			//					 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInChnPrf
			//					 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInWaiPrf

			//					);
			//			}
			//		}
			//	}


			//}
			double[] bl = new double[] { 0.0, 0.10, 0.25, 0.50, 0.75, 1.00 };
			double[] iCh = new double[] { 0.0, 0.10, 0.25, 0.50, 0.75, 1.00 };

			//for (int ic = 0; ic < 6; ic++)
			//{
			//	for (int b = 0; b < 6; b++)
			//	{
			//		for (int g = 0; g < 9; g++)
			//		{
			//			for (int i = 0; i < InstanceSize; i++)
			//			{
			//				GC.Collect();
			//				groupCounter++;
			//				if (groupCounter < 18)
			//				{
			//					//continue;
			//				}
			//				ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "Complexity", "NHASensitivity", "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
			//				read.data.AlgSettings.internBasedImpPercentage = iCh[ic];
			//				read.data.AlgSettings.bucketBasedImpPercentage = bl[b];
			//				Stopwatch stopwatch = new Stopwatch();
			//				stopwatch.Start();
			//				// MultiLevelSolutionMethodology.SequentialMethodology xy = new MultiLevelSolutionMethodology.SequentialMethodology(read.data, i.ToString());
			//				// GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
			//				NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
			//				stopwatch.Stop();
			//				int time = (int)stopwatch.ElapsedMilliseconds / 1000;
			//				using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
			//				{

			//					file.WriteLine(i + "\t" + time
			//						// NHA first Sol
			//						+ "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
			//						+ "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
			//						+ "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
			//						// NHA bucket list improvement
			//						+ "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
			//						+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
			//						+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
			//						// NHA intern based improvement
			//						+ "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
			//						+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
			//						+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
			//						+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxDisier + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxMin
			//						+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxRes + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxEmr
			//						+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxAcc

			//						);
			//				}
			//				//using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
			//				//{
			//				//	string output = i + "\t" + time + "\t" + xy.objFunction;
			//				//	for (int p = 0; p < read.data.General.TrainingPr; p++)
			//				//	{
			//				//		output += "\t" + xy.ElappesedTime_p[p]
			//				//		// NHA first Sol
			//				//		+ "\t" + xy.finalSol_p[p].Obj + "\t" + xy.finalSol_p[p].AveDes + "\t" + String.Join(" \t ", xy.finalSol_p[p].MinDis)
			//				//		+ "\t" + xy.finalSol_p[p].EmrDemand + "\t" + xy.finalSol_p[p].ResDemand
			//				//		+ "\t" + xy.finalSol_p[p].SlackDem + "\t" + xy.finalSol_p[p].NotUsedAccTotal;
			//				//	}
			//				//	file.WriteLine(output);
			//				//}
			//			}

			//		}
			//	}

			//}

			for (int g = 0; g < 9 ; g++)
			{
				for (int i = 0; i < InstanceSize; i++)
				{
					groupCounter++;
					if (groupCounter < 31)
					{
						//continue;
					}
					ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "TestCCYD", "NHA", "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
					read.data.AlgSettings.bucketBasedImpPercentage = 1;
					read.data.AlgSettings.internBasedImpPercentage = 0.5;
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					//MultiLevelSolutionMethodology.SequentialMethodology xy = new MultiLevelSolutionMethodology.SequentialMethodology(read.data, i.ToString());
					//GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString(), false, 7200);
					NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
					stopwatch.Stop();
					int time = (int)stopwatch.ElapsedMilliseconds / 1000;
                    //using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
                    //{

                    //	file.WriteLine(i + "\t" + time
                    //		// NHA first Sol
                    //		+ "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
                    //		+ "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
                    //		+ "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
                    //		// NHA bucket list improvement
                    //		+ "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
                    //		+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
                    //		+ "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
                    //		// NHA intern based improvement 
                    //		+ "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
                    //		+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
                    //		+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
                    //		+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxDisier + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxMin
                    //		+ "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxRes + "\t" + nha.improvementStep.internBasedLocalSearch.MaxProcessedNode
                    //		+ "\t" + nha.improvementStep.internBasedLocalSearch.RealProcessedNode
                    //		);
                    //}
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

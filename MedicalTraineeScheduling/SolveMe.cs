using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

using System.IO;
using System.Diagnostics;

namespace MedicalTraineeScheduling
{
    public class SolveMe
    {
        public void createInstanceObjCoeff(int totalInstance)
        {
            DataLayer.InstanceSetting inssetting = new InstanceSetting();
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "NHA", "");
            foreach (InstanceSetting insset in inssetting.AllinstanceSettings)
            {
                for (int i = 0; i < totalInstance; i++)
                {
                    SetAllPathForResult allpath = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "", "G_");
                    WriteInformation TMPinstance = new WriteInformation(insset, allpath.InstanceLocation + "\\", "Instance_" + i, false);
                    using (StreamWriter file = new StreamWriter(allpath.InstanceLocation + "\\FeasibleResult.txt", true))
                    {
                        string xx = String.Join(" \t ", TMPinstance.FeasibleSolution.MinDis);
                        file.WriteLine(i + "\t" + TMPinstance.FeasibleSolution.Obj + "\t" + TMPinstance.FeasibleSolution.AveDes + "\t" + xx
                            + "\t" + TMPinstance.FeasibleSolution.EmrDemand + "\t" + TMPinstance.FeasibleSolution.ResDemand
                            + "\t" + TMPinstance.FeasibleSolution.SlackDem + "\t" + TMPinstance.FeasibleSolution.NotUsedAccTotal);
                    }

                }
            }
        }

        public void solveObjCoeffGammaDelta(int totalGroup, int totalInstance, int totalMeasure)
        {
            string[] nameCoeff = new string[] { "Alpha", "Beta", "Gamma", "Lambda", "Noe" };
            string[] level = new string[] { "00", "01", "10", "100" };

            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "NHA", "");
            double[][][] result_checking_coeff_instance_measure = new double[totalGroup][][];
            for (int n = 0; n < totalGroup; n++)
            {
                result_checking_coeff_instance_measure[n] = new double[totalInstance][];
                for (int l = 0; l < totalInstance; l++)
                {
                    result_checking_coeff_instance_measure[n][l] = new double[totalMeasure];
                    for (int i = 0; i < totalMeasure; i++)
                    {
                        result_checking_coeff_instance_measure[n][l][i] = 0;
                    }
                }

            }
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            for (int g = 0; g < totalGroup; g++)
            {
                
                elapsed.Restart();
                for (int i = 0; i < totalInstance; i++)
                {
                    ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeffEjor", "NHA", "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
                    Stopwatch stopwatch = new Stopwatch();
                    read.data.AlgSettings.internBasedImpPercentage = 0.5;
                    read.data.AlgSettings.bucketBasedImpPercentage = 1;
                    NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
                    stopwatch.Stop();
                    double time = (int)stopwatch.ElapsedMilliseconds / 1000;

                    result_checking_coeff_instance_measure[g][i][0] = nha.improvementStep.demandBaseLocalSearch.Global.Obj;
                    result_checking_coeff_instance_measure[g][i][1] = nha.improvementStep.demandBaseLocalSearch.Global.AveDes;
                    result_checking_coeff_instance_measure[g][i][2] = nha.improvementStep.demandBaseLocalSearch.Global.MinDis[0];
                    result_checking_coeff_instance_measure[g][i][3] = nha.improvementStep.demandBaseLocalSearch.Global.MinDis[1];
                    result_checking_coeff_instance_measure[g][i][4] = -nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand;
                    result_checking_coeff_instance_measure[g][i][5] = -nha.improvementStep.demandBaseLocalSearch.Global.ResDemand;
                    result_checking_coeff_instance_measure[g][i][6] = -nha.improvementStep.demandBaseLocalSearch.Global.SlackDem;
                    result_checking_coeff_instance_measure[g][i][7] = -nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal;



                    using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
                    {
                        file.WriteLine(i + "\t" + time
                            // NHA first Sol
                            + "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
                            + "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
                            + "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
                            + "\t" + nha.nhaResult.wieghtedSumInHosPrf + "\t" + nha.nhaResult.wieghtedSumInDisPrf
                             + "\t" + nha.nhaResult.wieghtedSumPrDisPrf + "\t" + nha.nhaResult.wieghtedSumInChnPrf
                             + "\t" + nha.nhaResult.wieghtedSumInWaiPrf

                            // NHA bucket list improvement
                            + "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
                            + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
                            + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
                            + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInHosPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInDisPrf
                             + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInChnPrf
                             + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInWaiPrf

                            // NHA intern based improvement 
                            + "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
                            + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
                            + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
                            + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInHosPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInDisPrf
                             + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInChnPrf
                             + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInWaiPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.maxDesGap

                            );
                    }
                }

                Console.WriteLine("===============");
                Console.WriteLine(elapsed.ElapsedMilliseconds / 1000);
                Console.WriteLine("===============");
            }





            int[][] Pareto_DominatedCount_gi = new int[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_DominatedCount_gi[n] = new int[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_DominatedCount_gi[n][l] = 0;

                }

            }

            for (int i = 0; i < totalInstance; i++)
            {

                for (int n = 0; n < totalGroup; n++)
                {

                    bool atleastOneBetter = false;
                    bool result = true;
                    for (int nn = 0; nn < totalGroup; nn++)
                    {
                        if (nn == n)
                        {
                            continue;
                        }
                        for (int j = 1; j < totalMeasure; j++)
                        {

                            if (result_checking_coeff_instance_measure[n][i][j]
                                > result_checking_coeff_instance_measure[nn][i][j])
                            {
                                atleastOneBetter = true;
                            }
                            if (result_checking_coeff_instance_measure[n][i][j]
                              < result_checking_coeff_instance_measure[nn][i][j])
                            {
                                result = false;
                            }


                        }
                        if (result && atleastOneBetter)
                        {
                            Pareto_DominatedCount_gi[nn][i]++;
                        }
                        else if (result)
                        {
                            //Pareto_checking_coeff_instance[n][i] = true;
                        }
                    }


                }
            }

            bool[][] Pareto_checking_coeff_instance = new bool[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_checking_coeff_instance[n] = new bool[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_checking_coeff_instance[n][l] = false;
                    if (Pareto_DominatedCount_gi[n][l] == 0)
                    {
                        Pareto_checking_coeff_instance[n][l] = true;
                    }

                }

            }

            for (int g = 0; g < totalGroup; g++)
            {

                for (int i = 0; i < totalInstance; i++)
                {



                    using (StreamWriter file = new StreamWriter(allpathTotal.OutPutLocation + "\\ResultPareto.txt", true))
                    {
                        file.WriteLine(Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            );
                    }


                }


            }


            System.Xml.Serialization.XmlSerializer writer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(int[][]));
            System.IO.FileStream file1 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultDominatedCountgi.xml");
            writer1.Serialize(file1, Pareto_DominatedCount_gi);
            file1.Close();

            System.Xml.Serialization.XmlSerializer writer2 =
                new System.Xml.Serialization.XmlSerializer(typeof(double[][][]));
            System.IO.FileStream file2 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultValue_gij.xml");
            writer2.Serialize(file2, result_checking_coeff_instance_measure);
            file2.Close();
        }
        public void solveObjCoeffPareto(int totalGroup, int totalInstance, int totalMeasure)
        {
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };
           
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "NHA", "");
            double[][][] result_checking_coeff_instance_measure = new double[totalGroup][][];
            for (int n = 0; n < totalGroup; n++)
            {
                result_checking_coeff_instance_measure[n] = new double[totalInstance][];
                for (int l = 0; l < totalInstance; l++)
                {
                    result_checking_coeff_instance_measure[n][l] = new double[totalMeasure];
                    for (int i = 0; i < totalMeasure; i++)
                    {
                        result_checking_coeff_instance_measure[n][l][i] = 0;
                    }
                }

            }
            Stopwatch elapsed = new Stopwatch();
            elapsed.Start();
            for (int g = 0; g < totalGroup; g++)
            {
                elapsed.Restart();
                for (int i = 0; i < totalInstance; i++)
                    {
                        ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeffEjor", "NHA", "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
                        Stopwatch stopwatch = new Stopwatch();
                        read.data.AlgSettings.internBasedImpPercentage = 0.5;
                        read.data.AlgSettings.bucketBasedImpPercentage = 1;
                        NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(read.data, i.ToString());
                        stopwatch.Stop();
                        double time = (int)stopwatch.ElapsedMilliseconds / 1000;

                        result_checking_coeff_instance_measure[g][i][0] = nha.improvementStep.demandBaseLocalSearch.Global.Obj;
                        result_checking_coeff_instance_measure[g][i][1] = nha.improvementStep.demandBaseLocalSearch.Global.AveDes;
                        result_checking_coeff_instance_measure[g][i][2] = nha.improvementStep.demandBaseLocalSearch.Global.MinDis[0];
                        result_checking_coeff_instance_measure[g][i][3] = nha.improvementStep.demandBaseLocalSearch.Global.MinDis[1];
                        result_checking_coeff_instance_measure[g][i][4] = -nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand;
                        result_checking_coeff_instance_measure[g][i][5] = -nha.improvementStep.demandBaseLocalSearch.Global.ResDemand;
                        result_checking_coeff_instance_measure[g][i][6] = -nha.improvementStep.demandBaseLocalSearch.Global.SlackDem;
                        result_checking_coeff_instance_measure[g][i][7] = -nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal;



                    //using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
                    //{
                    //    file.WriteLine(i + "\t" + time
                    //        // NHA first Sol
                    //        + "\t" + nha.TimeForNHA + "\t" + nha.nhaResult.Obj + "\t" + nha.nhaResult.AveDes + "\t" + String.Join(" \t ", nha.nhaResult.MinDis)
                    //        + "\t" + nha.nhaResult.EmrDemand + "\t" + nha.nhaResult.ResDemand
                    //        + "\t" + nha.nhaResult.SlackDem + "\t" + nha.nhaResult.NotUsedAccTotal
                    //        + "\t" + nha.nhaResult.wieghtedSumInHosPrf + "\t" + nha.nhaResult.wieghtedSumInDisPrf
                    //         + "\t" + nha.nhaResult.wieghtedSumPrDisPrf + "\t" + nha.nhaResult.wieghtedSumInChnPrf
                    //         + "\t" + nha.nhaResult.wieghtedSumInWaiPrf

                    //        // NHA bucket list improvement
                    //        + "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
                    //        + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
                    //        + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
                    //        + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInHosPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInDisPrf
                    //         + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInChnPrf
                    //         + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInWaiPrf

                    //        // NHA intern based improvement 
                    //        + "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
                    //        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
                    //        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
                    //        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInHosPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInDisPrf
                    //         + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInChnPrf
                    //         + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInWaiPrf

                    //        );
                    //}
                }

                Console.WriteLine("===============");
                Console.WriteLine(elapsed.ElapsedMilliseconds / 1000);
                Console.WriteLine("===============");
            }


            
           

            int[][] Pareto_DominatedCount_gi = new int[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_DominatedCount_gi[n] = new int[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_DominatedCount_gi[n][l] = 0;

                }

            }

            for (int i = 0; i < totalInstance; i++)
            {

                for (int n = 0; n < totalGroup; n++)
                {

                    bool atleastOneBetter = false;
                    bool result = true;
                    for (int nn = 0; nn < totalGroup; nn++)
                    {
                        if (nn == n)
                        {
                            continue;
                        }
                        for (int j = 1; j < totalMeasure; j++)
                        {

                            if (result_checking_coeff_instance_measure[n][i][j]
                                > result_checking_coeff_instance_measure[nn][i][j])
                            {
                                atleastOneBetter = true;
                            }
                            if (result_checking_coeff_instance_measure[n][i][j]
                              < result_checking_coeff_instance_measure[nn][i][j])
                            {
                                result = false;
                            }


                        }
                        if (result && atleastOneBetter)
                        {
                            Pareto_DominatedCount_gi[nn][i]++;
                        }
                        else if (result)
                        {
                            //Pareto_checking_coeff_instance[n][i] = true;
                        }
                    }


                }
            }

            bool[][] Pareto_checking_coeff_instance = new bool[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_checking_coeff_instance[n] = new bool[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_checking_coeff_instance[n][l] = false;
                    if (Pareto_DominatedCount_gi[n][l] == 0)
                    {
                        Pareto_checking_coeff_instance[n][l] = true;
                    }

                }

            }

            for (int g = 0; g < totalGroup; g++)
            {

                for (int i = 0; i < totalInstance; i++)
                {
                    


                    using (StreamWriter file = new StreamWriter(allpathTotal.OutPutLocation + "\\ResultPareto.txt", true))
                    {
                        file.WriteLine(Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            + "\t" +   Pareto_DominatedCount_gi[g][i]
                            );
                    }


                }


            }


            System.Xml.Serialization.XmlSerializer writer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(int[][]));
            System.IO.FileStream file1 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultDominatedCountgi.xml");
            writer1.Serialize(file1, Pareto_DominatedCount_gi);
            file1.Close();

            System.Xml.Serialization.XmlSerializer writer2 =
                new System.Xml.Serialization.XmlSerializer(typeof(double[][][]));
            System.IO.FileStream file2 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultValue_gij.xml");
            writer2.Serialize(file2, result_checking_coeff_instance_measure);
            file2.Close();
        }

        public void solveObjCoeffParetoXML(int totalGroup, int totalInstance, int totalMeasure)
        {
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };

            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "NHA", "");
            double[][][] result_checking_coeff_instance_measure = new double[totalGroup][][];
            for (int n = 0; n < totalGroup; n++)
            {
                result_checking_coeff_instance_measure[n] = new double[totalInstance][];
                for (int l = 0; l < totalInstance; l++)
                {
                    result_checking_coeff_instance_measure[n][l] = new double[totalMeasure];
                    for (int i = 0; i < totalMeasure; i++)
                    {
                        result_checking_coeff_instance_measure[n][l][i] = 0;
                    }
                }

            }


            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(double[][][]));

            System.IO.StreamReader file2 = new System.IO.StreamReader(allpathTotal.OutPutLocation + "\\ResultValue_gij.xml");
            result_checking_coeff_instance_measure = (double[][][])reader.Deserialize(file2);
            file2.Close();




            int[][] Pareto_DominatedCount_gi = new int[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_DominatedCount_gi[n] = new int[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_DominatedCount_gi[n][l] = 0;

                }

            }

            for (int i = 0; i < totalInstance; i++)
            {

                for (int n = 0; n < totalGroup; n++)
                {

                    bool atleastOneBetter = false;
                    bool result = true;
                    for (int nn = 0; nn < totalGroup; nn++)
                    {
                        if (nn == n)
                        {
                            continue;
                        }
                        bool[] checkmeasure = new bool[totalMeasure];
                        for (int j = 1; j < totalMeasure; j++)
                        {

                            if (result_checking_coeff_instance_measure[n][i][j]
                                > result_checking_coeff_instance_measure[nn][i][j])
                            {
                                atleastOneBetter = true;
                                checkmeasure[j] = true;
                            }
                            else if (result_checking_coeff_instance_measure[n][i][j]
                              < result_checking_coeff_instance_measure[nn][i][j])
                            {
                                result = false;
                                checkmeasure[j] = false;
                            }
                            else
                            {
                                if (n < nn)
                                {
                                    checkmeasure[j] = true;
                                }
                                
                            }


                        }
                        bool NdomintesNN = true;
                        for (int j = 1; j < totalMeasure; j++)
                        {
                            if (!checkmeasure[j])
                            {
                                NdomintesNN = false;
                            }
                        }
                        if (NdomintesNN)
                        {
                            Pareto_DominatedCount_gi[nn][i]++;
                        }
                        if (result)
                        {
                            //Pareto_checking_coeff_instance[n][i] = true;
                        }
                    }


                }
            }

            bool[][] Pareto_checking_coeff_instance = new bool[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_checking_coeff_instance[n] = new bool[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_checking_coeff_instance[n][l] = false;
                    if (Pareto_DominatedCount_gi[n][l] == 0)
                    {
                        Pareto_checking_coeff_instance[n][l] = true;
                    }

                }

            }

            for (int g = 0; g < totalGroup; g++)
            {

                for (int i = 0; i < totalInstance; i++)
                {



                    using (StreamWriter file = new StreamWriter(allpathTotal.OutPutLocation + "\\ResultPareto.txt", true))
                    {
                        file.WriteLine(Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            );
                    }


                }


            }


            System.Xml.Serialization.XmlSerializer writer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(int[][]));
            System.IO.FileStream file1 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultDominatedCountgi.xml");
            writer1.Serialize(file1, Pareto_DominatedCount_gi);
            file1.Close();

        }

        public void solveObjCoeffParetoXMLTwoObjective(int totalGroup, int totalInstance, int totalMeasure)
        {
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };

            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeffEjor", "NHA", "");
            double[][][] result_checking_coeff_instance_measure1 = new double[totalGroup][][];
            for (int n = 0; n < totalGroup; n++)
            {
                result_checking_coeff_instance_measure1[n] = new double[totalInstance][];
                for (int l = 0; l < totalInstance; l++)
                {
                    result_checking_coeff_instance_measure1[n][l] = new double[totalMeasure];
                    for (int i = 0; i < totalMeasure; i++)
                    {
                        result_checking_coeff_instance_measure1[n][l][i] = 0;
                    }
                }

            }


            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(double[][][]));

            System.IO.StreamReader file2 = new System.IO.StreamReader(allpathTotal.OutPutLocation + "\\ResultValue_gij.xml");
            result_checking_coeff_instance_measure1 = (double[][][])reader.Deserialize(file2);
            file2.Close();
            double[] maxMeasure = new double[totalMeasure];
            for (int m = 0; m < totalMeasure; m++)
            {
                maxMeasure[m] = 0;
            }
            for (int n = 0; n < totalGroup; n++)
            {
                
                for (int l = 0; l < totalInstance; l++)
                {
                    
                    for (int i = 0; i < totalMeasure; i++)
                    {
                        if (Math.Abs(result_checking_coeff_instance_measure1[n][l][i]) > maxMeasure[i])
                        {
                            maxMeasure[i] = Math.Abs(result_checking_coeff_instance_measure1[n][l][i]);
                        }
                    }
                }

            }
            for (int n = 0; n < totalGroup; n++)
            {

                for (int l = 0; l < totalInstance; l++)
                {

                    for (int i = 0; i < totalMeasure; i++)
                    {
                        if (maxMeasure[i] == 0)
                        {
                            maxMeasure[i] = 1;
                        }
                        result_checking_coeff_instance_measure1[n][l][i]/=maxMeasure[i];
                   
                    }
                }

            }

            double[][][] result_checking_coeff_instance_Twomeasure = new double[totalGroup][][];
            for (int n = 0; n < totalGroup; n++)
            {
                result_checking_coeff_instance_Twomeasure[n] = new double[totalInstance][];
                for (int l = 0; l < totalInstance; l++)
                {
                    result_checking_coeff_instance_Twomeasure[n][l] = new double[2];

                    result_checking_coeff_instance_Twomeasure[n][l][0] = result_checking_coeff_instance_measure1[n][l][1] + result_checking_coeff_instance_measure1[n][l][2] + result_checking_coeff_instance_measure1[n][l][3];
                    result_checking_coeff_instance_Twomeasure[n][l][1] = result_checking_coeff_instance_measure1[n][l][4]
                                                                       + result_checking_coeff_instance_measure1[n][l][5]
                                                                       + result_checking_coeff_instance_measure1[n][l][6]
                                                                       + result_checking_coeff_instance_measure1[n][l][7];
                }

            }


            int[][] Pareto_DominatedCount_gi = new int[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_DominatedCount_gi[n] = new int[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_DominatedCount_gi[n][l] = 0;

                }

            }

            for (int i = 0; i < totalInstance; i++)
            {

                for (int n = 0; n < totalGroup; n++)
                {

                    bool atleastOneBetter = false;
                    bool result = true;
                    for (int nn = 0; nn < totalGroup; nn++)
                    {
                        if (nn == n)
                        {
                            continue;
                        }
                        bool[] checkmeasure = new bool[2];
                        for (int j = 0; j < 2; j++)
                        {

                            if (result_checking_coeff_instance_Twomeasure[n][i][j]
                                > result_checking_coeff_instance_Twomeasure[nn][i][j])
                            {
                                atleastOneBetter = true;
                                checkmeasure[j] = true;
                            }
                            else if (result_checking_coeff_instance_Twomeasure[n][i][j]
                              < result_checking_coeff_instance_Twomeasure[nn][i][j])
                            {
                                result = false;
                                checkmeasure[j] = false;
                            }
                            else
                            {
                                if (n < nn)
                                {
                                    checkmeasure[j] = true;
                                }

                            }


                        }
                        bool NdomintesNN = true;
                        for (int j = 0; j < 2; j++)
                        {
                            if (!checkmeasure[j])
                            {
                                NdomintesNN = false;
                            }
                        }
                        if (NdomintesNN)
                        {
                            Pareto_DominatedCount_gi[nn][i]++;
                        }
                        if (result)
                        {
                            //Pareto_checking_coeff_instance[n][i] = true;
                        }
                    }


                }
            }

            bool[][] Pareto_checking_coeff_instance = new bool[totalGroup][];
            for (int n = 0; n < totalGroup; n++)
            {
                Pareto_checking_coeff_instance[n] = new bool[totalInstance];
                for (int l = 0; l < totalInstance; l++)
                {
                    Pareto_checking_coeff_instance[n][l] = false;
                    if (Pareto_DominatedCount_gi[n][l] == 0)
                    {
                        Pareto_checking_coeff_instance[n][l] = true;
                    }

                }

            }

            for (int g = 0; g < totalGroup; g++)
            {

                for (int i = 0; i < totalInstance; i++)
                {



                    using (StreamWriter file = new StreamWriter(allpathTotal.OutPutLocation + "\\ResultParetoTwo.txt", true))
                    {
                        file.WriteLine(Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            + "\t" + Pareto_DominatedCount_gi[g][i]
                            );
                    }


                }


            }


            for (int i = 0; i < totalInstance; i++)
            {

                for (int g = 0; g < totalGroup; g++)
                {
                    using (StreamWriter file = new StreamWriter(allpathTotal.OutPutLocation + "\\ResultValueTwo.txt", true))
                    {
                        file.WriteLine(i
                            + "\t" + result_checking_coeff_instance_Twomeasure[g][i][0]
                            + "\t" + result_checking_coeff_instance_Twomeasure[g][i][1]
                            
                            );
                    }
                }
            }


            System.Xml.Serialization.XmlSerializer writer1 =
                new System.Xml.Serialization.XmlSerializer(typeof(int[][]));
            System.IO.FileStream file1 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultDominatedCountTwogi.xml");
            writer1.Serialize(file1, Pareto_DominatedCount_gi);
            file1.Close();

            System.Xml.Serialization.XmlSerializer writer2 =
                new System.Xml.Serialization.XmlSerializer(typeof(double[][][]));
            System.IO.FileStream file3 = System.IO.File.Create(allpathTotal.OutPutLocation + "\\ResultValueTwo.xml");
            writer2.Serialize(file3, result_checking_coeff_instance_Twomeasure);
            file3.Close();

        }

        public void solveEConstriant() 
        {
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeff", "eConstraint", "");
            ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeff", "eConstraint", "G_" + (0 + 1).ToString(), "Instance_" + 0 + ".txt");
            //GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP mip = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, (0).ToString(),true,7200);
            GeneralMIPAlgorithm.AugmentedEConstraintAlg augmeconstraint = new GeneralMIPAlgorithm.AugmentedEConstraintAlg(read.data, (0).ToString());
            foreach (GeneralMIPAlgorithm.ParetoPoints pareto in augmeconstraint.paretoSol)
            {
                using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
                {
                    file.WriteLine(pareto.displayMe());
                }
            }
        }

        public void solveOneFactorAtAtime()
        {
            int InstanceSize = 5;
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeff", "NHA", "");
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };
            for (int g = 0; g < 6; g++)
            {
                for (int l = 0; l < 4; l++)
                {
                    for (int i = 0; i < InstanceSize; i++)
                    {

                        ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeff", "NHA", nameCoeff[g] + level[l], "Instance_" + i + ".txt");
                        Stopwatch stopwatch = new Stopwatch();
                        read.data.AlgSettings.bucketBasedImpPercentage = 1;
                        read.data.AlgSettings.internBasedImpPercentage = 0.5;
                        //GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
                        stopwatch.Start();
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
                                + "\t" + nha.nhaResult.wieghtedSumInHosPrf + "\t" + nha.nhaResult.wieghtedSumInDisPrf
                                 + "\t" + nha.nhaResult.wieghtedSumPrDisPrf + "\t" + nha.nhaResult.wieghtedSumInChnPrf
                                 + "\t" + nha.nhaResult.wieghtedSumInWaiPrf

                                // NHA bucket list improvement
                                + "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
                                + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
                                + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
                                + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInHosPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInDisPrf
                                 + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInChnPrf
                                 + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.wieghtedSumInWaiPrf

                                // NHA intern based improvement 
                                + "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
                                + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
                                + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
                                + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInHosPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInDisPrf
                                 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumPrDisPrf + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInChnPrf
                                 + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.wieghtedSumInWaiPrf
                                 + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.maxDesGap // from the last step

                                );
                        }
                    }
                }
            }
        }

        public void readSolOneFactorAtAtime()
        {
            int InstanceSize = 5;
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult("ObjCoeff", "NHA", "");
            string[] nameCoeff = new string[6] { "Alpha", "Beta", "Gamma", "Delta", "Lambda", "Noe" };
            string[] level = new string[4] { "00", "01", "05", "10" };
            for (int g = 0; g < 6; g++)
            {
                for (int l = 0; l < 4; l++)
                {
                    for (int i = 0; i < InstanceSize; i++)
                    {

                        ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, "ObjCoeff", "MIP", nameCoeff[g] + level[l], "Instance_" + i + ".txt");
                        Stopwatch stopwatch = new Stopwatch();
                        read.data.AlgSettings.bucketBasedImpPercentage = 1;
                        read.data.AlgSettings.internBasedImpPercentage = 0.5;
                        //GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString());
                        stopwatch.Start();
                        OptimalSolution optimal = new OptimalSolution(read.data).readSol(read.data, "MIP"+i );
                        stopwatch.Stop();
                        int time = (int)stopwatch.ElapsedMilliseconds / 1000;

                        using (StreamWriter file = new StreamWriter(read.data.allPath.OutPutLocation + "\\Result.txt", true))
                        {
                            file.WriteLine( optimal.Obj + "\t" + optimal.AveDes + "\t" + String.Join(" \t ", optimal.MinDis)
                                + "\t" + optimal.EmrDemand + "\t" + optimal.ResDemand
                                + "\t" + optimal.SlackDem + "\t" + optimal.NotUsedAccTotal
                                + "\t" + optimal.wieghtedSumInHosPrf + "\t" + optimal.wieghtedSumInDisPrf
                                 + "\t" + optimal.wieghtedSumPrDisPrf + "\t" + optimal.wieghtedSumInChnPrf
                                 + "\t" + optimal.wieghtedSumInWaiPrf + "\t" + optimal.maxDesGap

                                );
                        }
                    }
                }
            }
        }

        public void solveThisDataSet(int totalGroup, int totalInstance, string datasetName, string nameOfProcsdure) 
        {
            int groupCounter = 0;
            SetAllPathForResult allpathTotal = new DataLayer.SetAllPathForResult(datasetName, nameOfProcsdure, "");
            for (int g = 0; g < totalGroup; g++)
            {
                for (int i = 0; i < totalInstance; i++)
                {
                    groupCounter++;
                    if (groupCounter < 31)
                    {
                        //continue;
                    }
                    ReadInformation read = new ReadInformation(allpathTotal.CurrentDir, datasetName , nameOfProcsdure, "G_" + (g + 1).ToString(), "Instance_" + i + ".txt");
                    read.data.AlgSettings.bucketBasedImpPercentage = 1;
                    read.data.AlgSettings.internBasedImpPercentage = 0.5;
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                   // BranchAndPriceAlgorithm.BranchAndPrice bp = new BranchAndPriceAlgorithm.BranchAndPrice(read.data, "Ins_" + i);
                    //MultiLevelSolutionMethodology.SequentialMethodology xy = new MultiLevelSolutionMethodology.SequentialMethodology(read.data, i.ToString());
                    //GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP xx = new GeneralMIPAlgorithm.MedicalTraineeSchedulingMIP(read.data, i.ToString(), false, 7200);
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
                        + "\t" + nha.improvementStep.TimeForbucketListImp + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.Obj + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.demandBaseLocalSearch.Global.MinDis)
                        + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.ResDemand
                        + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.SlackDem + "\t" + nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal
                        // NHA intern based improvement 
                        + "\t" + nha.improvementStep.TimeForInternBaseImp + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.Obj + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.AveDes + "\t" + String.Join(" \t ", nha.improvementStep.internBasedLocalSearch.finalSol.MinDis)
                        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.EmrDemand + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.ResDemand
                        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.SlackDem + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.NotUsedAccTotal
                        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxDisier + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxMin
                        + "\t" + nha.improvementStep.internBasedLocalSearch.finalSol.MaxRes + "\t" + nha.improvementStep.internBasedLocalSearch.MaxProcessedNode
                        + "\t" + nha.improvementStep.internBasedLocalSearch.RealProcessedNode
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



        }
    }
   
}

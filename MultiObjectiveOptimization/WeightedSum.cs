using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MultiObjectiveOptimization
{
    public class WeightedSum
    {
        public ArrayList allParetoWeights;
        public DataLayer.AllData data;

        public WeightedSum(DataLayer.AllData allData, int totalNumberOfWeight, string instanceName, int weightGenerator) 
        {
            initial(allData);
            switch (weightGenerator)
            {
                case 0:
                    setTheRandomlyWeighted(totalNumberOfWeight);
                    break;
                case 1:
                    setThePeriodicalVariationWeighted(totalNumberOfWeight);
                    break;
                default:
                    break;
            }
            solveWeightedNHA(instanceName);

        }
        public void initial(DataLayer.AllData allData) {
            data = allData;
            allParetoWeights = new ArrayList();
        }
        public void solveWeightedNHA(string instanceName) 
        {
            foreach (ParetoWeight pw in allParetoWeights)
            {
                for (int i = 0; i < data.General.Interns; i++)
                {
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_SumDesi = pw.w_sumDesire;
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_MinDesi = pw.w_minDesire;
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_ResCap = pw.w_resDemand;
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_EmrCap = pw.w_emrDemand;
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_MINDem = pw.w_minDemand;
                    data.TrainingPr[data.Intern[i].ProgramID].CoeffObj_NotUsedAcc = pw.w_regDemand;

                }
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                NestedHungarianAlgorithm.NHA nha = new NestedHungarianAlgorithm.NHA(data, instanceName);
                ParetoPoints paretoPoints = new ParetoPoints();
                paretoPoints.sumDesire = nha.improvementStep.demandBaseLocalSearch.Global.AveDes;
                paretoPoints.minDesire = 0;
                for (int p = 0; p < data.General.TrainingPr; p++)
                {
                    paretoPoints.minDesire += nha.improvementStep.demandBaseLocalSearch.Global.MinDis[p];
                }
                //paretoPoints.minDesire /= data.General.TrainingPr;
                paretoPoints.resDemand = nha.improvementStep.demandBaseLocalSearch.Global.ResDemand;
                paretoPoints.emrDemand = nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand;
                paretoPoints.minDemand = nha.improvementStep.demandBaseLocalSearch.Global.SlackDem;
                paretoPoints.regDemand = nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal;
                paretoPoints.elapsedTime = stopwatch.ElapsedMilliseconds / 1000;
                using (StreamWriter file = new StreamWriter(data.allPath.OutPutLocation + "\\Result.txt", true))
                {
                    file.WriteLine(paretoPoints.displayMeWithTime());
                }
            }
        }

        public void setTheRandomlyWeighted(int totalNumberOfWeight) 
        {
            Random random = new Random();
            for (int i = 0; i < totalNumberOfWeight; i++)
            {
                double sum = 0;
                ParetoWeight paretoWeight = new ParetoWeight();
                // create the weight
                paretoWeight.w_sumDesire = Math.Round(random.NextDouble(),3);
                sum += paretoWeight.w_sumDesire;

                paretoWeight.w_minDesire =  Math.Round(random.NextDouble(), 3);
                sum += paretoWeight.w_minDesire;

                paretoWeight.w_resDemand =  Math.Round(random.NextDouble(), 3);
                sum += paretoWeight.w_resDemand;

                paretoWeight.w_emrDemand =  Math.Round(random.NextDouble(), 3);
                sum += paretoWeight.w_emrDemand;

                paretoWeight.w_minDemand =  Math.Round(random.NextDouble(), 3);
                sum += paretoWeight.w_minDemand;

                paretoWeight.w_regDemand = Math.Round(random.NextDouble(), 3);
                sum += paretoWeight.w_regDemand;

                // the sum is already equal to zero
                if (sum == 0)
                {
                    paretoWeight.w_sumDesire = 1;
                    sum = 1;
                }

                paretoWeight.w_sumDesire /= sum;
                paretoWeight.w_minDesire /= sum;
                paretoWeight.w_resDemand /= sum;
                paretoWeight.w_emrDemand /= sum;
                paretoWeight.w_minDemand /= sum;
                paretoWeight.w_regDemand /= sum;

                addWeights(paretoWeight);
            }
        }

        public void setThePeriodicalVariationWeighted(int totalNumberOfWeight)
        {
            double counter = Math.Pow(totalNumberOfWeight, 0.17); // we have total 6 objective (1/6 = 0.17)
            Random random = new Random();
            for (int t1 = 0; t1 < Math.Ceiling(counter); t1++)
            {
                for (int t2 = 0; t2 < counter; t2++)
                {
                    for (int t3 = 0; t3 < counter; t3++)
                    {
                        for (int t4 = 0; t4 < counter; t4++)
                        {
                            for (int t5 = 0; t5 < counter; t5++)
                            {
                                for (int t6 = 0; t6 < counter; t6++)
                                {
                                    double sum = 0;

                                    ParetoWeight paretoWeight = new ParetoWeight();

                                    paretoWeight.w_sumDesire = Math.Abs(Math.Sin(2 * Math.PI * t1 / totalNumberOfWeight));
                                    sum += paretoWeight.w_sumDesire;

                                    paretoWeight.w_minDesire = Math.Abs(Math.Sin(2 * Math.PI * t2 / totalNumberOfWeight));
                                    sum += paretoWeight.w_minDesire;

                                    paretoWeight.w_resDemand = Math.Abs(Math.Sin(2 * Math.PI * t3 / totalNumberOfWeight));
                                    sum += paretoWeight.w_resDemand;

                                    paretoWeight.w_emrDemand = Math.Abs(Math.Sin(2 * Math.PI * t4 / totalNumberOfWeight));
                                    sum += paretoWeight.w_emrDemand;

                                    paretoWeight.w_minDemand = Math.Abs(Math.Sin(2 * Math.PI * t5 / totalNumberOfWeight));
                                    sum += paretoWeight.w_minDemand;

                                    paretoWeight.w_regDemand = Math.Abs(Math.Sin(2 * Math.PI * t6 / totalNumberOfWeight));
                                    sum += paretoWeight.w_regDemand;

                                    // the sum is already equal to zero
                                    if (sum == 0)
                                    {
                                        paretoWeight.w_sumDesire = 1;
                                        sum = 1;
                                    }
                                    paretoWeight.w_sumDesire /= sum;
                                    paretoWeight.w_minDesire /= sum;
                                    paretoWeight.w_resDemand /= sum;
                                    paretoWeight.w_emrDemand /= sum;
                                    paretoWeight.w_minDemand /= sum;
                                    paretoWeight.w_regDemand /= sum;

                                    addWeights(paretoWeight);
                                }
                                
                            }
                        }
                    }
                }
            }
            
        }

        public void addWeights(ParetoWeight paretoWeight) 
        {
            bool flagExist = false;
            foreach (ParetoWeight pw in allParetoWeights)
            {
                if (pw.compareMe(paretoWeight))
                {
                    flagExist = true;
                    break;
                }
            }

            if (!flagExist)
            {
                allParetoWeights.Add(paretoWeight);
            }
        }
    }
}

using System;
using System.Collections;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace MultiObjectiveOptimization
{
    /// <summary>
    /// for more information read the paper http://arxiv.org/abs/1709.02679
    /// </summary>
    /// 
    public class ArchivedPareto
    {
        public int ID;
        public ParetoPoints thePoint; // the point itself
        public ParetoWeight theWeight; // corresponding weight
        public double crowdingDegree; // calcultes the crowdness in obj space for this point
        public double[] euclidianDistanceFromQ; // distance from other points
        public int[] sortedDistanceIndex; // shows the index related to sorted distance 
        public double rKth; // niche radian from kth closest point
        public double r1st; // nich radian from the first closest point
        public int develpementRank; // shows how many solution from evolutionary set is in the 1st niche
        public int promissingRank; // shows how many solution from evolutionary set is outperformed by this point
        public bool ifIAmDominated;

        public ArchivedPareto() 
        {
            thePoint = new ParetoPoints();
            theWeight = new ParetoWeight();
        }

        public void setDistance(ArrayList arcivedSet) 
        {
            euclidianDistanceFromQ = new double[arcivedSet.Count];
            for (int q = 0; q < arcivedSet.Count; q++)
            {
                euclidianDistanceFromQ[q] = thePoint.euclidianDistanceFrom(((ArchivedPareto)arcivedSet[q]).thePoint);
            }
            sortDistance();
        }

        public void sortDistance() 
        {
            
            sortedDistanceIndex = new int[euclidianDistanceFromQ.Length];
            for (int i = 0; i < euclidianDistanceFromQ.Length; i++)
            {
                sortedDistanceIndex[i] = i;
            }
            // sort order 
            for (int i = 0; i < euclidianDistanceFromQ.Length; i++)
            {
                for (int j = i + 1; j < euclidianDistanceFromQ.Length; j++)
                {
                    if (euclidianDistanceFromQ[sortedDistanceIndex[j]] < euclidianDistanceFromQ[sortedDistanceIndex[i]])
                    {
                        int tmp = sortedDistanceIndex[i];
                        sortedDistanceIndex[i] = sortedDistanceIndex[j];
                        sortedDistanceIndex[j] = tmp;
                    }
                }
            }
        }

        public string displayMe() 
        {
            string outPut = thePoint.displayMe(theWeight);
            outPut += "\t" + crowdingDegree;
            outPut += "\t" + develpementRank;
            outPut += "\t" + promissingRank;
            return outPut;
        }
    }

    public class AdaptedWeightMethod
    {
        public ArrayList archivedSet;
        public ArrayList evolutionarySet;
        public ArrayList generatedWeight;
        public ArrayList allGeneratedWeight;
        public ParetoPoints refrencePoint;
        public int sizeOfArchived;
        DataLayer.AllData data;

        public AdaptedWeightMethod(DataLayer.AllData allData, string instanceName) 
        {
            initial(allData,instanceName);
            adaptiveWeightSettingAlg(instanceName);
        }

        public void initial(DataLayer.AllData allData,  string instanceName) 
        {
            data = allData;
            refrencePoint = new ParetoPoints();
            refrencePoint.makeRefrencePoint(data.AlgSettings.BigM);
            archivedSet = new ArrayList();
           // evolutionarySet = new ArrayList();
            initialGeneratedWeight();
            resetEvolutionarySet(instanceName);
        }

        public void adaptiveWeightSettingAlg(string instanceName) 
        {
            for (int it = 0; it < data.AlgSettings.totalIteration; it++)
            {
                archiveMaintenance();
                runNextGeneration(instanceName);
            }
            archiveMaintenance();
            foreach (ArchivedPareto archived in archivedSet)
            {
                using (StreamWriter file = new StreamWriter(data.allPath.OutPutLocation + "\\Result.txt", true))
                {
                    file.WriteLine(archived.thePoint.displayMeWithTime());
                }
            }
        }

        public void initialGeneratedWeight() 
        {
            allGeneratedWeight = new ArrayList();
            generatedWeight = new ArrayList();
            Random random = new Random();
            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 1,
                w_minDesire = 0,
                w_resDemand = 0,
                w_emrDemand = 0,
                w_minDemand = 0,
                w_regDemand = 0
            });
            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 0,
                w_minDesire = 1,
                w_resDemand = 0,
                w_emrDemand = 0,
                w_minDemand = 0,
                w_regDemand = 0
            });
            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 0,
                w_minDesire = 0,
                w_resDemand = 1,
                w_emrDemand = 0,
                w_minDemand = 0,
                w_regDemand = 0
            });

            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 0,
                w_minDesire = 0,
                w_resDemand = 0,
                w_emrDemand = 1,
                w_minDemand = 0,
                w_regDemand = 0
            });
            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 0,
                w_minDesire = 0,
                w_resDemand = 0,
                w_emrDemand = 0,
                w_minDemand = 1,
                w_regDemand = 0
            });
            generatedWeight.Add(new ParetoWeight
            {
                w_sumDesire = 0,
                w_minDesire = 0,
                w_resDemand = 0,
                w_emrDemand = 0,
                w_minDemand = 0,
                w_regDemand = 1
            });

            for (int i = 0; i < data.AlgSettings.sizeOfEvolutionaySet; i++)
            {
                double sum = 0;
                ParetoWeight paretoWeight = new ParetoWeight();
                // create the weight
                paretoWeight.w_sumDesire =random.NextDouble();
                sum += paretoWeight.w_sumDesire;

                paretoWeight.w_minDesire = (1 - sum) * random.NextDouble();
                sum += paretoWeight.w_minDesire;

                paretoWeight.w_resDemand = (1 - sum) * random.NextDouble();
                sum += paretoWeight.w_resDemand;

                paretoWeight.w_emrDemand = (1 - sum) * random.NextDouble();
                sum += paretoWeight.w_emrDemand;

                paretoWeight.w_minDemand = (1 - sum) * random.NextDouble();
                sum += paretoWeight.w_minDemand;

                paretoWeight.w_regDemand = (1 - sum);
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

                paretoWeight.w_sumDesire = Math.Round(paretoWeight.w_sumDesire, 3);
                paretoWeight.w_minDesire = Math.Round(paretoWeight.w_minDesire, 3);
                paretoWeight.w_resDemand = Math.Round(paretoWeight.w_resDemand, 3);
                paretoWeight.w_emrDemand = Math.Round(paretoWeight.w_emrDemand, 3);
                paretoWeight.w_minDemand = Math.Round(paretoWeight.w_minDemand, 3);
                paretoWeight.w_regDemand = Math.Round(paretoWeight.w_regDemand ,3);

                if (!addWeights(paretoWeight))
                {
                    generatedWeight.Add(paretoWeight);
                }
                
            }
        }

        public void resetEvolutionarySet(string instanceName) 
        {
            evolutionarySet = new ArrayList();
            foreach (ParetoWeight pw in generatedWeight)
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
                ArchivedPareto archived = new ArchivedPareto();
                archived.thePoint.sumDesire = nha.improvementStep.demandBaseLocalSearch.Global.AveDes;
                archived.thePoint.minDesire = 0;
                for (int p = 0; p < data.General.TrainingPr; p++)
                {
                    archived.thePoint.minDesire += nha.improvementStep.demandBaseLocalSearch.Global.MinDis[p];
                }
                //paretoPoints.minDesire /= data.General.TrainingPr;
                archived.thePoint.resDemand = nha.improvementStep.demandBaseLocalSearch.Global.ResDemand;
                archived.thePoint.emrDemand = nha.improvementStep.demandBaseLocalSearch.Global.EmrDemand;
                archived.thePoint.minDemand = nha.improvementStep.demandBaseLocalSearch.Global.SlackDem;
                archived.thePoint.regDemand = nha.improvementStep.demandBaseLocalSearch.Global.NotUsedAccTotal;
                // upgrade the refrenec point
                refrencePoint.upgradeRefrencePoint(archived.thePoint);
                archived.thePoint.elapsedTime = stopwatch.ElapsedMilliseconds / 1000;

                archived.theWeight.w_sumDesire = pw.w_sumDesire;
                archived.theWeight.w_minDesire = pw.w_minDesire;
                archived.theWeight.w_resDemand = pw.w_resDemand;
                archived.theWeight.w_emrDemand = pw.w_emrDemand;
                archived.theWeight.w_minDemand = pw.w_minDemand;
                archived.theWeight.w_regDemand = pw.w_regDemand;

                evolutionarySet.Add(archived);
            }
        }

        public void resetArchivedSet()
        {
            archivedSet = new ArrayList();
            foreach (ArchivedPareto archived in evolutionarySet)
            {
                if (!archived.ifIAmDominated)
                {
                    archivedSet.Add(archived);
                }
            }
            setCrowdingDegree();
            setDevelopementRank();
            setPromisingRank();

        }

        public void runNextGeneration(string instanceName) 
        {
            generatedWeight = new ArrayList();
            ArrayList candidate = new ArrayList();
            foreach (ArchivedPareto archived in archivedSet)
            {
                candidate.Add(archived);                
            }

            while (candidate.Count > data.AlgSettings.sizeOfEvolutionaySet)
            {
                // find the undeveloped vector and remove
                int minIndex = -1;
               
                for (int i = 0; i < candidate.Count; i++)
                {
                    if (((ArchivedPareto)candidate[i]).develpementRank > 0)
                    {
                        minIndex = i;
                        break;
                    }
                }
                if (minIndex >= 0)
                {
                    candidate.RemoveAt(minIndex);
                }
                else
                {
                    candidate.RemoveAt(candidate.Count - 1);
                }
                
            }

            while (candidate.Count > data.AlgSettings.sizeOfEvolutionaySet)
            {
                // find the less promissing and remove
                int minIndex = -1;
                double minValue = data.AlgSettings.BigM;
                for (int i = 0; i < candidate.Count; i++)
                {
                    if (((ArchivedPareto)candidate[i]).promissingRank < minValue)
                    {
                        minValue = ((ArchivedPareto)candidate[i]).promissingRank;
                        minIndex = i;
                    }
                }
                if (minIndex >= 0)
                {
                    candidate.RemoveAt(minIndex);
                }
                else
                {
                    candidate.RemoveAt(candidate.Count - 1);
                }

            }

            foreach (ArchivedPareto archived in candidate)
            {
                ArrayList paretoWeight = generatNeighbourWeight(archived, candidate.Count);
                foreach (ParetoWeight pw in paretoWeight)
                {
                    if (!addWeights(pw))
                    {
                        generatedWeight.Add(pw);
                    }
                }
                
            }
            resetEvolutionarySet(instanceName);
        }

        public void archiveMaintenance() 
        {
            // firs add evolutionary set
            setDominationFlag();

            // reset the archived set
            resetArchivedSet();

            // maintain now
            while (archivedSet.Count > data.AlgSettings.sizeOfArchivedSet)
            {
                // remove the most crowded solution
                int maxIndex = -1;
                double maxValue = -1;
                for (int i = 0; i < archivedSet.Count; i++)
                {
                    if (((ArchivedPareto)archivedSet[i]).crowdingDegree > maxValue)
                    {
                        maxValue = ((ArchivedPareto)archivedSet[i]).crowdingDegree;
                        maxIndex = i;
                    }
                }
                archivedSet.RemoveAt(maxIndex);
            }
        }

        public void setCrowdingDegree() 
        {
            // firs set the distance 
            foreach (ArchivedPareto archivedP in archivedSet)
            {
                archivedP.setDistance(archivedSet);
            }
            // set the raduis
            setNitchRadius();

            // calculate the crowding distance
            int counter = -1;
            
            foreach (ArchivedPareto archivedP in archivedSet)
            {
                counter++;
                archivedP.crowdingDegree = 1 - rPQ(archivedP, counter);
                
            }
        }

        public void setNitchRadius() 
        {
            double kthR = returnNicheRadiusForArchiveSet(3);
            double firsR = returnNicheRadiusForArchiveSet(1);
            foreach (ArchivedPareto archived in archivedSet)
            {
                archived.rKth = kthR;
                archived.r1st = firsR;
            }

        }

        public double returnNicheRadiusForArchiveSet(int kthNearst) 
        {
            double[] arrayofDistanceFromKth = new double[archivedSet.Count];
            int[] sortedIndex = new int[archivedSet.Count];
            double kthRadius = 0;
            if ( 1 + kthNearst >= archivedSet.Count - 1) // k = 6 archivesSet.count = 4 => k must be equal to 2, then k + 1 is in the range
            {
                kthNearst = archivedSet.Count - 1 - 1;
            }
            for (int a = 0; a < archivedSet.Count; a++)
            {
                sortedIndex[a] = a;
                
                int index = ((ArchivedPareto)archivedSet[a]).sortedDistanceIndex[1 + kthNearst];
                arrayofDistanceFromKth[a] = ((ArchivedPareto)archivedSet[a]).euclidianDistanceFromQ[index];
            }
            // sort it
            for (int a = 0; a < archivedSet.Count; a++)
            {
                for (int b = a+1; b < archivedSet.Count; b++)
                {
                    if (arrayofDistanceFromKth[sortedIndex[b]]< arrayofDistanceFromKth[sortedIndex[a]])
                    {
                        int tmp = sortedIndex[a];
                        sortedIndex[a] = sortedIndex[b];
                        sortedIndex[b] = tmp;
                    }
                }
            }

            if (archivedSet.Count % 2 == 0)
            {
                int median1 = archivedSet.Count / 2 - 1;
                int median2 = archivedSet.Count / 2;
                kthRadius = (arrayofDistanceFromKth[median1] + arrayofDistanceFromKth[median2]) / 2;
            }
            else
            {
                int median1 = (archivedSet.Count - 1) / 2;
                int median2 = (archivedSet.Count - 1) / 2;
                kthRadius = (arrayofDistanceFromKth[median1] + arrayofDistanceFromKth[median2]) / 2;
            }

            return kthRadius;
        }
        public double rPQ(ArchivedPareto archivedP, int indexOfThePoint) 
        {
            double r_pq = 1;
            double niche = archivedP.rKth;
            for (int i = 0; i < archivedSet.Count; i++)
            {
                if (i == indexOfThePoint)
                {
                    continue;
                }
                if (archivedP.euclidianDistanceFromQ[i] < niche)
                {
                    r_pq *= archivedP.euclidianDistanceFromQ[i] / niche;
                }
            }
            
            return r_pq;
        }
               
        public void setPromisingRank() 
        {
            foreach (ArchivedPareto archivedP in archivedSet) // in archive set
            {
                archivedP.promissingRank = 0;

                foreach (ArchivedPareto archivedQ in evolutionarySet) // solution in evolutionary set
                {
                    if (!archivedQ.ifIAmDominated) // if it is in archive set you do not need to check
                    {
                        continue;
                    }
                    // g(p,w_p) > g(q,w_p)
                    if (archivedP.thePoint.returnWeightedSum(archivedP.theWeight) > archivedQ.thePoint.returnWeightedSum(archivedP.theWeight))
                    {
                        archivedP.promissingRank++;
                    } // g(p,w_p) == g(q,w_p) && sum(i,fi(p)) > sum(i,fi(q))
                    else if(archivedP.thePoint.returnWeightedSum(archivedP.theWeight) == archivedQ.thePoint.returnWeightedSum(archivedP.theWeight)
                        && archivedP.thePoint.returnSum() > archivedQ.thePoint.returnSum())
                    {
                        archivedP.promissingRank++;
                    }
                }
            }
        }

        public void setDevelopementRank()
        {
            foreach (ArchivedPareto archivedP in archivedSet) // in archive set
            {
                archivedP.develpementRank = 0;

                foreach (ArchivedPareto archivedQ in evolutionarySet) // solution in evolutionary set
                {
                    if (!archivedQ.ifIAmDominated) // we have to compare with evolutionary not archived
                    {
                        continue;
                    }
                    if (archivedP.thePoint.euclidianDistanceFrom(archivedQ.thePoint) < archivedP.r1st)
                    {
                        archivedP.develpementRank++;
                    }
                }
            }
            

           
        }

        public void setDominationFlag()
        {
            // add archived set to evolutionary set
            foreach (ArchivedPareto archivedP in archivedSet)
            {
                evolutionarySet.Add(archivedP);
            }
            for (int i = 0; i < evolutionarySet.Count; i++)
            {
                ((ArchivedPareto)evolutionarySet[i]).ifIAmDominated = false;
                for (int j = 0; j < evolutionarySet.Count; j++)
                {
                    if (((ArchivedPareto)evolutionarySet[j]).ifIAmDominated)
                    {
                        continue;
                    }
                    if (i == j)
                    {
                        continue;
                    }
                    if (((ArchivedPareto)evolutionarySet[j]).thePoint.iWeaklyDominateThis(((ArchivedPareto)evolutionarySet[i]).thePoint))
                    {
                        ((ArchivedPareto)evolutionarySet[i]).ifIAmDominated = true;
                        break;
                    }

                }
            }

        }

        public ParetoWeight generatNewWeight(ArchivedPareto archivedP) 
        {
            
            ParetoWeight weight = new ParetoWeight();
            weight.w_sumDesire = (archivedP.thePoint.sumDesire - refrencePoint.sumDesire) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_minDesire = (archivedP.thePoint.minDesire - refrencePoint.minDesire) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_resDemand = -(archivedP.thePoint.resDemand - refrencePoint.resDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_emrDemand = -(archivedP.thePoint.emrDemand - refrencePoint.emrDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_minDemand = -(archivedP.thePoint.minDemand - refrencePoint.minDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_regDemand = -(archivedP.thePoint.regDemand - refrencePoint.regDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine(archivedP.theWeight.displayMe());
            //Console.WriteLine(archivedP.thePoint.displayMe());
            //Console.WriteLine(weight.displayMe());
            return weight;
        }

        public ArrayList generatNeighbourWeight(ArchivedPareto archivedP, int sizeOfNeighbour) 
        {
            Random random = new Random();
            ArrayList neighbourWeight = new ArrayList();
            ParetoWeight weight = archivedP.theWeight;
            neighbourWeight.Add(generatNewWeight(archivedP));
            while (neighbourWeight.Count < data.AlgSettings.sizeOfEvolutionaySet/sizeOfNeighbour)
            {
                int wIndex = random.Next(0, 6);
                double additive = 0;
                switch (wIndex)
                {
                    case 0:
                        additive = random.NextDouble() * weight.w_sumDesire;
                        weight.w_sumDesire -= additive;
                        break;
                    case 1:
                        additive = random.NextDouble() * weight.w_minDesire;
                        weight.w_minDesire -= additive;
                        break;
                    case 2:
                        additive = random.NextDouble() * weight.w_resDemand;
                        weight.w_resDemand -= additive;
                        break;
                    case 3:
                        additive = random.NextDouble() * weight.w_emrDemand;
                        weight.w_emrDemand -= additive;
                        break;
                    case 4:
                        additive = random.NextDouble() * weight.w_minDemand;
                        weight.w_minDemand -= additive;
                        break;
                    case 5:
                        additive = random.NextDouble() * weight.w_minDesire;
                        weight.w_regDemand -= additive;
                        break;
                    default:
                        break;
                }

                double portion = additive / 6;
                ParetoWeight paretoWeight = generatNewWeight(archivedP);
                for (int i = 0; i < 6; i++)
                {
                    int wIndexAdditive = random.Next(0, 6);

                    switch (wIndexAdditive)
                    {
                        case 0:
                            paretoWeight.w_sumDesire += portion;
                            break;
                        case 1:
                            paretoWeight.w_minDesire += portion;
                            break;
                        case 2:
                            paretoWeight.w_resDemand += portion;
                            break;
                        case 3:
                            paretoWeight.w_emrDemand += portion;
                            break;
                        case 4:
                            paretoWeight.w_minDemand += portion;
                            break;
                        case 5:
                            paretoWeight.w_minDesire += portion;
                            break;
                        default:
                            break;
                    }



                }

                neighbourWeight.Add(paretoWeight);

            }

            return neighbourWeight;
        }

        public bool addWeights(ParetoWeight paretoWeight)
        {
            bool flagExist = false;
            foreach (ParetoWeight pw in allGeneratedWeight)
            {
                if (pw.compareMe(paretoWeight))
                {
                    flagExist = true;
                    break;
                }
            }

            if (!flagExist)
            {
                allGeneratedWeight.Add(paretoWeight);
            }
            return flagExist;
        }

        

    }
}

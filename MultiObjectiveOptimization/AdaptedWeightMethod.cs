using System;
using System.Collections;
using System.Text;

namespace MultiObjectiveOptimization
{
    /// <summary>
    /// for more information read the paper http://arxiv.org/abs/1709.02679
    /// </summary>
    /// 
    public class ArchivedPareto
    {
        public int ID;
        public ParetoPoints thePoint;
        public ParetoWeight theWeight;
        public double crowdingDegree;
        public double[] euclidianDistanceFromQ;
        public int[] sortedDistanceIndex;
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
                    if (euclidianDistanceFromQ[sortedDistanceIndex[i]] < euclidianDistanceFromQ[sortedDistanceIndex[j]])
                    {
                        int tmp = sortedDistanceIndex[i];
                        sortedDistanceIndex[i] = sortedDistanceIndex[j];
                        sortedDistanceIndex[j] = tmp;
                    }
                }
            }
        }
    }

    public class AdaptedWeightMethod
    {
        public ArrayList archivedSet;
        public ArrayList evolutionarySet;
        public ParetoPoints refrencePoint;
        public void archiveMaintenance() { }

        public void weightAddition() { }

        public void weightGeneration() { }

        public void weightDeletion() { }

        public void setTheDistanceMatrix() { }

        public void setCrowdingDegree() 
        {
            // firs set the distance 
            foreach (ArchivedPareto archivedP in archivedSet)
            {
                archivedP.setDistance(archivedSet);
            }
            // calculate the crowding distance
            int counter = -1;
            foreach (ArchivedPareto archivedP in archivedSet)
            {
                counter++;
                archivedP.crowdingDegree = 1 - rPQ(archivedP, counter);
                
            }
        }

        public double rPQ(ArchivedPareto archivedP, int indexOfThePoint) 
        {
            double r_pq = 1;
            double niche = nicheRadian(archivedP, 6);
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

        public bool pOutperformsq(ParetoPoints paretoP, ParetoPoints paretoQ) 
        {
            bool outperforms = false;


            return outperforms;
        }

        public double nicheRadian(ArchivedPareto archivedP, int kthNearst) 
        {
            int indexKth = archivedP.sortedDistanceIndex[kthNearst+1]; // the first one in the sorted algorithm is the point itseld

            double r = ((ArchivedPareto)archivedSet[indexKth]).euclidianDistanceFromQ[((ArchivedPareto)archivedSet[indexKth]).sortedDistanceIndex[(archivedSet.Count / 2) + 1]];
            return r;
        }

        public bool isPromising(ArchivedPareto archivedP) 
        {
            

            return true; //?
        }

        public bool isUndeveloped()
        {


            return true; //?
        }

        public ParetoWeight generatNewWeight(ArchivedPareto archivedP) 
        {
            ParetoWeight weight = new ParetoWeight();
            weight.w_sumDesire = (archivedP.thePoint.sumDesire - refrencePoint.sumDesire) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_minDesire = (archivedP.thePoint.minDesire - refrencePoint.minDesire) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_resDemand = (archivedP.thePoint.resDemand - refrencePoint.resDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_emrDemand = (archivedP.thePoint.emrDemand - refrencePoint.emrDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_minDemand = (archivedP.thePoint.minDemand - refrencePoint.minDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            weight.w_regDemand = (archivedP.thePoint.regDemand - refrencePoint.regDemand) / (archivedP.thePoint.returnSum() - refrencePoint.returnSum());
            return weight;
        }


    }
}

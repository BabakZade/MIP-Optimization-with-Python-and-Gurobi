using System;
using System.Collections.Generic;
using System.Text;

namespace MultiObjectiveOptimization
{
    public class ParetoWeight
    {
        // corresponding weights
        public double w_sumDesire;
        public double w_minDesire;
        public double w_resDemand;
        public double w_emrDemand;
        public double w_minDemand;
        public double w_regDemand;

        public bool compareMe(ParetoWeight comparableW) 
        {
            if (w_sumDesire == comparableW.w_sumDesire &&
                w_minDesire == comparableW.w_minDesire &&
                w_resDemand == comparableW.w_resDemand &&
                w_emrDemand == comparableW.w_emrDemand &&
                w_minDemand == comparableW.w_minDemand &&
                w_regDemand == comparableW.w_regDemand)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string displayMe()
        {
            string disp = "";
            disp += w_sumDesire + "\t" + w_minDesire + "\t" + w_resDemand + "\t" + w_emrDemand + "\t" + w_minDemand + "\t" + w_regDemand;

            return disp;
        }
       
    }
    public class ParetoPoints
    {
        public double sumDesire;
        public double minDesire;
        public double resDemand;
        public double emrDemand;
        public double minDemand;
        public double regDemand;
        public long elapsedTime;

        public double euclidianDistanceFrom(ParetoPoints otherPoint) {
            double euDis = 0;
            euDis += Math.Pow((sumDesire - otherPoint.sumDesire),2);
            euDis += Math.Pow((minDesire - otherPoint.minDesire), 2);
            euDis += Math.Pow((resDemand - otherPoint.resDemand), 2);
            euDis += Math.Pow((emrDemand - otherPoint.emrDemand), 2);
            euDis += Math.Pow((minDemand - otherPoint.minDemand), 2);
            euDis += Math.Pow((regDemand - otherPoint.regDemand), 2);

            return Math.Sqrt(euDis);
        }

        public double returnWeightedSum(ParetoWeight objWeight) 
        {
            double normalSum = 0;

            normalSum += sumDesire * objWeight.w_sumDesire;
            normalSum += minDesire * objWeight.w_minDesire;
            normalSum += resDemand * objWeight.w_resDemand;
            normalSum += emrDemand * objWeight.w_emrDemand;
            normalSum += minDemand * objWeight.w_minDemand;
            normalSum += regDemand * objWeight.w_regDemand;

            return normalSum;
        }
        public double returnSum()
        {
            double normalSum = 0;

            normalSum += sumDesire;
            normalSum += minDesire;
            normalSum += resDemand;
            normalSum += emrDemand;
            normalSum += minDemand;
            normalSum += regDemand;

            return normalSum;
        }

        public bool compareMe(ParetoPoints comparableP)
        {
            if (sumDesire == comparableP.sumDesire &&
                minDesire == comparableP.minDesire &&
                resDemand == comparableP.resDemand &&
                emrDemand == comparableP.emrDemand &&
                minDemand == comparableP.minDemand &&
                regDemand == comparableP.regDemand)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool iWeaklyDominateThis(ParetoPoints point)
        {
            bool domination = true;
            bool atleastInOne = false;
            domination = domination && sumDesire >= point.sumDesire;
            domination = domination && minDesire >= point.minDesire;
            domination = domination && resDemand <= point.resDemand;
            domination = domination && emrDemand <= point.emrDemand;
            domination = domination && minDemand <= point.minDemand;
            domination = domination && regDemand <= point.regDemand;

            atleastInOne = atleastInOne || sumDesire > point.sumDesire;
            atleastInOne = atleastInOne || minDesire > point.minDesire;
            atleastInOne = atleastInOne || resDemand < point.resDemand;
            atleastInOne = atleastInOne || emrDemand < point.emrDemand;
            atleastInOne = atleastInOne || minDemand < point.minDemand;
            atleastInOne = atleastInOne || regDemand < point.regDemand;

            if (domination && atleastInOne)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string displayMe()
        {
            string disp = "";
            disp += sumDesire + "\t" + minDesire + "\t" + resDemand + "\t" + emrDemand + "\t" + minDemand + "\t" + regDemand;

            return disp;
        }
        public string displayMe(ParetoWeight paretoWeight)
        {
            string disp = paretoWeight.displayMe() + "\t";
            disp += sumDesire + "\t" + minDesire + "\t" + resDemand + "\t" + emrDemand + "\t" + minDemand + "\t" + regDemand;

            return disp;
        }
        public string displayMeWithTime()
        {
            string disp = "";
            disp += elapsedTime + "\t" + sumDesire + "\t" + minDesire + "\t" + resDemand + "\t" + emrDemand + "\t" + minDemand + "\t" + regDemand;

            return disp;
        }

    }
}

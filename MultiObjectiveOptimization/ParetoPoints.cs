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

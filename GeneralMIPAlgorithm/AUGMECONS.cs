using System;
using System.Collections.Generic;
using System.Text;

namespace GeneralMIPAlgorithm
{
    public class AUGMECONS
    {
        public int totalObjective;
        public double[] objectiveRange_o;
        public double[] epsilonCon_o;
        public double[] upperBound_o;
        public double[] lowerBound_o;
        public bool[] activeObj_o;
        public bool[] activeConst_o;
        public double[] weight_o;
        public int[] priority_o;
        public double[] absTols_o;
        public double[] relTols_o;

        public AUGMECONS(int totalObjective) 
        {
            this.totalObjective = totalObjective;
            objectiveRange_o = new double[totalObjective];
            epsilonCon_o = new double[totalObjective];
            upperBound_o = new double[totalObjective];
            lowerBound_o = new double[totalObjective];
            activeObj_o = new bool[totalObjective];
            activeConst_o = new bool[totalObjective];
            weight_o = new double[totalObjective];
            priority_o = new int[totalObjective];
            absTols_o = new double[totalObjective];
            relTols_o = new double[totalObjective];
            for (int o = 0; o < totalObjective; o++)
            {
                objectiveRange_o[o] = 0;
                epsilonCon_o[o] = 0;
                upperBound_o[o] = 0;
                lowerBound_o[o] = 0;
                activeObj_o[o] = false;
                activeConst_o[o] = false;
                weight_o[o] = 1;
                priority_o[o] = 0;
                absTols_o[o] = 0;
                relTols_o[o] = 0;
            }

        }
        public string displayMe() 
        {
            string disp = "";

            for (int o = 0; o < totalObjective; o++)
            {
                disp += "OBJ "+o+ ": R " +objectiveRange_o[o] +" eC "+ epsilonCon_o[o] + " UB " + upperBound_o[o] + " LB " + lowerBound_o[o] + " IfObj " + activeObj_o[o] + " IfConst " + activeConst_o[o] + "\n";
              
            }

            return disp;
        }
    }
}

using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
using GeneralMIPAlgorithm;

namespace MultiObjectiveOptimization
{
    public class AugmentedEConstraintAlg
    {
        public int totalObjective;
        public double[][] payOffTable;
        public double[] minRange_o;
        public double[] maxRange_o;
        public int[] rangeQ_o;
        DataLayer.AllData data;
        public ArrayList eConstraitn;
        public double[][] rangeInterval_oi;
        public ArrayList paretoSol;
        public int augCounter;
        public bool[][][][] warmStartTime_itdh;
        public int[][][][][][] range_DdREMR;
        public Stopwatch elapsedSW;
        public AugmentedEConstraintAlg(DataLayer.AllData alldata, string InsName) {
            
            initial(alldata);
            setAUGMECONSpayoff(InsName);
            setAUGMECONSRange(InsName);
            setAUGMECONSPareto(InsName);
        }

        public void initial(DataLayer.AllData alldata)
        {
            elapsedSW = new Stopwatch();
            elapsedSW.Start();
            totalObjective = 6;
            payOffTable = new double[totalObjective][];
            for (int o = 0; o < totalObjective; o++)
            {
                payOffTable[o] = new double[totalObjective];
                for (int oo = 0; oo < totalObjective; oo++)
                {
                    payOffTable[o][oo] = 0;
                }
            }
            minRange_o = new double[totalObjective];
            maxRange_o = new double[totalObjective];
            rangeQ_o = new int[totalObjective];
            for (int o = 0; o < totalObjective; o++)
            {
                minRange_o[o] = alldata.AlgSettings.BigM;
                maxRange_o[o] = 0;
                rangeQ_o[o] = 0;

            }
            rangeQ_o = new int[] { 3, 4, 4, 4, 4, 4 };
            data = alldata;
            eConstraitn = new ArrayList();
            paretoSol = new ArrayList();
            augCounter = 0;
            new DataLayer.ArrayInitializer().CreateArray(ref warmStartTime_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines + 1, data.General.Hospitals + 1, false);

        }

        public void setAUGMECONSpayoff(string InsName)
        {
            bool warmStartFlag = false;
            for (int o = 0; o < totalObjective; o++)
            {
                augCounter++;
                elapsedSW.Restart();
                AUGMECONS root = new AUGMECONS(totalObjective,augCounter);
                if (warmStartFlag)
                {
                    root.setWarmStartTime(data, warmStartTime_itdh);
                    root.useWarmStart = true;
                }
                string name = InsName + "AugID_" + root.ID;
                for (int oo = 0; oo < totalObjective; oo++)
                {
                    root.activeObj_o[oo] = true;
                    root.priority_o[oo] = totalObjective - oo;
                    if (o == oo)
                    {
                        root.priority_o[oo] = totalObjective + 1;// higher priority
                    }
                }
                
                // solve the problem find the objective
                MedicalTraineeSchedulingMIP mip = new MedicalTraineeSchedulingMIP(data, root, true, name);
                if (!mip.notFeasible)
                {
                    warmStartTime_itdh = mip.warmStartTime_itdh;
                    root.setWarmStartTime(data, warmStartTime_itdh);
                    warmStartFlag = true;
                    for (int oo = 0; oo < totalObjective; oo++)
                    {
                        payOffTable[o][oo] = mip.multiObjectiveValue[oo];
                        if (minRange_o[oo] > payOffTable[o][oo])
                        {
                            minRange_o[oo] = payOffTable[o][oo];
                            root.lowerBound_o[oo] = minRange_o[oo];
                        }
                        if (maxRange_o[oo] < payOffTable[o][oo])
                        {
                            maxRange_o[oo] = payOffTable[o][oo];
                            root.upperBound_o[oo] = maxRange_o[oo];
                        }
                    }
                    paretoSol.Add(new ParetoPoints
                    {
                        sumDesire = payOffTable[o][0],
                        minDesire = payOffTable[o][1],
                        resDemand = payOffTable[o][2],
                        emrDemand = payOffTable[o][3],
                        minDemand = payOffTable[o][4],
                        regDemand = payOffTable[o][5],
                        elapsedTime = elapsedSW.ElapsedMilliseconds/1000,
                    });
                }

                eConstraitn.Add(root); // we need them later
            }
        }

        public void setAUGMECONSRange(string InsName) 
        {
            StreamWriter swEC = new StreamWriter(data.allPath.OutPutGr + "econstGrid.txt");
            for (int o = 0; o < totalObjective; o++)
            {
                if (rangeQ_o[o] > (int)(maxRange_o[o] - minRange_o[o]))
                {
                    rangeQ_o[o] = (int)(maxRange_o[o] - minRange_o[o]); // the loops from hear can consider 0
                }
            }

            int counter = 0;
            for (int mnD = 0; mnD <= rangeQ_o[1]; mnD++)
            {
                for (int rsD = 0; rsD <= rangeQ_o[2]; rsD++)
                {
                    for (int emD = 0; emD <= rangeQ_o[3]; emD++)
                    {
                        for (int miD = 0; miD <= rangeQ_o[4]; miD++)
                        {
                            for (int rgD = 0; rgD <= rangeQ_o[5]; rgD++)
                            {
                                augCounter++;
                                AUGMECONS tmpAug = new AUGMECONS(totalObjective,augCounter);
                                counter++;
                                for (int o = 1; o < totalObjective; o++)
                                {
                                    tmpAug.useWarmStart = true;
                                    tmpAug.setWarmStartTime(data, ((AUGMECONS)eConstraitn[o]).warmStartTime_itdh);
                                    double econst = 0;
                                    if (rangeQ_o[o] == 0)
                                    {
                                        econst = minRange_o[o];
                                    }
                                    else
                                    {
                                        switch (o)
                                        {
                                            case 1:
                                                econst = minRange_o[o] + mnD * (maxRange_o[o] - minRange_o[o]) / rangeQ_o[o];
                                                break;
                                            case 2:
                                                econst = minRange_o[o] + rsD * (maxRange_o[o] - minRange_o[o]) / rangeQ_o[o];
                                                break;
                                            case 3:
                                                econst = minRange_o[o] + emD * (maxRange_o[o] - minRange_o[o]) / rangeQ_o[o];
                                                break;
                                            case 4:
                                                econst = minRange_o[o] + miD * (maxRange_o[o] - minRange_o[o]) / rangeQ_o[o];
                                                break;
                                            case 5:
                                                econst = minRange_o[o] + rgD * (maxRange_o[o] - minRange_o[o]) / rangeQ_o[o];
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                    
                                    tmpAug.upperBound_o[o] = maxRange_o[o];
                                    tmpAug.lowerBound_o[o] = minRange_o[o];
                                    tmpAug.epsilonCon_o[o] = (int)econst;
                                    tmpAug.activeConst_o[o] = true;
                                    tmpAug.activeObj_o[0] = true; // only the first objective is active
                                    tmpAug.objectiveRange_o[o] = maxRange_o[o] - minRange_o[o];

                                }
                                eConstraitn.Add(tmpAug);
                                swEC.WriteLine("==================="+counter+"=====================");
                                swEC.WriteLine(tmpAug.displayMe());
                                
                            }
                        }
                    }
                }
            }

        }

        public void setAUGMECONSPareto(string InsName) 
        {
            for (int o = 0; o < totalObjective; o++)
            {
                using (StreamWriter file = new StreamWriter(data.allPath.OutPutLocation + "\\Result.txt", true))
                {
                    file.WriteLine(((ParetoPoints)paretoSol[o]).displayMeWithTime());
                }
            }
            int counter = totalObjective - 1; // the firs 6 ones are root we already checked them for payoff
            for (int mnD = 0; mnD <= rangeQ_o[1]; mnD++)
            {
                for (int rsD = 0; rsD <= rangeQ_o[2]; rsD++)
                {
                    for (int emD = 0; emD <= rangeQ_o[3]; emD++)
                    {
                        for (int miD = 0; miD <= rangeQ_o[4]; miD++)
                        {
                            for (int rgD = 0; rgD <= rangeQ_o[5]; rgD++)
                            {
                                counter++;
                                elapsedSW.Restart();
                                AUGMECONS aug = (AUGMECONS)eConstraitn[counter];
                                
                                string name = InsName + "AugID_" + aug.ID;
                                MedicalTraineeSchedulingMIP tmp = new MedicalTraineeSchedulingMIP(data, aug, false, name);
                                
                                if (!tmp.notFeasible)
                                {
                                    if (counter < eConstraitn.Count - 1)
                                    {
                                        ((AUGMECONS)eConstraitn[counter + 1]).setWarmStartTime(data, tmp.warmStartTime_itdh);
                                    }
                                    paretoSol.Add(new ParetoPoints
                                    {
                                        sumDesire = tmp.multiObjectiveValue[0],
                                        minDesire = tmp.multiObjectiveValue[1],
                                        resDemand = tmp.multiObjectiveValue[2],
                                        emrDemand = tmp.multiObjectiveValue[3],
                                        minDemand = tmp.multiObjectiveValue[4],
                                        regDemand = tmp.multiObjectiveValue[5],
                                        elapsedTime = elapsedSW.ElapsedMilliseconds / 1000,
                                    });

                                    using (StreamWriter file = new StreamWriter(data.allPath.OutPutLocation + "\\Result.txt", true))
                                    {
                                        file.WriteLine(((ParetoPoints)paretoSol[counter]).displayMeWithTime());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}

using System;
using System.Collections;
using System.Text;
using DataLayer;
using ColumnAndBranchInfo;

namespace LowerBound
{
    public class GreedyLowerBound
    {
        public AllData data;

        public int[][][] demand_twh;
        public int[][][] tmpdemand_twh;
        public double[][][][] columnInfo_itdh;
        public OptimalSolution LBResult;
        public bool[] internStatus;
        public bool[] columnStatus;

        public GreedyLowerBound(AllData allData, ArrayList columns, string Name)
        {
            initial(allData, columns);
            setSol(columns);
            LBResult.WriteSolution(allData.allPath.OutPutGr, Name);

        }

        public void initial(AllData allData, ArrayList columns)
        {
            data = allData;
            new ArrayInitializer().CreateArray(ref internStatus, data.General.Interns, false);
            new ArrayInitializer().CreateArray(ref columnStatus, columns.Count, false);
            new ArrayInitializer().CreateArray(ref demand_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals,0);
            new ArrayInitializer().CreateArray(ref tmpdemand_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref columnInfo_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            LBResult = new OptimalSolution(data);
            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        demand_twh[t][w][h] = data.Hospital[h].HospitalMaxDem_tw[t][w] + data.Hospital[h].EmergencyCap_tw[t][w] + data.Hospital[h].ReservedCap_tw[t][w];
                        tmpdemand_twh[t][w][h] = demand_twh[t][w][h];
                    }
                }
            }
        }
        public void setSol(ArrayList columns) 
        {
            for (int i = 0; i < data.General.Interns; i++)
            {
                int counter = -1;
                int colIndex = -1;
                double maxVal = data.AlgSettings.RCepsi;                
                foreach (ColumnInternBasedDecomposition clmn in columns)
                {
                    counter++;
                    if (internStatus[clmn.theIntern] || columnStatus[counter] || clmn.xVal == 0)
                    {
                        continue;
                    }
                    if (maxVal < clmn.xVal)
                    {
                        colIndex = counter;
                        maxVal = clmn.xVal;
                    }
                }
                if (colIndex == -1)
                {
                    break;
                }
                if (demandWiseFeasibility((ColumnInternBasedDecomposition)columns[colIndex]))
                {
                    internStatus[((ColumnInternBasedDecomposition)columns[colIndex]).theIntern] = true;
                    columnStatus[colIndex] = true;
                }
                else
                {
                    columnStatus[colIndex] = true;
                    i--;
                }
                
            }
        }

        public bool demandWiseFeasibility(ColumnInternBasedDecomposition column) 
        {
            bool result = true;
            for (int d = 0; d < data.General.Disciplines && result; d++)
            {
                for (int h = 0; h < data.General.Hospitals && result; h++)
                {
                    for (int t = 0; t < data.General.TimePriods && result; t++)
                    {
                        if (column.S_tdh[t][d][h])
                        {
                            for (int w = 0; w < data.General.HospitalWard; w++)
                            {

                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    int theP = data.Intern[column.theIntern].ProgramID;
                                    for (int tt = t; tt < t + data.Discipline[d].Duration_p[theP]; tt++)
                                    {
                                        if (tmpdemand_twh[tt][w][h] > 0)
                                        {
                                            tmpdemand_twh[tt][w][h]--;

                                        }
                                        else
                                        {
                                            result = false;
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                        }                        
                    }
                }
            }
            if (result)
            {
                for (int h = 0; h < data.General.Hospitals && result; h++)
                {
                    for (int t = 0; t < data.General.TimePriods && result; t++)
                    {
                        for (int w = 0; w < data.General.HospitalWard; w++)
                        {
                            demand_twh[t][w][h] = tmpdemand_twh[t][w][h];
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w] && column.S_tdh[t][d][h]) 
                                {
                                    LBResult.Intern_itdh[column.theIntern][t][d][h] =true;
                                    break;
                                }
                            }
                        }
                    }
                }

            }
            else
            {
                for (int h = 0; h < data.General.Hospitals && result; h++)
                {
                    for (int t = 0; t < data.General.TimePriods && result; t++)
                    {
                        for (int w = 0; w < data.General.HospitalWard; w++)
                        {
                            tmpdemand_twh[t][w][h] = demand_twh[t][w][h];
                        }
                    }
                }
            }

            return result;
        }
    }
}

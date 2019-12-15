using System;
using System.Collections;
using System.Text;
using NestedHungarianAlgorithm;
using DataLayer;
using ColumnAndBranchInfo;

namespace LowerBound
{
    public class HungrianBasedLowerbound
    {

        public ArrayList ActiveList;
        public AllData data;
        
        public bool[][][][] motivationList_itdh;
        public double [][][][] columnInfo_itdh;
        public bool[][] NotRequiredSkill_id;
        

        HungarianNode Root;
        public OptimalSolution LBResult;

        public HungrianBasedLowerbound(AllData allData, ArrayList columns, string Name) {
            initial(allData, columns);
            procedure(columns,Name);

        }

        public void initial(AllData allData, ArrayList columns) 
        {
            data = allData;
            ActiveList = new ArrayList();
            new ArrayInitializer().CreateArray(ref motivationList_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, false);
            new ArrayInitializer().CreateArray(ref columnInfo_itdh, data.General.Interns, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref NotRequiredSkill_id, data.General.Interns, data.General.Disciplines, false);
            
        }

        public void procedure(ArrayList columns, string Name) 
        {
            setColumnInfo(columns);
            TimelineBasedHungarianAlgorithm(columns);

            setSolution(Name);
        }

        public void TimelineBasedHungarianAlgorithm(ArrayList columns)
        {
            setMotivationfForTime(0, new PositionMap[data.General.Interns], true, columns);
            Root = new HungarianNode(0, data, new HungarianNode(), motivationList_itdh, NotRequiredSkill_id);
            
            ActiveList.Add(Root);
            for (int t = 1; t < data.General.TimePriods; t++)
            {
                setMotivationfForTime(t, ((HungarianNode)ActiveList[ActiveList.Count - 1]).LastPosition_i, false, columns);
                HungarianNode nestedHungrian = new HungarianNode(t, data, (HungarianNode)ActiveList[t - 1], motivationList_itdh, NotRequiredSkill_id);
                ActiveList.Add(nestedHungrian);
            }
        }

        public void setColumnInfo(ArrayList columns) 
        {
            foreach (ColumnInternBasedDecomposition clmn in columns)
            {
                if (clmn.xVal > 0 + data.AlgSettings.RCepsi)
                {
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            for (int h = 0; h < data.General.Hospitals; h++)
                            {
                                if (clmn.S_tdh[t][d][h])
                                {
                                    columnInfo_itdh[clmn.theIntern][t][d][h] += clmn.xVal;
                                }
                            }
                        }
                    }
                }
            }
        }

        public void setMotivationfForTime(int timeIndex, PositionMap[] lastPos, bool ifroot, ArrayList columns) 
        {
            if (ifroot)
            {
                for (int i = 0; i < data.General.Interns; i++)
                {
                    int indexH = -1;
                    int indexD = -1;
                    double maxVal = 0;

                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            if (columnInfo_itdh[i][timeIndex][d][h] > maxVal)
                            {
                                maxVal = columnInfo_itdh[i][timeIndex][d][h];
                                indexD = d;
                                indexH = h;
                            }
                        }
                    }

                    if (indexH >= 0)
                    {
                        motivationList_itdh[i][timeIndex][indexD][indexH] = true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < data.General.Interns; i++)
                {
                    int dInd = lastPos[i].dIndex;
                    int hInd = lastPos[i].HIndex;
                    int tInd = lastPos[i].tIndex;
                    double maxVal = 0 + data.AlgSettings.RCepsi;
                    int colIndex = -1;
                    int counter = -1;
                    foreach (ColumnInternBasedDecomposition column in columns)
                    {
                        counter++;
                        if (column.xVal > maxVal && column.theIntern == i && column.S_tdh[tInd][dInd][hInd])
                        {
                            maxVal = column.xVal;
                            colIndex = counter;
                        }
                    }
                    for (int d = 0; d < data.General.Disciplines && colIndex >= 0; d++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            if (((ColumnInternBasedDecomposition)columns[colIndex]).S_tdh[timeIndex][d][h])
                            {
                                motivationList_itdh[i][timeIndex][d][h] = true;
                            }
                        }
                    }
                    
                }
            }
        }

        public void setSolution(string Name)
        {
            LBResult = new OptimalSolution(data);
            for (int i = 0; i < data.General.Interns; i++)
            {
                bool[] disStatus = new bool[data.General.Disciplines]; // if the hungarian waits we just now the duration of the prevuis discipline prolonged 
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    disStatus[d] = false;
                }

                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    bool assignedDisc = false;
                    for (int t = 0; t < data.General.TimePriods && !assignedDisc; t++)
                    {
                        for (int h = 0; h < data.General.Hospitals + 1 && !assignedDisc; h++)
                        {
                            if (((HungarianNode)ActiveList[ActiveList.Count - 1]).ResidentSchedule_it[i][t].dIndex == d
                                && ((HungarianNode)ActiveList[ActiveList.Count - 1]).ResidentSchedule_it[i][t].HIndex == h
                                && !disStatus[d])
                            {
                                assignedDisc = true;
                                LBResult.Intern_itdh[i][t][d][h] = true;
                                disStatus[d] = true;
                                break;
                            }
                        }
                    }
                }
            }
            LBResult.WriteSolution(data.allPath.OutPutGr, "NHA" + Name);
        }
    }
}

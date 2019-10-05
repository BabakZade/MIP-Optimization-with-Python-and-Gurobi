using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
using DataLayer;

namespace BranchAndPriceAlgorithm
{
    public class BranchAndPrice
    {
        public OptimalSolution optimalSol;
        public ArrayList active_list;

        public double upper_bound;


        public int active_list_count;
        public long tree_level;
        public bool there_is_no_branch;
        public int branch_counter;
        public bool upper_wrong;

        public long ElappsedTime;
        public bool is_mip;
     

        public int totalNumberOfColumn;
        public int totalCreatedColumn;
        AllData data;

        public BranchAndPrice(DataLayer.AllData allData, string insName)
        {
            Algorithem(allData, insName);
        }

        public void Algorithem(DataLayer.AllData allData, string insName)
        {
            Initialize(allData) ;
            branch_and_price(insName);
        }

        public void Initialize(DataLayer.AllData allData)
        {
            data = allData;
            optimalSol = new OptimalSolution(data);
            
            active_list = new ArrayList();

        }

        public void branch_and_price( string insName)
        {
            Stopwatch stopNow = new Stopwatch();
            stopNow.Start();

            upper_bound = 0;

            tree_level = 0;
            active_list_count = 0;

            Node root = new Node(data, insName);
            
            ElappsedTime = root.ElappsedTime;
            
            totalNumberOfColumn = root.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn = root.NodeCG.RMP.DataColumn.Count;

            is_mip = false;

            // add root to tree	
            if (root.is_mip)
            {
                
                setOptimalSol(root, insName);
                is_mip = true;
            }
            else
            {
                root.Node_id = 1;
                root.level = 1;
                active_list.Add(root);
                upper_bound = root.Upperbound;
            }
           

            branch_counter = 0;
            upper_wrong = false;
            while (active_list.Count != 0)
            {
                //break;
                if (stopNow.ElapsedMilliseconds / 1000 > data.AlgSettings.BPTime * 3)
                {
                    break;
                }
                branch_counter += 2;
                Branching(insName);
            }

        }

        public void Branching(string insName)
        {
            if (((Node)active_list[0]).Upperbound - optimalSol.Obj > -data.AlgSettings.RCepsi)
            {
                active_list.RemoveAt(0);
                return;
            }


            Branch left_node;
            Branch right_node;

            there_is_no_branch = false;
            left_node = new Branch();
            left_node = findBranchStrategyForPatientMaxVague((Node)active_list[0]);
            right_node = new Branch(left_node);
            left_node.branch_status = false;
            right_node.branch_status = true;


            Node tmp_left = new Node(data, (Node)active_list[0], left_node, insName);


            ElappsedTime += tmp_left.ElappsedTime;
            
            totalNumberOfColumn += tmp_left.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn += tmp_left.NodeCG.count_MIP;


            tmp_left.fatherNodeId = ((Node)active_list[0]).Node_id;
            tmp_left.Node_id = 2 * tmp_left.fatherNodeId + 1;
            tmp_left.level = ((Node)active_list[0]).level + 1;

            Node tmp_right = new Node(data, (Node)active_list[0], right_node, insName);

            
            ElappsedTime += tmp_right.ElappsedTime;
           
            totalNumberOfColumn += tmp_right.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn += tmp_right.NodeCG.count_MIP;
            
            tmp_right.level = ((Node)active_list[0]).level + 1;
            tmp_right.fatherNodeId = ((Node)active_list[0]).Node_id;
            tmp_right.Node_id = 2 * tmp_right.fatherNodeId + 1;
            active_list.RemoveAt(0);

            if (tmp_left.level > tree_level)
            {
                tree_level = tmp_left.level;
            }

            putInActiveListInOrder(tmp_left, insName);
            
            putInActiveListInOrder(tmp_right, insName);
            active_list_count = active_list.Count;

        }

        public void putInActiveListInOrder(Node theNode, string insName)
        {
           
            // update upperbound
            if (theNode.is_mip && theNode.Upperbound >= upper_bound)
            {
                upper_bound = theNode.Upperbound;
                setOptimalSol(theNode, insName);
            }

            int pos = -1;
            bool insert_in_br = false;
            if (!theNode.is_mip)
            {
                insert_in_br = true;
                for (int i = 0; i < active_list.Count; i++)
                {
                    if (theNode.Upperbound >= ((Node)active_list[i]).Upperbound)
                    {
                        pos = i;
                        break;
                    }
                    pos = active_list.Count;
                }
            }
            if (pos >= 0)
            {
                active_list.Insert(pos, theNode);
            }
            else if (insert_in_br && active_list.Count == 0)
            {
                active_list.Insert(0, theNode);
            }
        }

        /// <summary>
        /// this function first finds the day and room which has mmore column in base, then for this day room fiinds the patient with max vaguness
        /// </summary>
        /// <param name="theNode">the node which needs this branch</param>
        /// <returns></returns>
        public Branch findBranchStrategyForPatientMaxVague(Node theNode)
        {
            Branch brnch = new Branch();
            int[][][][] s_itdh = new int[data.General.Interns][][][];
            int[] internColumn = new int[data.General.Interns];
            for (int i = 0; i < data.General.Interns; i++)
            {
                internColumn[i] = 0;
                s_itdh[i] = new int[data.General.TimePriods][][];
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    s_itdh[i][t] = new int[data.General.Disciplines][];
                    for (int r = 0; r < data.General.Disciplines; r++)
                    {
                        s_itdh[i][t][r] = new int[data.General.Hospitals];
                        for (int d = 0; d < data.General.Hospitals; d++)
                        {
                            s_itdh[i][t][r][d] = 0;
                        }
                    }
                }
            }

            // find intern and column
            int internIndex = -1;
            int columnIndex = -1;
            int MaxVal = 0;
            int counter = -1;
            foreach (ColumnInternBasedDecomposition clmn in theNode.NodeCG.RMP.DataColumn)
            {
                counter++;
                if (clmn.xVal <= 0)
                {
                    continue;
                }
                for (int t = 0; t < data.General.TimePriods; t++)
                {                  
                    for (int d = 0; d < data.General.Disciplines; d++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            if (clmn.S_tdh[t][d][h])
                            {
                                s_itdh[clmn.theIntern][t][d][h]++;
                            }
                        }
                    }
                }
                internColumn[clmn.theIntern]++;
                if (internColumn[clmn.theIntern] > MaxVal)
                {
                    MaxVal = internColumn[clmn.theIntern];
                    internIndex = clmn.theIntern;
                    columnIndex = counter;
                }
            }

            // find the one with high reputation
            int MaxCout = 0;
            int discIndex = -1;

            for (int d = 0; d < data.General.Disciplines; d++)
            {
                int count = 0;
                bool checkDis = false;
                for (int t = 0; t < data.General.TimePriods && !checkDis; t++)
                {
                    for (int h = 0; h < data.General.Hospitals && !checkDis; h++)
                    {
                        if (((ColumnInternBasedDecomposition)theNode.NodeCG.RMP.DataColumn[columnIndex]).S_tdh[t][d][h])
                        {
                            checkDis = true;
                        }
                    }
                }
                if (!checkDis)
                {
                    continue;
                }
                for (int i = 0; i < data.General.Interns; i++)
                {
                    if (internColumn[i] == 1) // it is already integer
                    {
                        continue;
                    }
                    for (int t = 0; t < data.General.TimePriods; t++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            count += s_itdh[i][t][d][h] * internColumn[i];
                        }
                    }
                    if (count > MaxCout)
                    {
                        discIndex = d;
                        MaxCout = count;
                    }
                }
            }

            brnch.BrDisc = discIndex;
            // find pre discipline 
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                if (((ColumnInternBasedDecomposition)theNode.NodeCG.RMP.DataColumn[columnIndex]).Y_dD[d][discIndex])
                {
                    brnch.BrPrDisc = d;
                    break;
                }
            }
            return brnch;
        }

        public void setOptimalSol(Node theNode, string insName)
        {
            if (theNode.Upperbound > optimalSol.Obj)
            {
                
                optimalSol = new OptimalSolution(data);
                foreach (ColumnInternBasedDecomposition item in theNode.NodeCG.RMP.DataColumn)
                {
                    if (item.xVal > 1 - data.AlgSettings.RCepsi)
                    {
                        for (int t = 0; t < data.General.TimePriods; t++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                for (int h = 0; h < data.General.Hospitals + 1; h++)
                                {
                                    optimalSol.Intern_itdh[item.theIntern][t][d][h] = item.S_tdh[t][d][h];
                                }
                            }
                        }
                    }
                    

                }
                optimalSol.WriteSolution(data.allPath.OutPutGr, "BPOpt"+ insName);
            }
        }

        

    }
}

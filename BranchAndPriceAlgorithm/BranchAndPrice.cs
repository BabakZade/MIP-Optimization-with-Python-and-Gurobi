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
        public double best_sol;


        public int active_list_count;
        public long tree_level;
        public bool there_is_no_branch;
        public int branch_counter;
        public bool upper_wrong;

        public long ElappsedTime;
        public long master_Time;
        public long subMIP_time;
        public bool is_mip;
     

        public int totalNumberOfColumn;
        public int totalCreatedColumn;
        public bool[][][][] BrnchHistory_yidDh;
        ArrayList allBranches;
        
        AllData data;

        public BranchAndPrice(DataLayer.AllData allData, string insName)
        {
            Algorithem(allData, insName);
        }

        public void Algorithem(DataLayer.AllData allData, string insName)
        {
            Initialize(allData,insName) ;
            branch_and_price(insName);
        }

        public void Initialize(DataLayer.AllData allData, string insName)
        {
            data = allData;
            optimalSol = new OptimalSolution(data);
            active_list = new ArrayList();
            new ArrayInitializer().CreateArray(ref BrnchHistory_yidDh, data.General.Interns, data.General.Disciplines+1, data.General.Disciplines + 1, data.General.Hospitals, false);
            allBranches = new ArrayList();
            allBranches.Add(new Branch()); // adding the root
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
            master_Time = root.NodeCG.master_Time;
            subMIP_time = root.NodeCG.subMIP_time;
            is_mip = false;
            ((Branch)allBranches[0]).BrObj = root.Upperbound;
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
            if (((Node)active_list[0]).Upperbound - best_sol < data.AlgSettings.RCepsi)
            {
                active_list.RemoveAt(0);
                return;
            }
            Branch lastBr = new Branch();
            if (((Node)active_list[0]).branch_trace.Count > 0)
            {
                lastBr = (Branch)((Node)active_list[0]).branch_trace[0];
            }
            

            Branch left_node;
            Branch right_node;

            there_is_no_branch = false;
            left_node = new Branch();
            left_node = findBranchStrategyForPatientMaxVague((Node)active_list[0]);
            right_node = new Branch(left_node);
            left_node.branch_status = false;
            right_node.branch_status = true;
            left_node.BrID = lastBr.BrID * 2 + 1;
            right_node.BrID = left_node.BrID + 1;
            
            Node tmp_left = new Node(data, (Node)active_list[0], left_node, insName);

            putInActiveListInOrder(tmp_left, insName);
            ElappsedTime += tmp_left.ElappsedTime;
            
            totalNumberOfColumn += tmp_left.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn += tmp_left.NodeCG.count_MIP;
            master_Time += tmp_left.NodeCG.master_Time;
            subMIP_time += tmp_left.NodeCG.subMIP_time;

            tmp_left.fatherNodeId = ((Node)active_list[0]).Node_id;
            tmp_left.Node_id = 2 * tmp_left.fatherNodeId + 1;
            tmp_left.level = ((Node)active_list[0]).level + 1;

            Node tmp_right = new Node(data, (Node)active_list[0], right_node, insName);

            left_node.BrObj = tmp_left.Upperbound;
            right_node.BrObj = tmp_right.Upperbound;
            left_node.BrMIP = tmp_left.is_mip;
            right_node.BrMIP = tmp_right.is_mip;
            allBranches.Add(left_node);
            allBranches.Add(right_node);
            new DrawBranchingTree(allBranches, data.allPath.OutPutGr, insName);

            ElappsedTime += tmp_right.ElappsedTime;
           
            totalNumberOfColumn += tmp_right.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn += tmp_right.NodeCG.count_MIP;
            master_Time += tmp_right.NodeCG.master_Time;
            subMIP_time += tmp_right.NodeCG.subMIP_time;

            tmp_right.level = ((Node)active_list[0]).level + 1;
            tmp_right.fatherNodeId = ((Node)active_list[0]).Node_id;
            tmp_right.Node_id = 2 * tmp_right.fatherNodeId + 1;
            active_list.RemoveAt(0);

            if (tmp_left.level > tree_level)
            {
                tree_level = tmp_left.level;
            }

            
            
            putInActiveListInOrder(tmp_right, insName);
            active_list_count = active_list.Count;

        }

        public void putInActiveListInOrder(Node theNode, string insName)
        {
           
            // update upperbound
            if (theNode.is_mip)
            {
                setOptimalSol(theNode, insName);
            }
            if (theNode.Upperbound >= upper_bound)
            {
                upper_bound = theNode.Upperbound;
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
                if (clmn.xVal < data.AlgSettings.RCepsi || clmn.xVal > 1 - data.AlgSettings.RCepsi)
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
                                int prD = -1;
                                for (int dd = 0; dd < data.General.Disciplines + 1; dd++)
                                {
                                    if (clmn.Y_dDh[dd][d+1][h])
                                    {
                                        prD = dd;
                                    }
                                }
                                if (BrnchHistory_yidDh[clmn.theIntern][prD][d+1][h])
                                {
                                    continue;
                                }
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
            brnch.BrIntern = internIndex;
            brnch.BrDisc = discIndex + 1;
            // find pre discipline 
            for (int d = 0; d < data.General.Disciplines + 1; d++)
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    if (((ColumnInternBasedDecomposition)theNode.NodeCG.RMP.DataColumn[columnIndex]).Y_dDh[d][brnch.BrDisc][h])
                    {
                        brnch.BrPrDisc = d;
                        brnch.BrHospital = h;
                        break;
                    }
                }
                
            }
            BrnchHistory_yidDh[brnch.BrIntern][brnch.BrPrDisc][brnch.BrDisc][brnch.BrHospital] = true;
            return brnch;
        }

        public void setOptimalSol(Node theNode, string insName)
        {
            if (theNode.Upperbound > best_sol)
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
                best_sol = theNode.Upperbound;
            }
        }

        

    }
}

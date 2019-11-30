using System;
using System.Collections;
using System.Text;
using System.IO;
using System.Diagnostics;
using DataLayer;
using ColumnAndBranchInfo;


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
        public bool[][][][] BrnchHistory_Sidth;
        public bool[] BrnchIntern_i;
        ArrayList allBranches;
        ArrayList allColumns;
        
        AllData data;

        public BranchAndPrice(DataLayer.AllData allData, string insName, int procedureType)
        {
            Algorithem(allData, insName, procedureType);
        }

        public void Algorithem(DataLayer.AllData allData, string insName, int procedureType)
        {
            Initialize(allData,insName) ;
            branch_and_price(insName, procedureType);
        }

        public void Initialize(DataLayer.AllData allData, string insName)
        {
            data = allData;
            allColumns = new ArrayList();
            optimalSol = new OptimalSolution(data);
            active_list = new ArrayList();
            new ArrayInitializer().CreateArray(ref BrnchHistory_yidDh, data.General.Interns, data.General.Disciplines+1, data.General.Disciplines + 1, data.General.Hospitals, false);
            new ArrayInitializer().CreateArray(ref BrnchHistory_Sidth, data.General.Interns, data.General.Disciplines, data.General.TimePriods, data.General.Hospitals, false);
            new ArrayInitializer().CreateArray(ref BrnchIntern_i, data.General.Interns, false);
            allBranches = new ArrayList();
            allBranches.Add(new Branch()); // adding the root
            best_sol = -data.AlgSettings.BigM;
        }

        public void branch_and_price( string insName, int procedureType)
        {
            Stopwatch stopNow = new Stopwatch();
            stopNow.Start();

            upper_bound = 0;

            tree_level = 0;
            active_list_count = 0;

            Node root = new Node(data, insName, procedureType);
            
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
                addColumns(root);
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
                Branching(insName, procedureType);
            }

        }

        public void Branching(string insName, int procedureType)
        {
            if (((Node)active_list[0]).Upperbound - best_sol < data.AlgSettings.RCepsi)
            {
                active_list.RemoveAt(0);
                return;
            }

            // if all variables are integer the objective will be integer 
            if (Math.Floor(((Node)active_list[0]).Upperbound) - best_sol < data.AlgSettings.RCepsi)
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
            left_node = findBranch((Node)active_list[0]);
            right_node = new Branch(left_node);
            if (left_node.BrHospital == -1)
            {
                active_list.RemoveAt(0);
                return;
            }
            left_node.branch_status = false;
            right_node.branch_status = true;
            left_node.BrID = lastBr.BrID * 2 + 1;
            right_node.BrID = left_node.BrID + 1;

            Node tmp_left = new Node(data, (Node)active_list[0], allColumns, left_node, insName, procedureType);

            
            ElappsedTime += tmp_left.ElappsedTime;
            
            totalNumberOfColumn += tmp_left.NodeCG.RMP.DataColumn.Count;
            totalCreatedColumn += tmp_left.NodeCG.count_MIP;
            master_Time += tmp_left.NodeCG.master_Time;
            subMIP_time += tmp_left.NodeCG.subMIP_time;

            tmp_left.fatherNodeId = ((Node)active_list[0]).Node_id;
            tmp_left.Node_id = 2 * tmp_left.fatherNodeId + 1;
            tmp_left.level = ((Node)active_list[0]).level + 1;

            Node tmp_right = new Node(data, (Node)active_list[0], allColumns, right_node, insName, procedureType);

            left_node.BrObj = Math.Round( tmp_left.Upperbound, 2);
            right_node.BrObj = Math.Round(tmp_right.Upperbound,2);
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


            putInActiveListInOrder(tmp_left, insName);
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
            else
            {
                addColumns(theNode);
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
        public Branch findBranchStrategyDisciplinePrecedence(Node theNode)
        {
            Branch brnch = new Branch();
            int[][][][] y_idDh = new int[data.General.Interns][][][];
            int[] internColumn = new int[data.General.Interns];
            int[] spreadedDisc_D = new int[data.General.Disciplines + 1];
            for (int i = 0; i < data.General.Interns; i++)
            {
                internColumn[i] = 0;
                y_idDh[i] = new int[data.General.Disciplines+1][][];
                
                for (int t = 0; t < data.General.Disciplines+1; t++)
                {
                    spreadedDisc_D[t] = 0;
                    y_idDh[i][t] = new int[data.General.Disciplines+1][];
                    for (int r = 0; r < data.General.Disciplines+1; r++)
                    {
                        y_idDh[i][t][r] = new int[data.General.Hospitals];
                        for (int d = 0; d < data.General.Hospitals; d++)
                        {
                            y_idDh[i][t][r][d] = 0;
                        }
                    }
                }
            }

            // find intern and column
            int internIndex = -1;
            int columnIndex = -1;
            int MaxVal = 0;
            int counter = -1;
            foreach (ColumnInternBasedDecomposition clmn in theNode.relatedColumn)
            {
                counter++;
                if (clmn.xVal < data.AlgSettings.RCepsi || clmn.xVal > 1 - data.AlgSettings.RCepsi)
                {
                    continue;
                }
                for (int D = 0; D < data.General.Disciplines + 1; D++)
                {                  
                    for (int d = 0; d < data.General.Disciplines+1; d++)
                    {
                        for (int h = 0; h < data.General.Hospitals; h++)
                        {
                            if (clmn.Y_dDh[D][d][h])
                            {
                                y_idDh[clmn.theIntern][D][d][h]++;
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
                if (BrnchIntern_i[clmn.theIntern])
                {
                    break;
                }
            }

            // find the one with high reputation and spreaded 
            int MaxSp = 0;
            int discIndex = -1;
            int discPrIndex = -1;
            int hospIndex = -1;
            for (int d = 1; d < data.General.Disciplines + 1; d++)
            {
                for (int D = 0; D < data.General.Disciplines + 1; D++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        if (y_idDh[internIndex][d][D][h] > 0 && !BrnchHistory_yidDh[internIndex][d][D][h])
                        {
                            spreadedDisc_D[D]++;
                        }
                        if (MaxSp < spreadedDisc_D[D])
                        {
                            MaxSp = spreadedDisc_D[D];
                            discIndex = D;
                            discPrIndex = d;
                            hospIndex = h;
                        }
                    }
                }
            }

            
            brnch.BrTypePrecedence = true;
            brnch.BrPrDisc = discPrIndex;
            brnch.BrHospital = hospIndex;
            brnch.BrIntern = internIndex;
            brnch.BrDisc = discIndex;
            if (hospIndex != -1)
            {

                BrnchHistory_yidDh[brnch.BrIntern][brnch.BrPrDisc][brnch.BrDisc][brnch.BrHospital] = true;
                BrnchIntern_i[brnch.BrIntern] = true;
            }
            return brnch;
        }


        /// <summary>
        /// this function first finds the day and room which has mmore column in base, then for this day room fiinds the patient with max vaguness
        /// </summary>
        /// <param name="theNode">the node which needs this branch</param>
        /// <returns></returns>
        public Branch findBranchStrategyDisciplineStartTime(Node theNode)
        {
            Branch brnch = new Branch();
            int[][][][] s_itdh = new int[data.General.Interns][][][];
            int[] internColumn = new int[data.General.Interns];
            int[] spreadedDisc_D = new int[data.General.Disciplines];
            for (int i = 0; i < data.General.Interns; i++)
            {
                internColumn[i] = 0;
                s_itdh[i] = new int[data.General.TimePriods][][];
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    
                    s_itdh[i][t] = new int[data.General.Disciplines][];
                    for (int r = 0; r < data.General.Disciplines; r++)
                    {
                        spreadedDisc_D[r] = 0;
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
            foreach (ColumnInternBasedDecomposition clmn in theNode.relatedColumn)
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
                                if (BrnchHistory_Sidth[clmn.theIntern][d][t][h])
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
            int timeIndex = -1;
            int hospitalIndex = -1;
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                bool checkDis = false;
                for (int t = 0; t < data.General.TimePriods && !checkDis; t++)
                {
                    for (int h = 0; h < data.General.Hospitals && !checkDis; h++)
                    {
                        if (((ColumnInternBasedDecomposition)theNode.relatedColumn[columnIndex]).S_tdh[t][d][h])
                        {
                            checkDis = true;
                        }
                    }
                }
                if (!checkDis)
                {
                    continue;
                }
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        if (s_itdh[internIndex][t][d][h] > 0 && !BrnchHistory_Sidth[internIndex][d][t][h])
                        {
                            spreadedDisc_D[d]++;
                        }
                        if (spreadedDisc_D[d] > MaxCout)
                        {
                            discIndex = d;
                            MaxCout = spreadedDisc_D[d];                         
                        }
                    }
                }
            }

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int h = 0; h < data.General.Hospitals ; h++)
                {
                    if (((ColumnInternBasedDecomposition)theNode.relatedColumn[columnIndex]).S_tdh[t][discIndex][h])
                    {
                        hospitalIndex = h;
                        timeIndex = t;
                    }
                }
            }
            brnch.BrTypeStartTime = true;
            brnch.BrTime = timeIndex;
            brnch.BrIntern = internIndex;
            brnch.BrDisc = discIndex;
            brnch.BrHospital = hospitalIndex;
            if (hospitalIndex != -1)
            {

                BrnchIntern_i[brnch.BrIntern] = true;
                BrnchHistory_Sidth[brnch.BrIntern][brnch.BrDisc][brnch.BrTime][brnch.BrHospital] = true;
            }
            return brnch;
        }


        /// <summary>
        /// this function first finds the day and room which has mmore column in base, then for this day room fiinds the patient with max vaguness
        /// </summary>
        /// <param name="theNode">the node which needs this branch</param>
        /// <returns></returns>
        public Branch findBranchStrategyMinDemand(Node theNode)
        {
            Branch brnch = new Branch();
            
           
            int wardIndex = -1;
            int hospitalIndex = -1;
            int timeIndex = -1;
            double demvalue = -1;
            int counter = -1;
            bool findBranch = false;
            for (int t = 0; t < data.General.TimePriods && !findBranch; t++)
            {
                for (int w = 0; w < data.General.HospitalWard && !findBranch; w++)
                {
                    for (int h = 0; h < data.General.Hospitals && !findBranch; h++)
                    {
                        double value = Math.Round(theNode.minDem_twh[t][w][h],2);
                        if (Math.Ceiling(value) != Math.Floor(value))
                        {
                            findBranch = true;
                            wardIndex = w;
                            timeIndex = t;
                            hospitalIndex = h;
                            demvalue = value;
                        }
                    }
                }
            }

            brnch.BrTypeMinDemand = true;
            brnch.BrTime = timeIndex;
            brnch.BrWard = wardIndex;
            brnch.BrHospital = hospitalIndex;
            brnch.BrDemVal = demvalue;
            return brnch;
        }

        /// <summary>
        /// this function first finds the day and room which has mmore column in base, then for this day room fiinds the patient with max vaguness
        /// </summary>
        /// <param name="theNode">the node which needs this branch</param>
        /// <returns></returns>
        public Branch findBranchStrategyResDemand(Node theNode)
        {
            Branch brnch = new Branch();


            int wardIndex = -1;
            int hospitalIndex = -1;
            int timeIndex = -1;
            double demvalue = -1;
            int counter = -1;
            bool findBranch = false;
            for (int t = 0; t < data.General.TimePriods && !findBranch; t++)
            {
                for (int w = 0; w < data.General.HospitalWard && !findBranch; w++)
                {
                    for (int h = 0; h < data.General.Hospitals && !findBranch; h++)
                    {
                        double value = Math.Round(theNode.resDem_twh[t][w][h], 2);
                        if (Math.Ceiling(value) != Math.Floor(value))
                        {
                            findBranch = true;
                            wardIndex = w;
                            timeIndex = t;
                            hospitalIndex = h;
                            demvalue = value;
                        }
                    }
                }
            }

            brnch.BrTypeResDemand = true;
            brnch.BrTime = timeIndex;
            brnch.BrWard = wardIndex;
            brnch.BrHospital = hospitalIndex;
            brnch.BrDemVal = demvalue;
            return brnch;
        }

        /// <summary>
        /// this function first finds the day and room which has mmore column in base, then for this day room fiinds the patient with max vaguness
        /// </summary>
        /// <param name="theNode">the node which needs this branch</param>
        /// <returns></returns>
        public Branch findBranchStrategyEmrDemand(Node theNode)
        {
            Branch brnch = new Branch();


            int wardIndex = -1;
            int hospitalIndex = -1;
            int timeIndex = -1;
            double demvalue = -1;
            int counter = -1;
            bool findBranch = false;
            for (int t = 0; t < data.General.TimePriods && !findBranch; t++)
            {
                for (int w = 0; w < data.General.HospitalWard && !findBranch; w++)
                {
                    for (int h = 0; h < data.General.Hospitals && !findBranch; h++)
                    {
                        double value = Math.Round(theNode.emrDem_twh[t][w][h], 2);
                        if (Math.Ceiling(value) != Math.Floor(value))
                        {
                            findBranch = true;
                            wardIndex = w;
                            timeIndex = t;
                            hospitalIndex = h;
                            demvalue = value;
                        }
                    }
                }
            }

            brnch.BrTypeEmrDemand = true;
            brnch.BrTime = timeIndex;
            brnch.BrWard = wardIndex;
            brnch.BrHospital = hospitalIndex;
            brnch.BrDemVal = demvalue;
            return brnch;
        }


        public Branch findBranch(Node theNode)
        {
            // emergency demand
            Branch branch = new Branch();

            //branch = new Branch(findBranchStrategyEmrDemand(theNode));

            //if (branch.BrHospital != -1)
            //{
            //    return branch;
            //}
            //else // reserved demand
            //{
            //    branch = new Branch(findBranchStrategyResDemand(theNode));                
            //}
            
            //if (branch.BrHospital != -1)
            //{
            //    return branch;
            //}
            //else // min demand
            //{
            //    branch = new Branch(findBranchStrategyMinDemand(theNode));
            //}

            if (branch.BrHospital != -1)
            {
                return branch;
            }
            else // precedence 
            {
                branch = new Branch(findBranchStrategyDisciplinePrecedence(theNode));
            }

            if (branch.BrHospital != -1)
            {
                return branch;
            }
            // precedence 

            branch = new Branch(findBranchStrategyDisciplineStartTime(theNode));
            return branch;
            
        }

        public void setOptimalSol(Node theNode, string insName)
        {
            if (theNode.Upperbound > best_sol)
            {                
                optimalSol = new OptimalSolution(data);
                optimalSol.copyRosters(theNode.optimalsolution.Intern_itdh);
                
                optimalSol.WriteSolution(data.allPath.OutPutGr, "BPOpt"+ insName);
                best_sol = theNode.Upperbound;
            }
        }

        public void addColumns(Node theNode)
        {
            
            foreach (ColumnInternBasedDecomposition newColumn in theNode.relatedColumn)
            {
                bool exist = false;
                foreach (ColumnInternBasedDecomposition column in allColumns)
                {
                    if (column.Compare(newColumn,data))
                    {
                        exist = true;
                        break;
                    }
                }

                if (!exist)
                {
                    allColumns.Add(new ColumnInternBasedDecomposition(newColumn,data));
                }
            }
        }

    }
}

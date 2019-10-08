using System;
using System.Collections;
using System.Text;
using DataLayer;
using System.Diagnostics;

namespace BranchAndPriceAlgorithm
{
    public class Node
    {

        public long Node_id;
        public long level;
        public long fatherNodeId;
        public bool is_mip;
        public OptimalSolution optimalsolution;
        
        public double Upperbound;

        public long ElappsedTime;

        public int[] BranchCounter_i;

        AllData data;
       

        public ArrayList branch_trace;

        public ColumnGenration NodeCG;

        public Node(DataLayer.AllData allData, string insName)
        {
            data = allData;
            Upperbound = -1; 
            BranchCounter_i = new int[allData.General.Interns];
            for (int i = 0; i < allData.General.Interns; i++)
            {
                BranchCounter_i[i] = 1;
            }
            branch_trace = new ArrayList();                

            node_procedure(new ArrayList(), new ArrayList(), insName);

        }

        public Node(AllData allData, Node FatherNode, Branch new_branch, string insName)
        {
            data = allData;
            branch_trace = new ArrayList();
            BranchCounter_i = new int[allData.General.Interns];
            for (int i = 0; i < allData.General.Interns; i++)
            {
                BranchCounter_i[i] = 1;
            }
            branch_trace.Add(new_branch);
            if (FatherNode.branch_trace.Count > 0)
            {
                CopyBranchTrace(FatherNode.branch_trace);
            }
            
            node_procedure(FatherNode.NodeCG.RMP.DataColumn, branch_trace, insName);



        }

        public void CopyBranchTrace(ArrayList FatherBranch)
        {
            foreach (Branch item in FatherBranch)
            {
                branch_trace.Add(item);
            }
        }



        public void node_procedure(ArrayList FatherColumn, ArrayList AllBranch, string insName)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NodeCG = new ColumnGenration(data, FatherColumn, AllBranch, insName);
            sw.Stop();
            ElappsedTime = sw.ElapsedMilliseconds / 1000;
            
            if (NodeCG.RMP.is_mip)
            {
                is_mip = true;
            }
            setBounds();
        }
        public void setBounds()
        {
            Upperbound = NodeCG.BestSolution;
            if (is_mip)
            {
                optimalsolution = new OptimalSolution(data);
                foreach (ColumnInternBasedDecomposition item in NodeCG.RMP.DataColumn)
                {
                    if (item.xVal > 1 - data.AlgSettings.RCepsi)
                    {
                        for (int t = 0; t < data.General.TimePriods; t++)
                        {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                for (int h = 0; h < data.General.Hospitals + 1; h++)
                                {
                                    optimalsolution.Intern_itdh[item.theIntern][t][d][h] = item.S_tdh[t][d][h];
                                }
                            }
                        }
                    }


                }
                
            }
            
        }


    }
}

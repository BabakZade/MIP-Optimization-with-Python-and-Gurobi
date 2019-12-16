using System;
using System.Collections;
using System.Text;
using DataLayer;
using System.Diagnostics;
using ColumnAndBranchInfo;

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
        public  ArrayList relatedColumn;
       

        public ArrayList branch_trace;
        public bool[][][][] BrnchHistory_yidDh;
        public bool[][][][] BrnchHistory_Sidth;
        public bool[] BrnchIntern_i;

        public ColumnGenration NodeCG;
        public double[][][] minDem_twh;
        public double[][][] resDem_twh;
        public double[][][] emrDem_twh;
        public double lowerBound;


        public Node(DataLayer.AllData allData, string insName, int procedureType)
        {
            data = allData;
            intial();
            Upperbound = -1; 
            BranchCounter_i = new int[allData.General.Interns];
            for (int i = 0; i < allData.General.Interns; i++)
            {
                BranchCounter_i[i] = 1;
            }
            branch_trace = new ArrayList();                

            node_procedure(new ArrayList(), new ArrayList(), insName, procedureType);

        }

        public Node(AllData allData, Node FatherNode, ArrayList allColumn, Branch new_branch, string insName, int procedureType, double lowerBound)
        {
            data = allData;
            intial();
            this.lowerBound = lowerBound;
            BranchCounter_i = new int[allData.General.Interns];
            for (int i = 0; i < allData.General.Interns; i++)
            {
                BranchCounter_i[i] = 1;
            }
            
            if (new_branch.BrHospital >= 0)
            {
                CopyBranchTrace(FatherNode.branch_trace,new_branch);
            }

            node_procedure(allColumn, branch_trace, insName, procedureType);
        }
        public void intial() 
        {
            // initialize
            lowerBound = -data.AlgSettings.BigM;
            branch_trace = new ArrayList();
            new ArrayInitializer().CreateArray(ref BrnchHistory_yidDh, data.General.Interns, data.General.Disciplines + 1, data.General.Disciplines + 1, data.General.Hospitals, false);
            new ArrayInitializer().CreateArray(ref BrnchHistory_Sidth, data.General.Interns, data.General.Disciplines, data.General.TimePriods, data.General.Hospitals, false);
            new ArrayInitializer().CreateArray(ref BrnchIntern_i, data.General.Interns, false);
        }
        public void CopyBranchTrace(ArrayList FatherBranch, Branch newBranch)
        {
            

            branch_trace.Add(new Branch(newBranch));

            foreach (Branch item in FatherBranch)
            {
                branch_trace.Add(new Branch(item));
            }

            foreach (Branch brnch in branch_trace)
            {
                
                if (brnch.BrTypeStartTime)
                {
                    BrnchIntern_i[brnch.BrIntern] = true;
                    BrnchHistory_Sidth[brnch.BrIntern][brnch.BrDisc][brnch.BrTime][brnch.BrHospital] = true;
                }
                if (brnch.BrTypePrecedence)
                {
                    BrnchHistory_yidDh[brnch.BrIntern][brnch.BrPrDisc][brnch.BrDisc][brnch.BrHospital] = true;
                    BrnchIntern_i[brnch.BrIntern] = true;
                }
            }
        }



        public void node_procedure(ArrayList FatherColumn, ArrayList AllBranch, string insName, int procedureType)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            NodeCG = new ColumnGenration(data, FatherColumn, AllBranch, insName, procedureType);
            sw.Stop();
            ElappsedTime = sw.ElapsedMilliseconds / 1000;
            
            if (NodeCG.RMP.is_mip)
            {
                is_mip = true;
            }
            setBounds(insName);
        }
        public void setBounds(string insName)
        {
            Upperbound = NodeCG.BestSolution;
            
            if (is_mip)
            {
                if (lowerBound < NodeCG.BestSolution)
                {
                    lowerBound = NodeCG.BestSolution;
                }
                
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
            else
            {
                LowerBound.HungrianBasedLowerbound hLB = new LowerBound.HungrianBasedLowerbound(data, NodeCG.RMP.DataColumn, "HungLB" + insName);
                if (hLB.LBResult.Obj > lowerBound)
                {
                    lowerBound = hLB.LBResult.Obj;
                    hLB.LBResult.WriteSolution(data.allPath.OutPutGr, "LowerBound" + insName);
                }
                LowerBound.GreedyLowerBound gLB = new LowerBound.GreedyLowerBound(data, NodeCG.RMP.DataColumn, "GreedyLB" + insName);
                if (gLB.LBResult.Obj > lowerBound)
                {
                    lowerBound = gLB.LBResult.Obj;
                    gLB.LBResult.WriteSolution(data.allPath.OutPutGr, "LowerBound" + insName);
                }
            }
            saveInfo();
            
        }
        public void saveInfo()
        {
            // save the columns

            relatedColumn = new ArrayList();
            foreach (ColumnInternBasedDecomposition clmn in NodeCG.RMP.DataColumn)
            {
                relatedColumn.Add(new ColumnInternBasedDecomposition(clmn, data));
            }

            new ArrayInitializer().CreateArray(ref minDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref resDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref emrDem_twh, data.General.TimePriods, data.General.HospitalWard, data.General.Hospitals, 0);

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        minDem_twh[t][w][h] = NodeCG.RMP.minDem_twh[t][w][h];
                        resDem_twh[t][w][h] = NodeCG.RMP.resDem_twh[t][w][h];
                        emrDem_twh[t][w][h] = NodeCG.RMP.emrDem_twh[t][w][h];
                    }
                }
            }

        }


    }
}

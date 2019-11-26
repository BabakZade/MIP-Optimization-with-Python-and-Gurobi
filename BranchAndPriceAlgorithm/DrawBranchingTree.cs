using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using ColumnAndBranchInfo;
namespace BranchAndPriceAlgorithm
{
    public struct Print
    {
        public int pos;
        public string[] info;
        public Print[] dashBranch;

        public Print(int pos, string[] info)
        {
            this.pos = pos;
            this.info = info;
            dashBranch = null;
        }
        public Print(int pos, string[] info, int dashLength)
        {
            this.pos = pos;
            this.info = info;
            dashBranch = new Print[dashLength];
            
            for (int j = 0; j < dashLength; j++)
            {
                string br = "/";
                for (int i = 0; i <= j-1; i++)
                {
                    br += "  ";
                }
                br += "\\";
                dashBranch[j] = new Print(pos,new string[] { br});
            }
        }
    }
    public class DrawBranchingTree
    {
        public int treeLevel;
        public int maxWidth;
        public bool[] branchStatus;
        int maxLevel = 3;
        int minwidth = 128;
        public StreamWriter branchingTree;
        public ArrayList sortedBranches;
        public ArrayList printingbuffer;
        public DrawBranchingTree(ArrayList allBranches, string path, string insName)
        {
            
            initial(allBranches, path, insName);
        }

        public void initial(ArrayList allBranches, string path, string insName)
        {
            long maxID = 0;
            maxLevel = 6;
            minwidth = 128;
            branchStatus = new bool[allBranches.Count] ;
            int counter = -1;
            sortedBranches = new ArrayList();
            foreach (Branch item in allBranches)
            {
                int pos = 0;
                foreach (Branch srBr in sortedBranches)
                {
                    if (srBr.BrID < item.BrID)
                    {
                        pos++;
                    }
                    else
                    {
                        break;
                    }
                }
                sortedBranches.Insert(pos, item);
                counter++;
                if (item.BrID > maxID)
                {
                    maxID = item.BrID;
                }
                branchStatus[counter] = false;
            }

            treeLevel = (int)Math.Ceiling(Math.Log(maxID, 2));
            if (treeLevel > maxLevel)
            {
                treeLevel = maxLevel;
            }
            maxWidth = (int)Math.Pow(2, treeLevel + 3);
            if (maxWidth < minwidth)
            {
                maxWidth = minwidth;
            }
            branchingTree = new StreamWriter(path + "BranchingHistory" + insName + ".txt");
            printTree();
            branchingTree.Close();
            
        }

        public void printTree()
        {
            
            
            int length = 1;
            int level = 1;
            int[] listNumber= new int[] {-1,1,-1, -1, 2,-1 };
            int finish = 2;
            
            int counter = 0;
            printingbuffer = new ArrayList();
            int newWidth = maxWidth / 2;
            printingbuffer.Add(new Print(newWidth, new string[] { "id Brtype","val", "i-h/Val", "dd-d/d-t/w-h-t", "if MIP" }, (int)((double)maxWidth / (int)Math.Pow(2, level + 1))));
            printbuffer();           
            while (true)
            {
                counter++;
                if (counter == sortedBranches.Count)
                {
                    break;
                }

                if (level < maxLevel)
                {
                    if (((Branch)sortedBranches[counter]).BrID > finish)
                    {
                        printbuffer();
                        level++;
                        listNumber = new int[(int)Math.Pow(2, level + 1) + (int)Math.Pow(2, level)];
                        for (int x = 0; x < listNumber.Length; x++)
                        {
                            if (x == 0 || x == listNumber.Length - 1)
                            {
                                listNumber[x] = -1;
                                continue;
                            }

                            finish++;
                            listNumber[x] = finish;

                            x++;
                            listNumber[x] = -1;

                            if (x < listNumber.Length - 2)
                            {
                                x++;
                                listNumber[x] = -1;
                            }



                        }
                        counter--;
                        branchingTree.WriteLine();
                        branchingTree.WriteLine();
                        continue;

                    }
                    int powerLev = (int)Math.Pow(2, level + 1);
                    int widthofposition = (int)((double)maxWidth / (int)Math.Pow(2, level + 1));
                    int position = 0;
                    for (int j = 0; j < listNumber.Length; j++)
                    {
                        int place = position * widthofposition;
                        if (listNumber[j] == ((Branch)sortedBranches[counter]).BrID)
                        {
                            string info1 = "";
                            string info2 = "";
                            string info3 = "";
                            string info4 = "";
                            string info5 = "";
                            if (((Branch)sortedBranches[counter]).BrMIP)
                            {
                                info5 += "MIP";
                            }
                            else
                            {
                                info5 += " ";
                            }
                            if (((Branch)sortedBranches[counter]).BrTypePrecedence)
                            {
                                info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Pr";

                                info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                                info3 = ((Branch)sortedBranches[counter]).BrIntern + "-"
                                             + ((Branch)sortedBranches[counter]).BrHospital;

                                info4 = ((Branch)sortedBranches[counter]).BrPrDisc + "-"
                                             + ((Branch)sortedBranches[counter]).BrDisc;
                                info5 = "";
                                if (((Branch)sortedBranches[counter]).BrMIP)
                                {
                                    info5 += "MIP";
                                }
                                else
                                {
                                    info5 += " ";
                                }
                            }

                            if (((Branch)sortedBranches[counter]).BrTypeStartTime)
                            {
                                info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " St";

                                info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                                info3 = ((Branch)sortedBranches[counter]).BrIntern + "-"
                                             + ((Branch)sortedBranches[counter]).BrHospital;

                                info4 = ((Branch)sortedBranches[counter]).BrDisc + "-"
                                             + ((Branch)sortedBranches[counter]).BrTime;
                                info5 = "";
                                if (((Branch)sortedBranches[counter]).BrMIP)
                                {
                                    info5 += "MIP";
                                }
                                else
                                {
                                    info5 += " ";
                                }
                            }

                            if (((Branch)sortedBranches[counter]).BrTypeMinDemand)
                            {
                                info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Mi";

                                info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                                if (((Branch)sortedBranches[counter]).branch_status)
                                {
                                    info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                                }
                                else
                                {
                                    info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                                }


                                info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                             + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                             + ((Branch)sortedBranches[counter]).BrTime;
                                info5 = "";
                                if (((Branch)sortedBranches[counter]).BrMIP)
                                {
                                    info5 += "MIP";
                                }
                                else
                                {

                                }
                            }

                            if (((Branch)sortedBranches[counter]).BrTypeResDemand)
                            {
                                info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Re";

                                info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                                if (((Branch)sortedBranches[counter]).branch_status)
                                {
                                    info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                                }
                                else
                                {
                                    info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                                }

                                info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                             + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                             + ((Branch)sortedBranches[counter]).BrTime;
                                info5 = "";
                                if (((Branch)sortedBranches[counter]).BrMIP)
                                {
                                    info5 += "MIP";
                                }
                                else
                                {

                                }
                            }

                            if (((Branch)sortedBranches[counter]).BrTypeEmrDemand)
                            {
                                info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Em";

                                info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                                if (((Branch)sortedBranches[counter]).branch_status)
                                {
                                    info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                                }
                                else
                                {
                                    info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                                }


                                info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                             + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                             + ((Branch)sortedBranches[counter]).BrTime;
                                info5 = "";
                                if (((Branch)sortedBranches[counter]).BrMIP)
                                {
                                    info5 += "MIP";
                                }
                                else
                                {

                                }
                            }

                            printingbuffer.Add(new Print(place, new string[] { info1, info2, info3, info4, info5 }, (int)((double)maxWidth / (int)Math.Pow(2, level + 2 /*for the next level*/))));

                            break;
                        }
                        if (listNumber[j] == -1)
                        {
                            position++;
                        }


                    }
                }
                else
                {
                    if (printingbuffer.Count > 0)
                    {
                        printbuffer();
                    }

                    string info1 = "";
                    string info2 = "";
                    string info3 = "";
                    string info4 = "";
                    string info5 = "";
                    if (((Branch)sortedBranches[counter]).BrMIP)
                    {
                        info5 += "MIP";
                    }
                    else
                    {
                        info5 += " ";
                    }
                    if (((Branch)sortedBranches[counter]).BrTypePrecedence)
                    {
                        info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Pr";

                        info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                        info3 = ((Branch)sortedBranches[counter]).BrIntern + "-"
                                     + ((Branch)sortedBranches[counter]).BrHospital;

                        info4 = ((Branch)sortedBranches[counter]).BrPrDisc + "-"
                                     + ((Branch)sortedBranches[counter]).BrDisc;
                        info5 = "";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info5 += "MIP";
                        }
                        else
                        {
                            info5 += " ";
                        }
                    }

                    if (((Branch)sortedBranches[counter]).BrTypeStartTime)
                    {
                        info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " St";

                        info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                        info3 = ((Branch)sortedBranches[counter]).BrIntern + "-"
                                     + ((Branch)sortedBranches[counter]).BrHospital;

                        info4 = ((Branch)sortedBranches[counter]).BrDisc + "-"
                                     + ((Branch)sortedBranches[counter]).BrTime;
                        info5 = "";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info5 += "MIP";
                        }
                        else
                        {
                            info5 += " ";
                        }
                    }

                    if (((Branch)sortedBranches[counter]).BrTypeMinDemand)
                    {
                        info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Mi";

                        info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                        if (((Branch)sortedBranches[counter]).branch_status)
                        {
                            info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                        }
                        else
                        {
                            info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                        }


                        info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                     + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                     + ((Branch)sortedBranches[counter]).BrTime;
                        info5 = "";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info5 += "MIP";
                        }
                        else
                        {

                        }
                    }

                    if (((Branch)sortedBranches[counter]).BrTypeResDemand)
                    {
                        info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Re";

                        info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                        if (((Branch)sortedBranches[counter]).branch_status)
                        {
                            info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                        }
                        else
                        {
                            info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                        }

                        info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                     + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                     + ((Branch)sortedBranches[counter]).BrTime;
                        info5 = "";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info5 += "MIP";
                        }
                        else
                        {

                        }
                    }

                    if (((Branch)sortedBranches[counter]).BrTypeEmrDemand)
                    {
                        info1 = ((Branch)sortedBranches[counter]).BrID % 100 + " Em";
                        
                        info2 = ((Branch)sortedBranches[counter]).BrObj.ToString();
                        if (((Branch)sortedBranches[counter]).branch_status)
                        {
                            info3 += ">" + Math.Ceiling(((Branch)sortedBranches[counter]).BrDemVal);
                        }
                        else
                        {
                            info3 += "<" + Math.Floor(((Branch)sortedBranches[counter]).BrDemVal);
                        }


                        info4 = ((Branch)sortedBranches[counter]).BrWard + "-"
                                     + ((Branch)sortedBranches[counter]).BrHospital + "-"
                                     + ((Branch)sortedBranches[counter]).BrTime;
                        info5 = "";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info5 += "MIP";
                        }
                        else
                        {                            

                        }
                    }
                    string line = info1 + " " + info2 + " " + info3 + " " + info4 + " " + info5;
                    printingbuffer.Add(new Print(4+line.Length/2, new string[] { info1 +" "+ info2 + " " + info3 + " " + info4 + " " + info5 }, 0));
                    printbuffer();
                }

            }

            

            
        }

        public void printbuffer()
        {
            for (int i = 0; i < ((Print)printingbuffer[0]).info.Length; i++)
            {
                int pos = 0;
                string line = "";
                foreach (Print item in printingbuffer)
                {
                    while (pos < maxWidth)
                    {
                        if (pos == item.pos - item.info[i].Length / 2)
                        {
                            line += item.info[i];
                            pos += item.info[i].Length;
                            break;
                        }
                        else
                        {
                            pos++;
                            line += " ";
                        }
                    }
                }
                branchingTree.WriteLine(line);
            }

            for (int i = 0; i < ((Print)printingbuffer[0]).dashBranch.Length; i++)
            {
                int pos = 0;
                string line = "";
                foreach (Print item in printingbuffer)
                {
                    while (pos < maxWidth)
                    {
                        if (pos == item.dashBranch[i].pos - item.dashBranch[i].info[0].Length / 2)
                        {
                            line += item.dashBranch[i].info[0];
                            pos += item.dashBranch[i].info[0].Length;
                            break;
                        }
                        else
                        {
                            pos++;
                            line += " ";
                        }
                    }
                }
                branchingTree.WriteLine(line);
            }


            printingbuffer = new ArrayList();
        }
    }
}

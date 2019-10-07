using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
namespace BranchAndPriceAlgorithm
{
    public struct Print
    {
        public int pos;
        public string[] info;
        
        public Print(int pos, string[] info)
        {
            this.pos = pos;
            this.info = info;
        }
    }
    public class DrawBranchingTree
    {
        public int treeLevel;
        public int maxWidth;
        public bool[] branchStatus;
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
            
            maxWidth = (int)Math.Pow(2, treeLevel + 2);
            if (maxWidth < 64)
            {
                maxWidth = 64;
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
            printingbuffer.Add(new Print(newWidth, new string[] { "id,MIP","val", "i,dd,d,", "h,F/T" }));
            printbuffer();
            while (true)
            {
                counter++;
                if (counter == sortedBranches.Count)
                {
                    break;
                }               
                
                if (((Branch)sortedBranches[counter]).BrID > finish)
                {
                    printbuffer();
                    level++;
                    listNumber = new int[(int)Math.Pow(2,level+1) + (int)Math.Pow(2, level)];
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
                int powerLev = (int)Math.Pow(2, level+1);
                int widthofposition = (int)((double)maxWidth / (int)Math.Pow(2, level + 1));
                int position = 0;
                for (int j = 0; j < listNumber.Length; j++)
                {                    
                    int place = position * widthofposition;
                    if (listNumber[j] == ((Branch)sortedBranches[counter]).BrID)
                    {
                        string info = (((Branch)sortedBranches[counter]).BrID % 100) + ",";
                        if (((Branch)sortedBranches[counter]).BrMIP)
                        {
                            info += "," + "T";
                        }
                        else
                        {
                            info += "," + "F";
                        }
                        string info1 =    ((Branch)sortedBranches[counter]).BrObj.ToString() ;
                        string info2 = ((Branch)sortedBranches[counter]).BrIntern + ","
                                            + ((Branch)sortedBranches[counter]).BrPrDisc + ","
                                            + ((Branch)sortedBranches[counter]).BrDisc;
                                            
                        string info3 =  ((Branch)sortedBranches[counter]).BrHospital.ToString();
                        if (((Branch)sortedBranches[counter]).branch_status)
                        {
                            info3 += "," + "T";
                        }
                        else
                        {
                            info3 += "," + "F";
                        }
                        printingbuffer.Add(new Print(place, new string[] {info, info1, info2, info3}));
                        
                        break;
                    }
                    if (listNumber[j] == -1)
                    {
                        position++;
                    }
                    

                }

            }

            printbuffer();
        }

        public void printbuffer()
        {
            branchingTree.WriteLine();
            for (int i = 0; i < ((Print)printingbuffer[0]).info.Length; i++)
            {
                int pos = 0;
                string line = "";
                foreach (Print item in printingbuffer)
                {
                    while (true)
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
            
            
            
            printingbuffer = new ArrayList();
        }
    }
}

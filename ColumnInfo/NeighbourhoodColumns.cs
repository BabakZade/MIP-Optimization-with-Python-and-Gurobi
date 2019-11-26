using System;
using System.Collections;
using System.Text;

namespace ColumnAndBranchInfo
{
    // in order to have some idea about the function name pleas read the paper => Umbarkar, A. J., & Sheth, P. D. (2015). CROSSOVER OPERATORS IN GENETIC ALGORITHMS: A REVIEW. ICTACT journal on soft computing, 6(1).
    public class NeighbourhoodColumns
    {

        public ArrayList allColumns;
        public DataLayer.AllData data;


        public void changeSingleHospitalPosition(ColumnInternBasedDecomposition theColumn) 
        {

        }



        public ColumnInternBasedDecomposition createTheColumn(ColumnInternBasedDecomposition theColumn) 
        {
            ColumnInternBasedDecomposition column = new ColumnInternBasedDecomposition(data);
            




            return column;
        }

        public ArrayList findTheCrossoverPointHospitalBased(int totalPoint, ColumnInternBasedDecomposition theColumn)
        {
            ArrayList crossoverPointIndex = new ArrayList();
            int prvH = -1;
            int curH = -1;
            int counter = -1;
            foreach (RosterPosition pos in theColumn.theRoster)
            {
                counter++;
                if (prvH < 0)
                {
                    prvH = pos.theHospital;
                }
                else
                {
                    curH = pos.theHospital;
                    if (prvH != curH)
                    {
                        crossoverPointIndex.Add(counter);
                    }
                    prvH = curH;
                }
            }
            Random random = new Random();
            while (crossoverPointIndex.Count > totalPoint)
            {
                int index = random.Next(0, crossoverPointIndex.Count);
                crossoverPointIndex.RemoveAt(index);
            }


            return crossoverPointIndex;
        }

        /// <summary>
        /// if the new column has not been added in the arraylist it will be added to the arraylist
        /// </summary>
        /// <param name="theColumn">the new column</param>
        public void addColumn(ColumnInternBasedDecomposition theColumn) 
        {
            foreach (ColumnInternBasedDecomposition column in allColumns)
            {
                if (column.Compare(theColumn,data))
                {
                    return;
                }
            }

            allColumns.Add(theColumn);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    public class ColumnInternBasedDecomposition
    {
        public int theIntern;
        public bool[][][] S_tdh;
        public int totalChange;
        public double desire;
        public double xRC;
        public double xVal;
        public double objectivefunction;
        public bool[][][] Y_dDh;
        public ColumnInternBasedDecomposition()
        {
        }
        public ColumnInternBasedDecomposition(DataLayer.AllData data)
        {
            initial(data);
        }

        public void initial(DataLayer.AllData data)
        {
            theIntern = -1;
            new DataLayer.ArrayInitializer().CreateArray(ref S_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals + 1, false);
            new DataLayer.ArrayInitializer().CreateArray(ref Y_dDh, data.General.Disciplines + 1, data.General.Disciplines + 1, data.General.Hospitals + 1, false);
            totalChange = 0;
            desire = 0;
            xRC = 0;
            xVal = 0;
            objectivefunction = 0;
        }

        public ColumnInternBasedDecomposition(ColumnInternBasedDecomposition copyable, DataLayer.AllData data)
        {
            initial(data);
            theIntern = copyable.theIntern;

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals +1; h++)
                    {
                        S_tdh[t][d][h] = copyable.S_tdh[t][d][h];
                    }
                }
            }
            totalChange = copyable.totalChange;
            desire = copyable.desire;
            xRC = copyable.xRC;
            xVal = copyable.xVal;
            objectivefunction = copyable.objectivefunction;
            for (int d = 0; d < data.General.Disciplines + 1; d++)
            {
                for (int D = 0; D < data.General.Disciplines + 1; D++)
                {
                    for (int h = 0; h < data.General.Hospitals + 1; h++)
                    {
                        Y_dDh[d][D][h] = copyable.Y_dDh[d][D][h];
                    }
                }
            }
        }
        
        public double setReducedCost(double[] dual, DataLayer.AllData data)
        {
            double reducedcost = 0;
            desire = (int)desire;

            reducedcost += data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi * desire;          


            int Constraint_Counter = 0;

            // Constraint 2
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (i == theIntern)
                {
                    reducedcost -= dual[Constraint_Counter];
                }
                Constraint_Counter++;
            }

            // Constraint 3 
            for (int r = 0; r < data.General.Region; r++)
            {
                for (int t = 0; t < data.General.TimePriods; t++)
                {

                    if (data.Intern[theIntern].TransferredTo_r[r])
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            for (int h = 0; h < data.General.Hospitals; h++)
                            {
                                if (data.Hospital[h].InToRegion_r[r])
                                {
                                    int theP = data.Intern[theIntern].ProgramID;
                                    for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                    {
                                        if (S_tdh[tt][d][h])
                                        {
                                            reducedcost -= dual[Constraint_Counter];
                                        }
                                    }
                                }

                            }
                        }
                    }
                    Constraint_Counter++;
                }
            }

            // Constraint 4 

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[theIntern].isProspective)
                                {
                                    int theP = data.Intern[theIntern].ProgramID;
                                    for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                    {
                                        if (S_tdh[tt][d][h])
                                        {
                                            reducedcost -= dual[Constraint_Counter];
                                        }
                                    }
                                }


                            
                        }
                        Constraint_Counter++;

                    }
                }
            }

            // Constraint 5

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        
                            for (int d = 0; d < data.General.Disciplines; d++)
                            {
                                if (data.Hospital[h].Hospital_dw[d][w])
                                {
                                    int theP = data.Intern[theIntern].ProgramID;
                                    for (int tt = Math.Max(0, t - data.Discipline[d].Duration_p[theP] + 1); tt <= t; tt++)
                                    {
                                        if (S_tdh[tt][d][h])
                                        {
                                            reducedcost -= dual[Constraint_Counter];
                                        }
                                    }
                                }


                            }
                        
                        Constraint_Counter++;

                    }
                }
            }

            // Constraint 6   
            for (int i = 0; i < data.General.Interns; i++)
                {
                    if (theIntern == i)
                    {
                        reducedcost += dual[Constraint_Counter] * desire;                       
                    }
                    Constraint_Counter++;
                }
            
            return reducedcost;
        }

        public void WriteXML(string Path)
        {
            Path = Path + "Info.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ColumnInternBasedDecomposition));
            System.IO.FileStream file = System.IO.File.Create(Path);
            writer.Serialize(file, this);
            file.Close();
        }

        public ColumnInternBasedDecomposition ReadXML(string Path)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ColumnInternBasedDecomposition));

            System.IO.StreamReader file = new System.IO.StreamReader(Path);
            ColumnInternBasedDecomposition tmp = (ColumnInternBasedDecomposition)reader.Deserialize(file);
            file.Close();
            return tmp;
        }

        /// <summary>
        /// returns true if the comparable column is equal to this column
        /// </summary>
        /// <param name="compareableColumn"> the column that you want to compare with this column</param>
        /// <returns>true if they are same</returns>
        public bool Compare(ColumnInternBasedDecomposition compareableColumn, DataLayer.AllData data)
        {
            bool result = true;
            if (theIntern != compareableColumn.theIntern)
            {
                result = false;
            }
            if (desire != compareableColumn.desire)
            {
                result = false;
            }
            if (totalChange != compareableColumn.totalChange)
            {
                result = false;
            }
            for (int t = 0; t < data.General.TimePriods && result ; t++)
            {
                for (int d = 0; d < data.General.Disciplines && result; d++)
                {
                    for (int h = 0; h < data.General.Hospitals && result; h++)
                    {
                        if (S_tdh[t][d][h] != compareableColumn.S_tdh[t][d][h])
                        {
                            result = false;
                            
                        }
                    }
                }
            }

            return result;
        }
    }
}

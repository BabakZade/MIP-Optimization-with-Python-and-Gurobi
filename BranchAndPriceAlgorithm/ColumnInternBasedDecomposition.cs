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
        
        public void initial(int timePeriod, int discipline, int hospital)
        {
            theIntern = -1;
            new DataLayer.ArrayInitializer().CreateArray(ref S_tdh, timePeriod, discipline, hospital, false);
            totalChange = 0;
            desire = 0;
            xRC = 0;
            xVal = 0;
            objectivefunction = 0;
        }

        public double setReducedCost(double[] dual, DataLayer.AllData data)
        {
            double reducedcost = 0;

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
            for (int p = 0; p < data.General.TrainingPr; p++)
            {
                for (int i = 0; i < data.General.Interns; i++)
                {
                    int theP = data.Intern[theIntern].ProgramID;
                    if (p == theP && theIntern == i)
                    {
                        reducedcost += dual[Constraint_Counter] * desire;                       
                    }
                    Constraint_Counter++;
                }
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
    }
}

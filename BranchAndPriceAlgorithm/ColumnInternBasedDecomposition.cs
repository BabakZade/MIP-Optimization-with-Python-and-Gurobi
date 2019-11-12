using System;
using System.Collections;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    struct RoosterPosition {
        public int theTime;
        public int theHospital;
        public int theDiscipline;
        public RoosterPosition(RoosterPosition copyable) 
        {
            theTime = copyable.theTime;
            theHospital = copyable.theHospital;
            theDiscipline = copyable.theDiscipline;
        }
    }
    public class ColumnInternBasedDecomposition
    {
        public int theIntern;
        public bool[][][] S_tdh;
        public bool[] status_d;
        public int totalChange;
        public double desire;
        public double xRC;
        public double xVal;
        public double objectivefunction;
        public bool[][][] Y_dDh;
        public ArrayList neighbourColums;
        public ArrayList theRoster;
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
            theRoster = new ArrayList();
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
                    for (int h = 0; h < data.General.Hospitals + 1; h++)
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

            foreach (RoosterPosition position in copyable.theRoster)
            {
                theRoster.Add(new RoosterPosition(position));
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
            for (int t = 0; t < data.General.TimePriods && result; t++)
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

        public void swapPositionInRoster(DataLayer.AllData data) { }

        public void createTwinColumn(DataLayer.AllData data)
        {
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (data.Intern[i].twinIntern[i] && theIntern != i)
                {

                    ColumnInternBasedDecomposition clmn = new ColumnInternBasedDecomposition(this, data);
                    clmn.theIntern = i;


                    // check the oversea for I
                    bool flag = true;
                    for (int t = 0; t < data.General.TimePriods && flag; t++)
                    {
                        for (int d = 0; d < data.General.Disciplines && flag; d++)
                        {
                            if (data.Intern[i].OverSea_dt[d][t])
                            {
                                bool disExist = false;
                                for (int p = 0; p < clmn.theRoster.Count; p++)
                                {
                                    RoosterPosition position = (RoosterPosition)clmn.theRoster[p];
                                    if (position.theDiscipline == d) // we assigned this discipline to the student
                                    {
                                        disExist = true;
                                        // the easiest things to do
                                        if (position.theTime == t)
                                        {
                                            if (position.theHospital == data.General.Hospitals + 1)
                                            {
                                                // do nothing
                                            }
                                            else
                                            {
                                                position.theHospital = data.General.Hospitals + 1;
                                            }
                                        }
                                        //
                                    }
                                }
                            }
                        }

                    }
                }



            }
        }

        public bool isColumnFeasible(DataLayer.AllData data)
        {
            // in-feasibility signs 
            bool infeasibilityK_Assigned = false;
            bool[] infeasibilityK_Assigned_g = new bool[data.General.DisciplineGr];
            bool infeasibilityChangesInHospital = false;
            bool infeasibilityOverseaAbilityAve = false;
            bool infeasibilitySkill = false;
            bool infeasibilityKdiscOneHosp = false;

            bool IsFeasible = true;
           
            bool infeasibilityOverlap = false;
            
           
            


            string Result = "";

            int i = theIntern;
            int totalK = 0;
            int[] totalK_g = new int[data.General.DisciplineGr];
            for (int g = 0; g < data.General.DisciplineGr; g++)
            {
                totalK_g[g] = 0;
                infeasibilityK_Assigned_g[g] = false;
            }
            for (int d = 0; d < data.General.Disciplines; d++)
            {
                bool CheckedDis = false;
                for (int t = 0; t < data.General.TimePriods && !CheckedDis; t++)
                {
                    for (int g = 0; g < data.General.DisciplineGr && !CheckedDis; g++)
                    {

                        if (data.Intern[i].DisciplineList_dg[d][g])
                        {
                            for (int h = 0; h < data.General.Hospitals + 1 && !CheckedDis; h++)
                            {
                                if (S_tdh[t][d][h])
                                {
                                    totalK += data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
                                    totalK_g[g] += data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];

                                    if (data.Intern[i].OverSea_dt[d][t] && h != data.General.Hospitals)
                                    {
                                        infeasibilityOverseaAbilityAve = true;
                                        Result += "The intern " + i + " requested oversea at time " + t + " for discipline " + d + " but (s)he didnot get it \n";
                                        if (data.Intern[i].Abi_dh[d][h])
                                        {
                                            infeasibilityOverseaAbilityAve = true;
                                            Result += "The intern " + i + " does not has ability for hospital " + h + " for discipline " + d + " \n";
                                        }
                                        if (!data.Intern[i].Ave_t[t])
                                        {
                                            infeasibilityOverseaAbilityAve = true;
                                            Result += "The intern " + i + " is not available at time " + t + " \n";
                                        }
                                    }
                                    if (data.Intern[i].OverSea_dt[d][t] && h == data.General.Hospitals)
                                    {
                                        for (int dd = 0; dd < data.General.Disciplines; dd++)
                                        {
                                            if (data.Intern[i].FHRequirment_d[dd])
                                            {
                                                bool thereis = false;
                                                for (int tt = 0; tt < t && !thereis; tt++)
                                                {
                                                    for (int hh = 0; hh < data.General.Hospitals && !thereis; hh++)
                                                    {
                                                        if (S_tdh[tt][dd][hh])
                                                        {
                                                            thereis = true;
                                                        }
                                                    }
                                                }
                                                if (!thereis)
                                                {
                                                    infeasibilitySkill = true;
                                                    Result += "The intern " + i + " must fulfill discipline  " + dd + " before going abroad for discipline " + d + " \n";

                                                }
                                            }

                                        }
                                    }
                                    if (data.Discipline[d].requiresSkill_p[data.Intern[i].ProgramID])
                                    {
                                        for (int dd = 0; dd < data.General.Disciplines; dd++)
                                        {
                                            if (data.Discipline[d].Skill4D_dp[dd][data.Intern[i].ProgramID])
                                            {
                                                bool thereis = false;
                                                for (int tt = 0; tt < t && !thereis; tt++)
                                                {
                                                    for (int hh = 0; hh < data.General.Hospitals + 1 && !thereis; hh++)
                                                    {
                                                        if (S_tdh[tt][dd][hh])
                                                        {
                                                            thereis = true;
                                                        }
                                                    }
                                                }
                                                if (!thereis)
                                                {
                                                    infeasibilitySkill = true;
                                                    Result += "The intern " + i + " must fulfill discipline  " + dd + " before discipline " + d + " \n";
                                                    
                                                }
                                            }

                                        }
                                    }

                                    CheckedDis = true;
                                }
                            }
                        }
                    }


                }
            }

            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    if (data.Intern[i].Fulfilled_dhp[d][h][data.Intern[i].ProgramID])
                    {
                        totalK -= data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
                        for (int g = 0; g < data.General.DisciplineGr; g++)
                        {
                            if (data.Intern[i].DisciplineList_dg[d][g])
                            {
                                totalK_g[g] -= data.Discipline[d].CourseCredit_p[data.Intern[i].ProgramID];
                                break;
                            }
                        }

                        break;
                    }
                }
            }
            if (totalK != data.Intern[i].K_AllDiscipline)
            {
                Result += "The intern " + i + " should fulfill " + data.Intern[i].K_AllDiscipline + " but (s)he did  " + totalK + " \n";
                infeasibilityK_Assigned = true;
            }
            for (int g = 0; g < data.General.DisciplineGr; g++)
            {
                if (totalK_g[g] != data.Intern[i].ShouldattendInGr_g[g])
                {
                    Result += "The intern " + i + " should fulfill " + data.Intern[i].ShouldattendInGr_g[g] + " but (s)he did  " + totalK_g[g] + " in Group " + g + " \n";

                }
            }


            int IndH1 = -1;
            int IndH2 = -1;
            int PrChang_i = 0;
            int totalDis = 0;
            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals + 1; h++)
                    {
                        if (S_tdh[t][d][h])
                        {
                            int totalDiscInthisHosp = 0;
                            for (int dd = 0; dd < data.General.Disciplines; dd++)
                            {
                                for (int tt = 0; tt < data.General.TimePriods; tt++)
                                {
                                    if (S_tdh[t][dd][h])
                                    {
                                        totalDiscInthisHosp++;
                                        break;
                                    }
                                }

                            }
                            if (totalDiscInthisHosp > data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp)
                            {
                                infeasibilityKdiscOneHosp = true;
                                Result += "The intern " + i + " performed " + totalDiscInthisHosp + " in hospital  " + h + " instead of  " + data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp + " \n";

                            }

                            totalDis++;
                            if (IndH1 < 0)
                            {
                                IndH1 = h;
                            }
                            else
                            {
                                IndH2 = h;
                                if (IndH1 != IndH2)
                                {
                                    PrChang_i++;
                                }
                                IndH1 = IndH2;
                                IndH2 = -1;

                            }
                        }
                    }
                }
            }
            if (PrChang_i > data.TrainingPr[data.Intern[i].ProgramID].DiscChangeInOneHosp * totalDis)
            {
                infeasibilityChangesInHospital = true;
            }

            for (int d = 0; d < data.General.Disciplines; d++)
            {
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        if (S_tdh[t][d][h])
                        {
                            for (int tt = t; tt < t + data.Discipline[d].Duration_p[data.Intern[i].ProgramID]; tt++)
                            {
                                for (int dd = 0; dd < data.General.Disciplines; dd++)
                                {
                                    if (d == dd)
                                    {
                                        continue;
                                    }
                                    for (int hh = 0; hh < data.General.Hospitals + 1; hh++)
                                    {
                                        if (S_tdh[tt][dd][hh])
                                        {
                                            infeasibilityOverlap = true;
                                            
                                            Result += "The intern " + i + " has overlap for discipline " + d + " and  " + dd + " \n";

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (Result == "")
            {
                Result += "The solution is feasible";
                IsFeasible = true;
            }
            else
            {
                IsFeasible = false;

            }
           // Console.WriteLine(Result);
            IsFeasible = !infeasibilityChangesInHospital
                && !infeasibilityK_Assigned && !infeasibilityOverseaAbilityAve
                && !infeasibilitySkill  && !infeasibilityOverlap
                && !infeasibilityKdiscOneHosp;

            return IsFeasible;
        }
    }
}

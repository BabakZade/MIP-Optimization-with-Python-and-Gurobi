using System;
using System.Collections;
//using System.Data;
//using System.Data.Linq;
//using System.Linq;

using System.IO;


using System.Data.SqlClient;



using Excel = Microsoft.Office.Interop.Excel;


namespace DataLayer
{
    public class ImportData
    {
        public string connectionStr;
        SqlConnection connection;

        GeneralInfo tmpGeneral;
        TrainingProgramInfo[] tmpTrainingPr;
        HospitalInfo[] tmphospitalInfos;
        DisciplineInfo[] tmpdisciplineInfos;
        InternInfo[] tmpinternInfos;
        RegionInfo[] tmpRegionInfos;
        AlgorithmSettings tmpalgorithmSettings;

        ArrayList msrDisciplineInfo;
        ArrayList msrWardInfo;
        ArrayList msrDiscDurationPrInfo;
        ArrayList msrDiscGroupInfo;
        ArrayList msrDisciplineListInfo;
        ArrayList msrListInfo;
        ArrayList msrProblemSettingInfo;
        ArrayList msrRegionInfo;
        ArrayList msrHospitalInfo;
        ArrayList msrWardDiscHospitalInfo;
        ArrayList msrWardHospitalDemandInfo;
        ArrayList msrStudentInfo;
        ArrayList msrStudentRegionInfo;
        ArrayList msrStudentDiscPrfInfo;
        ArrayList msrStudentHospPrfInfo;
        ArrayList msrRosterInfo;
        ArrayList msrAvailablityInfo;
        ArrayList msrOverseaInfo;
     


        public ImportData(string path, string name)
        {
            initial();

            CreateInstancesRealLife();
            WriteInstance(path,name);
            CreateTheManualSolution(path, name+"Manaul");
        }
        public void initial()
        {
            connectionStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UGentMedicalStudentRostering;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connectionStr);

            setStudentInfo();
            setDisciplineInfo();
            setDisciplineDurationInfo();
            setWardInfo();
            setDiscGroupInfo();
            setDisciplineListInfo();
            setListInfo();
            setProblemSettingInfo();
            setRegionInfo();
            setHospitalInfo();
            setWardDiscHospitalInfo();
            setWardHospitalDemandInfo();
            setStudentDiscPrfInfo();
            setStudentHospPrfInfo();
            setRosterInfo();
            setAvailabilityInfo();
            setOverseaInfo();


        }


        public void setStudentInfo()
        {
            msrStudentInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRStudentInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRStudentInfo student = new MSRStudentInfo();
                    

                    student.IfPrespective = true;
                    student.StudentName = reader.GetString(reader.GetOrdinal("StudentName"));
                    student.IfTransferBru = reader.GetBoolean(reader.GetOrdinal("IfTransferBru"));
                    student.StudentSQLID = reader.GetInt32(reader.GetOrdinal("StudentSQLID"));
                    student.TrainingProgramID = reader.GetInt32(reader.GetOrdinal("TrainingProgramID"));
                    student.WeightChange = reader.GetInt32(reader.GetOrdinal("WeightChange"));
                    student.WeightDiscPrf = reader.GetInt32(reader.GetOrdinal("WeightDiscPrf"));
                    student.WeightHospPrf = reader.GetInt32(reader.GetOrdinal("WeightHospPrf"));
                    student.WeightWait = reader.GetInt32(reader.GetOrdinal("WeightWait"));


                    msrStudentInfo.Add(student);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }
        public void setDisciplineInfo()
        {
            
            msrDisciplineInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRDisciplineInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRDisciplineInfo disc = new MSRDisciplineInfo();

                    disc.CourseCredit = reader.GetInt32(reader.GetOrdinal("CourseCredit"));
                    disc.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    disc.DisciplineName = reader.GetString(reader.GetOrdinal("DisciplineName"));
                    msrDisciplineInfo.Add(disc);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }
        public void setDisciplineDurationInfo()
        {
            msrDiscDurationPrInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRDiscDurationPrInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRDiscDurationPrInfo disc = new MSRDiscDurationPrInfo();

                    disc.DiscDurationPrID = reader.GetInt32(reader.GetOrdinal("DiscDurationPrID"));
                    disc.DiscplineID = reader.GetInt32(reader.GetOrdinal("DiscplineID"));
                    disc.Duration = reader.GetInt32(reader.GetOrdinal("Duration"));
                    msrDiscDurationPrInfo.Add(disc);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }
        public void setWardInfo()
        {
            msrWardInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRWardInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRWardInfo tmp = new MSRWardInfo();

                    tmp.WardID = reader.GetInt32(reader.GetOrdinal("WardID"));
                    tmp.WardName = reader.GetString(reader.GetOrdinal("WardName"));
                   
                    msrWardInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }
        public void setDiscGroupInfo()
        {
            msrDiscGroupInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRDiscGroupInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRDiscGroupInfo tmp = new MSRDiscGroupInfo();

                    tmp.DiscGroupID = reader.GetInt32(reader.GetOrdinal("DiscGroupID"));
                    tmp.DiscGroupName = reader.GetString(reader.GetOrdinal("DiscGroupName"));         

                    msrDiscGroupInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }
        public void setDisciplineListInfo()
        {
            msrDisciplineListInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRDisciplineListInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRDisciplineListInfo tmp = new MSRDisciplineListInfo();

                    tmp.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    tmp.DiscListID = reader.GetInt32(reader.GetOrdinal("DiscListID"));
                    tmp.ListID = reader.GetInt32(reader.GetOrdinal("ListID"));
                    


                    msrDisciplineListInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setListInfo()
        {
            msrListInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRListInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRListInfo tmp = new MSRListInfo();

                    tmp.ListID = reader.GetInt32(reader.GetOrdinal("ListID"));
                    tmp.DiscGroupID = reader.GetInt32(reader.GetOrdinal("DiscGroupID"));
                    //tmp.DiscListID = reader.GetInt32(reader.GetOrdinal("DiscListID"));
                    tmp.InternID = reader.GetInt32(reader.GetOrdinal("InternID"));
                    tmp.Kperform = reader.GetInt32(reader.GetOrdinal("Kperform"));
                    tmp.ListName = reader.GetString(reader.GetOrdinal("ListName"));
              


                    msrListInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setProblemSettingInfo()
        {
            msrProblemSettingInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRProblemSettingInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRProblemSettingInfo tmp = new MSRProblemSettingInfo();

                    tmp.ProblemSettingID = reader.GetInt32(reader.GetOrdinal("ProblemSettingID"));
                    tmp.TimeSlotName = reader.GetString(reader.GetOrdinal("TimeSlotName"));
                    tmp.TimeSlotPerMonth = reader.GetInt32(reader.GetOrdinal("TimeSlotPerMonth"));
                    



                    msrProblemSettingInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setRegionInfo()
        {
            msrRegionInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRRegionInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRRegionInfo tmp = new MSRRegionInfo();

                    tmp.RegionID = reader.GetInt32(reader.GetOrdinal("RegionID"));
                    tmp.RegionName = reader.GetString(reader.GetOrdinal("RegionName"));
                    tmp.AccomodationCap = reader.GetInt32(reader.GetOrdinal("AccomodationCap"));



                    msrRegionInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setHospitalInfo()
        {
            msrHospitalInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRHospitalInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRHospitalInfo tmp = new MSRHospitalInfo();

                    tmp.HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID"));
                    tmp.HospitalName = reader.GetString(reader.GetOrdinal("HospitalName"));
                    tmp.HospitalRegionID = reader.GetInt32(reader.GetOrdinal("HospitalRegionID"));
                   
                   

                    msrHospitalInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setWardDiscHospitalInfo()
        {
            msrWardDiscHospitalInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRWardDiscHospitalInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRWardDiscHospitalInfo tmp = new MSRWardDiscHospitalInfo();

                    tmp.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    tmp.DisciplineWardHospitalID = reader.GetInt32(reader.GetOrdinal("DisciplineWardHospitalID"));
                    tmp.HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID"));
                    tmp.HospitalWardID = reader.GetInt32(reader.GetOrdinal("HospitalWardID"));
                    tmp.WardID = reader.GetInt32(reader.GetOrdinal("WardID"));
                   


                    msrWardDiscHospitalInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setAvailabilityInfo()
        {
            msrAvailablityInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRAvailablityInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRAvailablityInfo tmp = new MSRAvailablityInfo();

                    tmp.AvailbilityID = reader.GetInt32(reader.GetOrdinal("AvailbilityID"));
                    tmp.ifAvailable = reader.GetBoolean(reader.GetOrdinal("ifAvailable"));
                    tmp.MonthID = reader.GetInt32(reader.GetOrdinal("MonthID"));
                    
                    tmp.StudentID = reader.GetInt32(reader.GetOrdinal("StudentID")); ;



                    msrAvailablityInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setWardHospitalDemandInfo()
        {
            msrWardHospitalDemandInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRWardHospitalDemandInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRWardHospitalDemandInfo tmp = new MSRWardHospitalDemandInfo();
              
                    tmp.EmrDemHospitalWardPeriod = reader.GetInt32(reader.GetOrdinal("EmrDemHospitalWardPeriod"));
                    tmp.HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID"));
                    tmp.HospitalName = reader.GetString(reader.GetOrdinal("HospitalName"));
                    tmp.HospitalWardID = reader.GetInt32(reader.GetOrdinal("HospitalWardID"));
                    tmp.MaxDemHospitalWardPeriod = reader.GetInt32(reader.GetOrdinal("MaxDemHospitalWardPeriod"));
                    tmp.MaxDemHospitalWardYear = reader.GetInt32(reader.GetOrdinal("MaxDemHospitalWardYear"));
                    if (tmp.MaxDemHospitalWardYear == 0)
                    {
                        tmp.MaxDemHospitalWardYear = 12 * tmpGeneral.Interns;
                    }
                    tmp.MinDemHospitalWardPeriod = 0;
                    tmp.MinDemHospitalWardYear = reader.GetInt32(reader.GetOrdinal("MinDemHospitalWardYear"));
                    tmp.ResDemHospitalWardPeriod= reader.GetInt32(reader.GetOrdinal("ResDemHospitalWardPeriod"));
                    tmp.PeriodID = reader.GetInt32(reader.GetOrdinal("PeriodID"));
                    tmp.WardHospitalDemandPeriod = reader.GetInt32(reader.GetOrdinal("WardHospitalDemandPeriod"));
                 


                    msrWardHospitalDemandInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setStudentDiscPrfInfo()
        {
            msrStudentDiscPrfInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRStudentDiscPrfInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRStudentDiscPrfInfo tmp = new MSRStudentDiscPrfInfo();

                    tmp.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    tmp.StudentDisciplineID = reader.GetInt32(reader.GetOrdinal("StudentDisciplineID"));
                    tmp.StudentDiscPrf = reader.GetInt32(reader.GetOrdinal("StudentDiscPrf"));
                    tmp.StudentID = reader.GetInt32(reader.GetOrdinal("StudentID"));
                   

                    msrStudentDiscPrfInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setStudentHospPrfInfo()
        {
            msrStudentHospPrfInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRStudentHospPrfInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRStudentHospPrfInfo tmp = new MSRStudentHospPrfInfo();

                    tmp.HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID"));
                    tmp.StudentHospitalID = reader.GetInt32(reader.GetOrdinal("StudentHospitalID"));
                    tmp.StudentHospPrf = reader.GetInt32(reader.GetOrdinal("StudentHospPrf"));
                    tmp.StudentID = reader.GetInt32(reader.GetOrdinal("StudentID"));
                



                    msrStudentHospPrfInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setOverseaInfo()
        {
            msrOverseaInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSROverseaInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSROverseaInfo tmp = new MSROverseaInfo();

                    tmp.OverseaID = reader.GetInt32(reader.GetOrdinal("OverseaID"));
                    tmp.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    tmp.MonthID = reader.GetInt32(reader.GetOrdinal("MonthID"));
                    tmp.StudentID = reader.GetInt32(reader.GetOrdinal("StudentID"));
                    msrOverseaInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void setRosterInfo()
        {
            msrRosterInfo = new ArrayList();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            string commandText = "SELECT * FROM MSRRosterInfo";
            cmd.CommandText = commandText;

            cmd.Connection = connection;
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            try
            {
                while (reader.Read())
                {
                    MSRRosterInfo tmp = new MSRRosterInfo();

                    tmp.DisciplineID = reader.GetInt32(reader.GetOrdinal("DisciplineID"));
                    tmp.HospitalID = reader.GetInt32(reader.GetOrdinal("HospitalID"));
                    tmp.MonthID = reader.GetInt32(reader.GetOrdinal("MonthID"));
                    tmp.RosterID = reader.GetInt32(reader.GetOrdinal("RosterID"));
                    tmp.StudentID = reader.GetInt32(reader.GetOrdinal("StudentID"));



                    msrRosterInfo.Add(tmp);
                }
            }
            finally
            {
                reader.Close();
            }

            connection.Close();

        }

        public void CreateInstancesRealLife()
        {

            // general Info
            tmpGeneral = new GeneralInfo();
            tmpGeneral.Interns = msrStudentInfo.Count;
            tmpGeneral.Disciplines = msrDisciplineInfo.Count;
           
            tmpGeneral.HospitalWard = msrDisciplineInfo.Count;
            
            tmpGeneral.Hospitals = msrHospitalInfo.Count;
            
            tmpGeneral.Region = msrRegionInfo.Count;
            tmpGeneral.TrainingPr = 1;
            tmpGeneral.DisciplineGr = msrDiscGroupInfo.Count;

            //[groupName pr1 pr2 0 0]
            
            tmpGeneral.TimePriods = (int)((MSRProblemSettingInfo)msrProblemSettingInfo[0]).TimeSlotPerMonth * 12;
            


            //Create training program info
            tmpTrainingPr = new TrainingProgramInfo[tmpGeneral.TrainingPr];
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                tmpTrainingPr[p] = new TrainingProgramInfo(tmpGeneral.Disciplines, tmpGeneral.DisciplineGr);
                tmpTrainingPr[p].Name = (p + 1).ToString() + "_Year";
                tmpTrainingPr[p].AcademicYear = p + 1;
              
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    foreach (MSRListInfo list in msrListInfo)
                    {
                        if (list.DiscGroupID == g+1)
                        {
                            foreach (MSRDisciplineListInfo disc in msrDisciplineListInfo)
                            {
                                if (disc.ListID == list.ListID)
                                {
                                    int d = (int)disc.DisciplineID - 1;
                                    tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] = true;
                                    tmpTrainingPr[p].Prf_d[d] = 0;
                                }
                            }
                            break;
                        }
                    }
                }

                // assign the weight for preference
             
                tmpTrainingPr[p].weight_p = 0;
                // assign the coefficient 
                tmpTrainingPr[p].CoeffObj_SumDesi = 1;
                tmpTrainingPr[p].CoeffObj_MinDesi = 1;
                tmpTrainingPr[p].CoeffObj_ResCap = 1;
                tmpTrainingPr[p].CoeffObj_EmrCap = 1;
                tmpTrainingPr[p].CoeffObj_NotUsedAcc = 1;
                tmpTrainingPr[p].CoeffObj_MINDem = 1;

                tmpTrainingPr[p].DiscChangeInOneHosp = tmpGeneral.Disciplines;
      
            }

            // Create Hospital Info 
            tmphospitalInfos = new HospitalInfo[tmpGeneral.Hospitals];
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                tmphospitalInfos[h] = new HospitalInfo(tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.HospitalWard, tmpGeneral.Region);
                tmphospitalInfos[h].Name =((MSRHospitalInfo)msrHospitalInfo[h]).HospitalName;

                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    foreach (MSRWardHospitalDemandInfo cap in msrWardHospitalDemandInfo)
                    {
                        if (cap.HospitalID == ((MSRHospitalInfo)msrHospitalInfo[h]).HospitalID && cap.HospitalWardID == d+1)
                        {
                            int w = d;
                            tmphospitalInfos[h].Hospital_dw[d][w] = true;
                            tmphospitalInfos[h].HospitalMaxYearly_w[w] = (int)cap.MaxDemHospitalWardYear;
                            tmphospitalInfos[h].HospitalMinYearly_w[w] = 0;
                            int t = (int)cap.PeriodID -1;
                            tmphospitalInfos[h].HospitalMinDem_tw[t][w] = 0;
                            tmphospitalInfos[h].HospitalMaxDem_tw[t][w] = (int)cap.MaxDemHospitalWardPeriod;

                            tmphospitalInfos[h].EmergencyCap_tw[t][w] = 0;
                            tmphospitalInfos[h].ReservedCap_tw[t][w] = 0;
                            
                        }
                    }
                    
                }

                tmphospitalInfos[h].InToRegion_r[(int)((MSRHospitalInfo)msrHospitalInfo[h]).HospitalRegionID-1] = true;
             
            }

            // Create Discipline Info
            tmpdisciplineInfos = new DisciplineInfo[tmpGeneral.Disciplines];
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                tmpdisciplineInfos[d] = new DisciplineInfo(tmpGeneral.Disciplines, tmpGeneral.TrainingPr);
                tmpdisciplineInfos[d].Name = ((MSRDisciplineInfo)msrDisciplineInfo[d]).DisciplineName;

                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tmpdisciplineInfos[d].CourseCredit_p[p] = (int)((MSRDisciplineInfo)msrDisciplineInfo[d]).CourseCredit;
                    foreach (MSRDiscDurationPrInfo dur in msrDiscDurationPrInfo)
                    {
                        if (dur.DiscplineID == d+1)
                        {
                            tmpdisciplineInfos[d].Duration_p[p] = dur.Duration;
                            
                            break;
                        }
                    }
                }
            }

            

            // create Intern info
            tmpinternInfos = new InternInfo[tmpGeneral.Interns];
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tmpinternInfos[i] = new InternInfo(tmpGeneral.Hospitals, tmpGeneral.Disciplines, tmpGeneral.TimePriods, tmpGeneral.DisciplineGr, tmpGeneral.TrainingPr, tmpGeneral.Region);
                tmpinternInfos[i].ProgramID = (int)((MSRStudentInfo)msrStudentInfo[i]).TrainingProgramID;
            }
            setInternsList();
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tmpinternInfos[i].isProspective = true;
                // hand out group and discipline


                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        tmpinternInfos[i].Abi_dh[d][h] = true;
                    }                    
                }

                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    tmpinternInfos[i].Prf_d[d] = 1;
                    foreach (MSRStudentDiscPrfInfo prf in msrStudentDiscPrfInfo)
                    {
                        if (prf.DisciplineID == d +1 && prf.StudentID == i+1)
                        {
                            tmpinternInfos[i].Prf_d[d] = (int)prf.StudentDiscPrf;
                            break;
                        }
                    }
                }
                for (int d = 0; d < tmpGeneral.Hospitals; d++)
                {
                    tmpinternInfos[i].Prf_h[d] = 1;
                    foreach (MSRStudentHospPrfInfo prf in msrStudentHospPrfInfo)
                    {
                        if (prf.HospitalID == d + 1 && prf.StudentID == i + 1)
                        {
                            tmpinternInfos[i].Prf_h[d] = (int)prf.StudentHospPrf;
                            break;
                        }
                    }
                }



                // region 
                for (int r = 0; r < tmpGeneral.Region; r++)
                {
                    if ((bool)((MSRStudentInfo)msrStudentInfo[i]).IfTransferBru)
                    {
                        tmpinternInfos[i].TransferredTo_r[r] = true;
                    }
                }

                foreach (MSRAvailablityInfo ava in msrAvailablityInfo)
                {
                    if (ava.StudentID == i+1)
                    {
                        tmpinternInfos[i].Ave_t[(int)ava.MonthID - 1] = true;
                    }
                }


                tmpinternInfos[i].wieght_ch = -(int)((MSRStudentInfo)msrStudentInfo[i]).WeightChange;
                tmpinternInfos[i].wieght_d = (int)((MSRStudentInfo)msrStudentInfo[i]).WeightDiscPrf;
                tmpinternInfos[i].wieght_h = (int)((MSRStudentInfo)msrStudentInfo[i]).WeightHospPrf;
                tmpinternInfos[i].wieght_w = -(int)((MSRStudentInfo)msrStudentInfo[i]).WeightWait;
                
            }

            // create Region info
            tmpRegionInfos = new RegionInfo[tmpGeneral.Region];
            for (int r = 0; r < tmpGeneral.Region; r++)
            {
                tmpRegionInfos[r] = new RegionInfo(tmpGeneral.TimePriods);
                tmpRegionInfos[r].Name = ((MSRRegionInfo)msrRegionInfo[r]).RegionName;
                tmpRegionInfos[r].SQLID = r+1;
                
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    tmpRegionInfos[r].AvaAcc_t[t] = (int)((MSRRegionInfo)msrRegionInfo[r]).AccomodationCap;
                }
            }

            //Algorithm Settings
            tmpalgorithmSettings = new AlgorithmSettings();
            tmpalgorithmSettings.BPTime = 7200;
            tmpalgorithmSettings.MasterTime = 100;
            tmpalgorithmSettings.MIPTime = 7200;
            tmpalgorithmSettings.RCepsi = 0.000001;
            tmpalgorithmSettings.RHSepsi = 0.000001;
            tmpalgorithmSettings.SubTime = 600;
            tmpalgorithmSettings.NodeTime = 3600;
            tmpalgorithmSettings.BigM = 400000;
            tmpalgorithmSettings.MotivationBM = tmpalgorithmSettings.BigM / 100;
            tmpalgorithmSettings.bucketBasedImpPercentage = 0.25;
            tmpalgorithmSettings.internBasedImpPercentage = 0.25;



        }

        public void CreateTheManualSolution(string location, string name)
        {
            AllData allData = new AllData();
            allData.AlgSettings = tmpalgorithmSettings;
            allData.Discipline = tmpdisciplineInfos;
            allData.General = tmpGeneral;
            allData.Hospital = tmphospitalInfos;
            allData.Intern = tmpinternInfos;
            allData.Region = tmpRegionInfos;
            allData.TrainingPr = tmpTrainingPr;
            OptimalSolution manaual = new OptimalSolution(allData);
            foreach (MSRRosterInfo roster in msrRosterInfo)
            {
                if (roster.DisciplineID != null && roster.HospitalID != 1 && roster.MonthID !=null)
                {
                    int i = (int)roster.StudentID - 1;
                    int t=(int)roster.MonthID - 1;
                    int d=(int)roster.DisciplineID - 1;
                    int h=(int)roster.HospitalID - 1;
                    manaual.Intern_itdh[i][t][d][h] = true;
                }
                else if(roster.HospitalID == 1)
                {
                    int i = (int)roster.StudentID - 1;
                    int t = (int)roster.MonthID - 1;
                    int d = (int)roster.DisciplineID - 1;
                    int h = tmpGeneral.Hospitals;
                    manaual.Intern_itdh[i][t][d][h] = true;
                }
            }

            manaual.WriteSolution(location, name);
        }

        public void setInternsList()
        {
            int[] Kperform = new int[] { 7, 1, 1, 1, 1 };
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tmpinternInfos[i].K_AllDiscipline = 11;
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                    {
                        if (tmpTrainingPr[tmpinternInfos[i].ProgramID].InvolvedDiscipline_gd[g][d])
                        {
                            if (true)
                            {
                                tmpinternInfos[i].DisciplineList_dg[d][g] = true;
                                tmpinternInfos[i].ShouldattendInGr_g[g] = Kperform[g];
                                break;
                            }
                            else
                            {
                                tmpinternInfos[i].DisciplineList_dg[d][1] = true;
                                tmpinternInfos[i].ShouldattendInGr_g[1] = Kperform[1];
                                break;
                            }
                            
                        }
                    }

                }
            }
            foreach (MSROverseaInfo over in msrOverseaInfo)
            {
                int student = (int)over.StudentID - 1;
                int disc = (int)over.DisciplineID - 1;
                int month = (int)over.MonthID - 1 ;
                tmpinternInfos[student].OverSea_dt[disc][month] = true;
            }
        }

        public void WriteInstance(string location, string name)
        {
            TextWriter tw = new StreamWriter(location + name + ".txt");
            string strline;
            strline = "// Interns Disciplines Hospitals TimePriods TrPrograms HospitalWards DisciplineGr Region";

            //write the general Data
            tw.WriteLine(strline);
            tw.WriteLine(tmpGeneral.Interns + " " + tmpGeneral.Disciplines
                        + " " + tmpGeneral.Hospitals + " " + tmpGeneral.TimePriods
                        + " " + tmpGeneral.TrainingPr + " " + tmpGeneral.HospitalWard
                        + " " + tmpGeneral.DisciplineGr + " " + tmpGeneral.Region);
            tw.WriteLine();
            //write training program info
            strline = "// PrName Year WeightPrf Obj_SumDesi Obj_MinDesi Obj_ResCap Obj_EmrCap Obj_NUsedAcc AllowedChange [p]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                tw.WriteLine(tmpTrainingPr[p].Name + " " + tmpTrainingPr[p].AcademicYear
                             + " " + tmpTrainingPr[p].weight_p + " " + tmpTrainingPr[p].CoeffObj_SumDesi
                             + " " + tmpTrainingPr[p].CoeffObj_MinDesi + " " + tmpTrainingPr[p].CoeffObj_ResCap
                             + " " + tmpTrainingPr[p].CoeffObj_EmrCap + " " + tmpTrainingPr[p].CoeffObj_NotUsedAcc + " " + tmpTrainingPr[p].DiscChangeInOneHosp + " " + tmpTrainingPr[p].CoeffObj_MINDem);
            }

            tw.WriteLine();

            strline = "// Preference for discipline [p][d]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    tw.Write(tmpTrainingPr[p].Prf_d[d] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Involved discipline  [p][g][d]";
            tw.WriteLine(strline);
            for (int p = 0; p < tmpGeneral.TrainingPr; p++)
            {
                strline = "// Program  " + p;
                tw.WriteLine(strline);
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        tw.Write((tmpTrainingPr[p].InvolvedDiscipline_gd[g][d] == true ? 1 : 0).ToString() + " ");
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            // write Hospital Info 
            strline = "// Involved discipline in ward in hospital [h]  =>[w][d]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                tw.WriteLine("Hospital " + tmphospitalInfos[h].Name);
                for (int w = 0; w < tmpGeneral.HospitalWard; w++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmphospitalInfos[h].Hospital_dw[d][w])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }


            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Max Demand for ward in each time period [t][w] in Hospital " + tmphospitalInfos[h].Name;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMaxDem_tw[t][d] + " ");
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Min Demand for ward in each time period [t][w] in Hospital " + tmphospitalInfos[h].Name;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].HospitalMinDem_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].HospitalMinDem_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Reserved Demand for ward in each time period [t][w] in Hospital " + tmphospitalInfos[h].Name;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].ReservedCap_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].ReservedCap_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                strline = "// Emergency Demand for ward in each time period [t][w] in Hospital " + tmphospitalInfos[h].Name;
                tw.WriteLine(strline);
                for (int t = 0; t < tmpGeneral.TimePriods; t++)
                {
                    for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                    {
                        if (tmphospitalInfos[h].EmergencyCap_tw[t][d] > 0)
                        {
                            tw.Write(tmphospitalInfos[h].EmergencyCap_tw[t][d] + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }
            }

            tw.WriteLine();
            strline = "// Yearly Max demand [h][w]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                {
                    if (tmphospitalInfos[h].HospitalMaxYearly_w[d] > 0)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMaxYearly_w[d] + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();

            }

            tw.WriteLine();
            strline = "// Yearly Min demand [h][w]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int d = 0; d < tmpGeneral.HospitalWard; d++)
                {
                    if (tmphospitalInfos[h].HospitalMinYearly_w[d] > 0)
                    {
                        tw.Write(tmphospitalInfos[h].HospitalMinYearly_w[d] + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();

            }

            tw.WriteLine();
            strline = "// located in regions [h][r]";
            tw.WriteLine(strline);
            for (int h = 0; h < tmpGeneral.Hospitals; h++)
            {
                for (int t = 0; t < tmpGeneral.Region; t++)
                {
                    if (tmphospitalInfos[h].InToRegion_r[t])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }

                   
                }
                tw.WriteLine();
            }

            tw.WriteLine();



            // write Discipline Info
            strline = "// Discipline duration different in Program [d][p]";
            tw.WriteLine(strline);
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tw.Write(tmpdisciplineInfos[d].Duration_p[p] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();
            strline = "// Course credit different in Program [d][p]";
            tw.WriteLine(strline);
            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                {
                    tw.Write(tmpdisciplineInfos[d].CourseCredit_p[p] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();


            for (int d = 0; d < tmpGeneral.Disciplines; d++)
            {
                strline = "// required skill[d'][p] for discipline " + d;
                tw.WriteLine(strline);
                for (int dd = 0; dd < tmpGeneral.Disciplines; dd++)
                {
                    for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                    {
                        if (tmpdisciplineInfos[d].Skill4D_dp[dd][p])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

                tw.WriteLine();
            }
            tw.WriteLine();


            //write Intern info		
            strline = "// InternName Program isProspective W_disc W_hosp W_change W_wait  [i][w]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                tw.WriteLine(i + " " + tmpinternInfos[i].ProgramID + " " + tmpinternInfos[i].isProspective + " " + tmpinternInfos[i].wieght_d + " " + tmpinternInfos[i].wieght_h + " " + tmpinternInfos[i].wieght_ch + " " + tmpinternInfos[i].wieght_w);

            }

            tw.WriteLine();

            strline = "// Discipline list";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                strline = "// Intern " + i + ": [g][d]";
                tw.WriteLine(strline);
                for (int g = 0; g < tmpGeneral.DisciplineGr; g++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmpinternInfos[i].DisciplineList_dg[d][g])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            strline = "// total discipline should be fulfilled from each group [i][g]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.DisciplineGr; d++)
                {
                    tw.Write(tmpinternInfos[i].ShouldattendInGr_g[d] + " ");
                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Oversea request i - d - t - RequiredDiscipline";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                    {
                        if (tmpinternInfos[i].OverSea_dt[d][t])
                        {
                            tw.WriteLine(i + " " + d + " " + t);

                        }

                    }
                }

            }
            tw.WriteLine("---");

            strline = "// Oversea Discipline requirement  [i oversea] [d1 d2 d3]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                bool OVERSEA = false;
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int t = 0; t < tmpGeneral.TimePriods; t++)
                    {
                        if (tmpinternInfos[i].OverSea_dt[d][t])
                        {
                            OVERSEA = true;
                        }

                    }
                }
                for (int d = 0; d < tmpGeneral.Disciplines && OVERSEA; d++)
                {
                    if (tmpinternInfos[i].FHRequirment_d[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                if (OVERSEA)
                {
                    tw.WriteLine();
                }
            }
            tw.WriteLine();

            strline = "// Transfered to region [i][r]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Region; d++)
                {
                    if (tmpinternInfos[i].TransferredTo_r[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }
                }
                tw.WriteLine();
            }
            tw.WriteLine();


            strline = "// Ability";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                strline = "// Ability " + i + " [d][h]";
                tw.WriteLine(strline);
                for (int h = 0; h < tmpGeneral.Hospitals; h++)
                {
                    for (int d = 0; d < tmpGeneral.Disciplines; d++)
                    {
                        if (tmpinternInfos[i].Abi_dh[d][h])
                        {
                            tw.Write(1 + " ");
                        }
                        else
                        {
                            tw.Write(0 + " ");
                        }
                    }
                    tw.WriteLine();
                }

            }
            tw.WriteLine();

            strline = "// Fulfilled disciplines i-d-h-p";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    for (int h = 0; h < tmpGeneral.Hospitals; h++)
                    {
                        for (int p = 0; p < tmpGeneral.TrainingPr; p++)
                        {
                            if (tmpinternInfos[i].Fulfilled_dhp[d][h][p])
                            {
                                tw.WriteLine(i + " " + d + " " + h + " " + p);
                            }
                        }

                    }
                }
            }
            tw.WriteLine("---");

            strline = "// Preference for discipline [i][d]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Disciplines; d++)
                {
                    tw.Write(tmpinternInfos[i].Prf_d[d] + " ");


                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Preference for hospital [i][h]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.Hospitals; d++)
                {
                    tw.Write(tmpinternInfos[i].Prf_h[d] + " ");

                }
                tw.WriteLine();
            }
            tw.WriteLine();

            strline = "// Availability [i][t]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Interns; i++)
            {
                for (int d = 0; d < tmpGeneral.TimePriods; d++)
                {
                    if (tmpinternInfos[i].Ave_t[d])
                    {
                        tw.Write(1 + " ");
                    }
                    else
                    {
                        tw.Write(0 + " ");
                    }

                }
                tw.WriteLine();
            }
            tw.WriteLine();

            // write Region info
            strline = "// RegionNme SqlID";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Region; i++)
            {
                tw.WriteLine(tmpRegionInfos[i].Name + " " + tmpRegionInfos[i].SQLID);
            }
            tw.WriteLine();

            strline = "// Region Accommodation [r][t]";
            tw.WriteLine(strline);
            for (int i = 0; i < tmpGeneral.Region; i++)
            {
                for (int d = 0; d < tmpGeneral.TimePriods; d++)
                {
                    tw.Write(tmpRegionInfos[i].AvaAcc_t[d] + " ");


                }
                tw.WriteLine();
            }
            tw.WriteLine();


            // write algoritm setting
            strline = "// Algorithm Settings MIPtime BPtime Nodetime Mastertime Subtime RCepsi RCepsi BigM impBucket% impIntern%";
            tw.WriteLine(strline);
            tw.WriteLine(tmpalgorithmSettings.MIPTime + " " + tmpalgorithmSettings.BPTime + " " + tmpalgorithmSettings.NodeTime + " " + tmpalgorithmSettings.MasterTime + " " + tmpalgorithmSettings.SubTime + " " + tmpalgorithmSettings.RHSepsi + " " + tmpalgorithmSettings.RCepsi + " " + tmpalgorithmSettings.BigM + " " + tmpalgorithmSettings.bucketBasedImpPercentage + " " + tmpalgorithmSettings.internBasedImpPercentage);
            tw.WriteLine();


            // write Instance setting 
            


            tw.Close();
        }
    }
}

using System;
using System.Collections.Generic;
//using System.Data;
//using System.Data.Linq;
//using System.Linq;
using System.Data.SqlClient;

using System.Text;
using System.Threading.Tasks;

using System.IO;
using Excel = Microsoft.Office.Interop.Excel;


namespace DataLayer
{
    public class ImportData
    {
        public string connectionStr;
        SqlConnection connection;
        public ImportData()
        {
            initial();
            ImportTheDisciplines("DiscGroups.xlsx");

        }
        public void initial()
        {
            connectionStr = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=UGentMedicalStudentRostering;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            connection = new SqlConnection(connectionStr);
        }
        public void ImportTheDisciplines(string nameofexcelfile)
        {           

            Excel.Workbook MyBook = null;
            Excel.Application MyApp = null;
            Excel.Worksheet MySheet = null;

            MyApp = new Excel.Application();
            MyApp.Visible = false;
            MyBook = MyApp.Workbooks.Open(Directory.GetCurrentDirectory()+ "//ExcelFiles//" + nameofexcelfile);
            MySheet = (Excel.Worksheet)MyBook.Sheets[1]; // Explicit cast is not required here
                                                         //var lastRow = MySheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell).Row;
                                                         // int theWeek = setWeekDateInfo() + 1;

            Boolean Flag = true;
            // for (int index = 2; index <= lastRow; index++)
            var DiscName = (System.Array)MySheet.get_Range("A" +
                      1.ToString(), "AR" + 1.ToString()).Cells.Value;
            truncateDisciplineRelatedTable();
            for (int j = 2; j < 50; j++)
            {
                if (DiscName.GetValue(1, j).ToString() == null)
                {
                    break;
                }
                string disname = DiscName.GetValue(1, j).ToString();
                MSRDisciplineInfo dis = new MSRDisciplineInfo()
                {
                    CourseCredit = 1,
                    DisciplineName = disname
                };
                int discID = insertDiscipline(dis);
                MSRDiscDurationPrInfo dur = new MSRDiscDurationPrInfo()
                {
                    TrainginProgramID = 0,
                    DiscplineID = discID,
                    Duration = 1,
                };
                insertDisciplineDuration(dur);
            }

            int[] Kperform = new int[] { 7, 1, 1, 1, 1 };
            int groupID = 0;
            int listID = 0;
            for (int index = 2; Flag == true; index++)
            {
                Console.WriteLine("this is row " + index.ToString());
                try
                {
                    var MyValues = (System.Array)MySheet.get_Range("A" +
                       index.ToString(), "AR" + index.ToString()).Cells.Value;
                    if (MyValues.GetValue(1, 1) == null)
                    {
                        Flag = false;
                        continue;
                    }
                    string grname = MyValues.GetValue(1, 1).ToString();
                    MSRDiscGroupInfo Group = new MSRDiscGroupInfo()
                    {
                        DiscGroupName = grname,

                    };
                    insertDisciplineGroup(Group);
                    groupID++;
                    for (int i = 0; i < 325; i++)
                    {
                        MSRListInfo listInfo = new MSRListInfo();
                        listInfo.DiscGroupID = groupID;
                        listInfo.InternID = i + 1;
                        listInfo.Kperform = Kperform[groupID - 1];
                        listInfo.ListName = grname;
                        listID++;
                        for (int j = 2; j < 50; j++)
                        {
                            if (DiscName.GetValue(1, j).ToString() == null)
                            {
                                break;
                            }
                            if (MyValues.GetValue(1, j) == null)
                            {
                                continue;
                            }
                            else if (MyValues.GetValue(1, j) == "1")
                            {                    


                                MSRDisciplineListInfo disclist = new MSRDisciplineListInfo();
                                disclist.DisciplineID = j - 1;
                                //disclist.DiscListID = ?;
                                disclist.ListID = listID;
                                


                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }


        }

        public void truncateDisciplineRelatedTable()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "TRUNCATE TABLE dbo.MSRRegionInfo \n TRUNCATE TABLE MSRDisciplineInfo \n TRUNCATE TABLE MSRDiscGroupInfo";

            cmd.Connection = connection;
            connection.Open();
            int x = cmd.ExecuteNonQuery();

            connection.Close();
        }

        public int insertDiscipline(MSRDisciplineInfo disciplineInfo)
        {
            int discID = -1;
     
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT INTO MSRDisciplineInfo (CourseCredit, DisciplineName) VALUES (" + disciplineInfo.CourseCredit + ", " + disciplineInfo.DisciplineName + @");                             
                SET @DisciplineID = SCOPE_IDENTITY()";


            cmd.Connection = connection;
            connection.Open();
            cmd.ExecuteNonQuery();
            discID = (int)cmd.Parameters["@DisciplineID"].Value;

            connection.Close();

            return discID;
        }

        public void insertDisciplineDuration(MSRDiscDurationPrInfo durationPrInfo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.CommandText = "INSERT INTO MSRDiscDurationPrInfo (TrainginProgramID, DiscplineID, Duration) VALUES (" + durationPrInfo.TrainginProgramID  +" , " + durationPrInfo.DiscplineID+ ", " + durationPrInfo.Duration + ")";   


            cmd.Connection = connection;
            connection.Open();
            cmd.ExecuteNonQuery();   

            connection.Close();
            
        }

        public void insertDisciplineGroup(MSRDiscGroupInfo discGroupInfo)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;

            cmd.CommandText = "INSERT INTO MSRDiscGroupInfo (DiscGroupName) VALUES (" + discGroupInfo.DiscGroupName + ")";

            cmd.Connection = connection;
            connection.Open();
            cmd.ExecuteNonQuery();

            connection.Close();

        }

        public int insertDisciplineList(MSRDisciplineListInfo disciplineListInfo)
        {
            int discID = -1;

            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = "INSERT INTO MSRDisciplineInfo (DisciplineID, ListID) VALUES (" + disciplineListInfo.DisciplineID + ", " + disciplineListInfo.ListID + @");                             
                SET @DiscListID = SCOPE_IDENTITY()";


            cmd.Connection = connection;
            connection.Open();
            cmd.ExecuteNonQuery();
            discID = (int)cmd.Parameters["@DisciplineID"].Value;

            connection.Close();

            return discID;
        }
    }
}


using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Excel = Microsoft.Office.Interop.Excel;

namespace DataLayer
{
    public class ImportData
    {

        public UGentMSRDataContext context;

        public ImportData()
        {
            ImportTheDisciplines("DiscGroups.xlsx");
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
            context.ExecuteCommand("TRUNCATE TABLE MSRDisciplineInfo");
            context.ExecuteCommand("TRUNCATE TABLE MSRDiscDurationPrInfo");
            context.ExecuteCommand("TRUNCATE TABLE MSRDiscGroupInfo");
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
                context.MSRDisciplineInfos.InsertOnSubmit(dis);
                context.SubmitChanges();
                MSRDiscDurationPrInfo dur = new MSRDiscDurationPrInfo() {
                    TrainginProgramID = 0,
                    DiscplineID = dis.DisciplineID,
                    Duration = 1,
                };
                context.MSRDiscDurationPrInfos.InsertOnSubmit(dur);
                context.SubmitChanges();
                dis.DiscDurationPrID = dur.DiscDurationPrID;
                context.SubmitChanges();


            }

            int[] Kperform = new int[] { 7, 1, 1, 1, 1 };
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
                    context.MSRDiscGroupInfos.InsertOnSubmit(Group);
                    context.SubmitChanges();                   

                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }


        }
    }
}

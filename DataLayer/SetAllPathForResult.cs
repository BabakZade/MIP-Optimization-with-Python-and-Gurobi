using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace DataLayer
{
	public class SetAllPathForResult
	{
		public string OutPutLocation;
        public string OutPutGr;
        public string ColumnLoc;

        public string InstanceLocation;
		public string ExprimentLocation;
		public string InsGroupLocation;
		public string CurrentDir;
		public SetAllPathForResult(string Expriment, string Methodology, string InsGroupName)
		{
			
			CurrentDir = System.IO.Directory.GetCurrentDirectory() + "\\" + Expriment + "\\";
			
			InstanceLocation = CurrentDir + "Instance\\";
			if (!Directory.Exists(InstanceLocation))
			{
				Directory.CreateDirectory(InstanceLocation);
			}
			if (InsGroupName!="")
			{
				InsGroupLocation = CurrentDir + "Instance\\"+InsGroupName + "\\";
				if (!Directory.Exists(InsGroupLocation))
				{
					Directory.CreateDirectory(InsGroupLocation);
				}
            }
            if (Methodology != "")
            {
                ExprimentLocation = CurrentDir + "\\" + Methodology + "\\";
                if (!Directory.Exists(ExprimentLocation))
                {
                    Directory.CreateDirectory(ExprimentLocation);
                }
                OutPutLocation = ExprimentLocation  + "Result\\";
                if (!Directory.Exists(OutPutLocation))
                {
                    Directory.CreateDirectory(OutPutLocation);
                }
                OutPutGr = ExprimentLocation + InsGroupName + "\\";
                if (!Directory.Exists(OutPutLocation))
                {
                    Directory.CreateDirectory(OutPutGr);
                }
                ColumnLoc = ExprimentLocation + InsGroupName + "\\" + "Column\\";
                if (!Directory.Exists(ColumnLoc))
                {
                    Directory.CreateDirectory(ColumnLoc);
                }
            }

        }

	}
}

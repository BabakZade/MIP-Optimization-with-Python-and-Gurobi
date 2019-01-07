using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace DataLayer
{
	public class SetAllPathForResult
	{
		public string OutPutLocation;
		public string InstanceLocation;
		public string ExprimentLocation;
		
		public string CurrentDir;
		public SetAllPathForResult(string ExprimentName,string InsName)
		{
			
			CurrentDir = System.IO.Directory.GetCurrentDirectory();
			ExprimentLocation = CurrentDir + "\\" + ExprimentName + "\\";
			if (!Directory.Exists(ExprimentLocation))
			{
				Directory.CreateDirectory(ExprimentLocation);
			}
			OutPutLocation = ExprimentLocation + "Result\\";
			if (!Directory.Exists(OutPutLocation))
			{
				Directory.CreateDirectory(OutPutLocation);
			}
			InstanceLocation = ExprimentLocation + "\\Instance\\";
			if (!Directory.Exists(InstanceLocation))
			{
				Directory.CreateDirectory(InstanceLocation);
			}
			

		}

	}
}

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace MedicalTraineeScheduling
{
	public class SetAllPathForResult
	{
		string OutPutLocation;
		string ExprimentLocation;
		string CurrentDir;
		public SetAllPathForResult(string ExprimentName)
		{
			
			CurrentDir = System.IO.Directory.GetCurrentDirectory();
			OutPutLocation = CurrentDir + "\\OutPut\\";
			if (!Directory.Exists(OutPutLocation))
			{
				Directory.CreateDirectory(OutPutLocation);
			}
			ExprimentLocation = OutPutLocation + ExprimentName+"\\";
			if (!Directory.Exists(ExprimentLocation))
			{
				Directory.CreateDirectory(ExprimentLocation);
			}
		}
	}
}

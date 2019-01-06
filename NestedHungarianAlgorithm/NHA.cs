using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedHungarianAlgorithm
{
	public class NHA
	{
		public ArrayList ActiveList;
		public AllData data;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		HungarianNode Root;
		public NHA(AllData data)
		{
			this.data = data;
			Root = new HungarianNode(0,data, new HungarianNode());
			ActiveList.Add(Root);
		
		}
		public void Initialization()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			
		

		}
	}
}

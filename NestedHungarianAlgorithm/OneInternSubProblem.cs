using System;
using System.Collections.Generic;
using System.Text;
using DataLayer;

namespace NestedHungarianAlgorithm
{
	public class OneInternSubProblem
	{
		public AllData data;
		public int Interns;
		public int Disciplins;
		public int Hospitals;
		public int Timepriods;
		public int TrainingPr;
		public int Wards;
		public int Region;
		public int DisciplineGr;
		public PositionMap[][] internBestSch_it;

		public OneInternSubProblem(AllData allData)
		{
			data = allData;
		}
		public void Initial()
		{
			Disciplins = data.General.Disciplines;
			Hospitals = data.General.Hospitals;
			Timepriods = data.General.TimePriods;
			TrainingPr = data.General.TrainingPr;
			Interns = data.General.Interns;
			Wards = data.General.HospitalWard;
			Region = data.General.Region;
			DisciplineGr = data.General.DisciplineGr;
		}
	}
}

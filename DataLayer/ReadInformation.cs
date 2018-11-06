using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataLayer
{
	public class ReadInformation
	{
		public AllData data;
		public ReadInformation(string path)
		{
			ReadInstances(path);
			NormalizeData();
		}
		/// <summary>
		/// this function creates the instances and writes it in the given location
		/// </summary>
		public void ReadInstances(string path)
		{
			data = new AllData();
			StreamReader tw = new StreamReader(path);
			string line = tw.ReadLine();

			int indexN;
			
			//write the general Data
			
            // general Info
			data.General = new GeneralInfo();
			line = tw.ReadLine();
            indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
			data.General.Interns = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.Disciplines = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.Hospitals = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.TimePriods = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			

			data.General.TrainingPr = int.Parse(line);
			tw.ReadLine();

			//Create and write training program info
			data.TrainingPr = new TrainingProgramInfo[data.General.TrainingPr];
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				data.TrainingPr[p] = new TrainingProgramInfo(data.General.Disciplines);

			}

			
			tw.ReadLine();
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				line = tw.ReadLine();
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				data.TrainingPr[p].Name = line.Substring(0, indexN);
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].MandatoryD = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].ArbitraryD = int.Parse(line.Substring(0, indexN));

			}
			tw.ReadLine();

			tw.ReadLine();
			line = tw.ReadLine();
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.TrainingPr[p].Program_d[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				}
				line = tw.ReadLine();
			}
			tw.ReadLine();

			line = tw.ReadLine();
			
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.TrainingPr[p].PrManDis_d[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				}
				line = tw.ReadLine();
			}
			

			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				line = tw.ReadLine();
				line = tw.ReadLine();
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int dd = 0; dd < data.General.Disciplines; dd++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.TrainingPr[p].Alias_d_d[d][dd] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						
					}
					line = tw.ReadLine();
				}
			
			}


			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				line = tw.ReadLine();
				line = tw.ReadLine();
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int dd = 0; dd < data.General.Disciplines; dd++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.TrainingPr[p].Advance_d_d[d][dd] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

					}
					line = tw.ReadLine();
				}

			}
			// Create and write Hospital Info 
			data.Hospital = new HospitalInfo[data.General.Hospitals];
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				data.Hospital[h] = new HospitalInfo(data.General.Disciplines, data.General.TimePriods);
			}

			
			line = tw.ReadLine();
			line = tw.ReadLine();
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Hospital[h].Hospital_d[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);
					
				}
				line = tw.ReadLine();
			}

			tw.ReadLine();
			
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				line = tw.ReadLine();
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].HospitalMaxDem_td[t][d] = int.Parse(line.Substring(0, indexN)) ;
						line = line.Substring(indexN + 1);						
					}
					line = tw.ReadLine();
				}
			}
			line = tw.ReadLine();
			for (int h = 0; h < data.General.Hospitals; h++)
			{				
				line = tw.ReadLine();
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].HospitalMinDem_td[t][d] = int.Parse(line.Substring(0, indexN));
						line = line.Substring(indexN + 1);						
					}
					line = tw.ReadLine();
				}
			}

			line = tw.ReadLine();

			// Create and write Discipline Info
			data.Discipline = new DisciplineInfo[data.General.Disciplines];
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				data.Discipline[d] = new DisciplineInfo(data.General.Disciplines, data.General.TrainingPr);
			}

			
			line = tw.ReadLine();
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				for (int p = 0; p < data.General.TrainingPr; p++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Discipline[d].Duration_p[p] = int.Parse(line.Substring(0, indexN));
					line = line.Substring(indexN + 1);
					
				}
				line = tw.ReadLine();
			}
			tw.ReadLine();

			for (int d = 0; d < data.General.Disciplines; d++)
			{
				line = tw.ReadLine();
				for (int dd = 0; dd < data.General.Disciplines; dd++)
				{
					for (int p = 0; p < data.General.TrainingPr; p++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Discipline[d].Skill4D_dp[dd][p] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();
				}

				tw.ReadLine();
			}
			line = tw.ReadLine();

			// create and write Intern info
			data.Intern = new InternInfo[data.General.Interns];
			for (int i = 0; i < data.General.Interns; i++)
			{
				data.Intern[i] = new InternInfo(data.General.Hospitals, data.General.Disciplines, data.General.TimePriods);
			}

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				data.Intern[i].Name = line.Substring(0, indexN);
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Intern[i].ProgramID = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Intern[i].wieght_d = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Intern[i].wieght_h = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Intern[i].wieght_ch = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Intern[i].wieght_w = int.Parse(line.Substring(0, indexN));
				line = tw.ReadLine();
			}

			line = tw.ReadLine();

			
			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].Abi_d[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);					
				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].Fulfilled_d[d] = int.Parse(line.Substring(0, indexN))==1;
					line = line.Substring(indexN + 1);
				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].Prf_d[d] = int.Parse(line.Substring(0, indexN)) ;
					line = line.Substring(indexN + 1);
				}
				line = tw.ReadLine();
			}
			tw.ReadLine();
			
			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.Hospitals; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].Prf_h[d] = int.Parse(line.Substring(0, indexN));
					line = line.Substring(indexN + 1);
				}
				line = tw.ReadLine();
			}
			tw.ReadLine();

			
			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.TimePriods; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].Ave_t[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);

				}
				line = tw.ReadLine();
			}
			tw.ReadLine();

			//Algorithm Settings
			data.AlgSettings = new AlgorithmSettings();
			
			line = tw.ReadLine();
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
			data.AlgSettings.MIPTime = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.BPTime = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.NodeTime = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.MasterTime = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.SubTime = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.RHSepsi = Convert.ToDouble(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.RCepsi = Convert.ToDouble(line.Substring(0, indexN));
			tw.Close();
		}

		public void NormalizeData()
		{
			data.PrChan_i = new int[data.General.Interns];
			data.PrHosp_i = new int[data.General.Interns];
			data.PrWait_i = new int[data.General.Interns];
			data.PrDisc_i = new int[data.General.Interns];
			for (int i = 0; i < data.General.Interns; i++)
			{
				data.PrChan_i[i] = 1;
				data.PrHosp_i[i] = 1;
				data.PrWait_i[i] = 1;
				data.PrDisc_i[i] = 1;

				// if we need to normalize it 
				data.PrChan_i[i] = data.TrainingPr[data.Intern[i].ProgramID].ArbitraryD + data.TrainingPr[data.Intern[i].ProgramID].MandatoryD;
				data.PrHosp_i[i] = 0;
				data.PrWait_i[i] = data.General.TimePriods;
				data.PrDisc_i[i] = 0;
				for (int h = 0; h < data.General.Hospitals; h++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						if (data.Hospital[h].Hospital_d[d])
						{
							data.PrHosp_i[i] += data.Intern[i].Prf_h[h];
						}
					}
					
				}
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int h = 0; h < data.General.Hospitals; h++)
					{
						if (data.Hospital[h].Hospital_d[d])
						{
							data.PrDisc_i[i] += data.Intern[i].Prf_d[d];
						}
					}
					
				}
			}


		}
	}
}

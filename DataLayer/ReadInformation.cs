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
		public ReadInformation(string path, string ExprimentName, string Methodology, string groupName, string InsName)
		{
			ReadInstances(path, ExprimentName, Methodology, groupName, InsName);
			NormalizeData();
		}
		/// <summary>
		/// this function creates the instances and writes it in the given location
		/// </summary>
		public void ReadInstances(string path, string ExprimentName, string Methodology, string groupName, string InsName)
		{
			data = new AllData(ExprimentName,Methodology,groupName);
			StreamReader tw = new StreamReader(data.allPath.InsGroupLocation+InsName);
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
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.TrainingPr = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.HospitalWard = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.DisciplineGr = int.Parse(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.General.Region = int.Parse(line.Substring(0, indexN));

			line = tw.ReadLine();

			//Create and write training program info
			data.TrainingPr = new TrainingProgramInfo[data.General.TrainingPr];
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				data.TrainingPr[p] = new TrainingProgramInfo(data.General.Disciplines, data.General.DisciplineGr);
			}

			
			line = tw.ReadLine();
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				line = tw.ReadLine();
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				data.TrainingPr[p].Name = line.Substring(0, indexN);
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].AcademicYear = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].weight_p = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_SumDesi = double.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_MinDesi = double.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_ResCap = double.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_EmrCap = double.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_NotUsedAcc = double.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].DiscChangeInOneHosp = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.TrainingPr[p].CoeffObj_MINDem = double.Parse(line.Substring(0, indexN));
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			line = tw.ReadLine();
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.TrainingPr[p].Prf_d[d] = int.Parse(line.Substring(0, indexN));
					line = line.Substring(indexN + 1);
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			for (int p = 0; p < data.General.TrainingPr; p++)
			{
				line = tw.ReadLine();
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.TrainingPr[p].InvolvedDiscipline_gd[g][d] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					}
					line = tw.ReadLine();
				}
				
			}
			line = tw.ReadLine();
            line = tw.ReadLine();
            if (line.Contains("Also"))
            {
                for (int p = 0; p < data.General.TrainingPr; p++)
                {
                    line = tw.ReadLine();
                    for (int dd = 0; dd < data.General.Disciplines; dd++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                            data.TrainingPr[p].AKA_dD[dd][d] = int.Parse(line.Substring(0, indexN)) == 1;
                            line = line.Substring(indexN + 1);
                            indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                        }
                        line = tw.ReadLine();
                    }

                }

                line = tw.ReadLine();
                line = tw.ReadLine();

                for (int p = 0; p < data.General.TrainingPr; p++)
                {
                    line = tw.ReadLine();
                    for (int dd = 0; dd < data.General.Disciplines; dd++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                            data.TrainingPr[p].cns_dD[dd][d] = int.Parse(line.Substring(0, indexN));
                            line = line.Substring(indexN + 1);
                            indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                        }
                        line = tw.ReadLine();
                    }

                }
            }

			// Create and write Hospital Info 
			data.Hospital = new HospitalInfo[data.General.Hospitals];
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				data.Hospital[h] = new HospitalInfo(data.General.Disciplines, data.General.TimePriods, data.General.HospitalWard, data.General.Region);
			}			
			for (int h = 0; h < data.General.Hospitals; h++)
			{
                
				line = tw.ReadLine();
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].Hospital_dw[d][w] = int.Parse(line.Substring(0, indexN)) == 1;
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
					for (int d = 0; d < data.General.HospitalWard; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].HospitalMaxDem_tw[t][d] = int.Parse(line.Substring(0, indexN)) ;
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
					for (int d = 0; d < data.General.HospitalWard; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].HospitalMinDem_tw[t][d] = int.Parse(line.Substring(0, indexN));
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
					for (int d = 0; d < data.General.HospitalWard; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].ReservedCap_tw[t][d] = int.Parse(line.Substring(0, indexN));
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
					for (int d = 0; d < data.General.HospitalWard; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].EmergencyCap_tw[t][d] = int.Parse(line.Substring(0, indexN));
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();
				}
			}

			line = tw.ReadLine();
            if (line.Contains("Yearly"))// yearly max demand
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    line = tw.ReadLine();
                    for (int d = 0; d < data.General.HospitalWard; d++)
                    {
                        indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                        data.Hospital[h].HospitalMaxYearly_w[d] = int.Parse(line.Substring(0, indexN));
                        line = line.Substring(indexN + 1);
                    }
                }
                line = tw.ReadLine();
                line = tw.ReadLine();
            }
            if (line.Contains("Yearly")) // yearly min demand
            {
                for (int h = 0; h < data.General.Hospitals; h++)
                {
                    line = tw.ReadLine();
                    for (int d = 0; d < data.General.HospitalWard; d++)
                    {
                        indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
                        data.Hospital[h].HospitalMinYearly_w[d] = int.Parse(line.Substring(0, indexN));
                        line = line.Substring(indexN + 1);
                    }
                }
                line = tw.ReadLine();
                line = tw.ReadLine();
            }
			line = tw.ReadLine();
			for (int h = 0; h < data.General.Hospitals; h++)
			{
					for (int d = 0; d < data.General.Region; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Hospital[h].InToRegion_r[d] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();				
			}

			line = tw.ReadLine();

			// Create and write Discipline Info
			data.Discipline = new DisciplineInfo[data.General.Disciplines];
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				data.Discipline[d] = new DisciplineInfo(data.General.Disciplines, data.General.TrainingPr);
				data.Discipline[d].ID = d;
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
			line = tw.ReadLine();
			if (line.Contains("Course credit"))
			{
				line = tw.ReadLine();
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int p = 0; p < data.General.TrainingPr; p++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Discipline[d].CourseCredit_p[p] = int.Parse(line.Substring(0, indexN));
						line = line.Substring(indexN + 1);

					}
					line = tw.ReadLine();
				}
				line = tw.ReadLine();
			}
			

			for (int d = 0; d < data.General.Disciplines; d++)
			{
				line = tw.ReadLine();
				for (int dd = 0; dd < data.General.Disciplines; dd++)
				{
					for (int p = 0; p < data.General.TrainingPr; p++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Discipline[d].Skill4D_dp[dd][p] = int.Parse(line.Substring(0, indexN)) == 1;
						if (data.Discipline[d].Skill4D_dp[dd][p])
						{
							data.Discipline[d].requiresSkill_p[p] = true;
							data.Discipline[dd].requiredLater_p[p] = true;
						}
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();
				}

				line = tw.ReadLine();
			}
			line = tw.ReadLine();

			// create and write Intern info
			data.Intern = new InternInfo[data.General.Interns];
			for (int i = 0; i < data.General.Interns; i++)
			{
				data.Intern[i] = new InternInfo(data.General.Hospitals, data.General.Disciplines, data.General.TimePriods, data.General.DisciplineGr, data.General.TrainingPr, data.General.Region);
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

				data.Intern[i].isProspective = bool.Parse(line.Substring(0, indexN));
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
                if (indexN != line.Count())
                {
                    line = line.Substring(indexN + 1);
                    indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

                    data.Intern[i].wieght_cns = int.Parse(line.Substring(0, indexN));
                }
                

                line = tw.ReadLine();
			}

			line = tw.ReadLine();
							
		

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				line = tw.ReadLine();
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Intern[i].DisciplineList_dg[d][g] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();
				}
				
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.DisciplineGr; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].ShouldattendInGr_g[d] = int.Parse(line.Substring(0, indexN));
					line = line.Substring(indexN + 1);
				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();
			line = tw.ReadLine();
			while (line != "---")
			{
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				int IndexI = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				int indexD = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				int indext = int.Parse(line.Substring(0, indexN));
				data.Intern[IndexI].OverSea_dt[indexD][indext] = true;
				data.Intern[IndexI].overseaReq = true;
				line = tw.ReadLine();
			}

			line = tw.ReadLine();
			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{				
				bool OVERSEA = false;
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int t = 0; t < data.General.TimePriods; t++)
					{
						if (data.Intern[i].OverSea_dt[d][t])
						{
							OVERSEA = true;
						}

					}
				}
				for (int d = 0; d < data.General.Disciplines && OVERSEA; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].FHRequirment_d[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);
				}
				if (OVERSEA)
				{
					line = tw.ReadLine();
				}
			}
			line = tw.ReadLine();
			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				for (int d = 0; d < data.General.Region; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Intern[i].TransferredTo_r[d] = int.Parse(line.Substring(0, indexN)) == 1;
					line = line.Substring(indexN + 1);
				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			for (int i = 0; i < data.General.Interns; i++)
			{
				line = tw.ReadLine();
				for (int h = 0; h < data.General.Hospitals; h++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
						data.Intern[i].Abi_dh[d][h] = int.Parse(line.Substring(0, indexN)) == 1;
						line = line.Substring(indexN + 1);
					}
					line = tw.ReadLine();
				}
				
			}
			line = tw.ReadLine();

			line = tw.ReadLine();
			while (line != "---")
			{
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				int IndexI = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				int indexD = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				int indexH = int.Parse(line.Substring(0, indexN));
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				int indexP = int.Parse(line.Substring(0, indexN));
				data.Intern[IndexI].Fulfilled_dhp[indexD][indexH][indexP] = true;
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
			line = tw.ReadLine();
			
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
			line = tw.ReadLine();

			
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
			line = tw.ReadLine();

			// create and write Region info
			data.Region = new RegionInfo[data.General.Region];
			line = tw.ReadLine();
			for (int r = 0; r < data.General.Region; r++)
			{
				data.Region[r] = new RegionInfo(data.General.TimePriods);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
				data.Region[r].Name = line.Substring(0, indexN);
				line = line.Substring(indexN + 1);
				indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

				data.Region[r].SQLID = int.Parse(line.Substring(0, indexN));
                line = tw.ReadLine();
			}
			line = tw.ReadLine();
			line = tw.ReadLine();
		
			for (int i = 0; i < data.General.Region; i++)
			{
				for (int d = 0; d < data.General.TimePriods; d++)
				{
					indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;
					data.Region[i].AvaAcc_t[d] = int.Parse(line.Substring(0, indexN));
					line = line.Substring(indexN + 1);

				}
				line = tw.ReadLine();
			}
			line = tw.ReadLine();


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
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.BigM = Convert.ToInt32(line.Substring(0, indexN));
			data.AlgSettings.MotivationBM = data.AlgSettings.BigM / 1000;
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.bucketBasedImpPercentage = Convert.ToDouble(line.Substring(0, indexN));
			line = line.Substring(indexN + 1);
			indexN = (line.IndexOf(" ") > 0) ? line.IndexOf(" ") : line.Length;

			data.AlgSettings.internBasedImpPercentage = Convert.ToDouble(line.Substring(0, indexN));
			
			line = tw.ReadLine();

		
			tw.Close();
		}

		public void NormalizeData()
		{
			for (int i = 0; i < data.General.Interns; i++)
			{
				
				data.Intern[i].setKAllDiscipline(data);
				data.Intern[i].setAveInfo(data);
				data.Intern[i].setThePercetage(data);
				data.Intern[i].sortPrf(data.General.Hospitals, data.General.Disciplines, data.General.DisciplineGr, data.General.HospitalWard, data);
				data.Intern[i].SetSimplifiedScheduleForMaxPrf(data);
			}
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				data.Discipline[d].setIsRare(data);
			}
		}
	}
}

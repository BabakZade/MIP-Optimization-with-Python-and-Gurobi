using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace SubProblemDP
{
	public class StateSchedule
	{
		public int theHospital;
		public int theDiscipline;
	
		public StateSchedule(int theH, int theD)
		{
			theDiscipline = theD;
			theHospital = theH;
		}
		public StateSchedule()
		{
			theDiscipline = -1;
			theHospital = -1;
		}

		public StateSchedule(StateSchedule theCopy)
		{
			theDiscipline = theCopy.theDiscipline;
			theHospital = theCopy.theHospital;
		}

	}
	public class StateStage
	{
		public bool flagResD;
		public bool flagEmrD;
		public double Fx;
		public bool xStar;
		public bool x_wait;
		public int x_Disc;
		public int x_Hosp;
		public int x_group;
		public int[] x_K_g;
		public int x_K;
		public bool isRoot;
		public AllData data;
		public ArrayList possibleStates;
		public bool[] activeDisc;
		public StateSchedule[] theSchedule_t;
		public int tStage;
		public int totalChange;
		public StateStage(AllData alldata)
		{
			data = alldata;
			Fx = -1;
			Initial(data);
		}
		public void Initial(AllData alldata)
		{
			flagEmrD = false;
			flagResD = false;
			theSchedule_t = new StateSchedule[alldata.General.TimePriods];
			for (int t = 0; t < alldata.General.TimePriods; t++)
			{
				theSchedule_t[t] = new StateSchedule();
			}
			activeDisc = new bool[alldata.General.Disciplines];
			totalChange = 0;
		}
		public StateStage(StateStage stateInput, StateStage xInput, double desireCoeff, int theI, AllData alldata, int theStageTime, bool isRoot)
		{
			data = alldata;
			tStage = theStageTime;
			Initial(data);
			activeDisc = new bool[alldata.General.Disciplines];
            if (isRoot)
			{
				this.isRoot = true;
			}

			if (xInput.x_wait)
			{
				x_wait = true;
				if (stateInput.x_K > 0)
				{
					Fx = desireCoeff * data.Intern[theI].wieght_w;
					// otherwise the solution is complete
				}

				if (!isRoot)
				{
					// if it haven't had discipline yet
					// its status and its father's status are same
					// it only matters if its father is root and waited there
					if (stateInput.x_Disc < 0)
					{
						isRoot = stateInput.isRoot;
					}

					x_Disc = stateInput.x_Disc;
					x_Hosp = stateInput.x_Hosp;
					x_group = stateInput.x_group;
				}
				else
				{
					x_Disc = -1;
					x_Hosp = -1;
					x_group = -1;
                    Fx = desireCoeff * data.Intern[theI].wieght_w; // if it is root the x_k == 0 so it was not calculated above 
                }
			}
			else
			{
				x_Disc = xInput.x_Disc;
				x_Hosp = xInput.x_Hosp;
				x_group = xInput.x_group;
				x_wait = false;
				activeDisc[x_Disc] = true;
				if (x_Hosp < data.General.Hospitals)
				{
					Fx = desireCoeff * (data.Intern[theI].Prf_d[x_Disc] * data.Intern[theI].wieght_d
					+ data.Intern[theI].Prf_h[x_Hosp] * data.Intern[theI].wieght_h
					+ data.TrainingPr[data.Intern[theI].ProgramID].weight_p * data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[x_Disc]);
				}
				else // oversea
				{
					Fx = desireCoeff * (data.Intern[theI].Prf_d[x_Disc] * data.Intern[theI].wieght_d
					+ data.TrainingPr[data.Intern[theI].ProgramID].weight_p * data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[x_Disc]);
				}


			}
			if (!isRoot)
			{
				Fx += stateInput.Fx;
				if (!xInput.x_wait)
				{
					if (stateInput.x_Hosp != xInput.x_Hosp && stateInput.x_Hosp != -1)
					{
						Fx += desireCoeff * data.Intern[theI].wieght_ch;
						totalChange = stateInput.totalChange + 1;

					}
                    if (stateInput.x_Disc >= 0 && xInput.x_Disc >= 0)
                    {
                        Fx += desireCoeff * data.Intern[theI].wieght_cns * data.TrainingPr[data.Intern[theI].ProgramID].cns_dD[stateInput.x_Disc][xInput.x_Disc];
                    }
                    
                }
			}
			

			possibleStates = new ArrayList();

			setNextStates(stateInput, theI);


		}
		public void setNextStates(StateStage stateInput, int theI)
		{

			x_K = 0;
			x_K_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				if (isRoot)
				{
					x_K_g[g] = data.Intern[theI].ShouldattendInGr_g[g];
					x_K = data.Intern[theI].K_AllDiscipline;					
				}
				else
				{
					x_K_g[g] = stateInput.x_K_g[g];
					x_K = stateInput.x_K;
				}
			}

			if (isRoot)
			{
				for (int h = 0; h < data.General.Hospitals && x_Disc>=0; h++)
				{
					if (data.Intern[theI].Fulfilled_dhp[x_Disc][h][data.Intern[theI].ProgramID])
					{
						x_K -= data.Discipline[x_Disc].CourseCredit_p[data.Intern[theI].ProgramID];
						for (int g = 0; g < data.General.DisciplineGr; g++)
						{
							if (data.Intern[theI].DisciplineList_dg[x_Disc][g])
							{
								x_K_g[g] -= data.Discipline[x_Disc].CourseCredit_p[data.Intern[theI].ProgramID];
								break;
							}
						}

						break;
					}
				}

			}

			theSchedule_t = new StateSchedule[data.General.TimePriods];
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				if (isRoot)
				{
					theSchedule_t[t] = new StateSchedule();
				}
				else
				{
					theSchedule_t[t] = new StateSchedule(stateInput.theSchedule_t[t]);
				}
			}
			activeDisc = new bool[data.General.Disciplines];
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				if (isRoot)
				{
					activeDisc[d] = false;
				}
				else
				{
					activeDisc[d] = stateInput.activeDisc[d];
				}
				if (d == x_Disc)
				{
					activeDisc[d] = true;
				}

			}
			if (x_wait)
			{
				possibleStates.Add(this);
			}
			else if(x_Disc >= 0)
			{
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					if (data.Intern[theI].DisciplineList_dg[x_Disc][g] && x_K_g[g] >= data.Discipline[x_Disc].CourseCredit_p[data.Intern[theI].ProgramID])
					{
						for (int t = tStage; t < tStage + data.Discipline[x_Disc].Duration_p[data.Intern[theI].ProgramID]; t++)
						{
							theSchedule_t[t] = new StateSchedule(x_Hosp, x_Disc);
						}
						// insert at right place 
						int counter = 0;
						StateStage tmp = copyState(this, g, theI);
						foreach (StateStage item in possibleStates)
						{
							if (tmp.Fx > item.Fx)
							{
								break;
							}
							else
							{
								counter++;
							}
						}
						possibleStates.Insert(counter, tmp);
					}
				}
			}

		}

		// it is private function
		StateStage copyState(StateStage copyable, int theG, int theI)
		{
			StateStage duplicated = new StateStage(data);
			duplicated.Fx = copyable.Fx;
			duplicated.xStar = copyable.xStar;
			duplicated.x_wait = copyable.x_wait;
			duplicated.x_Disc = copyable.x_Disc;
			duplicated.x_Hosp = copyable.x_Hosp;
			duplicated.flagResD = copyable.flagResD;
			duplicated.flagEmrD = copyable.flagEmrD;
			duplicated.x_K_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				duplicated.x_K_g[g] = copyable.x_K_g[g];
				if (g == theG)
				{
					duplicated.x_K_g[g] -= data.Discipline[copyable.x_Disc].CourseCredit_p[data.Intern[theI].ProgramID];
				}
			}
			duplicated.x_K = copyable.x_K - data.Discipline[copyable.x_Disc].CourseCredit_p[data.Intern[theI].ProgramID];
			duplicated.isRoot = copyable.isRoot;
			duplicated.x_group = theG;
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				duplicated.activeDisc[d] = copyable.activeDisc[d];
			}
			duplicated.tStage = copyable.tStage;
			duplicated.theSchedule_t = new StateSchedule[data.General.TimePriods];
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				duplicated.theSchedule_t[t] = new StateSchedule(copyable.theSchedule_t[t]);

			}
			return duplicated;
		}

		public string DisplayMe()
		{
			string result = "";
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				if (theSchedule_t[t].theDiscipline >= 0)
				{
					result += theSchedule_t[t].theDiscipline.ToString("00") + "*"+ theSchedule_t[t].theHospital.ToString("00") +"  ";
				}
				
			}
			return result;
		}
	}
}

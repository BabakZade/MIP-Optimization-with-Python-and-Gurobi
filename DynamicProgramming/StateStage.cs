using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedDynamicProgrammingAlgorithm
{
	public class StateStage
	{
		public double Fx;
		public bool xStar;
		public bool x_wait;
		public int x_Disc;
		public int x_Hosp;
		public int[] x_K_g;
		public int x_K;
		public bool isRoot;
		public AllData data;
		public ArrayList possibleStates;
		public bool[] activeDisc;
		public int[] theSchedule_t;
		public int tStage;

		public StateStage()
		{

		}
		public StateStage(StateStage stateInput, StateStage xInput, int theI, AllData alldata, int theStage)
		{
			data = alldata;
			tStage = theStage;
			if (tStage == 0)
			{
				isRoot = true;
			}
			
			if (xInput.x_wait)
			{
				x_wait = true;
				if (stateInput.x_K>0)
				{
					Fx = data.Intern[theI].wieght_w;
					// otherwise the solution is complete
				}
				
				if (!isRoot)
				{
					x_Disc = stateInput.x_Disc;
					x_Hosp = stateInput.x_Hosp;
				}
				else
				{
					x_Disc = -1;
					x_Hosp = -1;
				}
			}
			else
			{
				x_Disc = xInput.x_Disc;
				x_Hosp = xInput.x_Hosp;
				x_wait = false;
				activeDisc[x_Disc] = true;
				if (x_Hosp < data.General.Hospitals)
				{
					Fx = data.Intern[theI].Prf_d[x_Disc] * data.Intern[theI].wieght_d
					+ data.Intern[theI].Prf_h[x_Hosp] * data.Intern[theI].wieght_h
					+ data.TrainingPr[data.Intern[theI].ProgramID].weight_p * data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[x_Disc];
				}
				else // oversea
				{
					Fx = data.Intern[theI].Prf_d[x_Disc] * data.Intern[theI].wieght_d					
					+ data.TrainingPr[data.Intern[theI].ProgramID].weight_p * data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[x_Disc];
				}
				
			
			}
			if (!isRoot)
			{
				Fx += stateInput.Fx;
				if (!xInput.x_wait)
				{
					if (stateInput.x_Hosp != xInput.x_Hosp && stateInput.x_Hosp != -1)
					{
						Fx += data.Intern[theI].wieght_ch;

					}
				}
				
			}

			setNextStates(stateInput, theI);
		}
		public void setNextStates(StateStage stateInput, int theI)
		{
			possibleStates = new ArrayList();
			x_K = 0;
			x_K_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				if (isRoot)
				{
					x_K_g[g] = data.Intern[theI].ShouldattendInGr_g[g];
					x_K+= data.Intern[theI].ShouldattendInGr_g[g];
				}
				else
				{
					x_K_g[g] = stateInput.x_K_g[g];
					x_K = stateInput.x_K;
				}
			}

			theSchedule_t = new int[data.General.TimePriods];
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				if (isRoot)
				{
					theSchedule_t[t] = -1;
				}
				else
				{
					theSchedule_t[t] = stateInput.theSchedule_t[t];
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
				
			}
			if (x_wait)
			{
				possibleStates.Add(this);
			}
			else
			{
				
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					if (data.Intern[theI].DisciplineList_dg[x_Disc][g] && x_K_g[g] > 0)
					{
						for (int t = tStage; t < tStage+ data.Discipline[x_Disc].Duration_p[data.Intern[theI].ProgramID]; t++)
						{
							theSchedule_t[tStage] = x_Disc;
						}
						// insert at right place 
						int counter = 0;
						StateStage tmp = copyState(this, g);
						foreach (StateStage item in possibleStates)
						{
							if (tmp.Fx > item.Fx )
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
		StateStage copyState(StateStage copyable, int theG)
		{
			StateStage duplicated = new StateStage();
			duplicated.Fx = copyable.Fx;
			duplicated.xStar = copyable.xStar ;
			duplicated.x_wait = copyable.x_wait;
			duplicated.x_Disc = copyable.x_Disc;
			duplicated.x_Hosp = copyable.x_Hosp;
			duplicated.x_K_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				duplicated.x_K_g[g] = copyable.x_K_g[g];
				if (g == theG)
				{
					duplicated.x_K_g[g]--;
				}
			}
			duplicated.x_K = copyable.x_K -1;
			duplicated.isRoot = copyable.isRoot;

			for (int d = 0; d < data.General.Disciplines; d++)
			{
				duplicated.activeDisc[d] = copyable.activeDisc[d];
			}
			duplicated.tStage = copyable.tStage;
			duplicated.theSchedule_t = new int[data.General.TimePriods];
			for (int t = 0; t < data.General.TimePriods; t++)
			{
				duplicated.theSchedule_t[t] = copyable.theSchedule_t[t];
				
			}
			return duplicated;
		}

	}
}

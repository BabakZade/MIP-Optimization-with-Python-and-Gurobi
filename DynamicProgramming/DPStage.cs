using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedDynamicProgrammingAlgorithm
{
	public class DPStage
	{
		public DPStage parentNode;
		public AllData data;
		public int theIntern;
		public bool rootStage;
		public int stageTime;
		public DPStage() { }
		public ArrayList[] activeStatesValue;
		public ArrayList FutureActiveState;
		public int ActiveStatesCount;
		public DPStage(ref ArrayList finalSchedule,  AllData alldata, DPStage parent, int theI, int theTime)
		{
			data = alldata;
			theIntern = theI;
			stageTime = theTime;
			if (theTime==0)
			{				
				rootStage = true;
			}
			else
			{
				rootStage = false;
				parentNode = parent;
			}			

		}
		public void setStateStage(ref ArrayList finalSchedule)
		{
			if (rootStage)
			{
				// there are no state available 
				activeStatesValue = new ArrayList[1];
				activeStatesValue[0] = new ArrayList();
				// add dummy state
				activeStatesValue[0].Add(new StateStage());
				// add first wait value
				StateStage tmpWait = new StateStage();
				tmpWait.isRoot = rootStage;
				tmpWait.x_wait = true;
				activeStatesValue[0].Add(tmpWait);
				// rest of the values
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					for (int d = 0; d < data.General.Disciplines; d++)
					{
						if (!((StateStage)parentNode.FutureActiveState[0]).activeDisc[d])
						{
							for (int h = 0; h < data.General.Hospitals; h++)
							{
								for (int w = 0; w < data.General.HospitalWard; w++)
								{
									if (data.Hospital[h].Hospital_dw[d][w] && data.Hospital[h].HospitalMaxDem_tw[stageTime][w] > 0
										&& checkDiscipline(d, h, (StateStage)parentNode.FutureActiveState[0]))
									{
										StateStage tmp = new StateStage();
										tmp.isRoot = true;
										tmp.x_Hosp = h;
										tmp.x_Disc = d;
										activeStatesValue[0].Add(tmp);
									}
								}
							}
						}
					}
				}
			}
			else
			{
				for (int c = 0; c < parentNode.FutureActiveState.Count; c++)
				{
					if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0)
					{
						ActiveStatesCount++;
					}
				}
				// states and values
				// the first one in each row is the states
				// values related to the states comes afterward
				activeStatesValue = new ArrayList[ActiveStatesCount];
				int counter = -1;
				for (int c = 0; c < parentNode.FutureActiveState.Count; c++)
				{
					if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0)
					{
						counter++;
						activeStatesValue[counter] = new ArrayList();
						activeStatesValue[counter].Add(parentNode.FutureActiveState[c]);
						// add first wait value
						StateStage tmpWait = new StateStage();
						tmpWait.isRoot = false;
						tmpWait.x_wait = true;
						activeStatesValue[counter].Add(tmpWait);
						// rest of the values
						for (int g = 0; g < data.General.DisciplineGr; g++)
						{
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								if (!((StateStage)parentNode.FutureActiveState[c]).activeDisc[d])
								{
									for (int h = 0; h < data.General.Hospitals; h++)
									{
										for (int w = 0; w < data.General.HospitalWard; w++)
										{
											if (data.Hospital[h].Hospital_dw[d][w] && data.Hospital[h].HospitalMaxDem_tw[stageTime][w] > 0 
												&& checkDiscipline(d,h, (StateStage)parentNode.FutureActiveState[c]))
											{
												StateStage tmp = new StateStage();
												tmp.isRoot = false;
												tmp.x_Hosp = h;
												tmp.x_Disc = d;
												activeStatesValue[counter].Add(tmp);
											}
										}
									}
								}
							}
						}
					}
					else
					{
						// it is a complete  solution 
						int Fxcounter = 0;
						foreach (StateStage sol in finalSchedule)
						{
							if (((StateStage)parentNode.FutureActiveState[c]).Fx > sol.Fx)
							{
								break;
							}
							else
							{
								Fxcounter++;
							}
						}
						finalSchedule.Insert(Fxcounter,parentNode.FutureActiveState[c]);
					}

				}
			}
		}

		public void setFutureState()
		{
			FutureActiveState = new ArrayList();
			for (int c = 0; c < ActiveStatesCount; c++)
			{
				// 0 is the current state
				for (int i = 1; i < activeStatesValue[c].Count; i++)
				{
					StateStage tmp = new StateStage((StateStage)activeStatesValue[0][i], (StateStage)activeStatesValue[c][i], theIntern, data, stageTime);
					foreach (StateStage item in tmp.possibleStates)
					{
						FutureActiveState.Add(item);
					}					
				}
			}
		}

		public void DPStageProcedure(ref ArrayList finalSchedule)
		{
			setStateStage(ref finalSchedule);
			setFutureState();
		}
		public bool checkDiscipline(int theDisc,int theH, StateStage theState)
		{
			bool result = true;
			// already assigned or the time is filled
			if (theState.activeDisc[theDisc] || theState.theSchedule_t[stageTime]!=-1)
			{
				result = false;
			}
			// check oversea
			if (result)
			{
				for (int t = 0; t < data.General.TimePriods; t++)
				{
					if (data.Intern[theIntern].OverSea_dt[theDisc][t])
					{
						if (t != stageTime)
						{
							result = false;
							break;
						}
						else
						{
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								if (data.Intern[theIntern].FHRequirment_d[d] && !theState.activeDisc[d])
								{
									result = false;
									break;
								}
							}
						}
					}
				}
			}
			// check skills
			if (result && data.Discipline[theDisc].requiresSkill_p[data.Intern[theIntern].ProgramID])
			{
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					if (data.Discipline[theDisc].Skill4D_dp[d][data.Intern[theIntern].ProgramID] && !theState.activeDisc[d])
					{
						result = false;
						break;
					}
				}
			}
			// check availability
			if (result)
			{
				if (stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID] >= data.General.TimePriods)
				{
					result = false;
				}
				else
				{
					for (int t = stageTime; t < stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID]; t++)
					{
						if (!data.Intern[theIntern].Ave_t[t])
						{
							result = false;
							break;
						}
					}
				}
			}
			//check ability
			if (result)
			{
				if (!data.Intern[theIntern].Abi_dh[theDisc][theH])
				{
					result = false;
				}
			}
				

			return result;
		}
	}
}

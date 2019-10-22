using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace NestedDynamicProgrammingAlgorithm
{
	public class DPStage
	{
		public bool incombentExist;
		public DPStage parentNode;
		public AllData data;
		public int theIntern;
		public bool rootStage;
		public bool solutionFound;
		public int stageTime;
		public DPStage() { }
		public ArrayList[] activeStatesValue;
		public ArrayList FutureActiveState;
		public int ActiveStatesCount;
		public int[][][] MaxDem_twh;
		public int[][][] MinDem_twh;
		public int[][][] ResDem_twh;
		public int[][][] EmrDem_twh;
        public int[][] MaxDemYearly_wh;
        public int[][] MinDemYearly_wh;
        public int[][] AccDem_tr;
		bool flagRes;
		bool flagEmr;
		public int MaxProcessedNode;
		public int RealProcessedNode;
		public DPStage(ref StateStage finalSchedule, AllData alldata, DPStage parent, int theI, int theTime, bool isRoot, int[][][] MaxDem_twh, int[][][] MinDem_twh, int[][] MaxDemYearly_wh, int[][] MinDemYearly_wh, int[][][] ResDem_twh, int[][][] EmrDem_twh, int[][] AccDem_tr, bool incombentExist)
		{
			this.incombentExist = incombentExist;
			this.MaxDem_twh = MaxDem_twh;
			this.MinDem_twh = MinDem_twh;
			this.ResDem_twh = ResDem_twh;
			this.EmrDem_twh = EmrDem_twh;
			this.AccDem_tr = AccDem_tr;
            this.MaxDemYearly_wh = MaxDemYearly_wh;
            this.MinDemYearly_wh = MinDemYearly_wh;
			data = alldata;
			theIntern = theI;
			stageTime = theTime;
			Initial(alldata);
			FutureActiveState = new ArrayList();
			//if (!data.Intern[theI].Ave_t[theTime])
			//{
			//	FutureActiveState = parent.FutureActiveState;
			//}
			if (isRoot)
			{
				rootStage = true;
			}
			else
			{
				rootStage = false;
				parentNode = parent;
			}
			DPStageProcedure(ref finalSchedule);

		}

		public void Initial(AllData alldata)
		{

		}
		public void setStateStage(ref StateStage finalSchedule)
		{
			solutionFound = false;
			if (rootStage)
			{
				// there are no state available 
				activeStatesValue = new ArrayList[1];
				activeStatesValue[0] = new ArrayList();
				ActiveStatesCount = 1;
				// add dummy state
				StateStage tmpdummy = new StateStage(data);
				activeStatesValue[0].Add(tmpdummy);


				// rest of the values
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					for (int h = 0; h < data.General.Hospitals + 1; h++)
					{
						for (int w = 0; w < data.General.HospitalWard; w++)
						{

							if (h < data.General.Hospitals && data.Hospital[h].Hospital_dw[d][w] && data.Hospital[h].HospitalMaxDem_tw[stageTime][w] > 0
								&& checkDiscipline(d, h, new StateStage(data) { }))
							{
								StateStage tmp = new StateStage(data);
								tmp.isRoot = true;
								tmp.x_Hosp = h;
								tmp.x_Disc = d;
								tmp.flagEmrD = flagEmr;
								tmp.flagResD = flagRes;
								activeStatesValue[0].Add(tmp);
							}
							if (h == data.General.Hospitals	&& checkDiscipline(d, h, new StateStage(data) { }))
							{
								StateStage tmp = new StateStage(data);
								tmp.isRoot = true;
								tmp.x_Hosp = h;
								tmp.x_Disc = d;
								tmp.flagEmrD = flagEmr;
								tmp.flagResD = flagRes;
								activeStatesValue[0].Add(tmp);
							}


						}
					}
				}

				// if no one added to fututre state add wait
				if (activeStatesValue[0].Count == 1)
				{
					// wait 
					StateStage tmpwait = new StateStage(data);
					tmpwait.isRoot = true;
					tmpwait.x_wait = true;
					activeStatesValue[0].Add(tmpwait);
				}
			}
			else
			{
				for (int c = 0; c < parentNode.FutureActiveState.Count; c++)
				{
					if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0 && ((StateStage)parentNode.FutureActiveState[c]).theSchedule_t[stageTime].theDiscipline == -1)
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
					if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0 && ((StateStage)parentNode.FutureActiveState[c]).theSchedule_t[stageTime].theDiscipline != -1)
					{
						FutureActiveState.Add((StateStage)parentNode.FutureActiveState[c]);
						((StateStage)FutureActiveState[FutureActiveState.Count - 1]).tStage++;
						continue;


					}
					if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0)
					{
						counter++;
						activeStatesValue[counter] = new ArrayList();
						//first add itself 
						activeStatesValue[counter].Add(parentNode.FutureActiveState[c]);

						// if you have to change every time choose the best one each time
						for (int d = 0; d < data.General.Disciplines; d++)
						{
							if (!((StateStage)parentNode.FutureActiveState[c]).activeDisc[d])
							{
								for (int h = 0; h < data.General.Hospitals + 1; h++)
								{
									for (int w = 0; w < data.General.HospitalWard; w++)
									{
										if (h < data.General.Hospitals && data.Hospital[h].Hospital_dw[d][w] && data.Hospital[h].HospitalMaxDem_tw[stageTime][w] > 0
											&& checkDiscipline(d, h, (StateStage)parentNode.FutureActiveState[c]))
										{
											StateStage tmp = new StateStage(data);
											tmp.isRoot = false;
											tmp.x_Hosp = h;
											tmp.x_Disc = d;

											tmp.flagEmrD = flagEmr;
											tmp.flagResD = flagRes;
											activeStatesValue[counter].Add(tmp);
										}
										if (h == data.General.Hospitals && checkDiscipline(d, h, (StateStage)parentNode.FutureActiveState[c]))
										{
											StateStage tmp = new StateStage(data);
											tmp.isRoot = false;
											tmp.x_Hosp = h;
											tmp.x_Disc = d;

											tmp.flagEmrD = flagEmr;
											tmp.flagResD = flagRes;
											activeStatesValue[counter].Add(tmp);
											break; // there is no ward checking 
										}
									}
								}
							}
						}
						// if there was no future state add wait and intern is available
						if (activeStatesValue[counter].Count == 1 && data.Intern[theIntern].AveDur * ((StateStage)activeStatesValue[counter][0]).x_K < data.General.TimePriods - stageTime)
						{
							// the wait 
							StateStage tmpwait = new StateStage(data);
							tmpwait.x_wait = true;
							activeStatesValue[counter].Add(tmpwait);
						}
					}
					else
					{
						// it is a complete  solution 
						// we need the best one 						
						
						if (((StateStage)parentNode.FutureActiveState[c]).Fx > finalSchedule.Fx )
						{
							finalSchedule = (StateStage)parentNode.FutureActiveState[c];
							solutionFound = true;
							//break;
						}
					}

				}
			}

		}

		public bool NotOptiamalAndCutTheBranches(double FinalObj, StateStage curentStat)
		{
			bool result = false;
			double obj = curentStat.Fx;
			if (curentStat.x_K > 0)
			{
				bool[] disStatus = new bool[data.General.Disciplines];
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					disStatus[d] = false;
				}
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					if (curentStat.x_K_g[g] > 0)
					{
						int length = curentStat.x_K_g[g];
						

						foreach (DataLayer.DesirePos item in data.Intern[theIntern].sortedPrf)
						{
							int theH = item.theH;
							bool itCanbeIntheHosp = true;
							if (data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp == 1)
							{
								for (int t = 0; t < data.General.TimePriods; t++)
								{
									if (curentStat.theSchedule_t[t].theHospital == theH)
									{
										itCanbeIntheHosp = false;
										break;
									}
								}
							}

							if (!itCanbeIntheHosp)
							{
								continue;
							}

							int theD = item.theD;
							if (!disStatus[theD] && !curentStat.activeDisc[theD])
							{
								obj += item.AbsDesire * (data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi);
								disStatus[theD] = true;
								length--;
							}
							else
							{
								continue;
							}

							if (length == 0)
							{
								break;
							}
						}
					}
				}
				
			}

			if (obj <= FinalObj)
			{
				result = true;
			}
			return result;

		}

		public void setFutureState()
		{
			// Console.WriteLine("====== Stage " + stageTime + " ======");
			for (int c = 0; c < ActiveStatesCount; c++)
			{
				// Console.WriteLine("*** " + ((StateStage)activeStatesValue[c][0]).x_Hosp + " " + ((StateStage)activeStatesValue[c][0]).x_Disc + " " + ((StateStage)activeStatesValue[c][0]).x_K + " " + ((StateStage)activeStatesValue[c][0]).x_wait + " " + ((StateStage)activeStatesValue[c][0]).Fx);
				// 0 is the current state
				for (int i = 1; i < activeStatesValue[c].Count; i++)
				{
					StateStage tmp = new StateStage((StateStage)activeStatesValue[c][0], (StateStage)activeStatesValue[c][i], theIntern, data, stageTime, rootStage);
					foreach (StateStage item in tmp.possibleStates)
					{
						item.Fx -= returnDemandCost(item.x_Disc, item.x_Hosp, stageTime, item.x_wait);

						// Console.WriteLine(item.x_Hosp + " " + item.x_Disc + " " + item.x_K + " " + item.x_wait + " " + item.Fx);
						FutureActiveState.Add(item);
						//int index = 0;
						//foreach (StateStage futur in FutureActiveState)
						//{
							
						//	if (futur.Fx / (data.Intern[theIntern].K_AllDiscipline - futur.x_K)  > item.Fx / (data.Intern[theIntern].K_AllDiscipline - item.x_K))
						//	{
						//		index++;
						//	}
						//	else
						//	{
						//		break;
						//	}
						//} 
						//FutureActiveState.Insert(index, item);
						
					}
				}
			}
			cleanTheActiveState();
		}

		public double returnDemandCost(int theD, int theH, int theT, bool waitingLable)
		{
			double result = 0;
			if (theH == data.General.Hospitals)
			{
				result = -data.AlgSettings.BigM;
				return result;
			}
			if (waitingLable || theD < 0 )
			{
				result = -data.AlgSettings.BigM;
				return result;
			}
            

            for (int w = 0; w < data.General.HospitalWard; w++)
			{
				if (data.Hospital[theH].Hospital_dw[theD][w]) 
				{
					for (int t = theT; t < theT + data.Discipline[theD].Duration_p[data.Intern[theIntern].ProgramID]; t++)
					{
						if (MaxDem_twh[t][w][theH] > 0)
						{
							result += 0;
						}
						else if (ResDem_twh[t][w][theH] > 0)
						{
							for (int p = 0; p < data.General.TrainingPr; p++)
							{
								result += data.TrainingPr[p].CoeffObj_ResCap;
							}
						}
						else if (EmrDem_twh[t][w][theH] > 0)
						{
							for (int p = 0; p < data.General.TrainingPr; p++)
							{
								result += data.TrainingPr[p].CoeffObj_EmrCap;
							}
						}
						if (MinDem_twh[t][w][theH] > 0)
						{
							for (int p = 0; p < data.General.TrainingPr; p++)
							{
								result -= data.TrainingPr[p].CoeffObj_MINDem;
							}
						}
					}
					break;
				}
			}

			for (int r = 0; r < data.General.Region; r++)
			{
				if (data.Intern[theIntern].TransferredTo_r[r] && data.Hospital[theH].InToRegion_r[r])
				{
					for (int t = theT; t < theT + data.Discipline[theD].Duration_p[data.Intern[theIntern].ProgramID]; t++)
					{
						if (AccDem_tr[t][r] > 0)
						{
							for (int p = 0; p < data.General.TrainingPr; p++)
							{
								result -= data.TrainingPr[p].CoeffObj_NotUsedAcc;
							}
						}
					}
				}
			}


			return result;
		}

		public void cleanTheActiveState()
		{
			//chooseBestHospIfChangeIsNecessary(); // we will face shorage in rare discipline and vital	
		MaxProcessedNode = FutureActiveState.Count;
		
		    Console.WriteLine("Before one disc: " + FutureActiveState.Count);
			// it will not work with unstable demand structure => at t we are in regular demand, t+1 it is reserved demand
			keepOneDisci(); 
			Console.WriteLine("After one disc: " + FutureActiveState.Count);
			cleanRedundantSequence();
			Console.WriteLine("After cleaning redundant node: " + FutureActiveState.Count);
			cutTheBranch();
			Console.WriteLine("After cutting not promissing nodes: " + FutureActiveState.Count);

			Console.WriteLine();
			Console.WriteLine();

			limittedFutureList();
			RealProcessedNode = FutureActiveState.Count;

		}
		//Necessary 
		public void chooseBestHospIfChangeIsNecessary()
		{
			// if you have to change it every time choose the best every time for each discipline
			if (data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp == 1)
			{
				ArrayList futurelist = new ArrayList();
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					if (data.Discipline[d].isRare)
					{
						continue;
					}
					for (int g = 0; g < data.General.DisciplineGr; g++)
					{
						double x = -data.AlgSettings.BigM;
						int maxIndex = -1;
						for (int c = 0; c < FutureActiveState.Count; c++)
						{
							if (d == ((StateStage)FutureActiveState[c]).x_Disc && g == ((StateStage)FutureActiveState[c]).x_group)
							{
								if (x < ((StateStage)FutureActiveState[c]).Fx)
								{
									x = ((StateStage)FutureActiveState[c]).Fx;
									if (maxIndex >= 0)
									{
										FutureActiveState.RemoveAt(maxIndex);
										c--;
									}
									maxIndex = c;
								}
								else
								{
									FutureActiveState.RemoveAt(c);
									c--;
								}
							}
						}


					}

				}
			}
		}

		// redundant schedule d1h1 - d2h2 - d3h3 === d2h2 - d1h1 -d3h3 (last one is fixed)
		public void cleanRedundantSequence()
		{
			int counter = -1;
			while (counter < FutureActiveState.Count-1 && counter >= - 1)
			{
				counter++;
				bool removeIT = false;
				bool firstLoopUpdate = false;
				StateStage current = (StateStage)FutureActiveState[counter];
				int length = data.Intern[theIntern].K_AllDiscipline - current.x_K;
				if (length < 2) 
				{
					continue;
				}
				for (int l = 2; l <= length; l++) 
				{
					int[] theH = new int[l];
					
					int[] theD = new int[l];
					bool[] theHStatus = new bool[data.General.Hospitals + 1];
					for (int h = 0; h < data.General.Hospitals + 1; h++)
					{
						theHStatus[h] = false;
					}
					int startTime = -1;
					int lastoneStart = -1;
					for (int ll = 0; ll < l; ll++)
					{
						theD[ll] = -1;
						theH[ll] = -1;
					}
					int lengCounter = 0;
					int nextT = 0;
					
					for (int t = nextT; t <= stageTime; t++)
					{
						bool noSeq = false;
						if (lengCounter == l)
						{
							break;
						}
						if (current.theSchedule_t[t].theHospital > -1)
						{
							if (theD[0] ==-1)
							{
								startTime = t;
								lengCounter = 0;
							}
							
							for (int ll = 0; ll < l; ll++)
							{
								if (current.theSchedule_t[t].theDiscipline == -1)
								{
									t++;
									ll--;
									
									if (t > stageTime)
									{
										noSeq = true;
										break;
									}
									else
									{
										continue;
									}
								}
								if (theD[ll] == -1)
								{
									lengCounter++;
									theD[ll] = current.theSchedule_t[t].theDiscipline;
									theH[ll] = current.theSchedule_t[t].theHospital;

									theHStatus[current.theSchedule_t[t].theHospital] = true;
									lastoneStart = t;
									t += data.Discipline[current.theSchedule_t[t].theDiscipline].Duration_p[data.Intern[theIntern].ProgramID];						
									
								}
							}
						}
						else
						{
							t++;
							continue;
						}
						if (noSeq)
						{
							break;
						}
						if (lengCounter == l)
						{
							nextT += data.Discipline[theD[0]].Duration_p[data.Intern[theIntern].ProgramID];
							t = nextT - 1 ;
						}

						// now check the sequence 
						int removeCounter = counter - 1;
						int maxCandidate = -1;
						double maxVal = -data.AlgSettings.BigM;

						while (removeCounter < FutureActiveState.Count - 1 && removeCounter >= -1)
						{
							removeCounter++;
							StateStage state = (StateStage)FutureActiveState[removeCounter];
							bool notSimmilar = false;
							for (int g = 0; g < data.General.DisciplineGr; g++)
							{
								if (state.x_K_g[g] != current.x_K_g[g])
								{
									notSimmilar = true;
									break;
								}
							}
							if (notSimmilar)
							{
								continue;
							}
							// we need to fix last one and check the sequence before
							int starttmp = startTime;
							bool remove = false;
							if (state.theSchedule_t[lastoneStart].theDiscipline == theD[l-1]  && state.theSchedule_t[lastoneStart].theHospital == theH[l - 1])
							{
								for (int ll = 0; ll < l - 1 ; ll++)
								{
									int theD1 = state.theSchedule_t[starttmp].theDiscipline;
									int theH1 = state.theSchedule_t[starttmp].theHospital;
									if (theD1 == -1)
									{
										starttmp++;
										if (starttmp > stageTime)
										{
											break;
										}
										else
										{
											ll--;
											continue;
										}
									}
									starttmp += data.Discipline[theD1].Duration_p[data.Intern[theIntern].ProgramID];
									remove = false;
								
									for (int lch = 0; lch < l - 1; lch++)
									{
										if (theD1 == theD[lch] )
										{
											remove = true;
											break;
										}
									}
									if (!remove)
									{
										break;
									}
									remove = false;
									//  hospitals must be from the already assigned hospitals 
									// if we already used d1h1 and d2h2
									// if we have d1h1 and  d2h1 we will remove it => we used a hospital twice 
									//if we have d1h2 and  d2h1 we will remove it => we used a used hospital with differnt order 
									for (int lch = 0; lch < l - 1; lch++)
									{
										if (theH1 == theH[lch]) 
										{
											remove = true;
											break;
										}
									}
									if (!remove && data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp == 1)
									{
										break;
									}
								}
							}
							
							if (remove)
							{
								if (state.Fx > maxVal)
								{
									maxVal = state.Fx;
									// delete ex max candidate
									if (maxCandidate >= 0)
									{
										FutureActiveState.RemoveAt(maxCandidate);
										firstLoopUpdate = true;
										//update max
										maxCandidate = removeCounter - 1; // because one is removed
										//update counter
										removeCounter--;
									}
									else
									{
										maxCandidate = removeCounter;
									}									
									
								}
								else
								{
									FutureActiveState.RemoveAt(removeCounter);
									removeCounter--;
								}
							}							
						} 
					}

					if (firstLoopUpdate)
					{
						counter--;
					}
					if (counter < -1)
					{
						counter = -1;
					}
				}
				
			}
		}


		// curent prf + Max praf <=  current objective function
		// cut the branch
		public void cutTheBranch()
		{
			int counter = -1;
			int p = data.Intern[theIntern].ProgramID;
		
			while (counter < FutureActiveState.Count - 1)
			{
				counter++;
				StateStage state = (StateStage)FutureActiveState[counter];
				int theD = state.theSchedule_t[stageTime].theDiscipline;
				bool cango = false;
				// oversea skill and availbility is already checked 
				if (state.x_K != 0)
				{
					continue;
				}


				// remove all other discipline
				bool flagFirst = true;
				int removeCounter = -1;
				while (removeCounter < FutureActiveState.Count - 1)
				{

					removeCounter++;
					StateStage current = (StateStage)FutureActiveState[removeCounter];
					if (NotOptiamalAndCutTheBranches(state.Fx, current))
					{
						if (flagFirst && state.Fx == current.Fx && current.x_K == 0)
						{
							flagFirst = false;
							continue;
						}
						FutureActiveState.RemoveAt(removeCounter);
						removeCounter--;
					}
				}
			}
		}

		// keep only one discipline 
		public void keepOneDisci()
		{
			//if (data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp > 1)
			//{
			//	return;
			//}
			int counter = -1;
			int p = data.Intern[theIntern].ProgramID;
			bool[] discStatus = new bool[data.General.Disciplines];
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				discStatus[d] = false;
			}
			while (counter < FutureActiveState.Count - 1)
			{
				counter++;
				StateStage state = (StateStage)FutureActiveState[counter];
				int theD = state.theSchedule_t[stageTime].theDiscipline;
				bool cango = false;
				// oversea skill and availbility is already checked 
				if (theD < 0 || discStatus[theD])
				{
					continue;
				}
				else 
				{
					discStatus[theD] = true;
				}
				
				
				// remove all other discipline
				int removeCounter = -1;
				while (removeCounter < FutureActiveState.Count - 1)
				{
					
					removeCounter++;
					StateStage current = (StateStage)FutureActiveState[removeCounter];

					
					int theDtmp = current.theSchedule_t[stageTime].theDiscipline;
					if (theDtmp < 0 || current.x_Hosp == data.General.Hospitals)
					{
						continue;
					}
					if (stageTime > 0)
					{
						if (current.theSchedule_t[stageTime-1].theDiscipline == theDtmp
							|| state.theSchedule_t[stageTime - 1].theDiscipline == theD)
						{
							continue;
						}
					}
					// check oversea otherwise the time is not matter 

					if (data.Discipline[theD].Duration_p[p] != data.Discipline[theDtmp].Duration_p[p])
					{
						continue;
					}

					double prfX1 = data.Intern[theIntern].wieght_d * data.Intern[theIntern].Prf_d[theD] + data.TrainingPr[p].weight_p * data.TrainingPr[p].Prf_d[theD] + data.Intern[theIntern].wieght_cns * maxConsecutiveness(theD, p);
					double prfX2 = data.Intern[theIntern].wieght_d * data.Intern[theIntern].Prf_d[theDtmp] + data.TrainingPr[p].weight_p * data.TrainingPr[p].Prf_d[theDtmp] + data.Intern[theIntern].wieght_cns * maxConsecutiveness(theDtmp, p);

					double demCost = returnDemandCost(state.x_Disc, state.x_Hosp, stageTime, state.x_wait);
					double tmpdemCost = returnDemandCost(current.x_Disc, current.x_Hosp, stageTime, current.x_wait);

					double obj1 = -demCost + prfX1 * (data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi );
					double obj2 = -tmpdemCost + prfX2 * (data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi );
					if (obj2 > obj1)
					{
						continue;
					}
					if (data.Intern[theIntern].takingDiscPercentage[theD] < 0.95)
					{
						if (data.Discipline[theD].Duration_p[p] == data.Discipline[theDtmp].Duration_p[p]
							&& !data.Discipline[theD].isRare 
						/*&& data.Intern[theIntern].takingDiscPercentage[theD] >= data.Intern[theIntern].takingDiscPercentage[theDtmp]*/)
						{
							// remove x2
						}
						else
						{
							continue;
						}
					}

                    // capacity control

                    double XX = 0;
                    for (int w = 0; w < data.General.HospitalWard; w++)
                    {
                        if (!current.x_wait && data.Hospital[current.x_Hosp].Hospital_dw[current.x_Disc][w])
                        {
                            for (int t = stageTime; t < data.General.TimePriods; t++)
                            {
                                if (MaxDem_twh[t][w][current.x_Hosp] > 0)
                                {
                                    XX++;
                                }
                            }
                        }
                    }
                    if (data.General.TimePriods - stageTime > 0 && XX / (data.General.TimePriods - stageTime) < 0.75) // if you can manage it to schedule it somewhere else 
                    {
                        continue;
                    }


                    if (theDtmp >= 0 && theDtmp != theD  
						&& !data.Intern[theIntern].FHRequirment_d[theDtmp] && !data.Discipline[theDtmp].requiredLater_p[data.Intern[theIntern].ProgramID])
					{
						FutureActiveState.RemoveAt(removeCounter);
						removeCounter--;
					}
				}
			}
		}


		//remove more than a number
		public void limittedFutureList()
		{
			int limit = data.General.Disciplines * data.General.Disciplines * data.General.Hospitals * data.General.Hospitals;
			//foreach (StateStage item in FutureActiveState)
			//{
			//	Console.WriteLine(item.DisplayMe());
			//}
			if (FutureActiveState.Count > limit)
			{
				Console.WriteLine("Total active: " + FutureActiveState.Count + " Limit: " + limit + " => Warning");
				//foreach (StateStage item in FutureActiveState)
				//{
				//	Console.WriteLine(item.DisplayMe());
				//}
				//FutureActiveState.RemoveRange(limit, FutureActiveState.Count - limit);
			}

		}

		public void DPStageProcedure(ref StateStage finalSchedule)
		{
			setStateStage(ref finalSchedule);
			if (solutionFound)
			{
				return;
			}
			setFutureState();
		}

		public bool checkDiscipline(int theDisc, int theH, StateStage theState)
		{			
			flagEmr = false;
			flagRes = false;
			bool result = true;
			// already assigned or the time is filled
			if (theState.activeDisc[theDisc] || theState.theSchedule_t[stageTime].theDiscipline!=-1 
				|| stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID] >= data.General.TimePriods)
			{
				result = false;
			}

			int thep = -1;
			for (int p = 0; p < data.General.TrainingPr && thep < 0; p++)
			{
				for (int h = 0; h < data.General.Hospitals && thep < 0; h++)
				{
					if (data.Intern[theIntern].Fulfilled_dhp[theDisc][h][p])
					{
						result = false;
					}
				}
			}
			// if the intern needs it 
			if (result)
			{
				result = false;
				for (int g = 0; g < data.General.DisciplineGr; g++)
				{
					if (data.Intern[theIntern].DisciplineList_dg[theDisc][g])
					{
						result = true;
						break;
					}
				}
			}

			// check oversea
			bool overseaNotexist = true;
			if (result)
			{
				for (int t = 0; t < data.General.TimePriods && result; t++)
				{
					// if this discipline is over sea
					if (data.Intern[theIntern].OverSea_dt[theDisc][t])
					{
						if (t != stageTime)
						{
							result = false;
							break;
						}
						else
						{
							if (theH != data.General.Hospitals)
							{
								result = false;
								break;
							}
							for (int d = 0; d < data.General.Disciplines; d++)
							{
								if (data.Intern[theIntern].FHRequirment_d[d] && !theState.activeDisc[d])
								{
									result = false;
									break;
								}
							}
							overseaNotexist = false;
							
						}					
					}
					// if there is other discipline for oversea
					for (int dd = 0; dd < data.General.Disciplines && result; dd++)
					{
						if (result && dd != theDisc
							&& data.Intern[theIntern].OverSea_dt[dd][t] && stageTime == t)
						{
							result = false;
							break;
						}

					}
					// remain time
					for (int dd = 0; dd < data.General.Disciplines && result; dd++)
					{
						if (result && dd != theDisc
							&& data.Intern[theIntern].OverSea_dt[dd][t] && stageTime < t
							&& stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID] > t)
						{
							result = false;
							break;
						}

					}

					// requirment 
					for (int dd = 0; dd < data.General.Disciplines && result; dd++)
					{
						if (result && data.Intern[theIntern].OverSea_dt[dd][t] && stageTime >= t
							&& data.Intern[theIntern].FHRequirment_d[theDisc])
						{
							result = false;
							break;
						}

					}
				}
				if (overseaNotexist && theH == data.General.Hospitals)
				{
					return false;
				}
				else if (theH == data.General.Hospitals)
				{
					return result;
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
			
			//check demand
			if (result)
			{
				result = false;
				
				// check capacity 
				
				for (int w = 0; w < data.General.HospitalWard; w++)
				{
					bool capLable = true;
					
					if (data.Hospital[theH].Hospital_dw[theDisc][w])
					{
						for (int t = stageTime; t < stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID]; t++)
						{
							if (MaxDem_twh[t][w][theH] + ResDem_twh[t][w][theH] + EmrDem_twh[t][w][theH] <= 0 || MaxDemYearly_wh[w][theH] <= 0)
							{
								capLable = false;
								break;
							}
						}
						if (!capLable)
						{
							continue;
						}
						else
						{
							result = true;
						}
						if (result && MaxDem_twh[stageTime][w][theH] > 0)
						{
							result = true;
							break;
						}
						else if(result && ResDem_twh[stageTime][w][theH] > 0)
						{
							result = true;
							flagRes = true;
							break;
						}
						else if (result && EmrDem_twh[stageTime][w][theH] > 0)
						{
							result = true;
							flagEmr = true;
							break;
						}
					}
				}
				// check minimum => we do not check minimum demand
				if (result && false)
				{
					bool goCheck = true;
					// if we need someone in this discipline
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						if (data.Hospital[theH].Hospital_dw[theDisc][w])
						{
							if (MinDem_twh[stageTime][w][theH] > 0)
							{
								goCheck = false;
							}
						}
					}
					// if this discipline does not have min demand check if we need this intern somewhere else 
					for (int d = 0; d < data.General.Disciplines && result && goCheck; d++)
					{
						for (int h = 0; h < data.General.Hospitals && result; h++)
						{
							for (int w = 0; w < data.General.HospitalWard && result; w++)
							{
								if (theH!=h && theDisc!=d && data.Hospital[h].Hospital_dw[d][w])
								{
									if (MinDem_twh[stageTime][w][h]>0 && incombentExist)
									{
										result = false;
									}
								}
							}
						}
					}
				}
			}

			// if there is no time to compelete the 
			if (result)
			{
				if (theState.x_K > data.General.TimePriods - theState.tStage)
				{
					result = false;
				}
			}

			// cut ruls
			// 1- total number of discipline in one hospital
			if (result)
			{
				int totaldisc = 0;
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					if (theState.activeDisc[d])
					{
						for (int t = 0; t < data.General.TimePriods; t++)
						{
							if (theState.theSchedule_t[t].theHospital == theH)
							{
								totaldisc++;
								break;
							}
						}
					}
				}
				if (totaldisc >= data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp)
				{
					result = false;
				}
			}

			// if it is here it means that he can attend to this hospital
			// 2- same hospital next together (if it is possible)	
			if (result && false)
			{
				result = differentDiscSameHospital(theDisc, theH, theState);
			}

			// if the consecutive disciplines happens to same hospital, they should follow chronological order 
			// 3- same hospital and consecutive time, the discipline should be chronological
			if (result)
			{
				result = chronologicalConsecutiveDiscSameHospital(theDisc, theH, theState);
			}
			return result;
		}

		public bool differentDiscSameHospital(int theDisc, int theH, StateStage theState)
		{
			bool result = true;
			if (rootStage)
			{
				return result;
			}
			if (theState.theSchedule_t[stageTime-1].theHospital == theH)
			{
				return result;
			}
			// if it is here it means that he can attend to this hospital
			// 2- same hospital next together (if it is possible)			
			for (int t = 0; t < stageTime; t++)
			{
				if (theState.theSchedule_t[t].theHospital == theH)
				{
					// find were the change in hospitals happens
					int theTT = t;
					for (int tt = t + 1; tt < stageTime && tt < data.General.TimePriods; tt++)
					{
						if (theState.theSchedule_t[tt].theHospital != theState.theSchedule_t[t].theHospital)
						{
							theTT = tt;
						}
					}
					if (theTT < stageTime - 1) // there is at least one space 
					{
						// check if you do not need any skills of the discipline in between
						if (data.Discipline[theDisc].requiresSkill_p[data.Intern[theIntern].ProgramID])
						{
							result = false;
							break;
						}
						else
						{
							// see till theTT the required skills are there
							bool itCanMove = true;
							for (int d = 0; d < data.General.Disciplines && itCanMove; d++)
							{
								if (data.Discipline[theDisc].Skill4D_dp[d][data.Intern[theIntern].ProgramID])
								{
									itCanMove = false;
									// check if it is not exist before theTT
									for (int tc = 0; tc < theTT; tc++)
									{
										if (theState.theSchedule_t[tc].theDiscipline == d)
										{
											itCanMove = true;
											break;
										}
									}
								}
							}

							if (itCanMove)
							{
								result = false;
								break;
							}
						}
					}

				}
			}

			return result;
		}

		public bool chronologicalConsecutiveDiscSameHospital(int theDisc, int theH, StateStage theState)
		{
			bool result = true;

			// if the consecutive disciplines happens to same hospital, they should follow chronological order 
			// 3- same hospital and consecutive time, the discipline should be chronological			
			if (!rootStage)
			{
				if (theState.theSchedule_t[stageTime-1].theHospital == theH && theDisc < theState.theSchedule_t[stageTime - 1].theDiscipline)
				{
					// see if there are capacity
					bool isTherecapacity = true;
					for (int t = 0; t < stageTime; t++)
					{
						if (theState.theSchedule_t[t].theDiscipline == theState.theSchedule_t[stageTime - 1].theDiscipline)
						{
							for (int tt = t; tt < stageTime && isTherecapacity; tt++)
							{
								for (int w = 0; w < data.General.HospitalWard && isTherecapacity; w++)
								{
									if (data.Hospital[theH].Hospital_dw[theDisc][w] && MaxDem_twh[tt][w][theH] == 0)
									{
										isTherecapacity = false;
									}
								}
							}
							break;
						}
						
					}
					if (isTherecapacity && !data.Discipline[theDisc].Skill4D_dp[theState.theSchedule_t[stageTime - 1].theDiscipline][data.Intern[theIntern].ProgramID])
					{
						result = false;
					}
				}
			}

			return result;
		}

		public bool differentConsecutiveHospital(int theDisc, int theH, StateStage theState)
		{
			bool result = true;
			if (!rootStage && theState.theSchedule_t[stageTime - 1].theHospital<=0)
			{
				return result;
			}
			if ((stageTime>0 && theState.theSchedule_t[stageTime - 1].theHospital == data.General.Hospitals) || theH == data.General.Hospitals)
			{
				return result;
			}
			// if the consecutive disciplines happens in different hospital, the second hospital must worth it 			
			if (!rootStage)
			{
				// first see if he can attend to same hospital
				int totaldisc = 0;
				for (int d = 0; d < data.General.Disciplines; d++)
				{
					if (theState.activeDisc[d])
					{
						for (int t = 0; t < data.General.TimePriods; t++)
						{
							if (theState.theSchedule_t[t].theHospital == theState.theSchedule_t[stageTime - 1].theHospital)
							{
								totaldisc++;
								break;
							}
						}
					}
				}
				// +1 because we want to assign more
				if (totaldisc + 1 > data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp)
				{
					return result;
				}

				// if he can go the previous hospital let him
				if (theState.theSchedule_t[stageTime - 1].theHospital != theH
					&& data.Intern[theIntern].Prf_h[theState.theSchedule_t[stageTime - 1].theHospital] >= data.Intern[theIntern].Prf_h[theH])
				{
					//see if the previous hospital has the discipline
					bool preH = false;
					for (int w = 0; w < data.General.HospitalWard; w++)
					{
						if (data.Hospital[theState.theSchedule_t[stageTime - 1].theHospital].Hospital_dw[theDisc][w] && MaxDem_twh[stageTime][w][theState.theSchedule_t[stageTime - 1].theHospital] > 0)
						{
							preH = true;
							break;
						}
					}
					if (preH )
					{
						result = false;
					}
				}
			}

			return result;
		}

		public bool bestHospitalForThisDiscipline(int theDisc, int theH, StateStage theState)
		{
			bool result = true;
			int hIndex = -1;
			int maxPrf = 0;
			for (int h = 0; h < data.General.Hospitals; h++)
			{
				if (data.Intern[theIntern].Prf_h[h] > maxPrf)
				{
					hIndex = h;
					maxPrf = data.Intern[theIntern].Prf_h[h];
				}
			}
			if (data.TrainingPr[data.Intern[theIntern].ProgramID].DiscChangeInOneHosp == 1)
			{

				for (int t = 0; t < data.General.TimePriods; t++)
				{
					if (theState.theSchedule_t[t].theHospital == hIndex)
					{
						result = false;
						break;
					}
				}
				
			}

			// if he was in hospital and you wanna assign a discipline assign it to previous one (which you already considered a state for that no need to consider this)
			if (result)
			{
				for (int t = 0; t < stageTime && result; t++)
				{
					if (theState.theSchedule_t[t].theHospital >=0 && data.Intern[theIntern].Prf_h[theState.theSchedule_t[t].theHospital] >= maxPrf)
					{
						result = false;
					}
				}
			}

			return result;
		}

        public int maxConsecutiveness(int theD, int theP)
        {
            int result = 0;
            for (int dd = 0; dd < data.General.Disciplines; dd++)
            {
                if (data.TrainingPr[theP].cns_dD[theD][dd] > result)
                {
                    result = data.TrainingPr[theP].cns_dD[theD][dd];
                }
            }

            return result;
        }
	}
}

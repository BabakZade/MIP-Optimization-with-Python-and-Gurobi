using System;
using System.Collections;
using System.Text;
using DataLayer;

namespace SubProblemDP
{
    public class DPStage
    {
        public DPStage parentNode;
        public AllData data;
        public int theIntern;
        public bool rootStage;
        public bool solutionFound;
        public int stageTime;
        public double[][][] RCStart_tdh;
        public double[][][] RCDual_tdh;
        public double RCPi2;
        public double RCDes;
        public DPStage() { }
        public ArrayList[] activeStatesValue;
        public ArrayList FutureActiveState;
        public int ActiveStatesCount;
        bool isHeuristic;
        public int MaxProcessedNode;
        public int RealProcessedNode;
        public DPStage(ref ArrayList finalSchedules, ArrayList AllBranches, double[] dual, AllData alldata, DPStage parent, int theI, int theTime, bool isRoot, bool isHeuristic)
        {
            this.isHeuristic = isHeuristic;
            data = alldata;
            theIntern = theI;
            stageTime = theTime;
            Initial(alldata);
            setRCStart(dual);
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
            DPStageProcedure(ref finalSchedules, AllBranches);

        }

        public void Initial(AllData alldata)
        {

        }
        public void setStateStage(ref ArrayList finalSchedules, ArrayList AllBranches)
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
                                && checkDiscipline(d, h, AllBranches, new StateStage(data) { }))
                            {
                                StateStage tmp = new StateStage(data);
                                tmp.isRoot = true;
                                tmp.x_Hosp = h;
                                tmp.x_Disc = d;

                                activeStatesValue[0].Add(tmp);
                            }
                            if (h == data.General.Hospitals && checkDiscipline(d, h, AllBranches, new StateStage(data) { }))
                            {
                                StateStage tmp = new StateStage(data);
                                tmp.isRoot = true;
                                tmp.x_Hosp = h;
                                tmp.x_Disc = d;
                                activeStatesValue[0].Add(tmp);
                            }


                        }
                    }
                }

               
                    // wait 
                    StateStage tmpwait = new StateStage(data);
                    tmpwait.isRoot = true;
                    tmpwait.x_wait = true;
                    activeStatesValue[0].Add(tmpwait);
                
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
                                            && checkDiscipline(d, h, AllBranches, (StateStage)parentNode.FutureActiveState[c]))
                                        {
                                            StateStage tmp = new StateStage(data);
                                            tmp.isRoot = false;
                                            tmp.x_Hosp = h;
                                            tmp.x_Disc = d;

                                            activeStatesValue[counter].Add(tmp);
                                        }
                                        if (h == data.General.Hospitals && checkDiscipline(d, h, AllBranches, (StateStage)parentNode.FutureActiveState[c]))
                                        {
                                            StateStage tmp = new StateStage(data);
                                            tmp.isRoot = false;
                                            tmp.x_Hosp = h;
                                            tmp.x_Disc = d;

                                            activeStatesValue[counter].Add(tmp);
                                            break; // there is no ward checking 
                                        }
                                    }
                                }
                            }
                        }
                        // the wait 
                        if (((StateStage)parentNode.FutureActiveState[c]).x_K > 0 && ((StateStage)parentNode.FutureActiveState[c]).theSchedule_t[stageTime].theDiscipline<0)
                        {
                            
                            StateStage tmpwait = new StateStage(data);
                            tmpwait.x_wait = true;
                            activeStatesValue[counter].Add(tmpwait);
                        }
                            
                        
                    }
                    else
                    {
                        // it is a complete  solution 
                        // we need all						
                        finalSchedules.Add(((StateStage)parentNode.FutureActiveState[c]));
                    }

                }
            }

        }


        public void setFutureState(ArrayList AllBranches)
        {
            //Console.WriteLine("====== Stage " + stageTime + " ======");
            for (int c = 0; c < ActiveStatesCount; c++)
            {
                //Console.WriteLine("*** " + ((StateStage)activeStatesValue[c][0]).x_Hosp + " " + ((StateStage)activeStatesValue[c][0]).x_Disc + " " + ((StateStage)activeStatesValue[c][0]).x_K + " " + ((StateStage)activeStatesValue[c][0]).x_wait + " " + ((StateStage)activeStatesValue[c][0]).Fx);
                // 0 is the current state
                for (int i = 1; i < activeStatesValue[c].Count; i++)
                {
                    StateStage tmp = new StateStage((StateStage)activeStatesValue[c][0], (StateStage)activeStatesValue[c][i], RCDes, theIntern, data, stageTime, rootStage);
                    foreach (StateStage item in tmp.possibleStates)
                    {
                        item.Fx += returnDemandCost(item.x_Disc, item.x_Hosp, stageTime, item.x_wait);

                         //Console.WriteLine(item.x_Hosp + " " + item.x_Disc + " " + item.x_K + " " + item.x_wait + " " + item.Fx);
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
            cleanTheActiveState(AllBranches);
        }

        public double returnDemandCost(int theD, int theH, int theT, bool waitingLable)
        {
            double result = 0;
            if (theH == data.General.Hospitals)
            {
                return result;
            }
            if (waitingLable || theD < 0)
            {
                return result;
            }
            result = RCStart_tdh[theT][theD][theH];
            return result;
        }

        public void cleanTheActiveState(ArrayList allBranches)
        {
            //chooseBestHospIfChangeIsNecessary(); // we will face shorage in rare discipline and vital	
            MaxProcessedNode = FutureActiveState.Count;
            
            Console.WriteLine("Before one disc: " + FutureActiveState.Count);
            // it will not work with unstable demand structure => at t we are in regular demand, t+1 it is reserved demand
            keepOneDisci();
            Console.WriteLine("After one disc: " + FutureActiveState.Count);
            cleanRedundantSequence();
            Console.WriteLine("After cleaning redundant node: " + FutureActiveState.Count);
            removeInfeasibleNode(allBranches);
            Console.WriteLine("After cutting infeasible branches: " + FutureActiveState.Count);

            Console.WriteLine();
            Console.WriteLine();

            limittedFutureList();
            RealProcessedNode = FutureActiveState.Count;

        }


        // redundant schedule d1h1 - d2h2 - d3h3 === d2h2 - d1h1 -d3h3 (last one is fixed)
        public void cleanRedundantSequence()
        {
            int counter = -1;
            while (counter < FutureActiveState.Count - 1 && counter >= -1)
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
                int lowerbound = length;
                if (isHeuristic)
                {
                    lowerbound = 2;
                }
                for (int l = lowerbound; l <= length; l++)
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
                            if (theD[0] == -1)
                            {
                                startTime = t;
                                lengCounter = 0;
                            }

                            for (int ll = 0; ll < l; ll++)
                            {
                                if (t > stageTime)
                                {
                                    noSeq = true;
                                    break;
                                }
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
                            t = nextT - 1;
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
                            if (state.theSchedule_t[lastoneStart].theDiscipline == theD[l - 1] && state.theSchedule_t[lastoneStart].theHospital == theH[l - 1])
                            {
                                for (int ll = 0; ll < l - 1; ll++)
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
                                        if (theD1 == theD[lch])
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

        // keep only one discipline 
        public void keepOneDisci()
        {
            if (!isHeuristic)
            {
                return;
            }
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
                        if (current.theSchedule_t[stageTime - 1].theDiscipline == theDtmp
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

                    double obj1 = demCost + prfX1 * (data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi);
                    double obj2 = tmpdemCost + prfX2 * (data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi);
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

        public void DPStageProcedure(ref ArrayList finalSchedules, ArrayList AllBranches)
        {
            setStateStage(ref finalSchedules, AllBranches);
            if (solutionFound)
            {
                return;
            }
            setFutureState(AllBranches);
        }

        public void removeInfeasibleNode(ArrayList AllBranches)
        {
            int counterr = -1;
            while (counterr < FutureActiveState.Count - 1)
            {
                counterr++;
                StateStage state = (StateStage)FutureActiveState[counterr];
                int timeLine = stageTime;
                for (int t = stageTime + 1; t < data.General.TimePriods; t++)
                {
                    if (state.theSchedule_t[t].theDiscipline == -1)
                    {
                        timeLine = t;
                        break;
                    }
                }

                int requiredTime = data.Intern[theIntern].requiredTime(state.activeDisc, state.x_K_g, state.x_K,data);
                if (requiredTime + timeLine > data.General.TimePriods)
                {
                    FutureActiveState.RemoveAt(counterr);
                    counterr--;
                }
            }
            int p = data.Intern[theIntern].ProgramID;
            foreach (ColumnAndBranchInfo.Branch brnch in AllBranches)
            {
                int counter = -1;
                if (brnch.branch_status)
                {
                    if (brnch.BrTypeStartTime)
                    {
                        while (counter < FutureActiveState.Count - 1)
                        {
                            counter++;
                            StateStage state = (StateStage)FutureActiveState[counter];
                            int theD = state.theSchedule_t[stageTime].theDiscipline;
                            bool cango = false;
                            if (state.x_K != 0)
                            {
                                continue;
                            }
                            if (state.theSchedule_t[brnch.BrTime].theDiscipline != brnch.BrDisc
                                && state.theSchedule_t[brnch.BrTime].theHospital != brnch.BrHospital) // if it is there, it started at the right time
                            {
                                FutureActiveState.RemoveAt(counter);
                                counter--;
                            }
                        }
                    }
                    if (brnch.BrTypePrecedence)
                    {
                        while (counter < FutureActiveState.Count - 1)
                        {
                            counter++;
                            StateStage state = (StateStage)FutureActiveState[counter];
                            int theD = state.theSchedule_t[stageTime].theDiscipline;
                            bool cango = false;
                            if (state.x_K != 0)
                            {
                                continue;
                            }
                            if (!state.activeDisc[brnch.BrDisc] ||
                                !state.activeDisc[brnch.BrPrDisc]) // if it exist, it started at the right time after pr
                            {
                                FutureActiveState.RemoveAt(counter);
                                counter--;

                            }

                        }
                    }
                }

            }
        }

        public bool checkDiscipline(int theDisc, int theH, ArrayList AllBranches, StateStage theState)
        {
            bool result = true;
            // branches
            foreach (ColumnAndBranchInfo.Branch branch in AllBranches)
            {
                if (branch.branch_status)
                {
                    if (branch.BrTypeStartTime)
                    {
                        if (stageTime == branch.BrTime && (theDisc != branch.BrDisc || theH != branch.BrHospital))
                        {
                            result = false;
                        }
                        else if (theDisc == branch.BrDisc && stageTime != branch.BrTime)
                        {
                            result = false;
                        }
                        else if (stageTime != branch.BrTime && stageTime + data.Discipline[theDisc].Duration_p[data.Intern[theIntern].ProgramID] > branch.BrTime)
                        {
                            result = false;
                        }
                    }
                    if (branch.BrTypePrecedence)
                    {
                        if (theDisc == branch.BrDisc && theH != branch.BrHospital)
                        {
                            result = false;
                        }
                        else if (theDisc == branch.BrDisc && theH == branch.BrHospital)
                        {
                            int prDis = -1;
                            for (int t = stageTime - 1; t >= 0; t--)
                            {
                                if (theState.theSchedule_t[t].theDiscipline >= 0)
                                {
                                    prDis = theState.theSchedule_t[t].theDiscipline;
                                    break;
                                }
                            }
                            if (prDis != branch.BrPrDisc)
                            {
                                result = false;
                            }
                        }
                    }
                }
                else
                {
                    if (branch.BrTypeStartTime)
                    {
                        if (stageTime == branch.BrTime && (theDisc == branch.BrDisc && theH == branch.BrHospital))
                        {
                            result = false;
                        }
                    }
                    if (branch.BrTypePrecedence)
                    {
                        if (theDisc == branch.BrDisc && theH == branch.BrHospital)
                        {
                            int prDis = -1;
                            for (int t = stageTime - 1; t >= 0; t--)
                            {
                                if (theState.theSchedule_t[t].theDiscipline >= 0)
                                {
                                    prDis = theState.theSchedule_t[t].theDiscipline;
                                    break;
                                }
                            }
                            if (prDis != branch.BrPrDisc)
                            {
                                result = false;
                            }
                        }
                    }
                }
            }

            // already assigned or the time is filled
            if (theState.activeDisc[theDisc] || theState.theSchedule_t[stageTime].theDiscipline != -1
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

        /// <summary>
        /// calculates the coefficient for each discipline and hospital if they start at the specefic time
        /// it also initialze the RCStartTime_tdh
        /// </summary>
        /// <param name="dual">the dual comming from Master problem</param>
        public void setRCStart(double[] dual)
        {
            new ArrayInitializer().CreateArray(ref RCDual_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            new ArrayInitializer().CreateArray(ref RCStart_tdh, data.General.TimePriods, data.General.Disciplines, data.General.Hospitals, 0);
            RCPi2 = 0;
            RCDes = 0;

            RCDes = data.TrainingPr[data.Intern[theIntern].ProgramID].CoeffObj_SumDesi;

            int Constraint_Counter = 0;

            // Constraint 2
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (i == theIntern)
                {
                    RCPi2 -= dual[Constraint_Counter];
                }

                Constraint_Counter++;
            }

            // Constraint 3 
            for (int r = 0; r < data.General.Region; r++)
            {
                for (int t = 0; t < data.General.TimePriods; t++)
                {
                    if (data.Intern[theIntern].TransferredTo_r[r])
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            for (int h = 0; h < data.General.Hospitals; h++)
                            {
                                if (data.Hospital[h].InToRegion_r[r])
                                {
                                    RCDual_tdh[t][d][h] -= dual[Constraint_Counter];

                                }

                            }
                        }

                    }

                    Constraint_Counter++;
                }
            }

            // Constraint 4 

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {

                            if (data.Hospital[h].Hospital_dw[d][w] && data.Intern[theIntern].isProspective)
                            {
                                RCDual_tdh[t][d][h] -= dual[Constraint_Counter];
                            }

                        }

                        Constraint_Counter++;

                    }
                }
            }

            // Constraint 5

            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int w = 0; w < data.General.HospitalWard; w++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        for (int d = 0; d < data.General.Disciplines; d++)
                        {
                            if (data.Hospital[h].Hospital_dw[d][w])
                            {
                                RCDual_tdh[t][d][h] -= dual[Constraint_Counter];

                            }
                        }

                        Constraint_Counter++;

                    }
                }
            }

            // Constraint 6   
            for (int i = 0; i < data.General.Interns; i++)
            {
                if (theIntern == i)
                {
                    RCDes += dual[Constraint_Counter];
                }


                Constraint_Counter++;
            }


            for (int t = 0; t < data.General.TimePriods; t++)
            {
                for (int d = 0; d < data.General.Disciplines; d++)
                {
                    for (int h = 0; h < data.General.Hospitals; h++)
                    {
                        int theP = data.Intern[theIntern].ProgramID;
                        for (int tt = t; tt < t + data.Discipline[d].Duration_p[theP] && tt < data.General.TimePriods; tt++)
                        {
                            RCStart_tdh[t][d][h] += RCDual_tdh[tt][d][h];
                        }


                    }
                }
            }

        }
    }
}

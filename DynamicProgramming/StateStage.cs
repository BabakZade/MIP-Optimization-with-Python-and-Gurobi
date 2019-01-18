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

		public StateStage()
		{

		}
		public StateStage(StateStage stateInput, StateStage xInput, int theI, AllData alldata, bool rootIndicator)
		{
			data = alldata;
			isRoot = rootIndicator;
			if (xInput.x_wait)
			{
				x_wait = true;
				Fx = data.Intern[theI].wieght_w;
				if (!rootIndicator)
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
				Fx = data.Intern[theI].Prf_d[x_Disc] * data.Intern[theI].wieght_d 
					+ data.Intern[theI].Prf_h[x_Hosp] * data.Intern[theI].wieght_h 
					+ data.TrainingPr[data.Intern[theI].ProgramID].weight_p * data.TrainingPr[data.Intern[theI].ProgramID].Prf_d[x_Disc];
			
			}
			if (!rootIndicator)
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

			setNextStates(stateInput, theI, rootIndicator);
		}
		public void setNextStates(StateStage stateInput, int theI, bool rootIndicator)
		{
			possibleStates = new ArrayList();
			x_K = 0;
			x_K_g = new int[data.General.DisciplineGr];
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				if (rootIndicator)
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
			x_K--;
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
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				if (!x_wait && data.Intern[theI].DisciplineList_dg[x_Disc][g] && x_K_g[g] > 0)
				{					
					possibleStates.Add(copyState(this, g));					
				}
			}
		}

		// it is private function
		StateStage copyState(StateStage copyable, int theG)
		{
			StateStage duplicated = new StateStage();
			Fx = copyable.Fx;
			xStar = copyable.xStar ;
			x_wait = copyable.x_wait;
			x_Disc = copyable.x_Disc;
			x_Hosp = copyable.x_Hosp;
			for (int g = 0; g < data.General.DisciplineGr; g++)
			{
				x_K_g[g] = copyable.x_K_g[g];
				if (g == theG)
				{
					x_K_g[g]--;
				}
			}
			x_K = copyable.x_K;
			isRoot= copyable.isRoot;
			for (int d = 0; d < data.General.Disciplines; d++)
			{
				activeDisc[d] = copyable.activeDisc[d];
			}
			return duplicated;
		}

	}
}

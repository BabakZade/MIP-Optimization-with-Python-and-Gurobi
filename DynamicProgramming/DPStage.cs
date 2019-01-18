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
		public DPStage() { }
		public ArrayList activeStates;
		public ArrayList activeValue;
		public DPStage(AllData alldata, DPStage parent, int theI)
		{
			data = alldata;
			theIntern = theI;
			if (parent==null)
			{
				rootStage = true;
			}
			else
			{
				rootStage = false;
				parentNode = parent;
			}			

		}
		public void Initial()
		{
			activeStates = new ArrayList();
			if (rootStage)
			{

			}
			else
			{
				foreach (StateStage state in parentNode.activeStates)
				{
					
				}
			}
		}
		public void DPStageProcedure()
		{
			foreach (StateStage item in activeValue)
			{

			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataLayer
{
	public class ArrayInitializer
	{

		/// <summary>
		/// This function creates array and initial it
		/// </summary>
		/// <param name="ArrayName">Refrence the name of array (call by refrence)</param>
		/// <param name="FirstDime">It is the size of the first dimantion </param>
		/// <param name="InitVal">It is the initialing value </param>
		public void CreateArray<T>(ref T[] ArrayName, int FirstDime, T InitVal)
		{
			ArrayName = new T[FirstDime];
			for (int i = 0; i < FirstDime; i++)
			{

				ArrayName[i] = InitVal;
			}
		}

		/// <summary>
		/// This function creates array and initial it
		/// </summary>
		/// <param name="ArrayName">Refrence the name of array (call by refrence)</param>
		/// <param name="FirstDime">It is the size of the first dimantion </param>
		/// <param name="SecondDime">It is the size of the second dimantion </param>
		/// <param name="InitVal">It is the initialing value </param>
		public void CreateArray<T>(ref T [][] ArrayName, int FirstDime, int SecondDime, T InitVal)
		{
			ArrayName = new T [FirstDime][];
			for (int i = 0; i < FirstDime; i++)
			{
				ArrayName[i] = new T [SecondDime];
				for (int j = 0; j < SecondDime; j++)
				{
					ArrayName[i][j] = InitVal;
				}
			}
		}

		/// <summary>
		/// This function creates array and initial it
		/// </summary>
		/// <param name="ArrayName">Refrence the name of array (call by refrence)</param>
		/// <param name="FirstDime">It is the size of the first dimantion </param>
		/// <param name="SecondDime">It is the size of the second dimantion </param>
		/// <param name="ThirdDime">It is the size of the third dimantion </param>
		/// <param name="InitVal">It is the initialing value </param>
		public void CreateArray<T>(ref T [][][] ArrayName, int FirstDime, int SecondDime, int ThirdDime, T InitVal)
		{
			ArrayName = new T [FirstDime][][];
			for (int i = 0; i < FirstDime; i++)
			{
				ArrayName[i] = new T [SecondDime][];
				for (int j = 0; j < SecondDime; j++)
				{
					ArrayName[i][j] = new T [ThirdDime];
					for (int k = 0; k < ThirdDime; k++)
					{
						ArrayName[i][j][k] = InitVal;
					}
					
				}
			}
		}

		/// <summary>
		/// This function creates array and initial it
		/// </summary>
		/// <param name="ArrayName">Refrence the name of array (call by refrence)</param>
		/// <param name="FirstDime">It is the size of the first dimantion </param>
		/// <param name="SecondDime">It is the size of the second dimantion </param>
		/// <param name="ThirdDime">It is the size of the third dimantion </param>
		/// <param name="ForthDime">It is the size of the forth dimantion </param>
		/// <param name="InitVal">It is the initialing value </param>
		public void CreateArray<T>(ref T [][][][] ArrayName, int FirstDime, int SecondDime, int ThirdDime, int ForthDime, T InitVal)
		{
			ArrayName = new T [FirstDime][][][];
			for (int i = 0; i < FirstDime; i++)
			{
				ArrayName[i] = new T [SecondDime][][];
				for (int j = 0; j < SecondDime; j++)
				{
					ArrayName[i][j] = new T [ThirdDime][];
					for (int k = 0; k < ThirdDime; k++)
					{
						ArrayName[i][j][k] = new T [ForthDime];
						for (int l = 0; l < ForthDime; l++)
						{

							ArrayName[i][j][k][l] = InitVal;

						}
					
					}

				}
			}
		}

	}
}

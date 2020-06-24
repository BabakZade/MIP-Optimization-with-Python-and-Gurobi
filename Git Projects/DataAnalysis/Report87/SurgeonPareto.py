
import os
import statistics as st
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
from xml.dom import minidom
from scipy import polyfit

import sys
import math

excelfile = 'Akbarzadeh_Report 087_01_260220_MultiLevelORScheduling.xlsx'

data = pd.ExcelFile(excelfile)
data.sheet_names
i = 0
data.sheet_names[i]
while data.sheet_names[i] != 'Info':
    i += 1

alldata = pd.read_excel(excelfile, sheet_name=i)  # data in the sheet that I want

import MultiObjective as MOO

solutionSet = []
partialSolSet = []
IdSet = []
for i in range(np.shape(alldata)[0]):
    if math.isnan(alldata['id'][i])==False and alldata['OREff'][i] > 0.62 and alldata['WLEff'][i] >= alldata['RealEff'][i]:
        IdSet.append(i)
        solutionSet.append([alldata['AssPat'][i],
                            -alldata['RePlPat'][i],
                            -alldata['ErRate'][i],
                            -alldata['TrRate'][i],
                            alldata['Assadd'][i],
                            alldata['AssEl'][i],
                            alldata['HospEff'][i],
                            alldata['MSSEff'][i],
                            -alldata['SurWait'][i],
                            -alldata['AssOpBl'][i],
                            -alldata['RLInGr'][i],
                            -alldata['RLOutGr'][i],
                            -alldata['OTimeIn'][i],
                            -alldata['OTimeOut'][i],
                            alldata['OREff'][i],
                            alldata['WLEff'][i]])
        partialSolSet.append([alldata['MSSEff'][i],
                            -alldata['SurWait'][i],
                            -alldata['AssOpBl'][i],
                            -alldata['RLInGr'][i],
                            -alldata['RLOutGr'][i],
                            -alldata['OTimeIn'][i],
                            -alldata['OTimeOut'][i]])

print(np.shape(IdSet))

domCount = np.zeros(np.shape(solutionSet)[0])
for j1 in range(np.shape(partialSolSet)[0]): # total row
    count = 0
    if partialSolSet[j1][np.shape(partialSolSet)[1]-1] == "NoValue":
        domCount[j1] = 1
        continue
    for j2 in range(np.shape(partialSolSet)[0]): # total row
        if j1 == j2:
            continue
        if partialSolSet[j2][np.shape(partialSolSet)[1]-1] == "NoValue":
            domCount[j2] = 1
            continue
        if domCount[j2] == 1:
            continue
        #print([ j1, j2])

        if MOO.AWeaklyDominatesB(partialSolSet[j2],partialSolSet[j1],np.shape(partialSolSet)[1]):
            domCount[j1] = 1
            break

# create radar chart for the 6 indicators
## Efficiency
nonDomSet = []
HLLevelSettings = []
ZelenyPointHL = np.zeros(np.shape(partialSolSet)[1])
first = True
for i in range(np.shape(domCount)[0]):
    #id in the data starts from 1 => use -1 in indexing
    if domCount[i] == 0:
        print(alldata['id'][IdSet[i]])
        # print([alldata['id'][IdSet[i]], alldata['HospEff'][IdSet[i]],
        #                      alldata['MSSEff'][IdSet[i]],
        #                      alldata['OREff'][IdSet[i]],
        #                      alldata['WLEff'][IdSet[i]]])
        HLLevelSettings.append([alldata['OpPr'][IdSet[i]],
                               alldata['Gsize'][IdSet[i]],
                               alldata['TcBl'][IdSet[i]],
                               alldata['OpORTime'][IdSet[i]],
                               alldata['OprBl'][IdSet[i]],
                               alldata['PatArr'][IdSet[i]],
                               alldata['SurPrf'][IdSet[i]],
                               alldata['DPD'][IdSet[i]],
                               alldata['BPD'][IdSet[i]]])
        nonDomSet.append([alldata['MSSEff'][IdSet[i]],
                            -alldata['SurWait'][IdSet[i]],
                            -alldata['AssOpBl'][IdSet[i]],
                            -alldata['RLInGr'][IdSet[i]],
                            -alldata['RLOutGr'][IdSet[i]],
                            -alldata['OTimeIn'][IdSet[i]],
                            -alldata['OTimeOut'][IdSet[i]]])
        if first:
            first = False
            for i in range(np.shape(nonDomSet)[1]):
                ZelenyPointHL[i] = nonDomSet[np.shape(nonDomSet)[0] - 1][i]
        for i in range(np.shape(partialSolSet)[1]):
            if ZelenyPointHL[i] < nonDomSet[np.shape(nonDomSet)[0] - 1][i]:
                ZelenyPointHL[i] = nonDomSet[np.shape(nonDomSet)[0] - 1][i]


#normalize dataset
nonDomSet = MOO.normalizeDataset(nonDomSet)

np.shape(nonDomSet)


labels=[r'$\qquad\qquad Eff^{MS}$',
        r'$W^{S}$',
        r'$Bl^{OB}\qquad$',
        r'$Bl^{Rl}_{In}\qquad$',
        r'$Bl^{Rl}_{Out}\qquad$',
        r'$Bl^{OT}_{In}\qquad$',
        r'$Bl^{OT}_{Out}\qquad$']


markers = [0, 0.05, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4]

exLegend = []
for i in range(np.shape(nonDomSet)[0]):
    exLegend.append(str(i))


# Create the zeleny Distance
MOO.make_radar_chart("SurPrtHL", nonDomSet ,exLegend ,labels, markers, ZelenyPointHL)


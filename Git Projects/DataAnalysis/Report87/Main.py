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
for i in range(np.shape(alldata)[0]):
    if math.isnan(alldata['id'][i])==False:
        solutionSet.append([alldata['AssPat'][i],
                            alldata['RePlPat'][i],
                            alldata['ErRate'][i],
                            alldata['TrRate'][i],
                            alldata['Ass add'][i],
                            alldata['Ass El'][i],
                            alldata['HospEff'][i],
                            alldata['SurEff'][i],
                            alldata['SurWait'][i],
                            alldata['AssOpBl'][i],
                            alldata['RLInGr'][i],
                            alldata['RLOutGr'][i],
                            alldata['OTimeIn'][i],
                            alldata['OTimeOut'][i]])




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
while data.sheet_names[i] != 'OnlyhigherInfo':
    i += 1

alldata = pd.read_excel(excelfile, sheet_name=i)  # data in the sheet that I want

# remove nan

nameOfTheHeaders = {
    'id': r'$ID$',
    'OpPr': 'OB%',
    'Gsize': r'$GS_{\delta}$',
    'TcBl': r'$|B|$',
    'OpORTime': 'OT%',
    'OprBl': r'$|SB|$',
    'PatArr': 'Add%',
    'SurPrf': r'$TSP$',
    'DPD': 'DPD',
    'BPD': 'BPD',
    'RePlPat': 'rePl',
    'Assadd': r'$R^{Ass}_{add}$',
    'AssEl': r'$R^{Ass}_{el}$',
    'HospEff': r'$Ef_{H}$',
    'WLEff': r'$Ef_{WL}$',
    'MSSEff': r'$Ef_{MSS}$',
    'SurWait': 'wSur',
    'OREff': r'$Ef_{OR}$',
    'RealEff': r'$Ef_{R}$',
    'BlChange': 'chSur',
    'ErTr': r'$R_{ErTr}$'
}
allHeaders = [ 'OpPr', 'Gsize', 'TcBl','OpORTime','OprBl','PatArr', 'SurPrf', 'DPD', 'BPD']
allLatexLevels = {
    'OpPr': ['0%', '25%', '50%', '75%', '100%'],
    'Gsize': ['1', r'$A_{\delta}$', r'$|\Delta|$'],
    'TcBl': ['1', '2', '4'],
    'OpORTime': ['0%', '25%', '50%', '75%', '100%'],
    'OprBl': [r'$|B|$', r'$2\times|B|$', r'$4\times|B|$'],
    'PatArr': ['0%', '25%', '50%', '75%', '100%'],
    'SurPrf': ['0%', '25%', '50%', '75%', '100%'],
    'DPD': ['0.0', '0.5', '1.0'],
    'BPD': ['0.0', '0.5', '1.0']
}


# settings for higher levels
allExcelLevels = {
    'OpPr': [0, 0.25, 0.5, 0.75, 1],
    'Gsize': [1, 2, 4],
    'TcBl': [1, 2, 4],
    'OpORTime': [0, 0.25, 0.5, 0.75, 1],
    'OprBl': [1, 2, 4],
    'PatArr': [1000, 4, 2, 3, 1],
    'SurPrf': [0, 0.25, 0.5, 0.75, 1],
    'DPD': [0, 0.5,  1],
    'BPD': [0,  0.5, 1],
}

def latexLevel(header, excelLevel, allExcellLevels, allLatexLevel):
    ouput = ''
    for i in range(np.shape(allExcellLevels[header])[0]):
        if allExcellLevels[header][i] == excelLevel:
            ouput = allLatexLevel[header][i]
            #print([ouput, header, excelLevel])
            break
    return ouput
def axesLevel(header, excelLevel, allExcellLevels):
    ouput = ''
    for i in range(np.shape(allExcellLevels[header])[0]):
        if allExcellLevels[header][i] == excelLevel:
            ouput = i
            #print([ouput, header, excelLevel])
            break
    return ouput



# set of data
def setAveDataSol(DataSet, nameOfFactor):
    aveData = []
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            for j in range(np.shape(aveData)[0]):
                if DataSet[nameOfFactor][i] == aveData[j][0]:
                    itIsFund = True
                    aveData[j][1] += 1
                    aveData[j][2] += DataSet['RePlPat'][i]
                    aveData[j][3] += DataSet['TrRate'][i] + DataSet['ErRate'][i]
                    aveData[j][4] += DataSet['HospEff'][i]
                    aveData[j][5] += DataSet['MSSEff'][i]
                    aveData[j][6] += DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i])
                    aveData[j][7] += (DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i])
                    aveData[j][8] += DataSet['OREff'][i]
                    aveData[j][9] += DataSet['WLEff'][i]
            if itIsFund == False:
                aveData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            (DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            DataSet['OREff'][i],
                            DataSet['WLEff'][i]])

    for i in range(np.shape(aveData)[0]):

        if aveData[i][1] != 0:
            aveData[i][2] /= aveData[i][1]
            aveData[i][3] /= aveData[i][1]
            aveData[i][4] /= aveData[i][1]
            aveData[i][5] /= aveData[i][1]
            aveData[i][6] /= aveData[i][1]
            aveData[i][7] /= aveData[i][1]
            aveData[i][8] /= aveData[i][1]
            aveData[i][9] /= aveData[i][1]
    return aveData

def setMinDataSol(DataSet, nameOfFactor):
    minData = []
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            for j in range(np.shape(minData)[0]):
                if DataSet[nameOfFactor][i] == minData[j][0]:
                    itIsFund = True
                    minData[j][1] += 1
                    minData[j][2] = min(DataSet['RePlPat'][i], minData[j][2])
                    minData[j][3] = min(DataSet['TrRate'][i] + DataSet['ErRate'][i], minData[j][3])
                    minData[j][4] = min(DataSet['HospEff'][i],minData[j][4])
                    minData[j][5] = min(DataSet['MSSEff'][i],minData[j][5])
                    minData[j][6] = min(DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),minData[j][6])
                    minData[j][7] = min((DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),minData[j][7])
                    minData[j][8] = min(DataSet['OREff'][i],minData[j][8])
                    minData[j][9] = min(DataSet['WLEff'][i],minData[j][9])
            if itIsFund == False:
                minData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            (DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            DataSet['OREff'][i],
                            DataSet['WLEff'][i]])

    return minData

def setMaxDataSol(DataSet, nameOfFactor):
    maxData = []
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            for j in range(np.shape(maxData)[0]):
                if DataSet[nameOfFactor][i] == maxData[j][0]:
                    itIsFund = True
                    maxData[j][1] += 1
                    maxData[j][2] = max(DataSet['RePlPat'][i], maxData[j][2])
                    maxData[j][3] = max(DataSet['TrRate'][i] + DataSet['ErRate'][i], maxData[j][3])
                    maxData[j][4] = max(DataSet['HospEff'][i],maxData[j][4])
                    maxData[j][5] = max(DataSet['MSSEff'][i],maxData[j][5])
                    maxData[j][6] = max(DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),maxData[j][6])
                    maxData[j][7] = max((DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i]), maxData[j][7])
                    maxData[j][8] = max(DataSet['OREff'][i],maxData[j][8])
                    maxData[j][9] = max(DataSet['WLEff'][i],maxData[j][9])
            if itIsFund == False:
                maxData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i]/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            ( DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][
                                i] * 3 + DataSet['RLOutGr'][i] * 4)/(DataSet['TcBl'][i]*DataSet['OprBl'][i]),
                            DataSet['OREff'][i],
                            DataSet['WLEff'][i]])

    return maxData

OpPrSolSet = setAveDataSol(alldata, 'OpPr')
GsizeSolSet = setAveDataSol(alldata, 'Gsize')
TcBlSolSet = setAveDataSol(alldata, 'TcBl')
OpORTimeSolSet = setAveDataSol(alldata, 'OpORTime')
OprBlSolSet = setAveDataSol(alldata, 'OprBl')
PatArrSolSet = setAveDataSol(alldata, 'PatArr')
SurPrfSolSet = setAveDataSol(alldata, 'SurPrf')
DPDSolSet = setAveDataSol(alldata, 'DPD')
BPDSolSet = setAveDataSol(alldata, 'BPD')
# min
minOpPrSolSet = setMinDataSol(alldata, 'OpPr')
minGsizeSolSet = setMinDataSol(alldata, 'Gsize')
minTcBlSolSet = setMinDataSol(alldata, 'TcBl')
minOpORTimeSolSet = setMinDataSol(alldata, 'OpORTime')
minOprBlSolSet = setMinDataSol(alldata, 'OprBl')
minPatArrSolSet = setMinDataSol(alldata, 'PatArr')
minSurPrfSolSet = setMinDataSol(alldata, 'SurPrf')
minDPDSolSet = setMinDataSol(alldata, 'DPD')
minBPDSolSet = setMinDataSol(alldata, 'BPD')

# max
maxOpPrSolSet = setMaxDataSol(alldata, 'OpPr')
maxGsizeSolSet = setMaxDataSol(alldata, 'Gsize')
maxTcBlSolSet = setMaxDataSol(alldata, 'TcBl')
maxOpORTimeSolSet = setMaxDataSol(alldata, 'OpORTime')
maxOprBlSolSet = setMaxDataSol(alldata, 'OprBl')
maxPatArrSolSet = setMaxDataSol(alldata, 'PatArr')
maxSurPrfSolSet = setMaxDataSol(alldata, 'SurPrf')
maxDPDSolSet = setMaxDataSol(alldata, 'DPD')
maxBPDSolSet = setMaxDataSol(alldata, 'BPD')


Names = [r'$rePl$', r'$ErTr$', r'$Eff^{H}$', r'$Eff^{MS}$', r'$Wait^{Sur}$', r'$ReSch^{Sur}$', r'$Eff^{OR}$',
         r'$Eff^{WL}$']

np.shape(OpPrSolSet)


def drawTheTrends(aveDataSet, minDataSet, maxDataSet,  nameOfX, nameOfYs, tickLabels):
    import numpy as np
    import matplotlib.pyplot as plt
    fig, axes = plt.subplots(nrows=4, ncols=2,figsize=(15,15))

    for i, ax in enumerate(axes.flatten()):
        if i < 8:
            x = []
            y = []
            yMin =[]
            yMax = []
            for j in range(np.shape(aveDataSet)[0]):
                x.append(axesLevel(nameOfX, aveDataSet[j][0], allExcelLevels))
                y.append(aveDataSet[j][i + 2])
                yMax.append(maxDataSet[j][i + 2])
                yMin.append(minDataSet[j][i + 2])
            ax.plot(x, y, "r--")
            ax.set_title(nameOfYs[i])

            #print(x)
            # data tip
            ii = -1
            for xy in zip(x, y):  # <--
                ii += 1
                label1 = '(' + "{:.1f}".format(yMin[ii]) +", "+ "{:.1f}".format(y[ii])+", "+ "{:.1f}".format(yMax[ii]) +")"
                
                ax.annotate(label1, xy=xy, textcoords='data')  # <--
            # regression for average

            z = np.polyfit(x, y, 3)
            maxX = max(x)
            newX = np.arange(0, maxX, 0.001)
            p = np.poly1d(z)
            ax.plot(newX, p(newX), 'k')
    #plt.subplots_adjust(left=None, bottom=None, right=None, top=None, wspace=0.25, hspace=0.9)
    name = 'Akbarzadeh_Report_087_' + 'TrendOne' + nameOfX+'.eps'
    tickesTmp = []
    for i in range(np.shape(aveDataSet)[0]):
        tickesTmp.append(axesLevel(nameOfX, aveDataSet[i][0], allExcelLevels))
    plt.setp(axes, xticks=tickesTmp, xticklabels=tickLabels)
    plt.savefig(name, format='eps')
    return plt.show()


drawTheTrends(GsizeSolSet, minGsizeSolSet, maxGsizeSolSet, "Gsize", Names,['1',r'$A_{\delta}$',r'$|\Delta|$'])

drawTheTrends(OpPrSolSet, minOpPrSolSet, maxOpPrSolSet, "OpPr", Names,['0%', '25%', '50%' , '75%', '100%'])

drawTheTrends(TcBlSolSet, minTcBlSolSet, maxTcBlSolSet, "TcBl", Names, ['1', '2', '4'])
drawTheTrends(OpORTimeSolSet, minOpORTimeSolSet, maxOpORTimeSolSet, "OpORTime", Names, ['0%','100%'])
drawTheTrends(OprBlSolSet, minOprBlSolSet, maxOprBlSolSet, "OprBl", Names, ['1','4'])
drawTheTrends(PatArrSolSet, minPatArrSolSet, maxPatArrSolSet, "PatArr", Names, ['0', '50%'])
drawTheTrends(SurPrfSolSet, minSurPrfSolSet, maxSurPrfSolSet, "SurPrf", Names, ['0%','100%'])
drawTheTrends(DPDSolSet, minDPDSolSet, maxDPDSolSet, "DPD", Names, ['0%','100%'])
drawTheTrends(BPDSolSet, minBPDSolSet, maxBPDSolSet, "BPD", Names, ['0%','100%'])

def draw3DEffects(data, nameX, nameY, nameZ, axNames):
    import matplotlib.pyplot as plt
    from matplotlib import cm
    import numpy as np
    import sklearn.linear_model

    from mpl_toolkits.mplot3d import Axes3D
    X = []
    Y = []
    Z = []
    X12 = []
    for i in range(np.shape(data[nameX])[0]):
        if math.isnan(data[nameX][i]) == False and math.isnan(data[nameY][i]) == False  and math.isnan(data[nameZ][i]) == False:
            X.append(data[nameX][i])
            Y.append(data[nameY][i])
            Z.append(data[nameZ][i])
            X12.append([data[nameX][i], data[nameY][i]])




    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    ax.scatter(X, Y, Z, marker='.', color='grey')
    #ax.plot_surface(X, Y, Z, rstride=8, cstride=8, alpha=0.3)
    ax.set_xlabel(axNames[0])
    ax.set_ylabel(axNames[1])
    ax.set_zlabel(axNames[2])

    #X1, Y1 = np.meshgrid(X, Y)
    model = sklearn.linear_model.LinearRegression()

    model.fit(X12, Z)
    maxX = max(X)
    minX = min(X)
    maxY = max(Y)
    minY = min(Y)
    minAll = min(minX,minY)
    maxAll = max(maxX,maxY)
        #
        #
    X_x = np.arange(minAll, maxAll, 0.01)
    Y_y = np.arange(minAll, maxAll, 0.01)
    X_test, Y_test = np.meshgrid(X_x, Y_y)

    coefs = model.coef_
    intercept = model.intercept_
    Z_pred = X_test * coefs[0] + Y_test * coefs[1] + intercept
    ax = plt.gca(projection='3d')
    #ax.plot_wireframe(X_test, Y_test, Z_pred, color='grey',alpha=0.1)

    # ploting x and y when z = 0
    X_xy = []
    Y_xy = []
    Z_xy = []
    for i in range(np.shape(data[nameX])[0]):
        if math.isnan(data[nameX][i]) == False and math.isnan(data[nameY][i]) == False  and math.isnan(data[nameZ][i]) == False:
            X_xy.append(data[nameX][i])
            Y_xy.append(data[nameY][i])
            Z_xy.append(0)

    Reg_xy = np.polyfit(X_xy, Y_xy, 3)
    maxX = max(X_xy)
    minX = min(X_xy)
    newX_xy = np.arange(minX, maxX, (maxX - minX)/1000)
    P_xy = np.poly1d(Reg_xy)
    Z_xy = []

    for i in range(np.shape(newX_xy)[0]):
        Z_xy.append(0)
    ax.scatter(newX_xy, P_xy(newX_xy), Z_xy, marker = '.', color='black',s=0.5)


    # ploting y and z when x = 0
    X_zy = []
    Y_zy = []
    Z_zy = []
    for i in range(np.shape(data[nameX])[0]):
        if math.isnan(data[nameX][i]) == False and math.isnan(data[nameY][i]) == False and math.isnan(
                data[nameZ][i]) == False:
            X_zy.append(0)
            Y_zy.append(data[nameY][i])
            Z_zy.append(data[nameZ][i])

    Reg_zy = np.polyfit(Z_zy, Y_zy, 3)
    maxZ = max(Z_zy)
    minZ = min(Z_zy)
    newZ_zy = np.arange(minZ, maxZ, (maxZ - minZ) / 1000)
    P_zy = np.poly1d(Reg_zy)
    X_zy = []

    for i in range(np.shape(newZ_zy)[0]):
        X_zy.append(0)
    ax.scatter(X_zy, P_zy(newZ_zy), newZ_zy, marker='.', color='black', s=0.5)

    # ploting x and z when y = 0
    X_xz = []
    Y_xz = []
    Z_xz = []
    for i in range(np.shape(data[nameX])[0]):
        if math.isnan(data[nameX][i]) == False and math.isnan(data[nameY][i]) == False and math.isnan(
                data[nameZ][i]) == False:
            X_xz.append(data[nameX][i])
            Y_xz.append(0)
            Z_xz.append(data[nameZ][i])

    Reg_xz = np.polyfit(X_xz, Z_xz, 3)
    maxX = max(X_xz)
    minX = min(X_xz)
    newX_xz = np.arange(minX, maxX, (maxX - minX) / 1000)
    P_xz = np.poly1d(Reg_xz)
    Y_xz = []

    for i in range(np.shape(newX_xz)[0]):
        Y_xz.append(1)
    ax.scatter(newX_xz,Y_xz , P_xz(newX_xz), marker='.', color='black', s=0.5)
    plt.xlim(0, 1)
    plt.ylim(0, 1)
    plt.xlim(0,1)
    name = 'Akbarzadeh_Report_087_' + '3DEffect' + nameX + '_' + nameY + '_' + nameY+'.eps'
    plt.savefig(name, format='eps')
    #ax.view_init(90, 60)
    return plt.show()

draw3DEffects(alldata,'OREff','WLEff','MSSEff', [ r'$Eff^{MS}$', r'$Eff^{OR}$', r'$Eff^{WL}$'])


# drawing two effects based on a factor

def setDataSolForTwoQualityOneFactor(DataSet, nameOfQualityOne, nameOfQualityTwo, nameOfFactor):

    totalData = []
    listFD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listFD.add(DataSet[nameOfFactor][i])
    firstD = len(listFD)
    listFD = sorted(listFD)

    secondD = 0
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            secondD += 1

    thirdD = 3

    for i in range(firstD):
        x = []
        for j in range(secondD):
            x.append([listFD[i], np.nan, np.nan])
        totalData.append(x)
    del listFD
    counter = -1
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            counter += 1
            # Open percent
            itIsFund = False
            for j in range(firstD):
                if itIsFund:
                    break
                if totalData[j][counter][0]  == DataSet[nameOfFactor][i]:
                    totalData[j][counter][1] = DataSet[nameOfQualityOne][i]
                    totalData[j][counter][2] = DataSet[nameOfQualityTwo][i]
                    itIsFund = True

    return totalData

def drowTrendForTwoQualityAndOneFactor(DataSet, nameOfQualityOne, nameOfQualityTwo, nameOfFactor,factorLatexTitle):
    import numpy as np
    import matplotlib.pyplot as plt
    threeDData = setDataSolForTwoQualityOneFactor(DataSet, nameOfQualityOne, nameOfQualityTwo, nameOfFactor)

    n_col =  2
    n_row = math.floor(np.shape(threeDData)[0]/2) + 1 # there is always one extra for complete trends
    fig, axes = plt.subplots(nrows=n_row, ncols=n_col, figsize=(20, 20))
    counter = -1
    for i, ax in enumerate(axes.flatten()):
        counter += 1
        if i < np.shape(threeDData)[0]:
            x = []
            y = []
            for j in range(np.shape(threeDData)[1]):
                if math.isnan(threeDData[i][j][1]) == True:
                    continue
                if type(threeDData[i][j][1]) == str or type(threeDData[i][j][2]) == str:
                    continue
                itexists = False
                for m in range(np.shape(x)[0]):
                    break
                    # if y[m] == threeDData[i][j][2]:
                    #     itexists = True
                    #     x[m] = (x[m ] + threeDData[i][j][1])/2
                    #     break
                    # elif x[m] == threeDData[i][j][1]:
                    #     itexists = True
                    #     y[m] = (y[m] + threeDData[i][j][2])/2

                if not itexists:
                    x.append(threeDData[i][j][1])
                    y.append(threeDData[i][j][2])
            ax.plot(x, y, "r.")
            titleN = factorLatexTitle[counter]
            ax.set_title(titleN)
            # regression for average
            z = np.polyfit(x, y, 2)
            maxX = max(x)
            minX = min(x)
            newX = np.arange(minX, maxX, (maxX-minX)/100)
            p = np.poly1d(z)
            ax.plot(newX, p(newX), 'k')
        else:
            x = []
            y = []
            for j in range(np.shape(alldata)[0]):
                if math.isnan(alldata[nameOfQualityOne][j]) == True :
                    continue
                if type(alldata[nameOfQualityOne][j]) == str or type(alldata[nameOfQualityTwo][j]) == str:
                    continue
                itexists = False
                for m in range(np.shape(x)[0]):
                    break
                    # if y[m] == alldata[nameOfQualityTwo][j]:
                    #     itexists = True
                    #     x[m]  = (x[m] + alldata[nameOfQualityOne][j])/2
                    #     break
                    # if x[m] == alldata[nameOfQualityOne][j]:
                    #     itexists = True
                    #     y[m]  = (y[m] + alldata[nameOfQualityTwo][j])/2
                    #   break
                if not itexists:
                    x.append(alldata[nameOfQualityOne][j])
                    y.append(alldata[nameOfQualityTwo][j])

            ax.plot(x, y, "r.")
            titleN = factorLatexTitle[counter]
            ax.set_title(titleN)

            # regression for average
            z = np.polyfit(x, y, 2)
            maxX = max(x)
            minX = min(x)
            newX = np.arange(minX, maxX, (maxX - minX) / 100)
            p = np.poly1d(z)
            ax.plot(newX, p(newX), 'k')
    # plt.subplots_adjust(left=None, bottom=None, right=None, top=None, wspace=0.25, hspace=0.9)
    name = 'Akbarzadeh_Report_087_' + nameOfQualityOne + '_'+  nameOfQualityTwo + '_' + nameOfFactor  + '.eps'

    plt.savefig(name, format='eps')
    return plt.show()


drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','OREff', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','OREff', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','OREff', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])


drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','OREff', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','OREff', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','OREff', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])

drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','MSSEff', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','MSSEff', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','MSSEff', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])


drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','SurWait', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','SurWait', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'BlChange','SurWait', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])

drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','WLEff', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','WLEff', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','WLEff', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])

drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','ErTr', "OpPr", [r'$OB = 0\%$',r'$OB = 25\%$',r'$OB = 50\%$',r'$OB = 75\%$',r'$OB = 100\%$', r'$OB\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','ErTr', "Gsize", [r'$GS = 1$',r'$GS = A_{\delta}$',r'$GS = |\Delta|$',r'$GS\ Aggrigated$'])
drowTrendForTwoQualityAndOneFactor(alldata,'RePlPat','ErTr', "TcBl", [r'$|B| = 1$', r'$|B| = 2$', r'$|B| = 4$', r'$|B|\ Aggrigated$'])


def setDataSol3FactorOneQuality(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfFactorThree, nameOfQuality):

    # first dime
    firstD = 0
    listFD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listFD.add(DataSet[nameOfFactorOne][i])
    firstD = len(listFD)
    listFD = sorted(listFD)



    secondD = 0
    listSD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listSD.add(DataSet[nameOfFactorTwo][i])
    secondD = len(listSD)
    listSD = sorted(listSD)

    thirdD = 0
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            thirdD += 1

    forthD = 4

    totalData4D = []

    for i in range(len(listFD)):
        x = []
        for j in range(len(listSD)):
            y = []
            for k in range(thirdD):
                z = []
                z.append(listFD[i])
                z.append(listSD[j])
                z.append(np.nan)
                z.append(np.nan)
                y.append(z)
            x.append(y)
        totalData4D.append(x)
    del listFD
    del listSD
    counter = -1
    writen = 0
    for i in range(np.shape(DataSet)[0]):

        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            counter += 1
            for j in range(np.shape(totalData4D)[0]):
                if itIsFund:
                    break
                for k in range(np.shape(totalData4D)[1]):
                    if itIsFund:
                        break
                    if DataSet[nameOfFactorOne][i] == totalData4D[j][k][0][0] and DataSet[nameOfFactorTwo][i] == totalData4D[j][k][0][1]: # check the factor
                        totalData4D[j][k][counter][2] = DataSet[nameOfFactorThree][i]
                        totalData4D[j][k][counter][3] = DataSet[nameOfQuality][i]
                        itIsFund = True
                        writen += 1
    return totalData4D



def drawA4DGraph(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfFactorThree, nameOfQuality):
    import numpy as np
    import matplotlib.pyplot as plt
    fourDData = setDataSol3FactorOneQuality(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfFactorThree, nameOfQuality)

    n_col =  np.shape(fourDData)[1]
    n_row = np.shape(fourDData)[0]
    fig, axes = plt.subplots(nrows=n_row, ncols=n_col, figsize=(20, 20))
    counter = -1

    for i, ax in enumerate(axes.flatten()):
        iIndex = int(math.floor(i/n_col))
        jIndex = int(math.fmod(i,n_col))
        print([iIndex, jIndex])
        x = []
        y = []

        for k in range(np.shape(fourDData)[2]):
            if math.isnan(fourDData[iIndex][jIndex][k][2]) == False:
                y.append(fourDData[iIndex][jIndex][k][2])
                x.append(fourDData[iIndex][jIndex][k][3])

        ax.plot(x, y, "r.")
        titleN = nameOfFactorOne + "  " + str(fourDData[iIndex][jIndex][k][0]) + " ~ " + nameOfFactorTwo + "  " + str(fourDData[iIndex][jIndex][k][1])
        ax.set_title(titleN)

        # regression for average
        z = np.polyfit(x, y, 2)
        maxX = max(x)
        minX = min(x)

        newX = np.arange(minX, maxX, (maxX-minX)/100)
        p = np.poly1d(z)
        ax.plot(newX, p(newX), 'k')
    # plt.subplots_adjust(left=None, bottom=None, right=None, top=None, wspace=0.25, hspace=0.9)
    name = 'Akbarzadeh_Report_087_' + nameOfFactorOne + '_'+  nameOfFactorTwo + '_' + nameOfFactorThree + '_' + nameOfQuality + '.eps'

    plt.savefig(name, format='eps')
    return plt.show()


drawA4DGraph(alldata, 'TcBl', 'Gsize' ,'RePlPat','OREff')


def setDataFor2FactorTileAndTrendFor2CriteriaWith2ClusteringCriteria(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfCriteria1, nameOfCriteria2, nameOfClustering1, nameOfClustering2):

    # first dime
    firstD = 0
    listFD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listFD.add(DataSet[nameOfFactorOne][i])
    firstD = len(listFD)
    listFD = sorted(listFD)



    secondD = 0
    listSD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listSD.add(DataSet[nameOfFactorTwo][i])
    secondD = len(listSD)
    listSD = sorted(listSD)

    thirdD = 0
    listTD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listTD.add(DataSet[nameOfClustering1][i])
    thirdD = len(listTD)
    listTD = sorted(listTD)

    forthD = 0
    listFoD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listFoD.add(DataSet[nameOfClustering2][i])
    forthD = len(listFoD)
    listFoD = sorted(listFoD)

    fifthD = 0
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            fifthD += 1



    sisthD = 6

    totalData6D = []

    for i in range(len(listFD)):
        x = []
        for j in range(len(listSD)):
            y = []
            for k in range(thirdD):
                z = []
                for l in range(forthD):
                    w = []
                    for m in range(fifthD):
                        v = []
                        v.append(listFD[i])
                        v.append(listSD[j])
                        v.append(listTD[k])
                        v.append(listFoD[l])
                        v.append(np.nan)
                        v.append(np.nan)
                        w.append(v)
                    z.append(w)
                y.append(z)
            x.append(y)
        totalData6D.append(x)


    del listFD
    del listSD
    del listTD
    del listFoD
    counter = -1
    writen = 0
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            counter += 1
            for j in range(np.shape(totalData6D)[0]):
                if itIsFund:
                    break
                for k in range(np.shape(totalData6D)[1]):
                    if itIsFund:
                        break
                    for l in range(np.shape(totalData6D)[2]):
                        if itIsFund:
                                break
                        for m in range(np.shape(totalData6D)[3]):
                            if itIsFund:
                                break
                            if DataSet[nameOfFactorOne][i] == totalData6D[j][k][l][m][0][0] and DataSet[nameOfFactorTwo][i] == totalData6D[j][k][l][m][0][1] and DataSet[nameOfClustering1][i] == totalData6D[j][k][l][m][0][2] and DataSet[nameOfClustering2][i] == totalData6D[j][k][l][m][0][3]: # check the factor

                                totalData6D[j][k][l][m][counter][4] = DataSet[nameOfCriteria1][i]
                                totalData6D[j][k][l][m][counter][5] = DataSet[nameOfCriteria2][i]
                                itIsFund = True
                                writen += 1
    return totalData6D

def DrawDataFor2FactorTileAndTrendFor2CriteriaWith2ClusteringCriteria(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfCriteria1, nameOfCriteria2, nameOfClustering1, nameOfClustering2,  excellLevels, latexLevels, latexHeaders):
    import numpy as np
    import matplotlib.pyplot as plt
    import matplotlib as allPlt
    sixDimeData = setDataFor2FactorTileAndTrendFor2CriteriaWith2ClusteringCriteria(DataSet, nameOfFactorOne, nameOfFactorTwo, nameOfCriteria1, nameOfCriteria2, nameOfClustering1, nameOfClustering2)

    n_col =  np.shape(sixDimeData)[1]
    n_row = np.shape(sixDimeData)[0]
    fig, axes = plt.subplots(nrows=n_row, ncols=n_col, figsize=(20, 20))
    fig.text(0.5, 0.04, latexHeaders[nameOfCriteria2], ha='center')
    fig.text(0.04, 0.5, latexHeaders[nameOfCriteria1], va='center', rotation='vertical')
    counter = -1
    colorName = []
    markersName = [".", "v", "1", "s", "*", "+", "x", "d", "p", "h"]
    totalNumberOfClusters = 0
    # xlim and ylim
    xlimMin = math.inf
    xlimMax = -math.inf
    ylimMin = math.inf
    ylimMax = -math.inf
    rgbIndex =  float((int((0.0 + 1 / (np.shape(sixDimeData)[2] * np.shape(sixDimeData)[3]))*100))/100)

    for i in range(np.shape(alldata)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            if DataSet[nameOfCriteria1][i] > ylimMax: # criteria one is on y ax
                ylimMax = DataSet[nameOfCriteria1][i]
            if DataSet[nameOfCriteria1][i] < ylimMin: # criteria one is on y ax
                ylimMin = DataSet[nameOfCriteria1][i]

            if DataSet[nameOfCriteria2][i] > xlimMax: # criteria two is on x ax
                xlimMax = DataSet[nameOfCriteria2][i]
            if DataSet[nameOfCriteria2][i] < xlimMin: # criteria one is on x ax
                xlimMin = DataSet[nameOfCriteria2][i]
    First = True
    for i, ax in enumerate(axes.flatten()):
        iIndex = int(math.floor(i/n_col))
        jIndex = int(math.fmod(i,n_col))
        counter = -1

        for k in range(np.shape(sixDimeData)[2]):
            for l in range(np.shape(sixDimeData)[3]):
                counter += 1
                x = []
                y = []
                for m in range(np.shape(sixDimeData)[4]):
                    if math.isnan(sixDimeData[iIndex][jIndex][k][l][m][4]) == False:
                        y.append(sixDimeData[iIndex][jIndex][k][l][m][4])
                        x.append(sixDimeData[iIndex][jIndex][k][l][m][5])
                nameLegend = latexHeaders[nameOfClustering1] + ' = ' + latexLevel(nameOfClustering1, sixDimeData[iIndex][jIndex][k][l][m][2], excellLevels, latexLevels)
                nameLegend += ', ' + latexHeaders[nameOfClustering2] + ' = ' + latexLevel(nameOfClustering2, sixDimeData[iIndex][jIndex][k][l][m][3], excellLevels, latexLevels)

                while counter > len(markersName):
                    counter -= len(markersName)

                if First:
                    #colorName.append(np.random.rand(3,))
                    rgb1 = np.random.rand()
                    totalNumberOfClusters += 1
                    while rgb1 < 0.2: # if it is white
                        rgb1 = np.random.rand()
                    rgb1 = totalNumberOfClusters * rgbIndex

                    colorName.append([1 - rgb1, 1 - rgb1, 1 - rgb1])

                ax.scatter(x,y,color= colorName[counter], label=nameLegend, marker = markersName[counter])
                ax.set_xlim([xlimMin, xlimMax])
                ax.set_ylim([ylimMin, ylimMax])

        First = False

        titleN = latexHeaders[nameOfFactorOne] + ' = ' + latexLevel(nameOfFactorOne, sixDimeData[iIndex][jIndex][k][0][0][0], excellLevels, latexLevels) + ', '
        titleN += latexHeaders[nameOfFactorTwo] + ' = ' + latexLevel(nameOfFactorTwo, sixDimeData[iIndex][jIndex][k][0][0][1], excellLevels, latexLevels)
        ax.set_title(titleN)
        handles, labels = ax.get_legend_handles_labels()
        fig.legend(handles, labels, loc='lower center', ncol =  2, frameon = False ,bbox_to_anchor= (0.5, 0.93) )

        # regression for average
        # z = np.polyfit(x, y, 2)
        # maxX = max(x)
        # minX = min(x)
        #
        # newX = np.arange(minX, maxX, (maxX-minX)/100)
        # p = np.poly1d(z)
        # ax.plot(newX, p(newX), 'k')
    # plt.subplots_adjust(left=None, bottom=None, right=None, top=None, wspace=0.25, hspace=0.9)
    name = 'Akbarzadeh_Report_087_' + nameOfFactorOne + '_'+  nameOfFactorTwo + '_'+ nameOfCriteria1 + '_'+ nameOfCriteria2 + '_'+ nameOfClustering1 + '_'+ nameOfClustering2+ '.eps'

    plt.savefig(name, format='eps')
    return plt.show()

DrawDataFor2FactorTileAndTrendFor2CriteriaWith2ClusteringCriteria(alldata, 'Gsize', 'OpPr' ,'WLEff','OREff', 'SurPrf' , 'TcBl', allExcelLevels, allLatexLevels, nameOfTheHeaders)


DrawDataFor2FactorTileAndTrendFor2CriteriaWith2ClusteringCriteria(alldata, 'Gsize', 'OpPr' ,'MSSEff','OREff', 'SurPrf' , 'TcBl', allExcelLevels, allLatexLevels, nameOfTheHeaders)


# regression tree
def regressionTree(DataSet, nameOfCriteria, nameOfAllFactors, excellLevels, latexLevels, latexHeaders):
    # Import the necessary modules and libraries
    import pandas as pd
    from sklearn.tree import DecisionTreeClassifier  # Import Decision Tree Classifier
    from sklearn.model_selection import train_test_split  # Import train_test_split function
    from sklearn import metrics  # Import scikit-learn metrics module for accuracy calculation
    from sklearn.externals.six import StringIO
    from IPython.display import Image
    import pydotplus
    from sklearn.tree import export_graphviz
    from subprocess import check_call
    DataSet = DataSet.dropna()

    allXY = DataSet.groupby(nameOfAllFactors).mean()[nameOfCriteria].reset_index()
    # Create a dataset
    X = allXY[nameOfAllFactors]  # Features
    #print(np.shape(X))
    Y = np.round(allXY[nameOfCriteria] * 100, 0)  # Target variable as an integer

    maxDepth = 4
    # Create Decision Tree classifer object
    clf = DecisionTreeClassifier(criterion="entropy", max_depth=maxDepth)
    #print(type( X[nameOfAllFactors[3]][2]))
    #Train Decision Tree Classifer

    clf = clf.fit(X, Y)

    yLable = []
    minY = min(Y)
    maxY = max(Y)
    for i in Y:
        yLable.append(str(i) + '%')


    dot_data = StringIO()
    export_graphviz(clf, out_file=dot_data,
                    filled=True, rounded=True,
                    special_characters=True, feature_names=nameOfAllFactors, class_names=yLable, impurity=False)

    graph = pydotplus.graph_from_dot_data(dot_data.getvalue())

    graph.write_png('Akbarzadeh_Report_087_' + nameOfCriteria + '_'+ str( maxDepth) +'.png')

    Image(graph.create_png())

#set the data to strings
regressionTree(alldata, 'OREff', ['OpPr', 'Gsize', 'TcBl', 'OpORTime', 'OprBl', 'PatArr', 'SurPrf', 'DPD', 'BPD'],
               allExcelLevels, allLatexLevels, nameOfTheHeaders)


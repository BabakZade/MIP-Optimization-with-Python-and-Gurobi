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
                    aveData[j][6] += DataSet['SurWait'][i]
                    aveData[j][7] += DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4
                    aveData[j][8] += DataSet['OREff'][i]
                    aveData[j][9] += DataSet['WLEff'][i]
        if itIsFund == False:
            aveData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i],
                            DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][
                                i] * 3 + DataSet['RLOutGr'][i] * 4,
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
                    minData[j][6] = min(DataSet['SurWait'][i],minData[j][6])
                    minData[j][7] = min(DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4,minData[j][7])
                    minData[j][8] = min(DataSet['OREff'][i],minData[j][8])
                    minData[j][9] = min(DataSet['WLEff'][i],minData[j][9])
        if itIsFund == False:
            minData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i],
                            DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][
                                i] * 3 + DataSet['RLOutGr'][i] * 4,
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
                    maxData[j][6] = max(DataSet['SurWait'][i],maxData[j][6])
                    maxData[j][7] = max(DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][
                        i] * 3 + DataSet['OTimeOut'][i] * 3 + DataSet['RLOutGr'][i] * 4,maxData[j][7])
                    maxData[j][8] = max(DataSet['OREff'][i],maxData[j][8])
                    maxData[j][9] = max(DataSet['WLEff'][i],maxData[j][9])
        if itIsFund == False:
            maxData.append([DataSet[nameOfFactor][i],
                            1,
                            DataSet['RePlPat'][i],
                            DataSet['TrRate'][i] + DataSet['ErRate'][i],
                            DataSet['HospEff'][i],
                            DataSet['MSSEff'][i],
                            DataSet['SurWait'][i],
                            DataSet['AssOpBl'][i] * 1 + DataSet['OTimeIn'][i] * 2 + DataSet['RLInGr'][i] * 3 +
                            DataSet['OTimeOut'][
                                i] * 3 + DataSet['RLOutGr'][i] * 4,
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
                x.append(aveDataSet[j][0])
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
        tickesTmp.append(aveDataSet[i][0])
    plt.setp(axes, xticks=tickesTmp, xticklabels=tickLabels)
    plt.savefig(name, format='eps')
    return plt.show()


drawTheTrends(OpPrSolSet, minOpPrSolSet, maxOpPrSolSet, "OpPr", Names,['0%', '25%', '50%' , '75%', '100%'])
drawTheTrends(GsizeSolSet, minGsizeSolSet, maxGsizeSolSet, "GroupSize", Names,['1',r'$A_{\delta}$',r'$|\Delta|$'])
drawTheTrends(TcBlSolSet, minTcBlSolSet, maxTcBlSolSet, "TcBl", Names, ['1', '2', '4'])
drawTheTrends(OpORTimeSolSet, minOpORTimeSolSet, maxOpORTimeSolSet, "OpORTime", Names, ['0%','100%'])
drawTheTrends(OprBlSolSet, minOprBlSolSet, maxOprBlSolSet, "OprBl", Names, ['1','4'])
drawTheTrends(PatArrSolSet, minPatArrSolSet, maxPatArrSolSet, "PatArr", Names, ['0', '50%'])
drawTheTrends(SurPrfSolSet, minSurPrfSolSet, maxSurPrfSolSet, "SurPrf", Names, ['0%','100%'])
drawTheTrends(DPDSolSet, minDPDSolSet, maxDPDSolSet, "DPD", Names, ['0%','100%'])
drawTheTrends(BPDSolSet, minBPDSolSet, maxBPDSolSet, "BPD", Names, ['0%','100%'])

def draw3DEffects(data, nameX, nameY, nameZ, axNames):
    import matplotlib.pyplot as plt
    import numpy as np
    import sklearn.linear_model

    from mpl_toolkits.mplot3d import Axes3D
    X = data[nameX]
    Y = data[nameY]
    Z = data[nameZ]

    X12 = []
    for i in range(np.shape(X)[0]):
        X12.append([X[i],Y[i]])

    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    ax.scatter(X, Y, Z,marker='.', color='black')
    ax.set_xlabel(axNames[0])
    ax.set_ylabel(axNames[1])
    ax.set_zlabel(axNames[2])

    # model = sklearn.linear_model.LinearRegression()
    # model.fit(X12, Z)
    # maxX = max(X)
    # minX = min(X)
    # maxY = max(Y)
    # minY = min(Y)
    # minAll = min(minX,minY)
    # maxAll = max(maxX,maxY)
    #
    #
    # X_x = np.arange(minAll, maxAll, 0.01)
    # Y_y = np.arange(minAll, maxAll, 0.01)
    # X_test, Y_test = np.meshgrid(X_x, Y_y)
    #
    # coefs = model.coef_
    # intercept = model.intercept_
    # Z_pred = X_test * coefs[0] + Y_test * coefs[1] + intercept
    # ax = plt.gca(projection='3d')
    # ax.plot_wireframe(X_test, Y_test, Z_pred,color='black')

    name = 'Akbarzadeh_Report_087_' + '3DEffect' + nameX + '_' + nameY + '_' + nameY+'.eps'
    plt.savefig(name, format='eps')
    return plt.show()

draw3DEffects(alldata,'OREff','WLEff','MSSEff', [ r'$Eff^{MS}$', r'$Eff^{OR}$', r'$Eff^{WL}$'])


# drawing two effects based on a factor

def setDataSolForTwoQualityOneFactor(DataSet, nameOfQualityOne, nameOfQualityTwo, nameOfFactor):

    totalData = []
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            for j in range(np.shape(totalData)[0]):
                if math.isnan(DataSet['id'][j]) == False:
                    if DataSet[nameOfFactor][i] == totalData[j][0][0]: # check the factor
                        itIsFund = True
                        break
            if itIsFund == False: # add all data with this factor it does not exist
                dataPerFactor = []
                for l in range(np.shape(DataSet)[0]):
                    if math.isnan(DataSet['id'][l]) == False and DataSet[nameOfFactor][i] == DataSet[nameOfFactor][l]:
                        dataPerFactor.append([])
                        dataPerFactor[np.shape(dataPerFactor)[0] - 1].append(DataSet[nameOfFactor][i])
                        dataPerFactor[np.shape(dataPerFactor)[0] - 1].append(DataSet[nameOfQualityOne][l])
                        dataPerFactor[np.shape(dataPerFactor)[0] - 1].append(DataSet[nameOfQualityTwo][l])
                    elif math.isnan(DataSet['id'][l]) == False: # to be sure that all 2Dimention data s are with the same size
                        dataPerFactor.append([DataSet[nameOfFactor][i], np.nan, np.nan])
                # add data to total data
                #print(np.shape(dataPerFactor))
                totalData.append(dataPerFactor)
                #print(np.shape(totalData))
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
    totalData4D = []
    # first dime
    firstD = 0
    listFD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listFD.add(DataSet[nameOfFactorOne][i])
    firstD = len(listFD)
    print(listFD)
    listFD.clear()

    secondD = 0
    listSD = set()
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            listSD.add(DataSet[nameOfFactorOne][i])
    secondD = len(listSD)
    listSD.clear()

    thirdD = 0
    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            thirdD += 1

    forthD = 4

    for i in range(np.shape(DataSet)[0]):
        if math.isnan(DataSet['id'][i]) == False:
            firstD += 1
    totalData4D = [[[[None] * forthD] * thirdD] * secondD] * firstD
    print( np.shape(totalData4D))

    for i in range(np.shape(DataSet)[0]):
        break
        if math.isnan(DataSet['id'][i]) == False:
            # Open percent
            itIsFund = False
            for j in range(np.shape(totalData4D)[0]):
                if math.isnan(DataSet['id'][j]) == False:
                    if DataSet[nameOfFactorOne][i] == totalData4D[j][0][0][0] and DataSet[nameOfFactorTwo][i] == totalData4D[j][0][0][1]: # check the factor
                        itIsFund = True
                        break
            if itIsFund == False: # add all data with this factor it does not exist
                dataPerQuality = []
                for l in range(np.shape(DataSet)[0]):
                    if math.isnan(DataSet['id'][l]) == False and DataSet[nameOfFactorOne][i] == DataSet[nameOfFactorOne][l] and DataSet[nameOfFactorTwo][i] == DataSet[nameOfFactorTwo][l]:
                        dataPerQuality.append([])
                        dataPerQuality[np.shape(dataPerQuality)[0] - 1].append(DataSet[nameOfFactorOne][i])
                        dataPerQuality[np.shape(dataPerQuality)[0] - 1].append(DataSet[nameOfFactorTwo][i])
                        dataPerQuality[np.shape(dataPerQuality)[0] - 1].append(DataSet[nameOfQuality][l])
                        dataPerQuality[np.shape(dataPerQuality)[0] - 1].append(DataSet[nameOfFactorThree][l])
                    elif math.isnan(DataSet['id'][l]) == False: # to be sure that all 2Dimention data s are with the same size
                        dataPerQuality.append([DataSet[nameOfFactorOne][i], DataSet[nameOfFactorTwo][i], np.nan, np.nan])

                print(['befor', np.shape(totalData4D)])
                addedAlready = False
                for k in range(np.shape(totalData4D)[0]):
                    if DataSet[nameOfFactorOne][i] == totalData4D[k][0][0][0]:
                        addedAlready = True
                        totalData4D[k].append([])
                        (totalData4D[k][np.shape(totalData4D)[1] - 1]).append(dataPerQuality)
                if not addedAlready:
                    totalData4D.append([])
                    totalData4D[np.shape(totalData4D)[0] - 1].append([])
                    totalData4D[np.shape(totalData4D)[0] - 1][np.shape(totalData4D)[1] - 1].append(dataPerQuality)

                print(np.shape(totalData4D))
    return totalData4D


setDataSol3FactorOneQuality(alldata, 'OpPr', 'Gsize' ,'TcBl','OREff')



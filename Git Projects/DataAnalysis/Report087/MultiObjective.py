

def AWeaklyDominatesB(A, B, totalDim):
    _AdominateB = False
    count = 0
    for i in range(totalDim):
        if round(A[i],5) >= round(B[i],5):
            count+=1
    if (count == totalDim):
        _AdominateB = True
    return _AdominateB

def AStronglyDominatesB(A, B, totalDim):
    _AdominateB = False
    count = 0
    dominateOne = False
    for i in range(totalDim):
        if A[i] >= B[i]:
            count+=1
        if A[i] > B[i]:
            dominateOne = True
    if (count == totalDim) & dominateOne:
        _AdominateB = True
    return _AdominateB

def drawTrends(Xset, Yset, Xname, Yname):
    import numpy as np
    import matplotlib.pyplot as plt
    plt.plot(Xset, Yset, 'o')

    # calc the trendline
    z = np.polyfit(Xset, Yset, 1)
    p = np.poly1d(z)
    plt.plot(Xset, p(Xset), "r--")
    plt.xlabel(Xname)
    plt.ylabel(Yname)
    plt.legend(frameon=False, facecolor="none")
    # the line equation:
    print
    "y=%.6fx+(%.6f)" % (z[0], z[1])
    name = 'Akbarzadeh_Report_087_' + 'Trend' + Xname + Yname
    plt.savefig(name, format='eps')
    plt.savefig(name, format='jpeg')
    return plt.show()

def make_radar_chart(name, data, legendlabel, attribute_labels, plot_markers, ZelenyPoint):
    import numpy as np
    import matplotlib.pyplot as plt
    import math
    from copy import copy, deepcopy
    lineStyle = ['-', ':', '--', '-.']
    #copy data
    stats = deepcopy(data)
    # labels = np.array(attribute_labels)
    angles = np.linspace(0, 2 * np.pi, len(attribute_labels), endpoint=False)
    sizeData = np.shape(stats)
    angles = np.concatenate((angles, [angles[0]]))
    fig = plt.figure(figsize=(12, 12))
    ax = fig.add_subplot(111, polar=True)
    #sort for colouring and line style
    aggdistance = []
    average = 0
    for i in range(np.shape(stats)[0]):
        aggdistance.append(0)
        for j in range(np.shape(stats)[1]):
            aggdistance[i] += math.pow(stats[i][j] - ZelenyPoint[j], 2)
            stats[i][j] = abs(stats[i][j] - ZelenyPoint[j])
        aggdistance[i] = math.sqrt(aggdistance[i])
        print(aggdistance[i])
        average += aggdistance[i]
    average = average / np.shape(stats)[0]
    #create array index
    index = []
    for i in range(sizeData[0]):
        index.append(i)
    for i in range(sizeData[0]):
        for j in range(i+1,sizeData[0], 1):
            if aggdistance[i] > aggdistance[j]:
                x = aggdistance[j]
                aggdistance[j] = aggdistance[i]
                aggdistance[i] = x
                #sort lable
                y = legendlabel[j]
                legendlabel[j] = legendlabel[i]
                legendlabel[i] = y
                # sort order
                z = index[j]
                index[j] = index[i]
                index[i] = z

    for j in range(np.shape(lineStyle)[0]): # only with 4 line is readable
        i = index[j]
        lbl = str (legendlabel[j]) + ' with agg dist: ' + str(round(aggdistance[j], 2))
        if aggdistance[i] <  average or True:
            data = np.concatenate((stats[i], [stats[i][0]]))
            ax.plot(angles, data, lineStyle[j], linewidth=3, label=lbl, color='k', zorder=20)
            ax.fill(angles, data, alpha=0.25, facecolor="none")
        #else:
            #data = np.concatenate((stats[i], [stats[i][0]]))
            #ax.plot(angles, data, lineStyle[i], linewidth=2, label=lbl, color='gray', zorder=20)
            #ax.fill(angles, data, alpha=0.25, facecolor="none")

    ax.set_thetagrids(angles * 180 / np.pi, attribute_labels, size=18)
    plt.yticks(plot_markers)
    #ax.set_title(name)
    ax.grid(True)
    ax.legend(bbox_to_anchor=(0.95, -0.1), frameon=False, loc='lower center', facecolor="none", fontsize=15)
    name = 'Akbarzadeh_Report_087_' + 'Radat' + name + '.eps'
    plt.savefig(name, format='eps')
    return plt.show()

def normalizeDataset(dataSet):
    import numpy as np
    MaxAbsHL = np.zeros(np.shape(dataSet)[1])
    MinAbsHL = np.zeros(np.shape(dataSet)[1])
    first = True
    for j in range(np.shape(dataSet)[0]):
        if first:
            first = False
            for i in range(np.shape(dataSet)[1]):
                MinAbsHL[i] = abs(dataSet[np.shape(dataSet)[0] - 1][i])
                MaxAbsHL[i] = abs(dataSet[np.shape(dataSet)[0] - 1][i])
        # max abs
        for i in range(np.shape(dataSet)[1]):
            if MaxAbsHL[i] < abs(dataSet[j][i]):
                MaxAbsHL[i] = abs(dataSet[j][i])
        # min abs
        for i in range(np.shape(dataSet)[1]):
            if MinAbsHL[i] > abs(dataSet[j][i]):
                MinAbsHL[i] = abs(dataSet[j][i])

    for j in range(np.shape(dataSet)[0]):
        for i in range(np.shape(dataSet)[1]):
            if(MaxAbsHL[i] + MinAbsHL[i] != 0):
                dataSet[j][i] = dataSet[j][i]/(MaxAbsHL[i] + MinAbsHL[i])
    return dataSet

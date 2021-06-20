from InputData import  InputData
import numpy as np

class ColumnInfo:

    def __init__(self, inputdata):
        self.initiateColumnX(inputdata)
        self.setColumnCost(inputdata)


    def initiateColumnX(self,inputdata):
        self.columnX_wt = []
        for w in inputdata.W:
            tmp = []
            for t in inputdata.T:
                if t == w:
                    tmp.append(True)
                else:
                    tmp.append(False)
            self.columnX_wt.append(tmp)


    def setColumnCost(self, inputdata):
        self.cost = 0
        for w in inputdata.W:
            for t in inputdata.T:
                if self.columnX_wt[w][t]:
                    self.cost += inputdata.cost_wt[w][t]

    def printMe(self, inputdata):
        for t in inputdata.T:
            for w in inputdata.W:
                if self.columnX_wt[w][t]:
                    print(inputdata.tasks[t] + " => " + inputdata.workers[w] + " cost = " + str(inputdata.cost_wt[w][t]))





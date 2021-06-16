from InputData import InputData
from Column import Column
from numpy import random
from gurobipy import *

class MasterProblem:
    def __init__(self,inputdata):
        pass

    def createInitialColumn(self, inputdata):
        random.seed(inputdata.randomSeed)
        tmpVar = self.inputdata.W
        tmpVar = random.shuffle(tmpVar)
        self.initialColumn = Column()

        for w in self.inputdata.W:
            indexT = tmpVar[w]
            tmp = []
            for t in self.inputdata.T:
                if t == indexT:
                    tmp.append(True)
                else:
                    tmp.append(False)
            self.initialColumn.append(tmp)
        self.initialColumn.setColumnCost(inputdata)
        self.columns = []
        self.columns.append(self.initialColumn)





    def initialModelVar(self):
        self.masterModel = Model("AssignmentLPModel")
        self.x_j = []
        for i in range(len(self.columns)):
            self.x_j.append(masterModel.addVar(0,1,GRB.INTEGER, name = "cl["+str(i)+"]"))

        self.masterModel.update()


    def addColumn(self, Column):
        pass

    def masterFormulation(self,inputdata):

        #objective
        # min SUM(Cost_j * column_j)
        self.objective = self.masterModel.setObjective(
            quicksum(self.columns[j].cost * self.x_j[j] for c in range(len(self.x_j)) == 1), GRB.MINIMIZE)

        # SUM(X_j, j) == 1
        self.constraint = self.masterModel.addConstr(quicksum(self.x_j[c] for c in range(len(self.x_j))) == 1,
                                                     name="OnePlanAtAll")

        self.masterModel.update();



from ColumnInfo import ColumnInfo
from numpy import random
from gurobipy import *

class MasterProblem:
    def __init__(self,inputdata):
        self.createInitialColumn(inputdata)

        self.initialModelVar()
        self.masterFormulation()
        self.solveMe(inputdata)

    def createInitialColumn(self, inputdata):

        self.columns = []
        for s in range(2):
            #random.seed(inputdata.randomSeed)
            tmpVar = list(inputdata.W)
            random.shuffle(tmpVar)
            initialColumn = ColumnInfo(inputdata)

            for w in inputdata.W:
                indexT = tmpVar[w]
                tmp = []
                for t in inputdata.T:
                    if t == indexT:
                        initialColumn.columnX_wt[w][t] = True
                    else:
                        initialColumn.columnX_wt[w][t] = False
            initialColumn.setColumnCost(inputdata)
            self.columns.append(initialColumn)


    def initialModelVar(self):
        self.masterModel = Model("AssignmentLPModel")
        self.x_j = {}
        for i in range(len(self.columns)):
            self.x_j[i] = self.masterModel.addVar(0, 1,vtype=GRB.CONTINUOUS, name = "cl["+str(i)+"]")

        self.masterModel.update()



    def masterFormulation(self):

        #objective
        # min SUM(Cost_j * column_j)
        self.objective = self.masterModel.setObjective(
            quicksum(self.columns[c].cost * self.x_j[c] for c in range(len(self.x_j))), GRB.MINIMIZE)

        # SUM(X_j, j) == 1
        self.constraint = self.masterModel.addConstr(quicksum(self.x_j[c] for c in range(len(self.x_j))) == 1,
                                                     name="OnePlanAtAll")

        self.masterModel.update()

    def solveMe(self, inputdata):

        self.masterModel.write("AssignemntRMP.lp")
        self.masterModel.optimize()

        self.printOutPut(inputdata)


    def printOutPut(self, inputdata):
        if self.masterModel.status == GRB.OPTIMAL:
            print(self.masterModel.objVal)
            self.objectiveValue = self.masterModel.objVal
            print(["dual",self.constraint.Pi])
            self.dual = self.constraint.Pi
            for c in range(len(self.columns)):
                if self.x_j[c].x > 0:
                    self.columns[c].printMe(inputdata)
        else:
            print("It is not optimal")


    def addTheColumn(self, thecolumn):
        clmn =  Column()


        self.columns.append(thecolumn)
        constrs = self.masterModel.getConstrs()
        # SUM(X_j, j) == 1
        clmn.addTerms(1, constrs[0])
        # min SUM(Cost_j * column_j)
        self.x_j[len(self.x_j)]  = self.masterModel.addVar(0, 1, obj= thecolumn.cost, vtype=GRB.CONTINUOUS, name="cl[" + str(len(self.x_j)) + "]", column = clmn)
        self.masterModel.update()







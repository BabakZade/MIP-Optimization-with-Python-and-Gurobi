
from gurobipy import *

class CompactMIP():
    def __init__(self, inputdata):
        self.initiateMIPVar(inputdata)
        self.createMIPModel(inputdata)
        self.solveMIP(inputdata)


    def createMIPModel(self, inputdata):
        #create the objective function
        # minimize SUM( cost_wt * x_wt, w,t)
        self.gorubiModel.setObjective(quicksum(inputdata.cost_wt[w][t] * self.X_wt[w, t] for w in inputdata.W for t in inputdata.T), GRB.MINIMIZE)


        # SUM(X_wt, t) <= 1  \forall w
        for w in inputdata.W:
            self.gorubiModel.addConstr(quicksum(self.X_wt[w, t] for t in inputdata.T) <= 1, name = "Assignment_w(" + str(w) + ")")

        # SUM(X_wt, w) >= 1  \forall t
        for t in inputdata.T:
            self.gorubiModel.addConstr(quicksum(self.X_wt[w, t] for w in inputdata.W) >= 1, name = "Assignment_t(" + str(t) + ")")


    def initiateMIPVar(self, inputdata):
        #define the model
        # Model
        self.gorubiModel = Model("AssignmentModel")
        # Defining the Variable
        self.X_wt = {}
        for w in inputdata.W:
            for t in inputdata.T:
                self.X_wt[w, t] = self.gorubiModel.addVar(vtype=GRB.BINARY, name = "X_wt[" + str(w) + "," +str(t) + "]")

    def solveMIP(self, inputdata):
        self.gorubiModel.write("AssignemntProblem.lp")
        self.gorubiModel.optimize()
        self.printOutPut(inputdata)

    def printOutPut(self, inputdata):
        print(self.gorubiModel.objVal)
        self.objectiveValue = self.gorubiModel.objVal
        for t in inputdata.T:
            for w in inputdata.W:
                if self.X_wt[w,t].x > 0:
                    print(inputdata.tasks[t] + " => " + inputdata.workers[w] + " cost = " + str(inputdata.cost_wt[w][t]))




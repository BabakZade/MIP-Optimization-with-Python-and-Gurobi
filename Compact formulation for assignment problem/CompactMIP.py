from InputData import InputData
from gurobipy import *

class CompactMIP():
    def __init__(self):
        self.inputdata = InputData()
        self.initiateMIPVar()
        self.createMIPModel()
        self.solveMIP()


    def createMIPModel(self):
        #create the objective function
        # minimize SUM( cost_wt * x_wt, w,t)
        self.gorubiModel.setObjective(quicksum(self.inputdata.cost_wt[w][t] * self.X_wt[w, t] for w in self.inputdata.W for t in self.inputdata.T), GRB.MINIMIZE)


        # SUM(X_wt, t) <= 1  \forall w
        for w in self.inputdata.W:
            self.gorubiModel.addConstr(quicksum(self.X_wt[w, t] for t in self.inputdata.T) <= 1, name = "Assignment_w(" + str(w) + ")")

        # SUM(X_wt, w) >= 1  \forall t
        for t in self.inputdata.T:
            self.gorubiModel.addConstr(quicksum(self.X_wt[w, t] for w in self.inputdata.W) >= 1, name = "Assignment_t(" + str(t) + ")")


    def initiateMIPVar(self):
        #define the model
        # Model
        self.gorubiModel = Model("AssignmentModel")
        # Defining the Variable
        self.X_wt = {}
        for w in self.inputdata.W:
            for t in self.inputdata.T:
                self.X_wt[w, t] = self.gorubiModel.addVar(vtype=GRB.BINARY, name = "X_wt[" + str(w) + "," +str(t) + "]")

    def solveMIP(self):
        self.gorubiModel.write("AssignemntProblem.lp")
        self.gorubiModel.optimize()
        self.printOutPut()


    def printOutPut(self):
        print(self.gorubiModel.objVal)
        for t in self.inputdata.T:
            for w in self.inputdata.W:
                if self.X_wt[w,t].x > 0:
                    print(self.inputdata.tasks[t] + " => " + self.inputdata.workers[w] + " cost = " + str(self.inputdata.cost_wt[w][t]))




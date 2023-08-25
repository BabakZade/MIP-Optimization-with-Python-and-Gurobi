from gurobipy import *
from  ColumnInfo import  ColumnInfo

class PricingProblem:
    def __init__(self, inputdata):
        self.initiateMIPVar(inputdata)
        self.createMIPModel(inputdata)
        self.solveMIP(inputdata)



    def createMIPModel(self, inputdata):
        #create the objective function
        # maximize C_j - C_B * B^-1 * a_j
        # C_j = cost_wt * x_wt
        # C_B * B^-1 = dual
        # a_j = 1 => the coefficient in the constaint in the master problem for this column
        # second part is constant  C_B * B^-1 * a_j
        self.pricingModel.setObjective(quicksum(inputdata.cost_wt[w][t] * self.X_wt[w, t] for w in inputdata.W for t in inputdata.T), GRB.MINIMIZE)


        # SUM(X_wt, t) <= 1  \forall w
        for w in inputdata.W:
            self.pricingModel.addConstr(quicksum(self.X_wt[w, t] for t in inputdata.T) <= 1, name = "Assignment_w(" + str(w) + ")")

        # SUM(X_wt, w) >= 1  \forall t
        for t in inputdata.T:
            self.pricingModel.addConstr(quicksum(self.X_wt[w, t] for w in inputdata.W) >= 1, name = "Assignment_t(" + str(t) + ")")



    def initiateMIPVar(self, inputdata):
        #define the model
        # Model
        self.pricingModel = Model("PricingModel")
        # Defining the Variable
        self.X_wt = {}
        for w in inputdata.W:
            for t in inputdata.T:
                self.X_wt[w, t] = self.pricingModel.addVar(vtype=GRB.BINARY, name = "X_wt[" + str(w) + "," +str(t) + "]")

    def solveMIP(self, inputdata):
        self.pricingModel.write("PricingProblem.lp")
        self.pricingModel.optimize()
        self.printOutPut(inputdata)
        self.setTheColumn(inputdata)

    def printOutPut(self, inputdata):
        print(self.pricingModel.objVal)

        for t in inputdata.T:
            for w in inputdata.W:
                if self.X_wt[w,t].x > 0:
                    print(inputdata.tasks[t] + " => " + inputdata.workers[w] + " cost = " + str(inputdata.cost_wt[w][t]))

    def setTheColumn(self,inputdata):
        self.coloumn = ColumnInfo(inputdata)
        for t in inputdata.T:
            for w in inputdata.W:
                if self.X_wt[w,t].x > 0:
                    self.coloumn.columnX_wt[w][t] = True
                else:
                    self.coloumn.columnX_wt[w][t] = False
        self.coloumn.setColumnCost(inputdata)


    def ifAddableColumn(self, dual):

        rc = self.pricingModel.objVal - dual
        if  rc < 0:
            print([self.pricingModel.objVal, dual])
            return True
        else:
            return False


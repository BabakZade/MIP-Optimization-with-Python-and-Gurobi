
from  PricingProblem import PricingProblem
from MasterProblem import MasterProblem

class ColumnGeneration:
    def __init__(self,inputdata):
        self.initialModels(inputdata)
        self.columnGeneration(inputdata)

    def initialModels(self,inputdata):
        self.mastermodel = MasterProblem(inputdata)
        self.objectiveValue = self.mastermodel.objectiveValue

    def columnGeneration(self,inputdata):
        counter = 0
        while True:
            counter += 1
            print(["Iteration: ", counter])
            self.pricingmodel = PricingProblem(inputdata)
            if self.pricingmodel.ifAddableColumn(self.mastermodel.dual):
                self.mastermodel.addTheColumn(self.pricingmodel.coloumn)
                self.mastermodel.solveMe(inputdata)
                self.objectiveValue = self.mastermodel.objectiveValue

            else:
                break








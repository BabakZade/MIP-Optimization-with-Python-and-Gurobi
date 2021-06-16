

class Column:

    def __init__(self):
        self.columnX_wt = []


    def setColumnCost(self, inputdata):
        for w in inputdata.W:
            for t in inputdata.T:
                if self.columnX_wt[w][t]:
                    self.cost += inputdata.cost_wt[w][t]





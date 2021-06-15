from numpy import random

class InputData:


    def __init__(self):
        # I use fixed random data
        self.generateData(0)


    def generateData(self, randomSeed):
        random.seed(randomSeed)
        self.min = 5
        self.max = 10
        self.dimension = random.random_integers(self.min, self.max,1)[0]
        self.workers = []
        self.tasks = []
        for d in range(self.dimension):
            self.workers.append("W" + str(d))
            self.tasks.append("T" + str(d))

        self.W = range(len(self.workers))
        self.T = range(len(self.tasks))

        self.cost_wt = []
        for w in self.W:
            cost_w = []
            for t in self.T:
                cost_w.append(random.random_integers(self.min, self.max,1)[0])
            self.cost_wt.append(cost_w)






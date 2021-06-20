from CompactMIP import CompactMIP
from InputData import InputData
from ColumnGeneration import ColumnGeneration
import os

clear = lambda: os.system('cls')
clear()

inputdata = InputData()

assignmentProblem = CompactMIP(inputdata)

cg = ColumnGeneration(inputdata)

print(["Compact Obj =", assignmentProblem.objectiveValue, " || CG Obj =", cg.objectiveValue])

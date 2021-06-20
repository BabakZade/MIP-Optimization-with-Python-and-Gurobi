\ Model AssignmentLPModel
\ LP format - for model browsing. Use MPS format to capture full model detail.
Minimize
  262 cl[0] + 235 cl[1] + 112 cl[2] + 112 cl[3]
Subject To
 OnePlanAtAll: cl[0] + cl[1] + cl[2] + cl[3] = 1
Bounds
 cl[0] <= 1
 cl[1] <= 1
 cl[2] <= 1
 cl[3] <= 1
End

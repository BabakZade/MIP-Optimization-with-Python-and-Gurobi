\ Model AssignmentLPModel
\ LP format - for model browsing. Use MPS format to capture full model detail.
Minimize
  103 cl[0] + 90 cl[1]
Subject To
 OnePlanAtAll: cl[0] + cl[1] = 1
Bounds
 cl[0] <= 1
 cl[1] <= 1
End

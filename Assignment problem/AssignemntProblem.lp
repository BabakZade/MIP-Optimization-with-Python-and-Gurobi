\ Model AssignmentModel
\ LP format - for model browsing. Use MPS format to capture full model detail.
Minimize
  6 X_wt[0,0] + 8 X_wt[0,1] + 20 X_wt[0,2] + 11 X_wt[0,3] + 14 X_wt[0,4]
   + 11 X_wt[0,5] + 12 X_wt[0,6] + 9 X_wt[0,7] + 9 X_wt[1,0] + 5 X_wt[1,1]
   + 18 X_wt[1,2] + 16 X_wt[1,3] + 5 X_wt[1,4] + 16 X_wt[1,5]
   + 20 X_wt[1,6] + 9 X_wt[1,7] + 11 X_wt[2,0] + 5 X_wt[2,1] + 11 X_wt[2,2]
   + 17 X_wt[2,3] + 5 X_wt[2,4] + 17 X_wt[2,5] + 7 X_wt[2,6] + 11 X_wt[2,7]
   + 7 X_wt[3,0] + 11 X_wt[3,1] + 14 X_wt[3,2] + 19 X_wt[3,3] + 8 X_wt[3,4]
   + 14 X_wt[3,5] + 7 X_wt[3,6] + 20 X_wt[3,7] + 10 X_wt[4,0] + 9 X_wt[4,1]
   + 7 X_wt[4,2] + 18 X_wt[4,3] + 5 X_wt[4,4] + 17 X_wt[4,5] + 15 X_wt[4,6]
   + 9 X_wt[4,7] + 15 X_wt[5,0] + 5 X_wt[5,1] + 17 X_wt[5,2] + 16 X_wt[5,3]
   + 20 X_wt[5,4] + 20 X_wt[5,5] + 9 X_wt[5,6] + 13 X_wt[5,7]
   + 10 X_wt[6,0] + 5 X_wt[6,1] + 14 X_wt[6,2] + 17 X_wt[6,3]
   + 19 X_wt[6,4] + 5 X_wt[6,5] + 7 X_wt[6,6] + 18 X_wt[6,7] + 11 X_wt[7,0]
   + 8 X_wt[7,1] + 17 X_wt[7,2] + 8 X_wt[7,3] + 16 X_wt[7,4] + 10 X_wt[7,5]
   + 9 X_wt[7,6] + 15 X_wt[7,7]
Subject To
 Assignment_w(0): X_wt[0,0] + X_wt[0,1] + X_wt[0,2] + X_wt[0,3] + X_wt[0,4]
   + X_wt[0,5] + X_wt[0,6] + X_wt[0,7] <= 1
 Assignment_w(1): X_wt[1,0] + X_wt[1,1] + X_wt[1,2] + X_wt[1,3] + X_wt[1,4]
   + X_wt[1,5] + X_wt[1,6] + X_wt[1,7] <= 1
 Assignment_w(2): X_wt[2,0] + X_wt[2,1] + X_wt[2,2] + X_wt[2,3] + X_wt[2,4]
   + X_wt[2,5] + X_wt[2,6] + X_wt[2,7] <= 1
 Assignment_w(3): X_wt[3,0] + X_wt[3,1] + X_wt[3,2] + X_wt[3,3] + X_wt[3,4]
   + X_wt[3,5] + X_wt[3,6] + X_wt[3,7] <= 1
 Assignment_w(4): X_wt[4,0] + X_wt[4,1] + X_wt[4,2] + X_wt[4,3] + X_wt[4,4]
   + X_wt[4,5] + X_wt[4,6] + X_wt[4,7] <= 1
 Assignment_w(5): X_wt[5,0] + X_wt[5,1] + X_wt[5,2] + X_wt[5,3] + X_wt[5,4]
   + X_wt[5,5] + X_wt[5,6] + X_wt[5,7] <= 1
 Assignment_w(6): X_wt[6,0] + X_wt[6,1] + X_wt[6,2] + X_wt[6,3] + X_wt[6,4]
   + X_wt[6,5] + X_wt[6,6] + X_wt[6,7] <= 1
 Assignment_w(7): X_wt[7,0] + X_wt[7,1] + X_wt[7,2] + X_wt[7,3] + X_wt[7,4]
   + X_wt[7,5] + X_wt[7,6] + X_wt[7,7] <= 1
 Assignment_t(0): X_wt[0,0] + X_wt[1,0] + X_wt[2,0] + X_wt[3,0] + X_wt[4,0]
   + X_wt[5,0] + X_wt[6,0] + X_wt[7,0] >= 1
 Assignment_t(1): X_wt[0,1] + X_wt[1,1] + X_wt[2,1] + X_wt[3,1] + X_wt[4,1]
   + X_wt[5,1] + X_wt[6,1] + X_wt[7,1] >= 1
 Assignment_t(2): X_wt[0,2] + X_wt[1,2] + X_wt[2,2] + X_wt[3,2] + X_wt[4,2]
   + X_wt[5,2] + X_wt[6,2] + X_wt[7,2] >= 1
 Assignment_t(3): X_wt[0,3] + X_wt[1,3] + X_wt[2,3] + X_wt[3,3] + X_wt[4,3]
   + X_wt[5,3] + X_wt[6,3] + X_wt[7,3] >= 1
 Assignment_t(4): X_wt[0,4] + X_wt[1,4] + X_wt[2,4] + X_wt[3,4] + X_wt[4,4]
   + X_wt[5,4] + X_wt[6,4] + X_wt[7,4] >= 1
 Assignment_t(5): X_wt[0,5] + X_wt[1,5] + X_wt[2,5] + X_wt[3,5] + X_wt[4,5]
   + X_wt[5,5] + X_wt[6,5] + X_wt[7,5] >= 1
 Assignment_t(6): X_wt[0,6] + X_wt[1,6] + X_wt[2,6] + X_wt[3,6] + X_wt[4,6]
   + X_wt[5,6] + X_wt[6,6] + X_wt[7,6] >= 1
 Assignment_t(7): X_wt[0,7] + X_wt[1,7] + X_wt[2,7] + X_wt[3,7] + X_wt[4,7]
   + X_wt[5,7] + X_wt[6,7] + X_wt[7,7] >= 1
Bounds
Binaries
 X_wt[0,0] X_wt[0,1] X_wt[0,2] X_wt[0,3] X_wt[0,4] X_wt[0,5] X_wt[0,6]
 X_wt[0,7] X_wt[1,0] X_wt[1,1] X_wt[1,2] X_wt[1,3] X_wt[1,4] X_wt[1,5]
 X_wt[1,6] X_wt[1,7] X_wt[2,0] X_wt[2,1] X_wt[2,2] X_wt[2,3] X_wt[2,4]
 X_wt[2,5] X_wt[2,6] X_wt[2,7] X_wt[3,0] X_wt[3,1] X_wt[3,2] X_wt[3,3]
 X_wt[3,4] X_wt[3,5] X_wt[3,6] X_wt[3,7] X_wt[4,0] X_wt[4,1] X_wt[4,2]
 X_wt[4,3] X_wt[4,4] X_wt[4,5] X_wt[4,6] X_wt[4,7] X_wt[5,0] X_wt[5,1]
 X_wt[5,2] X_wt[5,3] X_wt[5,4] X_wt[5,5] X_wt[5,6] X_wt[5,7] X_wt[6,0]
 X_wt[6,1] X_wt[6,2] X_wt[6,3] X_wt[6,4] X_wt[6,5] X_wt[6,6] X_wt[6,7]
 X_wt[7,0] X_wt[7,1] X_wt[7,2] X_wt[7,3] X_wt[7,4] X_wt[7,5] X_wt[7,6]
 X_wt[7,7]
End

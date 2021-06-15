\ Model AssignmentModel
\ LP format - for model browsing. Use MPS format to capture full model detail.
Minimize
  10 X_wt[0,0] + 5 X_wt[0,1] + 8 X_wt[0,2] + 8 X_wt[0,3] + 8 X_wt[0,4]
   + 6 X_wt[0,5] + 8 X_wt[0,6] + 10 X_wt[0,7] + 7 X_wt[0,8] + 9 X_wt[1,0]
   + 5 X_wt[1,1] + 5 X_wt[1,2] + 9 X_wt[1,3] + 7 X_wt[1,4] + 6 X_wt[1,5]
   + 5 X_wt[1,6] + 6 X_wt[1,7] + 10 X_wt[1,8] + 6 X_wt[2,0] + 10 X_wt[2,1]
   + 5 X_wt[2,2] + 6 X_wt[2,3] + 9 X_wt[2,4] + 8 X_wt[2,5] + 5 X_wt[2,6]
   + 8 X_wt[2,7] + 10 X_wt[2,8] + 5 X_wt[3,0] + 7 X_wt[3,1] + 8 X_wt[3,2]
   + 5 X_wt[3,3] + 6 X_wt[3,4] + 8 X_wt[3,5] + 10 X_wt[3,6] + 8 X_wt[3,7]
   + 8 X_wt[3,8] + 5 X_wt[4,0] + 6 X_wt[4,1] + 6 X_wt[4,2] + 6 X_wt[4,3]
   + 5 X_wt[4,4] + 7 X_wt[4,5] + 9 X_wt[4,6] + 8 X_wt[4,7] + 8 X_wt[4,8]
   + 7 X_wt[5,0] + 9 X_wt[5,1] + 7 X_wt[5,2] + 5 X_wt[5,3] + 5 X_wt[5,4]
   + 9 X_wt[5,5] + 10 X_wt[5,6] + 10 X_wt[5,7] + 5 X_wt[5,8] + 9 X_wt[6,0]
   + 6 X_wt[6,1] + 9 X_wt[6,2] + 6 X_wt[6,3] + 7 X_wt[6,4] + 7 X_wt[6,5]
   + 5 X_wt[6,6] + 6 X_wt[6,7] + 6 X_wt[6,8] + 6 X_wt[7,0] + 6 X_wt[7,1]
   + 8 X_wt[7,2] + 8 X_wt[7,3] + 7 X_wt[7,4] + 8 X_wt[7,5] + 5 X_wt[7,6]
   + 8 X_wt[7,7] + 10 X_wt[7,8] + 9 X_wt[8,0] + 6 X_wt[8,1] + 7 X_wt[8,2]
   + 9 X_wt[8,3] + 8 X_wt[8,4] + 9 X_wt[8,5] + 9 X_wt[8,6] + 9 X_wt[8,7]
   + 8 X_wt[8,8]
Subject To
 Assignment_w(0): X_wt[0,0] + X_wt[0,1] + X_wt[0,2] + X_wt[0,3] + X_wt[0,4]
   + X_wt[0,5] + X_wt[0,6] + X_wt[0,7] + X_wt[0,8] <= 1
 Assignment_w(1): X_wt[1,0] + X_wt[1,1] + X_wt[1,2] + X_wt[1,3] + X_wt[1,4]
   + X_wt[1,5] + X_wt[1,6] + X_wt[1,7] + X_wt[1,8] <= 1
 Assignment_w(2): X_wt[2,0] + X_wt[2,1] + X_wt[2,2] + X_wt[2,3] + X_wt[2,4]
   + X_wt[2,5] + X_wt[2,6] + X_wt[2,7] + X_wt[2,8] <= 1
 Assignment_w(3): X_wt[3,0] + X_wt[3,1] + X_wt[3,2] + X_wt[3,3] + X_wt[3,4]
   + X_wt[3,5] + X_wt[3,6] + X_wt[3,7] + X_wt[3,8] <= 1
 Assignment_w(4): X_wt[4,0] + X_wt[4,1] + X_wt[4,2] + X_wt[4,3] + X_wt[4,4]
   + X_wt[4,5] + X_wt[4,6] + X_wt[4,7] + X_wt[4,8] <= 1
 Assignment_w(5): X_wt[5,0] + X_wt[5,1] + X_wt[5,2] + X_wt[5,3] + X_wt[5,4]
   + X_wt[5,5] + X_wt[5,6] + X_wt[5,7] + X_wt[5,8] <= 1
 Assignment_w(6): X_wt[6,0] + X_wt[6,1] + X_wt[6,2] + X_wt[6,3] + X_wt[6,4]
   + X_wt[6,5] + X_wt[6,6] + X_wt[6,7] + X_wt[6,8] <= 1
 Assignment_w(7): X_wt[7,0] + X_wt[7,1] + X_wt[7,2] + X_wt[7,3] + X_wt[7,4]
   + X_wt[7,5] + X_wt[7,6] + X_wt[7,7] + X_wt[7,8] <= 1
 Assignment_w(8): X_wt[8,0] + X_wt[8,1] + X_wt[8,2] + X_wt[8,3] + X_wt[8,4]
   + X_wt[8,5] + X_wt[8,6] + X_wt[8,7] + X_wt[8,8] <= 1
 Assignment_t(0): X_wt[0,0] + X_wt[1,0] + X_wt[2,0] + X_wt[3,0] + X_wt[4,0]
   + X_wt[5,0] + X_wt[6,0] + X_wt[7,0] + X_wt[8,0] >= 1
 Assignment_t(1): X_wt[0,1] + X_wt[1,1] + X_wt[2,1] + X_wt[3,1] + X_wt[4,1]
   + X_wt[5,1] + X_wt[6,1] + X_wt[7,1] + X_wt[8,1] >= 1
 Assignment_t(2): X_wt[0,2] + X_wt[1,2] + X_wt[2,2] + X_wt[3,2] + X_wt[4,2]
   + X_wt[5,2] + X_wt[6,2] + X_wt[7,2] + X_wt[8,2] >= 1
 Assignment_t(3): X_wt[0,3] + X_wt[1,3] + X_wt[2,3] + X_wt[3,3] + X_wt[4,3]
   + X_wt[5,3] + X_wt[6,3] + X_wt[7,3] + X_wt[8,3] >= 1
 Assignment_t(4): X_wt[0,4] + X_wt[1,4] + X_wt[2,4] + X_wt[3,4] + X_wt[4,4]
   + X_wt[5,4] + X_wt[6,4] + X_wt[7,4] + X_wt[8,4] >= 1
 Assignment_t(5): X_wt[0,5] + X_wt[1,5] + X_wt[2,5] + X_wt[3,5] + X_wt[4,5]
   + X_wt[5,5] + X_wt[6,5] + X_wt[7,5] + X_wt[8,5] >= 1
 Assignment_t(6): X_wt[0,6] + X_wt[1,6] + X_wt[2,6] + X_wt[3,6] + X_wt[4,6]
   + X_wt[5,6] + X_wt[6,6] + X_wt[7,6] + X_wt[8,6] >= 1
 Assignment_t(7): X_wt[0,7] + X_wt[1,7] + X_wt[2,7] + X_wt[3,7] + X_wt[4,7]
   + X_wt[5,7] + X_wt[6,7] + X_wt[7,7] + X_wt[8,7] >= 1
 Assignment_t(8): X_wt[0,8] + X_wt[1,8] + X_wt[2,8] + X_wt[3,8] + X_wt[4,8]
   + X_wt[5,8] + X_wt[6,8] + X_wt[7,8] + X_wt[8,8] >= 1
Bounds
Binaries
 X_wt[0,0] X_wt[0,1] X_wt[0,2] X_wt[0,3] X_wt[0,4] X_wt[0,5] X_wt[0,6]
 X_wt[0,7] X_wt[0,8] X_wt[1,0] X_wt[1,1] X_wt[1,2] X_wt[1,3] X_wt[1,4]
 X_wt[1,5] X_wt[1,6] X_wt[1,7] X_wt[1,8] X_wt[2,0] X_wt[2,1] X_wt[2,2]
 X_wt[2,3] X_wt[2,4] X_wt[2,5] X_wt[2,6] X_wt[2,7] X_wt[2,8] X_wt[3,0]
 X_wt[3,1] X_wt[3,2] X_wt[3,3] X_wt[3,4] X_wt[3,5] X_wt[3,6] X_wt[3,7]
 X_wt[3,8] X_wt[4,0] X_wt[4,1] X_wt[4,2] X_wt[4,3] X_wt[4,4] X_wt[4,5]
 X_wt[4,6] X_wt[4,7] X_wt[4,8] X_wt[5,0] X_wt[5,1] X_wt[5,2] X_wt[5,3]
 X_wt[5,4] X_wt[5,5] X_wt[5,6] X_wt[5,7] X_wt[5,8] X_wt[6,0] X_wt[6,1]
 X_wt[6,2] X_wt[6,3] X_wt[6,4] X_wt[6,5] X_wt[6,6] X_wt[6,7] X_wt[6,8]
 X_wt[7,0] X_wt[7,1] X_wt[7,2] X_wt[7,3] X_wt[7,4] X_wt[7,5] X_wt[7,6]
 X_wt[7,7] X_wt[7,8] X_wt[8,0] X_wt[8,1] X_wt[8,2] X_wt[8,3] X_wt[8,4]
 X_wt[8,5] X_wt[8,6] X_wt[8,7] X_wt[8,8]
End

def AWeaklyDominatesB(A, B, totalDim):
    _AdominateB = False;
    count = 0
    for i in range(totalDim):
        if A[i] >= B[i]:
            count+=1
    if (count == totalDim):
        _AdominateB = True
    return _AdominateB

def AStronglyDominatesB(A, B, totalDim):
    _AdominateB = False;
    count = 0
    dominateOne = False
    for i in range(totalDim):
        if A[i] >= B[i]:
            count+=1
        if A[i] > B[i]:
            dominateOne = True
    if (count == totalDim) & dominateOne:
        _AdominateB = True
    return _AdominateB

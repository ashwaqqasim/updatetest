using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mill
{
    private static int NUMBER_OF_FIELDS_IN_MILL = 3;
    public List<int> MillIndices { get; private set; }
    public Mill(int firstIndex, int secondIndex, int thirdIndex)
    {
        MillIndices = new List<int>(NUMBER_OF_FIELDS_IN_MILL) { firstIndex, secondIndex, thirdIndex };
    }
}

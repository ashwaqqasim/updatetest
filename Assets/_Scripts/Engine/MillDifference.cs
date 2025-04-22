using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MillDifference
{
    public HashSet<Mill> TurnActiveMills { get; }
    public HashSet<Mill> NewMills { get; }

    public MillDifference(HashSet<Mill> turnActiveMills, HashSet<Mill> newMills)
    {
        TurnActiveMills = turnActiveMills;
        NewMills = newMills;
    }
}

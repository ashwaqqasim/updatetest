using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public int FromFieldIndex { get; }
    public int ToFieldIndex { get; }

    public Move(int fromFieldIndex, int toFieldIndex)
    {
        this.FromFieldIndex = fromFieldIndex;
        this.ToFieldIndex = toFieldIndex;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Heuristic
{
    double Evaluate(GameState gameState);
}

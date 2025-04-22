using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMillNumberHeuristic : SimplePawnNumberHeuristic
{
    private static readonly int DEFAULT_MILL_WEIGHT = 18;
    private static readonly int CLOSED_MILL_WEIGHT = 26;
    public override double Evaluate(GameState gameState)
    {
        double evaluation = base.Evaluate(gameState);
        foreach(Mill mill in gameState.ActiveMills)
        {
            PlayerNumber pawnOwner = gameState.CurrentBoard.Fields[mill.MillIndices[0]].PawnPlayerNumber;
            if(pawnOwner == PlayerNumber.FirstPlayer)
            {
                evaluation += DEFAULT_MILL_WEIGHT;
            } else
            {
                evaluation -= DEFAULT_MILL_WEIGHT;
            }
        }
        foreach (Mill mill in gameState.ClosedMills)
        {
            PlayerNumber pawnOwner = gameState.CurrentBoard.Fields[mill.MillIndices[0]].PawnPlayerNumber;
            if (pawnOwner == PlayerNumber.FirstPlayer)
            {
                evaluation += CLOSED_MILL_WEIGHT;
            }
            else
            {
                evaluation -= CLOSED_MILL_WEIGHT;
            }
        }
        return evaluation;
    }
}

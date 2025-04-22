using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnMoveNumberHeuristic : SimplePawnNumberHeuristic
{
    private static readonly int DEFAULT_MOVE_WEIGHT = 5;

    public override double Evaluate(GameState gameState)
    {
        double eval = base.Evaluate(gameState);
        eval += gameState.GetPossibleMovesNumberForPlayer(PlayerNumber.FirstPlayer) * DEFAULT_MOVE_WEIGHT;
        eval -= gameState.GetPossibleMovesNumberForPlayer(PlayerNumber.SecondPlayer) * DEFAULT_MOVE_WEIGHT;
        return eval;
    }
}

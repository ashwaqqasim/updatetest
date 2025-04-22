public class SimplePawnNumberHeuristic : Heuristic
{
    private static readonly int DEFAULT_WINNING_WEIGHT = 1086;
    private static readonly int DEFAULT_PAWN_WEIGHT = 9;

    public virtual double Evaluate(GameState gameState)
    {
        PlayerNumber winningPlayer = gameState.WinningPlayer;
        double evaluation = DEFAULT_PAWN_WEIGHT * (gameState.FirstPlayersPawnsLeft - gameState.SecondPlayersPawnsLeft);
        if (winningPlayer == PlayerNumber.FirstPlayer)
        {
            evaluation += DEFAULT_WINNING_WEIGHT;
        }
        else if (winningPlayer == PlayerNumber.SecondPlayer)
        {
            evaluation -= DEFAULT_WINNING_WEIGHT;
        }
        return evaluation;
    }
}

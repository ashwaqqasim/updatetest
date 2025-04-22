using System.Collections.Generic;

public class GameTreeNode {
    public GameState GameState;
    public double Evaluation;

    public GameTreeNode(GameState gameState, double evaluation)
    {
        GameState = gameState;
        Evaluation = evaluation;
    }
}

public class MinMaxAiPlayer : AiPlayer
{
    GameEngine game;
    Heuristic heuristic;
    PlayerNumber playerNumber;

    private int searchDepth;

    public MinMaxAiPlayer(GameEngine game, Heuristic heuristic, PlayerNumber playerNumber, int searchDepth)
    {
        this.game = game;
        this.heuristic = heuristic;
        this.playerNumber = playerNumber;
        this.searchDepth = searchDepth * 2;
    }

    public void MakeMove()
    {
        GameTreeNode bestPossibleMove = null;
        GameState currentState = game.GameState;
        if(playerNumber == PlayerNumber.FirstPlayer)
        {
            bestPossibleMove = MinMax(currentState, searchDepth, true);
        }
        else
        {
            bestPossibleMove = MinMax(currentState, searchDepth, false);
        }
        game.MakeMove(bestPossibleMove.GameState);
    }

    private GameTreeNode MinMax(GameState currentState, int depth, bool maximizingPlayer)
    {
        GameTreeNode bestMove = null;
        if (depth == 0 || currentState.WinningPlayer != PlayerNumber.None)
        {
            double evaluation = heuristic.Evaluate(currentState);
            bestMove = new GameTreeNode(currentState, evaluation);
        }

        else if (maximizingPlayer)
        {
            double maxEval = double.NegativeInfinity;
            List<GameState> nextStates = currentState.GetAllPossibleNextStates(PlayerNumber.FirstPlayer);
            foreach (var nextState in nextStates)
            {
                GameTreeNode bestChild = MinMax(nextState, depth - 1, false);
                if(maxEval < bestChild.Evaluation)
                {
                    bestMove = new GameTreeNode(nextState, bestChild.Evaluation);
                    maxEval = bestChild.Evaluation;
                }
            }

        }
        else
        {
            double minEval = double.PositiveInfinity;
            List<GameState> nextStates = currentState.GetAllPossibleNextStates(PlayerNumber.SecondPlayer);
            foreach (var nextState in nextStates)
            {
                GameTreeNode bestChild = MinMax(nextState, depth - 1, true);
                if (minEval > bestChild.Evaluation)
                {
                    bestMove = new GameTreeNode(nextState, bestChild.Evaluation);
                    minEval = bestChild.Evaluation;
                }
            }
        }
        return bestMove;
    }
}

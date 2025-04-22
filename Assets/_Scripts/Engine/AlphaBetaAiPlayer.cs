using System.Collections;
using System.Collections.Generic;
using System;
public class AlphaBetaAiPlayer : AiPlayer
{
    GameEngine game;
    Heuristic heuristic;
    PlayerNumber playerNumber;

    private int searchDepth;

    public AlphaBetaAiPlayer(GameEngine game, Heuristic heuristic, PlayerNumber playerNumber, int searchDepth)
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
        if (playerNumber == PlayerNumber.FirstPlayer)
        {
            bestPossibleMove = MinMax(currentState, searchDepth, Double.NegativeInfinity, Double.PositiveInfinity, true);
        }
        else
        {
            bestPossibleMove = MinMax(currentState, searchDepth, Double.NegativeInfinity, Double.PositiveInfinity, false);
        }
        game.MakeMove(bestPossibleMove.GameState);
    }

    private GameTreeNode MinMax(GameState currentState, int depth, double alpha, double beta, bool maximizingPlayer)
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
                GameTreeNode bestChild = MinMax(nextState, depth - 1, alpha, beta, false);
                if(bestChild == null)
                {
                    currentState.GetAllPossibleNextStates(PlayerNumber.FirstPlayer);
                }
                if (maxEval < bestChild.Evaluation)
                {
                    bestMove = new GameTreeNode(nextState, bestChild.Evaluation);
                    maxEval = bestChild.Evaluation;
                }
                alpha = Math.Max(alpha, bestChild.Evaluation);
                if(beta <= alpha)
                {
                    break;
                }
            }

        }
        else
        {
            double minEval = double.PositiveInfinity;
            List<GameState> nextStates = currentState.GetAllPossibleNextStates(PlayerNumber.SecondPlayer);
            foreach (var nextState in nextStates)
            {
                GameTreeNode bestChild = MinMax(nextState, depth - 1, alpha, beta, true);
                if (bestChild == null)
                {
                    currentState.GetAllPossibleNextStates(PlayerNumber.SecondPlayer);
                }
                if (minEval > bestChild.Evaluation)
                {
                    bestMove = new GameTreeNode(nextState, bestChild.Evaluation);
                    minEval = bestChild.Evaluation;
                }
                beta = Math.Min(beta, bestChild.Evaluation);
                if (beta <= alpha)
                {
                    break;
                }
            }
            
        }
        return bestMove;
    }
}

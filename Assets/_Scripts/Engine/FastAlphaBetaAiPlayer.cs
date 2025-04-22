using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class FastAlphaBetaAiPlayer : AiPlayer
{
    GameEngine game;
    Heuristic sortHeuristic;
    Heuristic evaluationHeuristic;
    PlayerNumber playerNumber;

    private int searchDepth;
    private int maxStatesToSearch = 10; // تقليل عدد الحالات العشوائية المدروسة

    public FastAlphaBetaAiPlayer(GameEngine game, Heuristic evaluationHeuristic, PlayerNumber playerNumber, int searchDepth, Heuristic sortHeuristic = null)
    {
        this.game = game;
        this.evaluationHeuristic = evaluationHeuristic;
        this.playerNumber = playerNumber;
        this.searchDepth = searchDepth;
        this.sortHeuristic = sortHeuristic ?? evaluationHeuristic;
    }

    public void MakeMove()
    {
        GameState currentState = game.GameState;

        // If the game is waiting for a pawn to be removed, don't let the AI act
        if (currentState.PawnsToRemove > 0)
        {
            UnityEngine.Debug.Log("⛔ AI halted: waiting for human to remove a pawn.");
            return;
        }

        int effectiveDepth = searchDepth;
        int totalPawns = currentState.FirstPlayersPawnsLeft + currentState.SecondPlayersPawnsLeft;

        if (totalPawns > 12)
        {
            effectiveDepth = Math.Min(searchDepth, 2); // Optimize early game
        }

        GameTreeNode bestMove = (playerNumber == PlayerNumber.FirstPlayer)
            ? MinMax(currentState, effectiveDepth, double.NegativeInfinity, double.PositiveInfinity, true)
            : MinMax(currentState, effectiveDepth, double.NegativeInfinity, double.PositiveInfinity, false);

        if (bestMove == null)
        {
            UnityEngine.Debug.Log("❌ AI found no possible move.");
            return;
        }

        UnityEngine.Debug.Log("🤖 AI applying best move.");
        game.MakeMove(bestMove.GameState); // ✅ Apply the full move — even if it forms a mill
    }




    private GameTreeNode MinMax(GameState currentState, int depth, double alpha, double beta, bool maximizingPlayer)
    {
        GameTreeNode bestMove = null;

        if (depth == 0 || currentState.WinningPlayer != PlayerNumber.None)
        {
            double evaluation = evaluationHeuristic.Evaluate(currentState);
            return new GameTreeNode(currentState, evaluation);
        }

        List<GameState> nextStates = currentState.GetAllPossibleNextStates(
            maximizingPlayer ? PlayerNumber.FirstPlayer : PlayerNumber.SecondPlayer
        );

        foreach (var state in nextStates)
        {
            state.Evaluation = sortHeuristic.Evaluate(state);
        }

        if (maximizingPlayer)
        {
            nextStates = nextStates.OrderByDescending(s => s.Evaluation).Take(maxStatesToSearch).ToList();
            double maxEval = double.NegativeInfinity;

            foreach (var nextState in nextStates)
            {
                GameTreeNode child = MinMax(nextState, depth - 1, alpha, beta, false);
                if (child.Evaluation > maxEval)
                {
                    bestMove = new GameTreeNode(nextState, child.Evaluation);
                    maxEval = child.Evaluation;
                }
                alpha = Math.Max(alpha, child.Evaluation);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }
        else
        {
            nextStates = nextStates.OrderBy(s => s.Evaluation).Take(maxStatesToSearch).ToList();
            double minEval = double.PositiveInfinity;

            foreach (var nextState in nextStates)
            {
                GameTreeNode child = MinMax(nextState, depth - 1, alpha, beta, true);
                if (child.Evaluation < minEval)
                {
                    bestMove = new GameTreeNode(nextState, child.Evaluation);
                    minEval = child.Evaluation;
                }
                beta = Math.Min(beta, child.Evaluation);
                if (beta <= alpha)
                {
                    break;
                }
            }
        }

        return bestMove;
    }
}

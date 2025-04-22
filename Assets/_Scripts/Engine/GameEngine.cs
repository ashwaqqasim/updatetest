
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEngine
{
    public static GameEngine Instance { get; private set; } // ✅ إضافة متغير Instance

    public delegate void GameFinished(PlayerNumber winningPlayerNumber);
    public event GameFinished OnGameFinished = delegate { };

    public delegate void BoardChanged(Board newBoard);
    public event BoardChanged OnBoardChanged = delegate { };

    public delegate void PlayerTurnChanged(PlayerNumber currentMovingPlayerNumber);
    public event PlayerTurnChanged OnPlayerTurnChanged = delegate { };

    public delegate void OnLastFieldSelected();
    public event OnLastFieldSelected OnLastFieldSelectedChanged = delegate { };

    private bool shouldLogToFile;

    public GameState GameState { get; private set; }

    private PlayerNumber lastPlayerTurn;

    public GameEngine()
    {
        Instance = this; // ✅ حفظ نسخة Singleton من GameEngine

        lastPlayerTurn = PlayerNumber.FirstPlayer;
        RegisterNewGameState(new GameState());
    }

    public void HandleSelection(int fieldIndex)
    {
        GameState.HandleSelection(fieldIndex);
        OnBoardChanged(GameState.CurrentBoard); // تأكد من وجود هذا الاستدعاء هنا!
    }

    public void MakeMove(GameState gameState)
    {
        RegisterNewGameState(gameState);
    }

    private void OnGameStateChanged()
    {
        OnBoardChanged(GameState.CurrentBoard);
        if (GameState.WinningPlayer != PlayerNumber.None)
        {
            OnGameFinished(GameState.WinningPlayer);
        }
        if (GameState.CurrentMovingPlayer != lastPlayerTurn)
        {
            lastPlayerTurn = GameState.CurrentMovingPlayer;
            UnityEngine.Debug.Log("🎯 OnPlayerTurnChanged fired: " + lastPlayerTurn);
            OnPlayerTurnChanged(lastPlayerTurn);
        }
        UpdateLastFieldSelected();
    }

    private void RegisterNewGameState(GameState gameState)
    {
        if (GameState != null)
        {
            GameState.OnGameStateChanged -= OnGameStateChanged;
            GameState.OnLastSelectedFieldChanged -= UpdateLastFieldSelected;
        }
        GameState = gameState;
        GameState.OnGameStateChanged += OnGameStateChanged;
        GameState.OnLastSelectedFieldChanged += UpdateLastFieldSelected;
        OnGameStateChanged();
    }

    private void UpdateLastFieldSelected()
    {
        OnLastFieldSelectedChanged();
    }

    public HashSet<int> GetCurrentPossibleMoves()
    {
        return GameState.GetCurrentPlayerPossibleMoveIndices();
    }
}


using System.Collections;
using System.Collections.Generic;
using Random = System.Random;

public class RandomAiPlayer : AiPlayer
{
    private PlayerNumber myPlayerNumber;
    private GameEngine gameEngine;
    private Random randomGenerator;

    public RandomAiPlayer(PlayerNumber myPlayerNumber, GameEngine gameEngine)
    {
        this.myPlayerNumber = myPlayerNumber;
        this.gameEngine = gameEngine;
        randomGenerator = new Random();
    }

    public void MakeMove()
    {
        List<GameState> possibleMoves = gameEngine.GameState.GetAllPossibleNextStates(myPlayerNumber);
        int numberOfPossibleMoves = possibleMoves.Count;
        if(numberOfPossibleMoves == 0)
        {
            gameEngine.MakeMove(null);
        } else
        {
            int moveIndex = randomGenerator.Next(0, numberOfPossibleMoves);
            gameEngine.MakeMove(possibleMoves[moveIndex]);
        }
    }
}

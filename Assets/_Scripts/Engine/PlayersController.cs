using System.Diagnostics;
public class PlayersController
{
    private AiPlayer firstAiPlayer;
    private AiPlayer secondAiPlayer;
    private bool gameEngineReady;
    private PlayerNumber currentPlayerTurn;
    private Stopwatch stopWatch;

    public PlayersController(AiPlayer firstAiPlayer = null, AiPlayer secondAiPlayer = null)
    {
        this.firstAiPlayer = firstAiPlayer;
        this.secondAiPlayer = secondAiPlayer;
        this.currentPlayerTurn = PlayerNumber.FirstPlayer;
        this.gameEngineReady = true;
    }

    public long CheckStep()
    {
        if (gameEngineReady)
        {
            stopWatch = Stopwatch.StartNew();
            if(currentPlayerTurn == PlayerNumber.FirstPlayer)
            {
                HandleAiMove(firstAiPlayer);
            }
            else
            {
                HandleAiMove(secondAiPlayer);
            }
            stopWatch.Stop();
            return stopWatch.ElapsedMilliseconds;
        } else
        {
            return 0;
        }
    }

    public void OnPlayerTurnChanged(PlayerNumber playerNumber)
    {
        this.currentPlayerTurn = playerNumber;
        this.gameEngineReady = true;
    }
    private void HandleAiMove(AiPlayer player)
    {
        gameEngineReady = false;
        if (player != null)
        {
            player.MakeMove();

            // ✅ تحديث الواجهة بعد أول حركة للذكاء الصناعي
            UnityEngine.Object.FindObjectOfType<GameUIController>().UpdateStonesUI(2);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class RPSGameManager : MonoBehaviour
{
    public static int whoStarts = 1;

    public GameObject rpsPanel;
    public GameObject boardPanel;

    public HumanHandController player1HandController;
    public HumanHandController player2HandController;

    public GameObject player1HandObject;
    public GameObject player2HandObject;

    public GameObject panelPlayer1;
    public GameObject panelPlayer2;
    public GameObject panelResult;
    public Text resultText;
    public GameObject restartButton;
    public Text congratsText;

    public GameObject redPawnContainer;
    public GameObject yellowPawnContainer;

    public GameObject[] redPawns;
    public GameObject[] yellowPawns; // 🟡 هذا هو المتغير الجديد اللي لازم يظهر في Inspector

    private string lastWinner = "None";
    private int player1Choice = -1;
    private int player2Choice = -1;

    private enum PlayerTurn { Player1, Player2, ShowResult }
    private PlayerTurn currentTurn = PlayerTurn.Player1;


    public void OnChoice(int choice)
    {
        if (currentTurn == PlayerTurn.Player1)
        {
            player1Choice = choice;
            panelPlayer1.SetActive(false);
            panelPlayer2.SetActive(true);
            currentTurn = PlayerTurn.Player2;
        }
        else if (currentTurn == PlayerTurn.Player2)
        {
            player2Choice = choice;
            panelPlayer2.SetActive(false);
            panelResult.SetActive(true);

            player1HandObject.SetActive(true);
            player2HandObject.SetActive(true);

            player1HandController.PlayChoice(player1Choice);
            player2HandController.PlayChoice(player2Choice);

            string result = DetermineWinner(player1Choice, player2Choice);
            resultText.text = result;

            if (result == "تعادل!")
            {
                restartButton.SetActive(false);
                resultText.text = "";
                Invoke(nameof(ShowResultTextAndRestartButton), 4f);
            }
            else
            {
                restartButton.SetActive(false);
                resultText.text = "";
                Invoke(nameof(ShowWinnerText), 4f);
                Invoke(nameof(TransitionToBoard), 4.5f);
                currentTurn = PlayerTurn.ShowResult;
            }

            currentTurn = PlayerTurn.ShowResult;
        }
    }

    string DetermineWinner(int p1, int p2)
    {
        if (p1 == p2)
        {
            lastWinner = "None";
            return "تعادل!";
        }

        if ((p1 == 0 && p2 == 2) || (p1 == 1 && p2 == 0) || (p1 == 2 && p2 == 1))
        {
            lastWinner = "Player1";
            return "اللاعب الأول فاز!";
        }

        lastWinner = "Player2";
        return "اللاعب الثاني فاز!";
    }

    private void ShowRestartButton()
    {
        restartButton.SetActive(true);
    }

    private void ShowResultTextAndRestartButton()
    {
        resultText.text = "تعادل!";
        restartButton.SetActive(true);
    }

    private void ShowWinnerText()
    {
        resultText.text = DetermineWinner(player1Choice, player2Choice);
    }

    public void RestartGame()
    {
        player1Choice = -1;
        player2Choice = -1;
        currentTurn = PlayerTurn.Player1;

        panelResult.SetActive(false);
        panelPlayer1.SetActive(true);

        player1HandObject.SetActive(true);
        player2HandObject.SetActive(true);

        player1HandController.ResetButtons();
        player2HandController.ResetButtons();

        restartButton.SetActive(false);

        ActivateNextPawnChoice(); // ✅ تفعيل الحجر المناسب حسب الفائز
    }

    private void TransitionToBoard()
    {
        rpsPanel.SetActive(false);
        boardPanel.SetActive(true);

        whoStarts = resultText.text == "اللاعب الأول فاز!" ? 2 : 1;

        GameObject.Find("GameUIController").GetComponent<GameUIController>().StartGame();

        int stonesLeft = (whoStarts == 2)
            ? GameEngine.Instance.GameState.SecondPlayersPawnsToPlaceLeft
            : GameEngine.Instance.GameState.FirstPlayersPawnsToPlaceLeft;

        if (stonesLeft >= 0 && stonesLeft < 9)
        {
            if (whoStarts == 2)
                yellowPawns[stonesLeft].SetActive(false);
            else
                redPawns[stonesLeft].SetActive(false);
        }
    }














    public void ActivateNextPawnChoice()
    {
        if (lastWinner == "Player1")
        {
            redPawnContainer.SetActive(true);
            yellowPawnContainer.SetActive(false);
        }
        else if (lastWinner == "Player2")
        {
            redPawnContainer.SetActive(false);
            yellowPawnContainer.SetActive(true);
        }
        else
        {
            redPawnContainer.SetActive(false);
            yellowPawnContainer.SetActive(false);
        }
    }
}

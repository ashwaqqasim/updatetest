using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RockPaperScissorsOnline : MonoBehaviour
{
    public Image player1Image, player2Image;
    public Sprite[] choicesSprites; // [Rock, Paper, Scissors]
    public Button rockButton, paperButton, scissorsButton;
    public TextMeshProUGUI resultText;
    public GameObject rpsPanel, boardPanel;

    private bool hasChosen = false;
    private string[] choices = { "rock", "paper", "scissors" };

    void Start()
    {
        resultText.text = "";
        boardPanel.SetActive(false);
        rpsPanel.SetActive(true);

        rockButton.onClick.AddListener(() => Choose("rock"));
        paperButton.onClick.AddListener(() => Choose("paper"));
        scissorsButton.onClick.AddListener(() => Choose("scissors"));
    }

    void Choose(string choice)
    {
        if (hasChosen) return;

        hasChosen = true;

        // إرسال الاختيار
        OnlineGameManager.Instance.SetMyChoice(choice);
        UpdateImage(choice, player1Image);
        resultText.text = "Waiting for opponent...";

        // نبدأ التحقق من الخصم
        InvokeRepeating(nameof(CheckOpponentChoice), 1f, 1f);
    }

    void CheckOpponentChoice()
    {
        var mgr = OnlineGameManager.Instance;
        if (!string.IsNullOrEmpty(mgr.opponentChoice))
        {
            CancelInvoke(nameof(CheckOpponentChoice));
            UpdateImage(mgr.opponentChoice, player2Image);
            ShowResult();
        }
    }


    void ShowResult()
    {
        var mgr = OnlineGameManager.Instance;
        string result;

        if (mgr.myChoice == mgr.opponentChoice)
        {
            result = "Draw! Try again.";
        }
        else if (mgr.isMyTurn)
        {
            result = "You go first!";
        }
        else
        {
            result = "Opponent goes first.";
        }

        resultText.text = result;

        Invoke(nameof(ActivateBoard), 3f);
    }


    void ActivateBoard()
    {
        rpsPanel.SetActive(false);
        boardPanel.SetActive(true);

        RPSGameManager.whoStarts = OnlineGameManager.Instance.isMyTurn ? 1 : 2;

        // 🟢 تحديد من يبدأ الدور في GameEngine
        PlayerNumber starter = OnlineGameManager.Instance.isMyTurn ? PlayerNumber.FirstPlayer : PlayerNumber.SecondPlayer;
        GameEngine.Instance.GameState.CurrentMovingPlayer = starter;

        // 🔄 تحديث GameState يدويًا لبدء اللعبة بعد تعيين من يبدأ
        GameEngine.Instance.GameState.TriggerGameStateChanged();
    }

    void UpdateImage(string choice, Image target)
    {
        int index = System.Array.IndexOf(choices, choice);
        if (index >= 0 && index < choicesSprites.Length)
        {
            target.sprite = choicesSprites[index];
        }
    }
}

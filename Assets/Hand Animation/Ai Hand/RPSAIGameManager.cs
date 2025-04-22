using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class RPSAIGameManager : MonoBehaviour
{
    public AIHandController aiHandController;
    public HumanHandController playerHandController;
    
    public GameObject panelPlayer;
    public GameObject panelResult;
    public Text resultText;
    public GameObject restartButton;

    private int playerChoice = -1;
    private int aiChoice = -1;

    public void OnPlayerChoice(int choice)
    {
        playerChoice = choice;
        panelPlayer.SetActive(false);
        panelResult.SetActive(true);

        playerHandController.PlayChoice(playerChoice); // أنيميشن اللاعب
        aiChoice = aiHandController.PlayRandomChoiceOnlyAnimation(); // أنيميشن AI فقط

        Invoke(nameof(AIPlays), 1.5f); // بعدها نحدد النتيجة
    }


    private void AIPlays()
    {
        aiChoice = aiHandController.PlayRandomChoice();

        string result = DetermineWinner(playerChoice, aiChoice);
        resultText.text = result;

        restartButton.SetActive(false); // نخفيه أول شي

        if (result == "تعادل!")
        {
            Invoke(nameof(ShowRestartButton), 2f); // يظهر بعد الأنيميشن
        }
        else
        {
            RPSGameManager.whoStarts = (result == "أنت الفائز!") ? 1 : 2;
            Invoke(nameof(StartTraditionalGame), 2.5f);
        }
    }


    public void ReplayRPS()
    {
        // إعادة الأيدي للوضع الأساسي
        playerHandController.ResetButtons();
        aiHandController.ResetAIHand();

        // إعادة الحالة
        panelPlayer.SetActive(true);
        panelResult.SetActive(false);
        restartButton.SetActive(false);

        playerChoice = -1;
        aiChoice = -1;
    }


    private void StartTraditionalGame()
    {
        GameUIController ui = GameObject.Find("GameUIController").GetComponent<GameUIController>();
        ui.StartGame();

    }

    private void ShowRestartButton()
    {
        restartButton.SetActive(true);
    }

    string DetermineWinner(int p1, int ai)
    {
        if (p1 == ai) return "تعادل!";
        if ((p1 == 0 && ai == 2) || (p1 == 1 && ai == 0) || (p1 == 2 && ai == 1))
            return "أنت الفائز!";
        return "الذكاء الاصطناعي فاز!";
    }
}



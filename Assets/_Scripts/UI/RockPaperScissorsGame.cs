using UnityEngine;
using UnityEngine.UI;
using TMPro; // لا تنسي إضافة هذا السطر لاستخدام TextMeshPro

public class RockPaperScissorsGame : MonoBehaviour
{
    public Image player1Image, player2Image;  // صور اللاعبين
    public Sprite[] choicesSprites; // مصفوفة الصور (حجر - ورقة - مقص)
    public Button player1Button, player2Button, restartButton; // أزرار الضغط + زر الإعادة
    public TextMeshProUGUI resultText; // نص النتيجة

    private string[] choices = { "Rock", "Paper", "Scissors" };
    private string player1Choice = "", player2Choice = "";
    private bool player1Locked = false, player2Locked = false;
    private int currentSpriteIndex = 0;

    void Start()
    {
        // جعل الصور تتحرك بسرعة
        InvokeRepeating("ChangeImages", 0.1f, 0.1f);

        // إخفاء النتيجة وزر الإعادة في البداية
        resultText.text = "";
        restartButton.gameObject.SetActive(false);

        // ربط الأزرار بوظائف التوقف
        player1Button.onClick.AddListener(() => LockChoice(1));
        player2Button.onClick.AddListener(() => LockChoice(2));
        restartButton.onClick.AddListener(RestartGame);
    }

    void ChangeImages()
    {
        // تغيير الصورة بسرعة ليبدو وكأنها تتحرك
        currentSpriteIndex = (currentSpriteIndex + 1) % choicesSprites.Length;

        if (!player1Locked)
            player1Image.sprite = choicesSprites[currentSpriteIndex];
        if (!player2Locked)
            player2Image.sprite = choicesSprites[currentSpriteIndex];
    }

    void LockChoice(int player)
    {
        // إيقاف الصورة العشوائية واختيار واحدة منها
        int finalChoiceIndex = Random.Range(0, choices.Length);

        if (player == 1 && !player1Locked)
        {
            player1Image.sprite = choicesSprites[finalChoiceIndex];
            player1Choice = choices[finalChoiceIndex];
            player1Locked = true;
        }
        else if (player == 2 && !player2Locked)
        {
            player2Image.sprite = choicesSprites[finalChoiceIndex];
            player2Choice = choices[finalChoiceIndex];
            player2Locked = true;
        }

        // عند اختيار اللاعبين الاثنين، يتم تحديد الفائز
        if (player1Locked && player2Locked)
        {
            DetermineWinner();
        }
    }

    void DetermineWinner()
    {
        string result;

        if (player1Choice == player2Choice)
        {
            result = "Draw! Try again.";
            restartButton.gameObject.SetActive(true); // إظهار زر الإعادة عند التعادل
        }
        else if ((player1Choice == "Rock" && player2Choice == "Scissors") ||
                 (player1Choice == "Scissors" && player2Choice == "Paper") ||
                 (player1Choice == "Paper" && player2Choice == "Rock"))
        {
            result = "🏆 Player 1 Wins!";
        }
        else
        {
            result = "🏆 Player 2 Wins!";
        }

        // عرض النتيجة
        resultText.text = result;
    }

    void RestartGame()
    {
        // إعادة ضبط القيم الأساسية
        player1Locked = false;
        player2Locked = false;
        player1Choice = "";
        player2Choice = "";
        resultText.text = "";
        restartButton.gameObject.SetActive(false); // إخفاء زر الإعادة

        // إعادة تشغيل حركة الصور
        CancelInvoke("ChangeImages"); // إيقاف الحركة الحالية (لضمان إعادة التشغيل من جديد)
        InvokeRepeating("ChangeImages", 0.1f, 0.1f);
    }
}
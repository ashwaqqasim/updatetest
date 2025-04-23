using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;


public class GameUIController : MonoBehaviour
{

    private static readonly int HUMAN_DROPDOWN_NUMBER = 0;
    private static readonly int AI_DROPDOWN_NUMBER = 1;
    private static readonly int MIN_MAX_DROPDOWN_NUMBER = 0;
    private static readonly int ALPHA_BETA_DROPDOWN_NUMBER = 1;
    private static readonly int FAST_ALPHA_BETA_DROPDOWN_NUMBER = 1;

    private static Dictionary<int, Func<Heuristic>> heuristicDictionary;
    [SerializeField] private GameObject[] yellowPawns; // الأحجار الصفراء
    [SerializeField] private GameObject[] redPawns; // الأحجار الحمراء

    [SerializeField] private TMP_Dropdown firstPlayerTypeDropdown = null;
    [SerializeField] private TMP_Dropdown firstPlayerAlgorithmDropdown = null;
    [SerializeField] private TMP_Dropdown firstPlayerHeuristicDropdown = null;
    [SerializeField] private TMP_Dropdown firstPlayerSearchDepthDropdown = null;

    [SerializeField] private TMP_Dropdown secondPlayerTypeDropdown = null;
    [SerializeField] private TMP_Dropdown secondPlayerAlgorithmDropdown = null;
    [SerializeField] private TMP_Dropdown secondPlayerHeuristicDropdown = null;
    [SerializeField] private TMP_Dropdown secondPlayerSearchDepthDropdown = null;

    [SerializeField] private TextMeshProUGUI numberOfMovesText = null;
    [SerializeField] private TextMeshProUGUI timerText = null;

    [SerializeField] private string numberOfMovesTemplateText = "Moves: {0}";
    [SerializeField] private string timerTemplateText = "Time[s]: {0}";
    [SerializeField] private string currentMovingPlayerTemplateText = "Turn: Player {0}";
    [SerializeField] private TextMeshProUGUI currentMovingPlayerText = null;

    [SerializeField] private Button playButton = null;
    [SerializeField] private Toggle logToFileToggle = null;

    [SerializeField] private Button[] pawnButtons = null;

    [SerializeField] private Sprite firstPlayerPawnImage = null;
    [SerializeField] private Sprite secondPlayerPawnImage = null;
    [SerializeField] private Sprite emptyField = null;
    [SerializeField] private string winningPlayerTextTemplate = "Won: ";
    [SerializeField] private TextMeshProUGUI winningPlayerTextField = null;
    // المتغيرات الخاصة بالـ Popups
    [SerializeField] private GameObject gameModePopup; // يحدد محلي أو المحنكة
    [SerializeField] private GameObject difficultyPopup; // يحدد مستوى الصعوبة
    [SerializeField] private GameObject startPopup; // يحتوي على زر "ابدأ اللعب"
    [SerializeField] private GameObject board;

    [SerializeField] private Button localButton;
    [SerializeField] private Button m7nkaButton;
    [SerializeField] private Button easyButton;
    [SerializeField] private Button mediumButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button startGameButton;

    [SerializeField] private GameObject gameModePanel;
    [SerializeField] private GameObject rpsPanel;
    [SerializeField] private GameObject boardPanel;

    [SerializeField] private GameObject profileMask; 
    [SerializeField] private Image profileImagePreview;
    [SerializeField] private Image aiAvatarImage;
    [SerializeField] private TextMeshProUGUI profileNameText;
    [SerializeField] private TextMeshProUGUI aiNameText;

    [SerializeField] private TextMeshProUGUI profileScoreText;
    [SerializeField] private GameObject scoreimg;


    private string aiPlayerName = "Muhankah";
    public GameObject RPSAI_Panel;
    public GameObject mainMenuPanel;


    private Color emptyColor = new Color(255, 255, 255, 0);
    private Color nonEmptyColor = new Color(255, 255, 255, 255);

    private Color firstPlayerColor = new Color(255, 255, 255, 255);
    private Color secondPlayerColor = new Color(0, 0, 0, 255);




    private float timePassed;
    private bool shouldLogToFile;
    private GameEngine gameEngine = null;
    private PlayersController aiPlayersController = null;


    private bool isAI = false;
    private int searchDepth = 1; // افتراضيًا يكون سهل
    private GameObject selectedPawn = null; // الحجر الذي يتم سحبه
    private Vector3 lastValidPosition; // آخر موقع صالح للحجر
    private CanvasGroup canvasGroup;
    [SerializeField] private Transform[] allowedPositions; // جميع أماكن الأحجار على البورد
    private int player1Stones = 9;
    private int player2Stones = 9;
    [SerializeField] private GameObject lobbyPanel;

    private static Dictionary<Vector3, GameObject> occupiedPositions = new Dictionary<Vector3, GameObject>(); // تخزين الأحجار في البورد


static GameUIController()
    {
        heuristicDictionary = new Dictionary<int, Func<Heuristic>>();
        heuristicDictionary[0] = () => new SimplePawnNumberHeuristic();
        heuristicDictionary[1] = () => new PawnMillNumberHeuristic();
        heuristicDictionary[2] = () => new PawnMoveNumberHeuristic();
    }
    private void Awake()
    {
        profileMask.SetActive(false); // عشان ما تطلع إلا لما يتحقق الشرط
        aiAvatarImage.gameObject.SetActive(false);
        profileNameText.gameObject.SetActive(false);
        profileScoreText.gameObject.SetActive(false);
        board.SetActive(false); // تعطيل البورد عند بدء اللعبة

        InitPawnButtonHandlers();

        localButton.onClick.AddListener(SelectLocal);
        m7nkaButton.onClick.AddListener(SelectM7nka);
        easyButton.onClick.AddListener(() => SelectDifficulty(1));
        mediumButton.onClick.AddListener(() => SelectDifficulty(2));
        hardButton.onClick.AddListener(() => SelectDifficulty(3));
        startGameButton.onClick.AddListener(StartGame);

        ShowGameModePopup();
        gameModePanel.SetActive(true);  // تظهر أول شي
        rpsPanel.SetActive(false);      // مخفية بالبداية
        boardPanel.SetActive(false);    // البورد مخفي بالبداية
        startPopup.SetActive(false);
        RPSAI_Panel.SetActive(false);
        difficultyPopup.SetActive(false);
        

        InitPawnButtonHandlers();

        localButton.onClick.AddListener(ShowRPSPanel);
    }

    private void InitPawnButtonHandlers()
    {
        for (int i = 0; i < pawnButtons.Length; i++)
        {
            int x = i;
            pawnButtons[i].onClick.AddListener(() => HandleButtonClick(x));
        }
    }
    public void UpdateStonesUI(int player)
    {
        if (GameEngine.Instance == null) return; // تأكد أن GameEngine موجود

        int stonesLeft = (player == 1) ? GameEngine.Instance.GameState.FirstPlayersPawnsToPlaceLeft
                                       : GameEngine.Instance.GameState.SecondPlayersPawnsToPlaceLeft;

        Debug.Log($"🎯 تحديث للأحجار! لاعب {player} لديه {stonesLeft} حجر متبقي.");

        if (stonesLeft >= 0 && stonesLeft < yellowPawns.Length)
        {
            if (player == 1)
            {
                yellowPawns[stonesLeft].SetActive(false);
            }
            else
            {
                redPawns[stonesLeft].SetActive(false);
            }
        }
    }

    public void OnPlayWithFriendClicked()
    {
        lobbyPanel.SetActive(true);

        gameModePopup.SetActive(false);
    }

    private void ShowGameModePopup()
    {
        gameModePopup.SetActive(true);
        difficultyPopup.SetActive(false);
    }

    public void SelectLocal()
    {
        isAI = false;
        gameModePopup.SetActive(false);

        // بدلًا من StartGame مباشرة، ننتقل لشاشة RPS
        ShowRPSPanel();
    }


    public void SelectM7nka()
    {
        isAI = true;
        gameModePopup.SetActive(false);
        ShowDifficultyPopup(); // ❌ لا تشغل RPS هنا
    }



    private void ShowDifficultyPopup()
    {
        difficultyPopup.SetActive(true);
    }


    public void SelectDifficulty(int depth)
    {
        searchDepth = depth;
        difficultyPopup.SetActive(false);
        ShowRPSAI(); // ✅ الحين بس تشغل حجرة ورقة مقص بعد ما يختار الصعوبة
        if (isAI)
        {
            LoadProfileImagePreview();
        }
        if (isAI && aiAvatarImage != null)
        {
            aiAvatarImage.gameObject.SetActive(true);
        }
        if (isAI && aiNameText != null)
        {
            aiNameText.text = aiPlayerName;
            aiNameText.gameObject.SetActive(true);
        }

    }







    // ✅ زر "كانسل" في `Game Mode Popup`
    public void CancelGameModePopup()
    {
        gameModePopup.SetActive(false);

        SceneManager.LoadScene("um9 menu page");
    }

    // ✅ زر "كانسل" في `Difficulty Popup` يعيد اللاعب إلى `Game Mode Popup`
    public void CancelDifficultyPopup()
    {
        difficultyPopup.SetActive(false);
        ShowGameModePopup();
    }


    public void StartGame()
    {
        // 🔥 أول شيء: نخفي كل الواجهات غير البورد
        gameModePanel.SetActive(false);
        difficultyPopup.SetActive(false);
        rpsPanel.SetActive(false);
        RPSAI_Panel.SetActive(false);
        boardPanel.SetActive(true); // ✅ نفعّل البورد فقط

        board.SetActive(true); // تفعيل البورد عند بدء اللعب

        gameEngine = new GameEngine();

        // ✅ تحديد من الفائز في RPS ليبدأ أولًا
        if (RPSGameManager.whoStarts == 1)
        {
            Debug.Log("🎯 الدور الأول للاعب 1 (بفوزه في حجرة ورقة مقص)");
            gameEngine.GameState.CurrentMovingPlayer = PlayerNumber.FirstPlayer;
        }
        else
        {
            Debug.Log("🎯 الدور الأول للاعب 2 (بفوزه في حجرة ورقة مقص)");
            gameEngine.GameState.CurrentMovingPlayer = PlayerNumber.SecondPlayer;
        }
        if (ScoreManager.Instance != null && profileScoreText != null)
        {
            profileScoreText.text = "" + ScoreManager.Instance.Score.ToString();
        }
        AiPlayer firstPlayer = null;
        AiPlayer secondPlayer = isAI
            ? new FastAlphaBetaAiPlayer(
                gameEngine,
                new SimplePawnNumberHeuristic(),
                PlayerNumber.SecondPlayer,
                searchDepth,
                new SimplePawnNumberHeuristic()
            )
            : null;

        aiPlayersController = new PlayersController(firstPlayer, secondPlayer);
        timePassed = 0;
        OnBoardUpdated(gameEngine.GameState.CurrentBoard);
        gameEngine.OnBoardChanged += OnBoardUpdated;
        gameEngine.OnGameFinished += OnGameFinished;
        gameEngine.OnPlayerTurnChanged += OnPlayerTurnChanged;
        gameEngine.OnPlayerTurnChanged += aiPlayersController.OnPlayerTurnChanged;
        gameEngine.OnLastFieldSelectedChanged += UpdatePossibleMoveIndicators;
        UpdateWinningPlayerText(PlayerNumber.None);
        playButton.interactable = false;

        // ✅ 🔹 تجهيز الأحجار لكل لاعب
        for (int i = 0; i < 9; i++)
        {
            yellowPawns[i].SetActive(true);
            redPawns[i].SetActive(true);
        }

        // ✅ 🔹 ضبط أماكن الأحجار
        GameObject allowedPositionsObject = GameObject.Find("AllowedPositions");
        if (allowedPositionsObject != null)
        {
            allowedPositions = allowedPositionsObject
                .GetComponentsInChildren<Transform>()
                .Where(t => t != allowedPositionsObject.transform)
                .ToArray();
        }

        Debug.Log("🎯 تم تجهيز الأحجار وإعداد البورد!");
        Debug.Log($"✅ بدأ اللعب: Player1 = Human | Player2 = {(isAI ? "AI" : "Human")} | Depth = {searchDepth}");
        if (gameEngine.GameState.CurrentMovingPlayer == PlayerNumber.SecondPlayer)
        {
            aiPlayersController.OnPlayerTurnChanged(PlayerNumber.SecondPlayer);
        }

    }

    private async void LoadProfileImagePreview()
    {
        profileMask.SetActive(false);
        profileNameText.gameObject.SetActive(false);

        if (UnityServices.State == Unity.Services.Core.ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        // تحميل كل البيانات دفعة وحدة
        var keys = new HashSet<string> { "ProfileImage", "PlayerName", "SelectedCharacter" };
        var savedData = await CloudSaveService.Instance.Data.LoadAsync(keys);

        // ✅ تحميل الصورة حسب الشخصية المختارة
        if (savedData.TryGetValue("SelectedCharacter", out var character))
        {
            string fileName = character == "girl" ? "girl.png" : "boy.png";
            string path = Path.Combine(Application.dataPath, $"Images/{fileName}");

            if (File.Exists(path))
            {
                byte[] imageBytes = File.ReadAllBytes(path);
                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(imageBytes);
                profileImagePreview.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                profileMask.SetActive(true);
            }
        }

        // ✅ تحميل الاسم
        if (savedData.TryGetValue("PlayerName", out var playerName))
        {
            profileNameText.text = playerName;
            profileNameText.gameObject.SetActive(true);
        }

        // ✅ عرض السكور
        if (ScoreManager.Instance != null && profileScoreText != null)
        {
            profileScoreText.text = "" + ScoreManager.Instance.Score.ToString();
            profileScoreText.gameObject.SetActive(true);
        }
        if (ScoreManager.Instance != null && profileScoreText != null)
        {
            profileScoreText.text = "" + ScoreManager.Instance.Score.ToString();
            profileScoreText.gameObject.SetActive(true);

            if (scoreimg != null)
            {
                scoreimg.SetActive(true); // ⬅️ يظهر الصورة الخلفية
            }
        }

    }






    private AiPlayer InitPlayer(PlayerNumber playerNumber)
    {
        // اللاعب الأول يكون إنسان دائمًا، لذلك نعيد null إذا كان PlayerNumber.FirstPlayer
        if (playerNumber == PlayerNumber.FirstPlayer)
        {
            return null;
        }

        // اللاعب الثاني يكون ذكاءً اصطناعيًا فقط إذا تم اختيار "المحنكة"
        if (isAI)
        {
            Heuristic bestHeuristic = new PawnMillNumberHeuristic(); // أفضل Heuristic دائمًا
            Heuristic sortHeuristic = new SimplePawnNumberHeuristic(); // يتم استخدامه للفرز

            // إنشاء لاعب الذكاء الاصطناعي مع مستوى الصعوبة المحدد
            return new FastAlphaBetaAiPlayer(gameEngine, bestHeuristic, playerNumber, searchDepth, sortHeuristic);
        }

        return null; // في حالة اللعب المحلي، لا يوجد AI
    }

    public void ShowRPSPanel()
    {
        gameModePanel.SetActive(false);
        rpsPanel.SetActive(true);
        boardPanel.SetActive(false);
    }

    public void ShowRPSAI()
    {
        RPSAI_Panel.SetActive(true);

        // 🔴 طفي كل شيء ثاني
        gameModePanel.SetActive(false);
        rpsPanel.SetActive(false);
        difficultyPopup.SetActive(false);
    }

    private void OnBoardUpdated(Board newBoard)
    {
        for (int i = 0; i < pawnButtons.Length; i++)
        {
            Field field = newBoard.GetField(i);
            if (field.PawnPlayerNumber == PlayerNumber.FirstPlayer)
            {
                pawnButtons[i].image.sprite = firstPlayerPawnImage;
                pawnButtons[i].image.color = nonEmptyColor;
            }
            else if (field.PawnPlayerNumber == PlayerNumber.SecondPlayer)
            {
                pawnButtons[i].image.sprite = secondPlayerPawnImage;
                pawnButtons[i].image.color = nonEmptyColor;
            }
            else
            {
                pawnButtons[i].image.color = emptyColor;
            }
        }
    }

    private void OnPlayerTurnChanged(PlayerNumber currentMovingPlayerNumber)
    {
        if (currentMovingPlayerNumber == PlayerNumber.FirstPlayer)
        {
            UpdateTurnText(1);
        }
        else
        {
            UpdateTurnText(2);
        }
    }

    private void UpdateTurnText(int playerNumber)
    {
        currentMovingPlayerText.text = string.Format(currentMovingPlayerTemplateText, playerNumber);
        currentMovingPlayerText.faceColor = playerNumber == 1 ? firstPlayerColor : secondPlayerColor;
    }

    public AudioSource winAudioSource;


    private void OnGameFinished(PlayerNumber winningPlayer)
    {
        UpdateWinningPlayerText(winningPlayer);
        SaveLogs();

        gameEngine.OnBoardChanged -= OnBoardUpdated;
        gameEngine.OnGameFinished -= OnGameFinished;
        gameEngine.OnPlayerTurnChanged -= OnPlayerTurnChanged;
        gameEngine.OnPlayerTurnChanged -= aiPlayersController.OnPlayerTurnChanged;
        gameEngine.OnLastFieldSelectedChanged -= UpdatePossibleMoveIndicators;

        gameEngine = null;
        aiPlayersController = null;

        playButton.interactable = true;

        // ضد الذكاء الاصطناعي
        if (isAI)
        {
            if (winningPlayer == PlayerNumber.FirstPlayer)
            {
                Debug.Log("🎉 اللاعب فاز ضد المحنكة - الانتقال إلى مشهد Mabrook");
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.ResetWinFlag();
                }

                SceneManager.LoadScene("Mabrook");
            }
            else if (winningPlayer == PlayerNumber.SecondPlayer)
            {
                Debug.Log("😢 اللاعب خسر ضد المحنكة - الانتقال إلى مشهد Loser");
                SceneManager.LoadScene("Loser");
            }

            if (winAudioSource != null)
            {
                winAudioSource.Play();
                StartCoroutine(LoadSceneAfterSound("Mabrook", winAudioSource.clip.length));
            }
            else
            {
                SceneManager.LoadScene("Mabrook");
            }
        }
        else if (winningPlayer == PlayerNumber.SecondPlayer && isAI)
        {
            if (winningPlayer == PlayerNumber.FirstPlayer)
            {
                Debug.Log("🎉 اليد اليسار فازت - الانتقال إلى MabrookLocal2");
                SceneManager.LoadScene("MabrookLocal2");
            }
            else if (winningPlayer == PlayerNumber.SecondPlayer)
            {
                Debug.Log("🎉 اليد اليمين فازت - الانتقال إلى MabrookLocal1");
                SceneManager.LoadScene("MabrookLocal1");
            }
        }
    }

    private IEnumerator LoadSceneAfterSound(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }


    private void SaveLogs()
    {
        if (shouldLogToFile)
        {
            string moves = gameEngine.GameState.MovesUntilNow;
            try
            {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt", moves);
            }
            catch (Exception e)
            {

            }
        }
    }

    private void UpdateWinningPlayerText(PlayerNumber winningPlayer)
    {
        Color winningPlayerColor = firstPlayerColor;
        string winningPlayerString = "Player 1";
        if (winningPlayer == PlayerNumber.SecondPlayer)
        {
            winningPlayerString = "Player 2";
            winningPlayerColor = secondPlayerColor;
        }
        else if (winningPlayer == PlayerNumber.None)
        {
            winningPlayerString = "";
        }
        winningPlayerTextField.text = winningPlayerTextTemplate + winningPlayerString;
        winningPlayerTextField.faceColor = winningPlayerColor;
    }

    private void HandleButtonClick(int fieldIndex)

    {

        if (gameEngine != null)
        {
            Debug.Log("🟡 اللاعب الحالي هو: " + gameEngine.GameState.CurrentMovingPlayer);
            gameEngine.HandleSelection(fieldIndex);
        }


        // ✅ شرط الأونلاين فقط: يمنع الضغط إذا مو دورك
        if (OnlineGameManager.Instance != null && !OnlineGameManager.Instance.isMyTurn)
        {
            Debug.Log("🚫 ليس دورك الآن!");
            return;
        }

        if (gameEngine != null)
        {
            Debug.Log("🟡 اللاعب الحالي هو: " + gameEngine.GameState.CurrentMovingPlayer);
            gameEngine.HandleSelection(fieldIndex);

            // ✅ إذا كنا أونلاين، نرسل الحركة للطرف الثاني ونعكس الدور
            if (OnlineGameManager.Instance != null)
            {
                OnlineGameManager.Instance.SendMoveToOpponent(fieldIndex); // (بنضيف هالدالة)
                OnlineGameManager.Instance.isMyTurn = false;
            }
        }
    }

    private void Update()
    {
        if (isAI) // فقط نفذ خطوة الذكاء الاصطناعي إذا كان هناك AI
        {
            MakeAiControllerStep();
        }

        // ✅ التحقق من وجود حركة أونلاين مستلمة
        if (OnlineGameManager.Instance != null)
        {
            int index = OnlineGameManager.Instance.receivedFieldIndex;
            if (index != -1)
            {
                Debug.Log("🟢 تنفيذ حركة من الخصم: " + index);

                if (gameEngine != null)
                {
                    gameEngine.HandleSelection(index);
                }

                OnlineGameManager.Instance.receivedFieldIndex = -1;
                OnlineGameManager.Instance.isMyTurn = true;
            }
        }

        UpdateGameStateData();
    }
    public void ForceMoveFromOpponent(int fieldIndex)
    {
        if (gameEngine != null)
        {
            Debug.Log("🛠️ تنفيذ حركة الخصم: " + fieldIndex);
            gameEngine.HandleSelection(fieldIndex);
        }
    }



    private void MakeAiControllerStep()
    {
        if (aiPlayersController != null)
        {
            long timeMilis = aiPlayersController.CheckStep();
            timePassed += timeMilis / 1000f;
        }
    }

    private void UpdateGameStateData()
    {
        if (gameEngine != null)
        {
            UpdateMoveNumberText();
            UpdateTime();
        }
    }

    private void UpdateTime()
    {
        if (!gameEngine.GameState.GameFinished)
        {
            timePassed += Time.deltaTime;
            timerText.text = string.Format(timerTemplateText, Math.Truncate(timePassed * 100) / 100);
        }
    }

    private void UpdateMoveNumberText()
    {
        numberOfMovesText.text = string.Format(numberOfMovesTemplateText, gameEngine.GameState.MovesMade);
    }

    private void UpdatePossibleMoveIndicators()
    {
        HashSet<int> possibleMoveIndices = gameEngine.GetCurrentPossibleMoves();
        if (possibleMoveIndices == null)
        {
            for (int i = 0; i < pawnButtons.Length; i++)
            {
                Image[] images = pawnButtons[i].GetComponentsInChildren<Image>();
                images[1].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < pawnButtons.Length; i++)
            {
                Image[] images = pawnButtons[i].GetComponentsInChildren<Image>();
                images[1].enabled = possibleMoveIndices.Contains(i);
            }
        }
    }


}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;

public class ProfileManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private Button editNameButton;
    [SerializeField] private Button editIDButton;
    [SerializeField] private Button saveButton;
    [SerializeField] private Image profileImage;
    [SerializeField] private Button selectBoyButton;
    [SerializeField] private Button selectGirlButton;
    [SerializeField] private TextMeshProUGUI scoreText;


    private string selectedCharacter = "default";
    private bool isNameEditing = false;
    private bool isIDEditing = false;
    [SerializeField] private GameObject goToGameButton;

    async void Start()
    {
        await InitializeUnityServices();
        await LoadDataFromCloud();

        if (ScoreManager.Instance != null)
        {
            await ScoreManager.Instance.LoadScoreFromCloud(); // ← حمل السكور بعد ما يكتمل تسجيل الدخول
            if (scoreText != null)
            {
                scoreText.text = " " + ScoreManager.Instance.Score.ToString();
            }
        }

        LoadDefaultProfileImage();
        nameInputField.interactable = false;
        idInputField.interactable = false;

        editNameButton.onClick.AddListener(ToggleNameEdit);
        editIDButton.onClick.AddListener(ToggleIDEdit);
        saveButton.onClick.AddListener(OnClick_Save);
        Invoke("LoadDefaultProfileImage", 0.1f);
        selectBoyButton.onClick.AddListener(SelectBoy);
        selectGirlButton.onClick.AddListener(SelectGirl);
    }

    private void LoadDefaultProfileImage()
    {
        if (profileImage == null)
        {
            Debug.LogError("❌ profileImage غير مضبوط في Inspector!");
            return;
        }

        // نستخدم الصورة الموجودة في Source Image مباشرة
        selectedCharacter = "default";
        Debug.Log("✅ تم استخدام الصورة الافتراضية الحالية.");
    }

    public void SelectBoy()
    {
        string path = Path.Combine(Application.dataPath, "Images/boy.png");

        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            profileImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            selectedCharacter = "boy";
            Debug.Log("👦 تم اختيار شخصية الولد.");
        }
        else
        {
            Debug.LogError("❌ صورة الولد غير موجودة في Assets/Images/");
        }
    }

    public void SelectGirl()
    {
        string path = Path.Combine(Application.dataPath, "Images/girl.png");

        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            profileImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            selectedCharacter = "girl";
            Debug.Log("👧 تم اختيار شخصية البنت.");
        }
        else
        {
            Debug.LogError("❌ صورة البنت غير موجودة في Assets/Images/");
        }
    }



    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
            await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

    }

    private async Task LoadDataFromCloud()
    {
        string fallbackID = AuthenticationService.Instance.PlayerId;
        var keys = new HashSet<string> { "PlayerName", "PlayerID", "SelectedCharacter" };
        var savedData = await CloudSaveService.Instance.Data.LoadAsync(keys);

        nameInputField.text = savedData.TryGetValue("PlayerName", out var nameValue) ? nameValue : "Player";
        idInputField.text = savedData.TryGetValue("PlayerID", out var idValue) ? idValue : fallbackID;

        if (savedData.TryGetValue("SelectedCharacter", out var charValue))
        {
            selectedCharacter = charValue;

            if (selectedCharacter == "boy")
                SelectBoy();
            else if (selectedCharacter == "girl")
                SelectGirl();
            else
                LoadDefaultProfileImage();
        }
        else
        {
            LoadDefaultProfileImage(); // fallback
        }
    }


    private void ToggleNameEdit()
    {
        isNameEditing = !isNameEditing;
        nameInputField.interactable = isNameEditing;
    }

    private void ToggleIDEdit()
    {
        isIDEditing = !isIDEditing;
        idInputField.interactable = isIDEditing;
    }

    public void OnClick_Save()
    {
        _ = SaveProfileToCloud().ContinueWith(_ =>
        {
            Debug.Log("✅ تم حفظ جميع البيانات في الكلاود، ويتم تحديث الصورة الآن...");
        });

    }

    public void GoToGamesPage()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Games page");
    }


    private async Task SaveProfileToCloud()
    {
        string fixedName = Regex.Replace(nameInputField.text.Trim(), "[^a-zA-Z0-9 _-]", "");
        string fixedID = idInputField.text.Trim();

        var dataToSave = new Dictionary<string, object>
    {
        { "PlayerName", fixedName },
        { "PlayerID", fixedID },
        { "SelectedCharacter", selectedCharacter } // نحفظ فقط اسم الشخصية
    };

        nameInputField.interactable = false;
        idInputField.interactable = false;
        isNameEditing = false;
        isIDEditing = false;

        Debug.Log($"📝 Saving to cloud:\nName: {fixedName}, ID: {fixedID}, Character: {selectedCharacter}");

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(dataToSave);
            Debug.Log("✅ تم حفظ جميع البيانات في الكلاود!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ فشل الحفظ في الكلاود: {ex.Message}");
        }
    }


}

using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.CloudSave;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AccountManager : MonoBehaviour
{
    private static string playerIDKey = "PlayerID";
    private static string guestKey = "IsGuest";
    private static string playerNameKey = "PlayerName";
    private static string playerScoreKey = "PlayerScore";
    [SerializeField] private GameObject WarningImage; // صورة التحذير
    [SerializeField] private GameObject aiButton; // زر المحنكه
    public static bool isGuest = true;




    void Start()
    {
        DontDestroyOnLoad(gameObject);
        InitializeUnityServices();
        // تأكد أن زر المحنكه يظهر دائمًا
        if (aiButton != null)
        {
            aiButton.SetActive(true);
        }
    }

    private async Task InitializeUnityServices()
    {
        if (UnityServices.State == ServicesInitializationState.Uninitialized)
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            PlayerPrefs.SetString(playerIDKey, AuthenticationService.Instance.PlayerId);
            PlayerPrefs.Save();
        }

        /* // ✅ منع تغيير المشهد إذا اللاعب ضيف
         if (!IsGuest() && PlayerPrefs.HasKey(playerIDKey))
         {
             Scene currentScene = SceneManager.GetActiveScene();
             if (currentScene.name != "ProfileScene")
             {
                 SceneManager.LoadScene("ProfileScene");
             }
         }
     } */
    }

    public void OpenProfileScene()
    {
        SceneManager.LoadScene("ProfileScene");
    }

    public static bool IsGuest()
    {
        return PlayerPrefs.GetInt(guestKey, 1) == 1;
    }

    public async void Register(string playerName)
    {
        PlayerPrefs.SetInt(guestKey, 0);
        isGuest = false; // ✅ نلغي حالة الضيف هنا

        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("❌ Please enter a valid name!");
            return;
        }

        await InitializeUnityServices();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        string newPlayerId = AuthenticationService.Instance.PlayerId;

        PlayerPrefs.SetString(playerNameKey, playerName);
        PlayerPrefs.SetString(playerIDKey, newPlayerId);
        PlayerPrefs.SetInt(guestKey, 0);
        PlayerPrefs.SetInt(playerScoreKey, 0);
        PlayerPrefs.Save();

        Debug.Log($"✅ Account created: {playerName} - ID: {newPlayerId}");
        SceneManager.LoadScene("ProfileScene");
    }

    public void PlayAsGuest()
    {
        if (!PlayerPrefs.HasKey(playerIDKey))
        {
            string guestID = "Guest_" + Guid.NewGuid().ToString();
            PlayerPrefs.SetString(playerIDKey, guestID);
        }

        PlayerPrefs.SetInt(guestKey, 1);
        PlayerPrefs.Save();

        isGuest = true; // ✅ لازم نخزنها بالمتغير الثابت
        Debug.Log("🚀 تسجيل الدخول كضيف.");
        SceneManager.LoadScene("Um9 menu page");
    }


    [SerializeField] private GameObject warningPanel;
    [SerializeField] private TMPro.TMP_Text warningText;

    public void TryEnterGameMode(string sceneName)
    {
        if (isGuest && sceneName != "Um9 menu page")
        {
            ShowWarningImage();
            Debug.LogWarning("⚠️ الضيف لا يمكنه الدخول لهذا الطور!");
            return;
        }

 

        SceneManager.LoadScene(sceneName);
    }
    public void RegisterAsRealPlayer()
    {
        isGuest = false; // ✅ كأنك فتحت القفل
        PlayerPrefs.SetInt("Guest", 0); // نخزنها لمرات الجاي
        Debug.Log("✅ تم تفعيل الحساب كلاعب مسجل.");
    }



    public void ShowWarningImage()
    {
        if (WarningImage != null)
        {
            WarningImage.SetActive(true);
            Invoke("HideWarningImage", 3f); // إخفاء الصورة بعد 3 ثوانٍ تلقائياً
        }
    }

    private void HideWarningImage()
    {
        WarningImage.SetActive(false);
    }


    private void HideWarningMessage()
    {
        warningPanel.SetActive(false);
    }

    public static string GetPlayerName()
    {
        return PlayerPrefs.GetString(playerNameKey, "Unknown Player");
    }

    public static string GetPlayerID()
    {
        return PlayerPrefs.GetString(playerIDKey, "UnknownPlayer");
    }

    public static int GetPlayerScore()
    {
        return PlayerPrefs.GetInt(playerScoreKey, 0);
    }

    public static void UpdateScore(int score)
    {
        int currentScore = PlayerPrefs.GetInt(playerScoreKey, 0);
        currentScore += score;
        PlayerPrefs.SetInt(playerScoreKey, currentScore);
        PlayerPrefs.Save();
    }

    public async Task UpdatePlayerIDCloud(string newID)
    {
        if (string.IsNullOrEmpty(newID))
        {
            Debug.Log("❌ Please enter a valid Player ID!");
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "PlayerID", newID }
        };

        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        Debug.Log($"✅ Player ID updated in cloud: {newID}");
    }

    public async Task<string> LoadPlayerIDFromCloud()
    {
        var keys = new HashSet<string> { "PlayerID" };
        var savedData = await CloudSaveService.Instance.Data.LoadAsync(keys);

        if (savedData.TryGetValue("PlayerID", out var id))
        {
            return id;
        }

        return "UnknownPlayer";
    }


    public async Task UpdatePlayerNameCloud(string newName)
    {
        if (string.IsNullOrEmpty(newName))
        {
            Debug.Log("❌ Please enter a valid name!");
            return;
        }

        var data = new Dictionary<string, object>
        {
            { "PlayerName", newName }
        };

        await CloudSaveService.Instance.Data.ForceSaveAsync(data);
        Debug.Log($"✅ Player name updated in cloud: {newName}");
    }

    public async Task<string> LoadPlayerNameFromCloud()
    {
        var keys = new HashSet<string> { "PlayerName" };
        var savedData = await CloudSaveService.Instance.Data.LoadAsync(keys);

        if (savedData.TryGetValue("PlayerName", out var name))
        {
            return name;
        }

        return "Unknown Player";
    }   
    
}

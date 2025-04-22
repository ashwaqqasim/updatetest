using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public int Score { get; private set; }
    private bool scoreGivenForWin = false;

    private string playerID;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // لا تفعل شيء هنا
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async Task InitAndLoadScore()
    {
        await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
          

        playerID = AuthenticationService.Instance.PlayerId;
        await LoadScoreFromCloud();
    }

    private string GetScoreKey()
    {
        return "Score_" + playerID;
    }

    public async void SaveScoreToCloud()
    {
        var data = new Dictionary<string, object> { { GetScoreKey(), Score } };

        try
        {
            await CloudSaveService.Instance.Data.ForceSaveAsync(data);
            Debug.Log("✅ تم حفظ السكور لـ " + playerID + ": " + Score);
        }
        catch (Exception e)
        {
            Debug.LogError("❌ خطأ أثناء حفظ السكور: " + e.Message);
        }
    }

    public async Task LoadScoreFromCloud()
    {
        var result = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> { GetScoreKey() });

        if (result.TryGetValue(GetScoreKey(), out var scoreValue))
        {
            Score = Convert.ToInt32(scoreValue);
            Debug.Log("📥 تم تحميل السكور لـ " + playerID + ": " + Score);
        }
        else
        {
            Debug.Log("ℹ️ لا يوجد سكور محفوظ لهذا المستخدم، تعيين إلى 0");
            Score = 0;
        }
    }

    public void AddScoreOncePerWin(int amount)
    {
        if (!scoreGivenForWin)
        {
            Score += amount;
            SaveScoreToCloud();
            scoreGivenForWin = true;
        }
    }

    public void ResetWinFlag()
    {
        scoreGivenForWin = false;
    }
}

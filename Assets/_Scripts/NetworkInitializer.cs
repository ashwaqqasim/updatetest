using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class NetworkInitializer : MonoBehaviour
{
    public static bool isInitialized = false;

    async void Awake()
    {
        if (!isInitialized)
        {
            await InitializeUnityServices();
            isInitialized = true;
        }
    }

    private async Task InitializeUnityServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("✅ Signed in as: " + AuthenticationService.Instance.PlayerId);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Unity Services failed: " + e.Message);
        }
    }
}


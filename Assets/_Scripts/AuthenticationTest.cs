using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class AuthenticationTest : MonoBehaviour
{
    async void Start()
    {
        await InitializeUnityServices();

        if (AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log($"✅ تسجيل الدخول ناجح! معرف اللاعب: {AuthenticationService.Instance.PlayerId}");
        }
        else
        {
            Debug.Log("❌ لم يتم تسجيل الدخول.");
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
            Debug.Log($"🔹 تسجيل دخول مجهول: {AuthenticationService.Instance.PlayerId}");
        }
    }
}


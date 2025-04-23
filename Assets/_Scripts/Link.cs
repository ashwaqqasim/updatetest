using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // لاستيراد Button

public class Link : MonoBehaviour
{
    public AudioSource audioSource; // مرجع لمكون AudioSource
    public AudioClip clickSound; // مرجع للصوت (AudioClip)
    private Button button; // مرجع للزر

       void Start()
    {
        // 👇 تحميل مستوى الصوت المحفوظ وتطبيقه على AudioSource
        float savedVolume = PlayerPrefs.GetFloat("BoardVolume", 1f);
        if (audioSource != null)
        {
            audioSource.volume = savedVolume;
        }

        // ربط الزر بالصوت
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => PlaySound());
        }
    }

    private void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            Debug.Log("Playing sound: " + clickSound.name);
            audioSource.PlayOneShot(clickSound);
        }
        else
        {
            Debug.Log("AudioSource or ClickSound is missing!");
        }
    }

    public void PlaySoundAndLoadScene(string sceneName)
    {
        PlaySound();
        SceneManager.LoadScene(sceneName);
    }

    public void GoToGamesPage() => PlaySoundAndLoadScene("Games page");
    public void GoToFristPage() => PlaySoundAndLoadScene("frist");
    public void GoToSecondPage() => PlaySoundAndLoadScene("second");
    public void GoToThirdPage() => PlaySoundAndLoadScene("Third");
    public void GoToFourthPage() => PlaySoundAndLoadScene("Fourth");
    public void GoToFifthPage() => PlaySoundAndLoadScene("Fifth");
    public void GoToSixthPage() => PlaySoundAndLoadScene("Sixth");
    public void GoToSeventhPage() => PlaySoundAndLoadScene("Seventh");
    
    public void GoToChooseTheCharacterPage() => PlaySoundAndLoadScene("Choose");

    public void GoToChooseTheCharacterPageback() => PlaySoundAndLoadScene("um9 menu page");
    
}




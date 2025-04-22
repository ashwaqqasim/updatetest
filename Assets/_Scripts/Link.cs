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
        // الحصول على مكون الزر وإضافة مستمع للنقر
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

    private void PlaySoundAndLoadScene(string sceneName)
    {
        PlaySound();
        SceneManager.LoadScene(sceneName); 
    }

    public void GoToGamesPage() => PlaySoundAndLoadScene("Games page");
    public void GoToFristPage() => PlaySoundAndLoadScene("Frist");
    public void GoToSecondPage() => PlaySoundAndLoadScene("Second");
    public void GoToThirdPage() => PlaySoundAndLoadScene("Third");
    public void GoToFourthPage() => PlaySoundAndLoadScene("Fourth");
    public void GoToFifthPage() => PlaySoundAndLoadScene("Fifth");
    public void GoToSixthPage() => PlaySoundAndLoadScene("Sixth");
    public void GoToSeventhPage() => PlaySoundAndLoadScene("Seventh");
    
    public void GoToChooseTheCharacterPage() => PlaySoundAndLoadScene("Choose");
}




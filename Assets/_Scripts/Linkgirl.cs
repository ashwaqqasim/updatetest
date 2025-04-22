using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // لاستيراد Button

public class LinkGirl : MonoBehaviour
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
        PlaySound(); // تشغيل الصوت
        SceneManager.LoadScene(sceneName); // تحميل المشهد
    }

    public void GoToGamesPageGirl() => PlaySoundAndLoadScene("Games page");
    public void GoToFristPageGirl() => PlaySoundAndLoadScene("Frist Girl");
    public void GoToSecondPageGirl() => PlaySoundAndLoadScene("Second Girl");
    public void GoToThirdPageGirl() => PlaySoundAndLoadScene("Third Girl");
    public void GoToFourthPageGirl() => PlaySoundAndLoadScene("Fourth Girl");
    public void GoToFifthPageGirl() => PlaySoundAndLoadScene("Fifth Girl");
    public void GoToSixthPageGirl() => PlaySoundAndLoadScene("Sixth Girl");
    public void GoToSeventhPageGirl() => PlaySoundAndLoadScene("Seventh Girl");
    public void GoToChooseTheCharacterPageGirl() => PlaySoundAndLoadScene("Choose");
}

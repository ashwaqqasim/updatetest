using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // ·Ê » ‰ ﬁ· ·„‘Âœ À«‰Ì

public class LoadingBar : MonoBehaviour
{
    public Slider slider;
    public float loadingSpeed = 0.3f;
    public string nextSceneName = "YourNextScene"; // €Ì—Â« ·Ê » ‰ ﬁ·

    void Start()
    {
        slider.value = 0f;
    }

    void Update()
    {
        if (slider.value < 1f)
        {
            slider.value += loadingSpeed * Time.deltaTime;
        }
        else
        {
            // ≈–«  »€Ï  ‰ ﬁ· »⁄œ „« Ìﬂ„· «··Êœ‰ﬁ
            // SceneManager.LoadScene(nextSceneName);
            Debug.Log(" Õ„Ì· «ﬂ „·!");
        }
    }
}

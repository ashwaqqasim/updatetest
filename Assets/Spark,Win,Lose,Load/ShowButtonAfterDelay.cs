using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowButtonAfterDelay : MonoBehaviour
{
    public GameObject targetButton;  // «·“— «··Ì —«Õ ÌŸÂ— »⁄œ Êﬁ 
    public float delaySeconds = 2f;

    void Start()
    {
        targetButton.SetActive(false);  // ‰Œ›Ì «·“— √Ê· „« Ì‘ €· «·„‘Âœ
        StartCoroutine(ShowAfterDelay());
    }

    IEnumerator ShowAfterDelay()
    {
        yield return new WaitForSeconds(delaySeconds);
        targetButton.SetActive(true);
    }

    // «” œ⁄ˆ Â–Â «·œ«·… „‰ «·“—
    public void GoToGamesPage()
    {
        SceneManager.LoadScene("Games page");
    }
}

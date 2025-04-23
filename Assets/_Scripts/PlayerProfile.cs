using UnityEngine;
using UnityEngine.SceneManagement;

public class GenderSceneLoader : MonoBehaviour
{
    public void LoadSceneBasedOnGender()
    {
        string gender = PlayerPrefs.GetString("SelectedCharacter", "default");
        
        if (gender == "boy") {
            SceneManager.LoadScene("frist"); // اسم مشهد الولد
        }
        else if (gender == "girl") {
            SceneManager.LoadScene("Frist Girl"); // اسم مشهد البنت
        }
        else {
            SceneManager.LoadScene("frist"); // المشهد الافتراضي
        }
    }
}
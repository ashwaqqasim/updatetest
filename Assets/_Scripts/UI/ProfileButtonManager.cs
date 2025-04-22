using UnityEngine;
using UnityEngine.SceneManagement;

public class ProfileButtonManager : MonoBehaviour
{
    public string profileSceneName = "ProfileScene"; //  √ﬂœ „‰ «”„ «·„‘Âœ «·’ÕÌÕ

    public void OpenProfileScene()
    {
        SceneManager.LoadScene(profileSceneName);
    }
}

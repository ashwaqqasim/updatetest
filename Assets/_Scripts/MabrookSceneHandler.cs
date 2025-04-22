using UnityEngine;

public class MabrookSceneHandler : MonoBehaviour
{
    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScoreOncePerWin(20);
            Debug.Log("Score increased by 20 (once per win)");
        }
    }
}


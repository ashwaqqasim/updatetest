using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePopupPanel;
    public GameObject gameBoardPanel;
    public GameObject gameModePanel; // لو حاب ترجع للاختيار

    public void OnMenuButtonPressed()
    {
        pausePopupPanel.SetActive(true);
        Time.timeScale = 0f; // يوقف اللعبة مؤقتًا
    }

    public void OnContinueButtonPressed()
    {
        pausePopupPanel.SetActive(false);
        Time.timeScale = 1f; // يكمل اللعبة
    }

    public void OnCancelButtonPressed()
    {
        pausePopupPanel.SetActive(false);
        Time.timeScale = 1f; // يكمل اللعبة
    }

    public void OnExitButtonPressed()
    {
        Time.timeScale = 1f;
        gameBoardPanel.SetActive(false);
        gameModePanel.SetActive(true);
        pausePopupPanel.SetActive(false);
        Time.timeScale = 1f; // نرجع اللعبة تشتغل
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // تعيد تحميل المشهد من جديد
    }
}

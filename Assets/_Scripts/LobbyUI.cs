using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyUI : MonoBehaviour
{
    [Header("Lobby UI Elements")]
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private TMP_InputField lobbyCodeInput;
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TMP_Text lobbyCodeText;
    [SerializeField] private Button cancelButton;

    [Header("Panels")]
    [SerializeField] private GameObject panelJoin;
    [SerializeField] private GameObject panelCode;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private Button confirmJoinButton;

    void Start()
    {
        createLobbyButton.onClick.AddListener(() =>
        {
            LobbyManager.Instance.CreateLobby();
            Invoke(nameof(UpdateLobbyCode), 2f); // ننتظر شوي حتى يتم إنشاء اللوبي
            panelCode.SetActive(true);
            panelJoin.SetActive(false);
            lobbyPanel.SetActive(false);
        });

        joinLobbyButton.onClick.AddListener(() =>
        {
            panelJoin.SetActive(true);
            lobbyPanel.SetActive(false);
            panelCode.SetActive(false);
        });

        cancelButton.onClick.AddListener(() =>
        {
            panelJoin.SetActive(false);
            panelCode.SetActive(false);
            lobbyPanel.SetActive(true);
        });
        confirmJoinButton.onClick.AddListener(() =>
        {
            string code = lobbyCodeInput.text;
            LobbyManager.Instance.JoinLobby(code);
        });

    }

    public void OnJoinConfirm()
    {
        string code = lobbyCodeInput.text;
        LobbyManager.Instance.JoinLobby(code);
    }

    void UpdateLobbyCode()
    {
        if (!string.IsNullOrEmpty(LobbyManager.Instance.lobbyCode))
        {
            lobbyCodeText.text = "كود الغرفة: " + LobbyManager.Instance.lobbyCode;
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies.Models;


public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;
    [SerializeField] private GameObject panelCode;
    [SerializeField] private GameObject panelJoin;
    [SerializeField] private GameObject rpsPanel;


    public Lobby currentLobby;
    private float heartbeatTimer = 0f;
    private float lobbyPollTimer = 0f;

    public string playerName = "Player";
    public string lobbyCode = "";
    public bool isHost = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            await UnityServices.InitializeAsync();

        if (!AuthenticationService.Instance.IsSignedIn)
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

        playerName = PlayerPrefs.GetString("PlayerName", "Unknown");
    }

    void Update()
    {
        if (currentLobby != null)
        {
            HandleHeartbeat();
            HandleLobbyPolling();
        }
    }

    private async void HandleHeartbeat()
    {
        if (!isHost) return;

        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer <= 0f)
        {
            heartbeatTimer = 15f;
            await LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
        }
    }

    private async void HandleLobbyPolling()
    {
        lobbyPollTimer -= Time.deltaTime;
        if (lobbyPollTimer <= 0f)
        {
            lobbyPollTimer = 5f; // أو حتى 10f


            try
            {
                currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);

                if (currentLobby.Players.Count == 2)
                {
                    Debug.Log("✅ لاعبين اثنين متصلين، تحميل المشهد التالي...");
                    rpsPanel.SetActive(true);
                    panelCode?.SetActive(false);
                    panelJoin?.SetActive(false);

                }
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError("❌ Lobby polling error: " + e.Message);
            }
        }
    }

    public async void CreateLobby()
    {
        try
        {
            string lobbyName = "GameLobby_" + UnityEngine.Random.Range(1000, 9999);
            int maxPlayers = 2;

            var player = new Player
            {
                Data = new Dictionary<string, PlayerDataObject>
    {
        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
    }
            };

            var lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Player = player
            };


            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, lobbyOptions);
            isHost = true;
            lobbyCode = currentLobby.LobbyCode;

            Debug.Log("✅ Lobby Created! Code: " + lobbyCode);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("❌ Failed to create lobby: " + e.Message);
        }
    }

    public async void JoinLobby(string code)
    {
        try
        {
            var joinOptions = new JoinLobbyByCodeOptions
            {
                Player = new Player
                {
                    Data = new Dictionary<string, PlayerDataObject>
                    {
                        { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
                    }
                }
            };

            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);
            isHost = false;
            lobbyCode = code;

            Debug.Log("✅ Joined Lobby! Code: " + code);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError("❌ Failed to join lobby: " + e.Message);
        }
    }
}


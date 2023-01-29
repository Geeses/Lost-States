using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System;

public class LobbyManager
{
    int maxPlayers = 3;
    private NetworkVariable<bool> canStartGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public event Action OnClientJoinedLobby;

    public LobbyManager()
    {
        canStartGame.OnValueChanged += (bool previousValue, bool newValue) => {
            Debug.Log($"Starting Game... " + SceneManager.GetSceneByBuildIndex(0).name);
            NetworkManager.Singleton.SceneManager.LoadScene("2PlayerMap", LoadSceneMode.Single);
        };
    }
    public async Task<Lobby> TryCreateLobbyAsync(string lobbyName, bool isPrivate)
    {
        var relayData = await RelayManager.Instance.SetupRelay();
        CreateLobbyOptions options = new CreateLobbyOptions();
        options.IsPrivate = isPrivate;
        options.Data = new Dictionary<string, DataObject>()
        {
            {
                "joinCode", new DataObject(
                    visibility: DataObject.VisibilityOptions.Member,
                    value: relayData.JoinCode
                    )
            },
        };

        Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
        Debug.Log("Lobby Was Created");
        InitializeServices.Instance.InitializeHeartbeatLobbyCoroutine(lobby.Id, 15);
        Debug.Log("Heartbit Corroutine Started");
        NetworkManager.Singleton.StartHost();
        Debug.Log("Host Started");
        return lobby;
    }

    public async Task<List<Lobby>> GetAllLobbies()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 5;

            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await Lobbies.Instance.QueryLobbiesAsync(options);
            return lobbies.Results;
        } 
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return new List<Lobby>();
        }
    }

    public async Task JoinLobby(string id)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id);
            string joinCode = lobby.Data["joinCode"].Value;
            Debug.Log("Received code: " + joinCode);
            var relayData = await RelayManager.Instance.JoinRelay(joinCode);
            Debug.Log("Lobby players: " + lobby.Players.Count);
            Debug.Log("LobbyId: " + lobby.Players.Count);
            Debug.Log("Joined Lobby");
            NetworkManager.Singleton.StartClient();
            OnClientJoinedLobby.Invoke();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void StartGame()
    {
        canStartGame.Value = true;
    }
}
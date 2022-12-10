using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region Attributes
    public bool gameHasStarted;

    public event Action OnGameStart;
    public event Action OnGameEnd;

    private static GameManager s_instance;

    private List<ulong> _connectedPlayersId = new List<ulong>();
    #endregion

    #region Properties
    public static GameManager Instance { get { return s_instance; } }
    public List<ulong> ConnectedPlayersId { get => _connectedPlayersId; private set => _connectedPlayersId = value; }
    #endregion

    #region Monobehavior Functions
    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }
    }

    private void Start()
    {
        NetworkManager.OnServerStarted += () => StartCoroutine(WaitForLobbyJoined());
    }

    private void OnDisable()
    {
        NetworkManager.OnServerStarted -= () => StartCoroutine(WaitForLobbyJoined());
    }
    #endregion

    private IEnumerator WaitForLobbyJoined()
    {
        // Server updates list of all incoming players
        AddClient(NetworkManager.ServerClientId);
        NetworkManager.OnClientConnectedCallback += AddClient;

        // wait until we have every player connected
        yield return new WaitUntil(() => ConnectedPlayersId.Count == 2);

        // After everyone connected, we share the list of players with everyone else
        List<ulong> tmp = new List<ulong>(ConnectedPlayersId);
        foreach (var player in tmp)
        {
            SynchronizeLobbyDataClientRpc(player);
        }

        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        gameHasStarted = true;
        OnGameStart?.Invoke();
    }

    private void AddClient(ulong clientId)
    {
        ConnectedPlayersId.Add(clientId);
    }

    [ClientRpc]
    private void SynchronizeLobbyDataClientRpc(ulong playerId)
    {
        if(!IsServer)
            ConnectedPlayersId.Add(playerId);
    }
}

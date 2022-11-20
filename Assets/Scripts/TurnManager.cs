using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    public TMPro.TMP_Text playerIdText;
    public TMPro.TMP_Text currentTurnPlayerIdText;

    private List<ulong> _connectedPlayersId = new List<ulong>();
    private Queue<ulong> _playerTurnQueueId = new Queue<ulong>();
    private ulong _currentTurnPlayerId;
    private Player _currentTurnPlayer;
    private Player _localPlayer;
    private static TurnManager s_instance;

    public static TurnManager Instance { get { return s_instance; } }

    public List<ulong> ConnectedPlayersId { get => _connectedPlayersId; private set => _connectedPlayersId = value; }
    public Queue<ulong> PlayerTurnQueueId { get => _playerTurnQueueId; private set => _playerTurnQueueId = value; }
    public ulong CurrentTurnPlayerId { get => _currentTurnPlayerId; private set => _currentTurnPlayerId = value; }

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

    private IEnumerator WaitForLobbyJoined()
    {
        // Server updates list of all incoming players
        if (IsServer)
        {
            AddClient(NetworkManager.LocalClientId);
            NetworkManager.OnClientConnectedCallback += AddClient;
        }

        // wait until we have every player connected
        yield return new WaitUntil(() => ConnectedPlayersId.Count == 2);

        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        // After everyone connected, we share the list of players with everyone else
        if (IsServer)
        {
            List<ulong> tmp = new List<ulong>(ConnectedPlayersId);
            foreach (var player in tmp)
            {
                SynchronizeLobbyDataClientRpc(player);
            }
        }

        Debug.Log(NetworkManager.LocalClient);
        _localPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
        _currentTurnPlayer = NetworkManager.ConnectedClients[0].PlayerObject.GetComponent<Player>();
        StartGame();
    }

    [ClientRpc]
    private void SynchronizeLobbyDataClientRpc(ulong playerId)
    {
        ConnectedPlayersId.Add(playerId);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    private void AddClient(ulong clientId)
    {
        ConnectedPlayersId.Add(clientId);
    }

    private void StartGame()
    {
        if (IsServer)
        {
            // put every player into the queue
            ConnectedPlayersId.ForEach(o => PlayerTurnQueueId.Enqueue(o));
            // first player in queue is current player
            StartTurnClientRpc(PlayerTurnQueueId.Peek());
        }
    }

    // the turnmanager on the server runs everything, clients dont need to inform the server about anything.
    // thats why we just need to update the clients
    [ClientRpc]
    public void StartTurnClientRpc(ulong playerId)
    {
        CurrentTurnPlayerId = playerId;
        currentTurnPlayerIdText.text = "Player " + playerId + "´s turn.";
        playerIdText.text = "PlayerId: " + NetworkManager.LocalClientId;
    }

    // simple wrapper function to enable a ServerRpc through a button
    public void EndTurn()
    {
        if (CurrentTurnPlayerId == NetworkManager.LocalClientId && _localPlayer.moveCount == 0)
        {
            EndTurnServerRpc(NetworkManager.LocalClientId);
        }
    }

    // only a client can end a turn, so we need to share this information with the server, and the server needs to inform all the other clients about this aswell
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(ulong playerId)
    {
        PlayerTurnQueueId.Enqueue(playerId);
        PlayerTurnQueueId.Dequeue();
        CurrentTurnPlayerId = PlayerTurnQueueId.Peek();
        _currentTurnPlayer = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
        StartTurnClientRpc(CurrentTurnPlayerId);
    }
}

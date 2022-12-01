using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    #region Attributes
    public TMPro.TMP_Text playerIdText;
    public TMPro.TMP_Text currentTurnPlayerIdText;
    public TMPro.TMP_Text currentTurnPlayerMovesText;

    //TODO: implement game end/start methods in GameManager
    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action<ulong> OnTurnStart;
    public event Action<ulong> OnTurnEnd;

    private List<ulong> _connectedPlayersId = new List<ulong>();
    private Queue<ulong> _playerTurnQueueId = new Queue<ulong>();
    private ulong _currentTurnPlayerId;
    private Player _currentTurnPlayer;
    private Player _localPlayer;
    private int _currentTurnNumber;
    private int _totalTurnCount;

    private static TurnManager s_instance;
    #endregion

    #region Properties
    public static TurnManager Instance { get { return s_instance; } }

    public List<ulong> ConnectedPlayersId { get => _connectedPlayersId; private set => _connectedPlayersId = value; }
    public Queue<ulong> PlayerTurnQueueId { get => _playerTurnQueueId; private set => _playerTurnQueueId = value; }
    public ulong CurrentTurnPlayerId { get => _currentTurnPlayerId; private set => _currentTurnPlayerId = value; }
    public Player CurrentTurnPlayer { get => _currentTurnPlayer; set => _currentTurnPlayer = value; }
    public int CurrentTurnNumber { get => _currentTurnNumber; set => _currentTurnNumber = value; }
    public int TotalTurnCount { get => _totalTurnCount; set => _totalTurnCount = value; }
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

        CurrentTurnNumber = 1;
        StartGameClientRpc();
    }

    [ClientRpc]
    private void SynchronizeLobbyDataClientRpc(ulong playerId)
    {
        ConnectedPlayersId.Add(playerId);
    }

    private void AddClient(ulong clientId)
    {
        ConnectedPlayersId.Add(clientId);
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        _localPlayer = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
        
        // put every player into the queue
        ConnectedPlayersId.ForEach(o => PlayerTurnQueueId.Enqueue(o));

        OnGameStart?.Invoke();
        // first player in queue is current player
        StartTurnServerRpc(PlayerTurnQueueId.Peek());
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTurnServerRpc(ulong playerId)
    {
        // TODO: server needs to set the movecount of the player whose turn it is to its amount, and inform other players that this happened
        CurrentTurnPlayer = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
        //CurrentTurnPlayer.MoveCount = 5;
        CurrentTurnPlayer.MaximumPlayableMovementCards = 1;
        CurrentTurnPlayer.PlayedMovementCards = 0;

        CurrentTurnNumber += 1;
        TotalTurnCount += 1;
        if(CurrentTurnNumber > 10)
        {
            CurrentTurnNumber = 1;
        }

        StartTurnClientRpc(playerId);
    }

    [ClientRpc]
    public void StartTurnClientRpc(ulong playerId)
    {
        //_localPlayer.MoveCount = 5;

        CurrentTurnPlayerId = playerId;
        currentTurnPlayerIdText.text = "Player " + playerId + "´s turn.";
        playerIdText.text = "PlayerId: " + NetworkManager.LocalClientId;

        OnTurnStart?.Invoke(playerId);
    }

    // simple wrapper function to enable a ServerRpc through a button
    public void EndTurn()
    {
        if (CurrentTurnPlayerId == NetworkManager.LocalClientId && _localPlayer.MoveCount == 0 && 
            CurrentTurnPlayer.PlayedMovementCards == CurrentTurnPlayer.MaximumPlayableMovementCards)
        {
            EndTurnServerRpc(NetworkManager.LocalClientId);
        }
    }

    // only a client can end a turn, so we need to share this information with the server, and the server needs to inform all the other clients about this aswell
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(ulong playerId)
    {
        EndTurnClientRpc(playerId);
    }

    [ClientRpc]
    public void EndTurnClientRpc(ulong playerId)
    {
        PlayerTurnQueueId.Enqueue(playerId);
        PlayerTurnQueueId.Dequeue();
        CurrentTurnPlayerId = PlayerTurnQueueId.Peek();

        OnTurnEnd?.Invoke(playerId);

        StartTurnServerRpc(CurrentTurnPlayerId);
    }
}

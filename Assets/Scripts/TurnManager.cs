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
    private Player currentTurnPlayer;
    private static TurnManager s_instance;
    private List<NetworkClient> _connectedClients = new List<NetworkClient>();

    public static TurnManager Instance { get { return s_instance; } }

    public List<ulong> ConnectedPlayersId { get => _connectedPlayersId; }
    public Queue<ulong> PlayerTurnQueueId { get => _playerTurnQueueId; }
    public ulong CurrentTurnPlayerId { get => _currentTurnPlayerId;}

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

    private IEnumerator Start()
    {
        _currentTurnPlayerId = 99;
        NetworkManager.OnClientConnectedCallback += AddClient;

        // wait until we have every player connected
        yield return new WaitUntil(() => _connectedPlayersId.Count == 2);

        StartGame();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnClientConnectedCallback -= AddClient;
    }

    private void AddClient(ulong clientId)
    {
        _connectedClients.Add(NetworkManager.ConnectedClients[clientId]);
        currentTurnPlayer = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<Player>();
        currentTurnPlayer.clientId = clientId;
        currentTurnPlayer.moveCount = 5;
        _connectedPlayersId.Add(clientId);
    }

    private void StartGame()
    {
        if (IsServer)
        {
            // put every player into the queue
            _connectedPlayersId.ForEach(o => _playerTurnQueueId.Enqueue(o));
            // first player in queue is current player
            StartTurnClientRpc(_playerTurnQueueId.Peek());
        }
    }

    // the turnmanager on the server runs everything, clients dont need to inform the server about anything.
    // thats why we just need to update the clients
    [ClientRpc]
    public void StartTurnClientRpc(ulong playerId)
    {
        _currentTurnPlayerId = playerId;
        currentTurnPlayerIdText.text = "Player " + playerId + "´s turn.";
        playerIdText.text = "PlayerId: " + NetworkManager.LocalClientId;
    }

    // simple wrapper function to enable a ServerRpc through a button
    public void EndTurn()
    {
        if (_currentTurnPlayerId == NetworkManager.LocalClientId && currentTurnPlayer.moveCount == 0)
        {
            EndTurnServerRpc(NetworkManager.LocalClientId);
        }
    }

    // only a client can end a turn, so we need to share this information with the server, and the server needs to inform all the other clients about this aswell
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(ulong playerId)
    {
        _playerTurnQueueId.Enqueue(playerId);
        _playerTurnQueueId.Dequeue();
        _currentTurnPlayerId = _playerTurnQueueId.Peek();
        StartTurnClientRpc(_currentTurnPlayerId);
    }
}

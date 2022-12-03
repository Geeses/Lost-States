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

    private Queue<ulong> _playerTurnQueueId = new Queue<ulong>();
    private ulong _currentTurnPlayerId;
    private Player _currentTurnPlayer;
    private int _currentTurnNumber;
    private int _totalTurnCount;

    private static TurnManager s_instance;
    #endregion

    #region Properties
    public static TurnManager Instance { get { return s_instance; } }

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
        CurrentTurnNumber = 0;

        GameManager.Instance.OnGameStart += CheckForTurnRotation;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameStart -= CheckForTurnRotation;
    }

    #endregion

    public void CheckForTurnRotation()
    {
        if (!IsServer)
            return;

        // put every player into the queue
        GameManager.Instance.ConnectedPlayersId.ForEach(o => PlayerTurnQueueId.Enqueue(o));

        CurrentTurnNumber += 1;
        TotalTurnCount += 1;

        if (CurrentTurnNumber > 10)
        {
            CurrentTurnNumber = 1;
        }
    
        ShareTurnNumberClientRpc(CurrentTurnNumber, TotalTurnCount);
        StartTurnServerRpc(PlayerTurnQueueId.Peek());
    }

    [ClientRpc]
    private void ShareTurnNumberClientRpc(int currentTurnNumber, int totalTurnCount)
    {
        CurrentTurnNumber = currentTurnNumber;
        TotalTurnCount = totalTurnCount;
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartTurnServerRpc(ulong playerId)
    {
        if (PlayerTurnQueueId.Count != 0)
        {
            CurrentTurnPlayer = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();

            CurrentTurnPlayer.MaximumPlayableMovementCards = 1;
            CurrentTurnPlayer.PlayedMovementCards = 0;
            CurrentTurnPlayer.movedInCurrentTurn.Value = 0;

            StartTurnClientRpc(playerId);
        }
        else
        {
            CheckForTurnRotation();
        }
    }

    [ClientRpc]
    public void StartTurnClientRpc(ulong playerId)
    {
        CurrentTurnPlayerId = playerId;
        currentTurnPlayerIdText.text = "Player " + playerId + "´s turn.";
        playerIdText.text = "PlayerId: " + NetworkManager.LocalClientId;

        OnTurnStart?.Invoke(playerId);
    }

    // simple wrapper function to enable a ServerRpc through a button
    public void EndTurn()
    {
        EndTurnServerRpc(NetworkManager.LocalClientId);
    }

    // only a client can end a turn, so we need to share this information with the server, and the server needs to inform all the other clients about this aswell
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(ulong playerId)
    {
        if (CurrentTurnPlayerId == playerId && CurrentTurnPlayer.MoveCount == 0 &&
            CurrentTurnPlayer.PlayedMovementCards == CurrentTurnPlayer.MaximumPlayableMovementCards)
        {
            PlayerTurnQueueId.Dequeue();

            if (PlayerTurnQueueId.Count != 0)
            {
                CurrentTurnPlayerId = PlayerTurnQueueId.Peek();
            }

            EndTurnClientRpc(playerId);
            StartTurnServerRpc(CurrentTurnPlayerId);
        }
    }

    [ClientRpc]
    public void EndTurnClientRpc(ulong playerId)
    {
        OnTurnEnd?.Invoke(playerId);
    }
}

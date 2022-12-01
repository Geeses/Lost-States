using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : NetworkBehaviour
{
    #region Attributes
    [Header("Movement Card References")]
    public List<MovementCardBase> movementCards = new List<MovementCardBase>();
    public HorizontalLayoutGroup cardParent;

    [Header("Options")]
    public int MoveCardListInStack = 3;

    private List<int> _movementCardStack = new List<int>();
    private int _movementCardStackPosition;

    private static CardManager s_instance;
    #endregion

    #region Properties
    public static CardManager Instance { get { return s_instance; } }
    public List<int> MovementCardStack { get => _movementCardStack; set => _movementCardStack = value; }
    public int MovementCardStackPosition { get => _movementCardStackPosition; set => _movementCardStackPosition = value; }
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

        CreateMovementCardStack();
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnStart += AddMovementCardsToPlayer;
    }
    #endregion

    public MovementCardBase GetCardById(int id)
    {
        foreach (var card in movementCards)
        {
            if (card.id.Equals(id))
            {
                return card;
            }
        }

        return null;
    }

    private void CreateMovementCardStack()
    {
        MovementCardStack.Clear();
        MovementCardStackPosition = 0;

        for (int i = 0; i < MoveCardListInStack; i++)
        {
            foreach (MovementCardBase card in movementCards)
            {
                MovementCardStack.Add(card.id);
            }
        }

        MovementCardStack.Shuffle();
    }

    private void AddMovementCardsToPlayer(ulong playerId)
    {
        if (!IsServer)
            return;

        Player player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
        int addCardAmount = player.MovementCardAmountPerCycle;

        for (int i = 0; i < addCardAmount; i++)
        {
            // if we have no more cards left in our stack
            if(MovementCardStack.Count == MovementCardStackPosition)
            {
                CreateMovementCardStack();
            }

            player.AddMovementCardClientRpc(MovementCardStack[MovementCardStackPosition]);

            MovementCardStackPosition += 1;
        }
    }

    [ServerRpc]
    public void TryPlayMovementCardServerRpc(int cardId, int instanceId, ulong playerId)
    {
        Player player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();

        // if they are still allowed to play movementcards and its their turn
        if(player.PlayedMovementCards <= player.MaximumPlayableMovementCards &&
            playerId == TurnManager.Instance.CurrentTurnPlayerId)
        {
            //GetCardById(cardId).PlayCard();
        }
    }
}

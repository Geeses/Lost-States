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
    public List<Card> movementCards = new List<Card>();
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
        TurnManager.Instance.OnTurnStart += TryAddMovementCardsToPlayer;
    }
    #endregion

    public Card GetCardById(int id)
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
            foreach (Card card in movementCards)
            {
                MovementCardStack.Add(card.id);
            }
        }

        MovementCardStack.Shuffle();
    }

    private void TryAddMovementCardsToPlayer(ulong playerId)
    {
        // only execute function if its the server
        if (!IsServer)
            return;

        if (TurnManager.Instance.CurrentTurnNumber == 1 || TurnManager.Instance.CurrentTurnNumber == 6)
        {
            AddMovementCardsToPlayer(playerId);
        }
    }

    private void AddMovementCardsToPlayer(ulong playerId)
    {
        Player player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();
        int addCardAmount = player.MovementCardAmountPerCycle;

        for (int i = 0; i < addCardAmount; i++)
        {
            // if we have no more cards left in our stack
            if(MovementCardStack.Count == MovementCardStackPosition)
            {
                CreateMovementCardStack();
            }
            Debug.Log("add card to player");
            player.AddMovementCardClientRpc(MovementCardStack[MovementCardStackPosition]);

            MovementCardStackPosition += 1;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryPlayMovementCardServerRpc(int cardId, int instanceId, ulong playerId)
    {
        Player player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();

        // if they are still allowed to play movementcards and its their turn
        if(player.PlayedMovementCards <= player.MaximumPlayableMovementCards &&
            playerId == TurnManager.Instance.CurrentTurnPlayerId)
        {
            // remove UI object from player that sent the request
            NetworkManagerUI.Instance.RemoveCardFromPlayerUiClientRpc(playerId, instanceId);

            // increment move card played counter
            player.ChangePlayedMoveCardsClientRpc(1);

            Card card = GetCardById(cardId);
            List<CardEffect> effects = card.cardEffects;
            player.ChangeMoveCountClientRpc(card.baseMoveCount);

            foreach (CardEffect effect in effects)
            {
                effect.Initialize(player);
                effect.ExecuteEffect();
            }
        }
    }

    // execute card effects
    [ClientRpc]
    private void ExecuteCardEffectClientRpc(int cardId, ulong playerId)
    {

    }
}

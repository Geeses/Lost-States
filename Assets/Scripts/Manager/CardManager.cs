using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CardManager : NetworkBehaviour
{
    #region Attributes
    [Header("Card References")]
    public List<Card> movementCards = new List<Card>();
    public List<Card> chestCards = new List<Card>();
    public HorizontalLayoutGroup cardParent;

    [Header("Options")]
    private int moveCardListInStack = 3;
    private int chestCardListInStack = 3;

    private List<int> _movementCardStack = new List<int>();
    private List<int> _chestCardStack = new List<int>();
    private int _movementCardStackPosition;
    private int _chestCardStackPosition;


    private static CardManager s_instance;
    #endregion

    #region Properties
    public static CardManager Instance { get { return s_instance; } }
    public List<int> MovementCardStack { get => _movementCardStack; private set => _movementCardStack = value; }
    public int MovementCardStackPosition { get => _movementCardStackPosition; private set => _movementCardStackPosition = value; }
    public List<int> ChestCardStack { get => _chestCardStack; private set => _chestCardStack = value; }
    public int ChestCardStackPosition { get => _chestCardStackPosition; private set => _chestCardStackPosition = value; }
    public int MoveCardListInStack { get => moveCardListInStack; private set => moveCardListInStack = value; }
    public int ChestCardListInStack { get => chestCardListInStack; private set => chestCardListInStack = value; }
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
        CreateChestCardStack();
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnStart += TryAddMovementCardsToPlayer;
    }
    #endregion

    #region Movement Card Functions

    public Card GetMovementCardById(int id)
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

        // if its the total beginning of the game, we give each player 2 more cards 
        if(TurnManager.Instance.TotalTurnCount == 1)
            addCardAmount += 2;

        for (int i = 0; i < addCardAmount; i++)
        {
            // if we have no more cards left in our stack
            if (MovementCardStack.Count == MovementCardStackPosition)
            {
                CreateMovementCardStack();
            }

            player.movementCards.Add(MovementCardStack[MovementCardStackPosition]);

            MovementCardStackPosition += 1;
        }
    }

    // temporary card means, that this card doesnt really exist and is only played for its effects
    // used for example in chest card effects
    [ServerRpc(RequireOwnership = false)]
    public void TryPlayMovementCardServerRpc(int cardId, int instanceId, ulong playerId, bool temporaryCard = false)
    {
        Debug.Log("Try play Card. " + temporaryCard + " " + playerId);
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        // if they are still allowed to play movementcards and its their turn
        if (player.PlayedMovementCards < player.MaximumPlayableMovementCards &&
            playerId == TurnManager.Instance.CurrentTurnPlayerId || temporaryCard)
        {
            // remove UI object from player that sent the request
            NetworkManagerUI.Instance.RemoveCardFromPlayerUiClientRpc(playerId, instanceId);

            // Sending the ClientRPC only to the playerId
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerId }
                }
            };

            CardEffectManager.Instance.InitializeCardEffectClientRpc(cardId, playerId, CardType.Movement, clientRpcParams);

            Card card = GetMovementCardById(cardId);
            player.moveCount.Value += card.baseMoveCount;

            PlayMovementCardClientRpc(cardId, playerId, temporaryCard);
        }
    }

    [ClientRpc]
    private void PlayMovementCardClientRpc(int cardId, ulong playerId, bool temporaryCard = false)
    {
        Debug.Log("Play card " + temporaryCard);
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        if(!temporaryCard)
        {
            // increment move card played counter
            player.ChangePlayedMoveCardsClientRpc(1);
            player.discardedMovementCards.Add(cardId);
        }
    }

    #endregion

    #region Chest Card Functions
    public Card GetChestCardById(int id)
    {
        foreach (var card in chestCards)
        {
            if (card.id.Equals(id))
            {
                return card;
            }
        }

        return null;
    }


    private void CreateChestCardStack()
    {
        ChestCardStack.Clear();
        ChestCardStackPosition = 0;

        for (int i = 0; i < ChestCardListInStack; i++)
        {
            foreach (Card card in chestCards)
            {
                ChestCardStack.Add(card.id);
            }
        }

        ChestCardStack.Shuffle();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddChestCardToPlayerServerRpc(ulong playerId)
    {
        Debug.Log("start add");

        Player player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>();

        // if we have no more cards left in our stack
        if (ChestCardStack.Count == ChestCardStackPosition)
        {
            CreateChestCardStack();
        }

        player.inventoryChestCards.Add(ChestCardStack[ChestCardStackPosition]);
        Debug.Log("added chestcard. current chestcard count: " + player.inventoryChestCards.Count);

        ChestCardStackPosition += 1;
    }

    [ServerRpc(RequireOwnership = false)]
    public void TryPlayChestCardServerRpc(int cardId, int instanceId, ulong playerId)
    {
        // if its their turn
        if (playerId == TurnManager.Instance.CurrentTurnPlayerId)
        {
            // remove UI object from player that sent the request
            NetworkManagerUI.Instance.RemoveCardFromPlayerUiClientRpc(playerId, instanceId);

            // Sending the ClientRPC only to the playerId
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { playerId }
                }
            };
            CardEffectManager.Instance.InitializeCardEffectClientRpc(cardId, playerId, CardType.Chest, clientRpcParams);
        }
    }
    #endregion
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : NetworkBehaviour
{
    [SerializeField] private Button _hostButton;
    [SerializeField] private Button _clientButton;
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private Button _skipTurnButton;
    [SerializeField] private TMPro.TMP_Text _moveCountText;
    [SerializeField] private TMPro.TMP_Text _playerId;
    [SerializeField] private TMPro.TMP_Text _currentTurnPlayerId;
    [SerializeField] private TMPro.TMP_Text _currentTurnPlayerMoves;
    [SerializeField] private HorizontalLayoutGroup _cardLayoutGroup;

    private static NetworkManagerUI s_instance;
    private Player _player;
    private List<Card> _cards = new List<Card>();
    private List<CardUiScript> _cardUis = new List<CardUiScript>();

    public static NetworkManagerUI Instance { get { return s_instance; } }

    public HorizontalLayoutGroup CardLayoutGroup { get => _cardLayoutGroup; set => _cardLayoutGroup = value; }

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

        _hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        _clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });

    }
    private void Start()
    {
        if (GameManager.Instance.gameHasStarted)
        {
            InitCards();
        }
        else
        {
            GameManager.Instance.OnGameStart += InitCards;
        }
    }

    private void InitCards()
    {
        _player = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();

        foreach (int id in _player.movementCards)
        {
            InstantiateMovementCard(id);
        }

        _player.movementCards.OnListChanged+= ChangeMovementCards;
        _player.inventoryChestCards.OnListChanged += ChangeChestCards;
    }

    private void ChangeChestCards(NetworkListEvent<int> changeEvent)
    {
        if(changeEvent.Type == NetworkListEvent<int>.EventType.Add)
        {
            InstantiateChestCard(changeEvent.Value);
        }
        /*
        else if(changeEvent.Type == NetworkListEvent<int>.EventType.Remove || changeEvent.Type == NetworkListEvent<int>.EventType.RemoveAt)
        {
            GameObject removedCard = null;
            CardUiScript toBeRemoved = null;

            foreach (CardUiScript cardUi in _cardUis)
            {
                if(cardUi != null)
                {
                    if (cardUi.CardId == changeEvent.Value && cardUi.CardType == CardType.Chest)
                    {
                        removedCard = cardUi.gameObject;
                    }
                }
                else
                {
                    Debug.LogWarning("Null Card in UI Cards.");
                    toBeRemoved = cardUi;
                }
            }

            if(toBeRemoved != null)
            {
                _cardUis.Remove(toBeRemoved);
            }

            Destroy(removedCard);
        }
        */
    }

    private void ChangeMovementCards(NetworkListEvent<int> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<int>.EventType.Add)
        {
            InstantiateMovementCard(changeEvent.Value);
        }
        /*
        else if (changeEvent.Type == NetworkListEvent<int>.EventType.Remove || changeEvent.Type == NetworkListEvent<int>.EventType.RemoveAt)
        {
            GameObject removedCard = null;
            CardUiScript toBeRemoved = null;

            foreach (CardUiScript cardUi in _cardUis)
            {
                if (cardUi != null)
                {
                    if (cardUi.CardId == changeEvent.Value && cardUi.CardType == CardType.Movement)
                    {
                        removedCard = cardUi.gameObject;
                    }
                }
                else
                {
                    Debug.LogError("Null Card in UI Cards.");
                    //toBeRemoved = cardUi;
                }
            }
            
            if (toBeRemoved != null)
            {
                _cardUis.Remove(toBeRemoved);
            }
            
            Debug.Log("Destroy Card");
            Destroy(removedCard);
        }
        */
    }

    private void InstantiateMovementCard(int id)
    {
        Card card = CardManager.Instance.GetMovementCardById(id);
        _cards.Add(card);
        CardUiScript cardUi = Instantiate(card.CardUi, CardLayoutGroup.transform).GetComponent<CardUiScript>();
        cardUi.Initialize(card);
        _cardUis.Add(cardUi);
    }

    private void InstantiateChestCard(int id)
    {
        Card card = CardManager.Instance.GetChestCardById(id);
        _cards.Add(card);
        CardUiScript cardUi = Instantiate(card.CardUi, CardLayoutGroup.transform).GetComponent<CardUiScript>();
        cardUi.Initialize(card);
        _cardUis.Add(cardUi);
    }

    [ClientRpc]
    public void RemoveCardFromPlayerUiClientRpc(int cardId, CardType type, int instanceId, bool removeRandom = false, ClientRpcParams clientRpcParams = default)
    {
        CardUiScript objectToRemove = null;

        if(!removeRandom)
        {
            foreach (CardUiScript card in _cardUis)
            {
                if (card != null)
                {
                    if (card.gameObject.GetInstanceID().Equals(instanceId))
                    {
                        objectToRemove = card;
                        Debug.Log("found card to remove " + objectToRemove.name);
                    }
                }
            }
        }
        else
        {
            foreach (CardUiScript card in _cardUis)
            {
                if (card != null)
                {
                    if (card.CardType == type && card.CardId == cardId)
                    {
                        objectToRemove = card;
                        Debug.Log("found card to remove " + objectToRemove.name);
                    }
                }
            }

        }

        if(objectToRemove != null)
        {
            Debug.Log("Destroy prefab");
            _cardUis.Remove(objectToRemove);
            Destroy(objectToRemove.gameObject);
        }
    }
}

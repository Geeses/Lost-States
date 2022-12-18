using System;
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

        foreach (int id in _player.MovementCards)
        {
            InstantiateMovementCard(id);
        }

        _player.MovementCards.CollectionChanged += ChangeMovementCards;
        _player.InventoryChestCards.CollectionChanged += ChangeChestCards;
    }

    private void ChangeChestCards(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (int id in e.NewItems)
            {
                InstantiateChestCard(id);
            }
        }

        if (e.OldItems != null)
        {
            Debug.Log(e.OldItems.Count);
            foreach (int id in e.OldItems)
            {
                GameObject go = _cardUis.Find(x => x.CardId == id && x.CardType == CardType.Chest).gameObject;
                Debug.Log(go.name, go);
                Destroy(go);
            }
        }
    }

    private void ChangeMovementCards(object sender, NotifyCollectionChangedEventArgs e)
    {
        // Debug.Log("movementcard list changed");
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                InstantiateMovementCard((int)item);
            }
        }
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
    public void RemoveCardFromPlayerUiClientRpc(ulong playerId, int instanceId)
    {
        CardUiScript objectToRemove = null;

        foreach (CardUiScript card in _cardUis)
        {
            if(card.gameObject.GetInstanceID().Equals(instanceId))
            {
                objectToRemove = card;
                Debug.Log("found card to remove " + objectToRemove.name);
            }
        }

        if(objectToRemove != null)
        {
            _cardUis.Remove(objectToRemove);
            Destroy(objectToRemove.gameObject);
        }
    }
}

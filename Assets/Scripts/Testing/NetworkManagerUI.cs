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
    [SerializeField] private TMPro.TMP_Text _moveCountText;
    [SerializeField] private TMPro.TMP_Text _playerId;
    [SerializeField] private TMPro.TMP_Text _currentTurnPlayerId;
    [SerializeField] private TMPro.TMP_Text _currentTurnPlayerMoves;
    [SerializeField] private HorizontalLayoutGroup _cardLayoutGroup;

    private static NetworkManagerUI s_instance;
    private Player _player;
    private List<Card> _cards = new List<Card>();
    private List<CardUi> _cardUis = new List<CardUi>();

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
        Debug.Log(GameManager.Instance.gameHasStarted);
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

        Debug.Log(_player.MovementCards.Count);
        foreach (int id in _player.MovementCards)
        {
            InstantiateCard(id);
        }

        _player.MovementCards.CollectionChanged += ChangeMovementCards;
    }

    private void ChangeMovementCards(object sender, NotifyCollectionChangedEventArgs e)
    {
        Debug.Log("card list changed");
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                InstantiateCard((int)item);
            }
        }
    }

    private void InstantiateCard(int id)
    {
        Card card = CardManager.Instance.GetCardById(id);
        _cards.Add(card);
        CardUi cardUi = Instantiate(card.CardUi, CardLayoutGroup.transform).GetComponent<CardUi>();
        cardUi.Initialize(card);
        _cardUis.Add(cardUi);
    }

    [ClientRpc]
    public void RemoveCardFromPlayerUiClientRpc(ulong playerId, int instanceId)
    {
        CardUi objectToRemove = null;

        foreach (CardUi card in _cardUis)
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

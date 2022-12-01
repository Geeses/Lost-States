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
    private List<CardBase> _cards = new List<CardBase>();

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
        TurnManager.Instance.OnGameStart += GetLocalPlayer;
    }

    private void GetLocalPlayer()
    {
        _player = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
        _player.MovementCards.CollectionChanged += ChangeMovementCards;
    }

    private void ChangeMovementCards(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                CardBase card = CardManager.Instance.GetCardById((int)item);
                _cards.Add(card);
                Instantiate(card, CardLayoutGroup.transform);
            }
        }
    }
}

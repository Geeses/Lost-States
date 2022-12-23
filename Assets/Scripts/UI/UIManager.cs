using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text moveCountText;
    [SerializeField] private VerticalLayoutGroup oponentsGroup;
    [SerializeField] private GameObject opponentsPrefab;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TMPro.TMP_Text ressourcesCount;
    [SerializeField] private TMPro.TMP_Text bagFoodText;
    [SerializeField] private TMPro.TMP_Text bagWaterText;
    [SerializeField] private TMPro.TMP_Text bagSteelText;
    [SerializeField] private TMPro.TMP_Text bagWoodText;
    [SerializeField] private TMPro.TMP_Text bagCoinText;
    [SerializeField] private TMPro.TMP_Text safeFoodText;
    [SerializeField] private TMPro.TMP_Text safeWaterText;
    [SerializeField] private TMPro.TMP_Text safeSteelText;
    [SerializeField] private TMPro.TMP_Text safeWoodText;

    private Player _player;
    private List<Player> _enemyPlayers;

    protected Player Player { get => _player; set => _player = value; }
    protected List<Player> EnemyPlayers { get => _enemyPlayers; private set => _enemyPlayers = value; }

    private void Start()
    {
        Debug.Log("UIManager.Start was called");
        if (GameManager.Instance.gameHasStarted)
        {
            Initialize();
        }
        else
        {
            GameManager.Instance.OnGameStart += Initialize;
        }
    }

    private void Initialize()
    {
        Debug.Log("UIManager.Initialize was called");
        _player = NetworkManager.LocalClient.PlayerObject.GetComponent<Player>();
        EnemyPlayers = new List<Player>();

        foreach (KeyValuePair<ulong, Player> entry in PlayerNetworkManager.Instance.PlayerDictionary)
        {
            if (!entry.Key.Equals(_player.clientId.Value))
            {
                _enemyPlayers.Add(entry.Value);
                GameObject newOpponent = Instantiate(opponentsPrefab, oponentsGroup.transform);
                if (newOpponent.TryGetComponent(out OpponentsUI ui)) {
                    ui.Initialize(entry.Value);
                }
            }
        }

        // Register Listeners
        _player.moveCount.OnValueChanged += UpdatePlayerMoves;
        _endTurnButton.onClick.AddListener(TurnManager.Instance.EndTurn);
    }

    private void UpdatePlayerMoves(int previousValue, int newValue)
    {
        moveCountText.text = newValue.ToString();
    }
}

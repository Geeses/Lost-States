using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : NetworkBehaviour
{
    [SerializeField] private TMPro.TMP_Text moveCountText;
    [SerializeField] private VerticalLayoutGroup oponentsGroup;
    [SerializeField] private GameObject opponentsPrefab;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TMPro.TMP_Text bagFruitText;
    [SerializeField] private TMPro.TMP_Text bagWaterText;
    [SerializeField] private TMPro.TMP_Text bagSteelText;
    [SerializeField] private TMPro.TMP_Text bagWoodText;
    [SerializeField] private TMPro.TMP_Text bagCoinText;
    [SerializeField] private TMPro.TMP_Text safeFoodText;
    [SerializeField] private TMPro.TMP_Text safeWaterText;
    [SerializeField] private TMPro.TMP_Text safeSteelText;
    [SerializeField] private TMPro.TMP_Text safeWoodText;
    [SerializeField] private Image[] dayNightStones;

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
        endTurnButton.onClick.AddListener(TurnManager.Instance.EndTurn);
        _player.inventoryRessources.OnListChanged += UpdateRessourcesLabels;
        _player.savedRessources.OnListChanged += UpdateSavedRessourcesLabels;
        TurnManager.Instance.OnTurnStart += UpdateTurnCounter;

        StartCoroutine(InitializeRessourceUi());
    }

    private IEnumerator InitializeRessourceUi()
    {
        yield return new WaitUntil(() => _player.RessourceCollectionCard != null);        
                
        safeFoodText.text = "0/" + _player.RessourceCollectionCard.fruitAmount;
        safeWaterText.text = "0/" + _player.RessourceCollectionCard.waterAmount;
        safeSteelText.text = "0/" + _player.RessourceCollectionCard.steelAmount;
        safeWoodText.text = "0/" + _player.RessourceCollectionCard.woodAmount;

        bagFruitText.text = "0";
        bagWaterText.text = "0";
        bagSteelText.text = "0";
        bagWoodText.text = "0";
    }

    private void UpdateTurnCounter(ulong obj)
    {
        Debug.Log("UIManager.UpdateTurnCounter was called");
        int index = 0;
        Debug.Log("UIManager.UpdateTurnCounter TurnManager.Instance.CurrentTurnNumber: " + TurnManager.Instance.CurrentTurnNumber);
        foreach (Image img in dayNightStones)
        {
            Debug.Log("UIManager.UpdateTurnCounter Inside for each");
            if (index == TurnManager.Instance.CurrentTurnNumber - 1)
            {
                img.color = Color.white;
            }
            else
            {
                img.color = Color.black;
            }
            index++;
        }
    }

    private void UpdateRessourcesLabels(NetworkListEvent<int> changeEvent)
    {
        // change Value is the Objects.Count
        Debug.Log("UIManager.UpdateRessourcesLabels changeEvent.Value: " + changeEvent.Value);

        int bagFruitCount = 0;
        int bagWaterCount = 0;
        int bagSteelCount = 0;
        int bagWoodCount = 0;

        foreach (int ressource in _player.inventoryRessources)
        {
            switch (ressource)
            {
                case (int)Ressource.fruit:
                    bagFruitCount += 1;
                    break;
                case (int)Ressource.water:
                    bagWaterCount += 1;
                    break;
                case (int)Ressource.steel:
                    bagSteelCount += 1;
                    break;
                case (int)Ressource.wood:
                    bagWoodCount += 1;
                    break;
                case (int)Ressource.none:
                    Debug.Log("No ressource was found");
                    break;
            }
        }

        bagFruitText.text = bagFruitCount.ToString();
        bagWaterText.text = bagWaterCount.ToString();
        bagSteelText.text = bagSteelCount.ToString();
        bagWoodText.text = bagWoodCount.ToString();
    }

    private void UpdateSavedRessourcesLabels(NetworkListEvent<int> changeEvent)
    {
        // change Value is the Objects.Count
        Debug.Log("UIManager.UpdateRessourcesLabels changeEvent.Value: " + changeEvent.Value);

        int bagFruitCount = 0;
        int bagWaterCount = 0;
        int bagSteelCount = 0;
        int bagWoodCount = 0;

        foreach (int ressource in _player.savedRessources)
        {
            switch (ressource)
            {
                case (int)Ressource.fruit:
                    bagFruitCount += 1;
                    break;
                case (int)Ressource.water:
                    bagWaterCount += 1;
                    break;
                case (int)Ressource.steel:
                    bagSteelCount += 1;
                    break;
                case (int)Ressource.wood:
                    bagWoodCount += 1;
                    break;
                case (int)Ressource.none:
                    Debug.Log("No ressource was found");
                    break;
            }
        }

        safeFoodText.text = bagFruitCount.ToString() + "/" + _player.RessourceCollectionCard.fruitAmount;
        safeWaterText.text = bagWaterCount.ToString() + "/" + _player.RessourceCollectionCard.waterAmount;
        safeSteelText.text = bagSteelCount.ToString() + "/" + _player.RessourceCollectionCard.steelAmount;
        safeWoodText.text = bagWoodCount.ToString() + "/" + _player.RessourceCollectionCard.woodAmount;
    }

    private void UpdatePlayerMoves(int previousValue, int newValue)
    {
        moveCountText.text = newValue.ToString();
    }
}

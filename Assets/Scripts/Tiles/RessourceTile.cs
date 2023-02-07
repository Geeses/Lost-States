using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RessourceTile : NetworkBehaviour, ITileExtension
{
    #region Attributes
    [Header("Options")]
    public int ressourceCooldown;
    public Ressource ressourceType;
    public int ressourceCount;

    [Header("References")]
    [SerializeField] private SpriteRenderer ressourceRenderer;
    [SerializeField] private SpriteRenderer podestRenderer;
    [SerializeField] private TMPro.TextMeshProUGUI respawnText;

    private Tile _tile;
    private Player _cachedPlayer;
    private int _startRessourceCount;
    private int _startRessourceCooldown;
    private int _playerCount;
    #endregion

    #region Monobehavior Functions
    private void Start()
    {
        _tile = GetComponent<Tile>();
        ressourceCount = 1;
        TurnManager.Instance.OnTurnStart += ChangeRessourceCooldown;
        Initialize();
    }

    private void OnDisable()
    {
        TurnManager.Instance.OnTurnStart -= ChangeRessourceCooldown;
        //GameManager.Instance.OnGameStart -= Initialize;
    }

    #endregion

    private void Initialize()
    {
        _playerCount = PlayerNetworkManager.Instance.PlayerDictionary.Count;

        _tile.OnStepOnTile += GivePlayerRessource;
        _startRessourceCount = ressourceCount;  
        ressourceCooldown *= _playerCount;
        _startRessourceCooldown = ressourceCooldown;
        respawnText.gameObject.SetActive(false);

        if (ressourceCount > 0)
        {
            ressourceRenderer.enabled = true;
        }
    }

    private void GivePlayerRessource(Player player)
    {
        _cachedPlayer = player;

        for (int i = 0; i < ressourceCount; i++)
        {
            if(IsServer)
                _cachedPlayer.inventoryRessources.Add((int)ressourceType);
        }

        DisableResourceClientRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void GivePlayerRessourceServerRpc(ulong playerId)
    {
        _cachedPlayer = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        for (int i = 0; i < ressourceCount; i++)
        {
            _cachedPlayer.inventoryRessources.Add((int)ressourceType);
        }

        DisableResourceClientRpc();
    }

    [ClientRpc]
    private void DisableResourceClientRpc()
    {
        ressourceCount = 0;
        ressourceRenderer.enabled = false;
        respawnText.gameObject.SetActive(true);
    }

    private void ChangeRessourceCooldown(ulong obj)
    {
        if(_startRessourceCount != 0 && ressourceCount == 0 && ressourceCooldown != 0)
        {
            respawnText.gameObject.SetActive(true);
            ressourceCooldown -= 1;
            respawnText.text = (ressourceCooldown / _playerCount).ToString();
        }
        else
        {
            respawnText.gameObject.SetActive(false);
            ressourceCount = _startRessourceCount;
            ressourceRenderer.enabled = true;
            ressourceCooldown = _startRessourceCooldown;
        }
    }
}

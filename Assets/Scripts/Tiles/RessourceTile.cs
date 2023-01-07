using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RessourceTile : NetworkBehaviour, ITileExtension
{
    #region Attributes
    [Header("Options")]
    public Ressource ressourceType;
    public int ressourceCount;

    [Header("References")]
    [SerializeField] private SpriteRenderer ressourceRenderer;
    [SerializeField] private SpriteRenderer podestRenderer;

    private Tile _tile;
    private Player _cachedPlayer;
    #endregion

    #region Monobehavior Functions
    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += GivePlayerRessource;

        if(ressourceCount > 0)
        {
            ressourceRenderer.enabled = true;
        }
    }
    #endregion

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
    }
}

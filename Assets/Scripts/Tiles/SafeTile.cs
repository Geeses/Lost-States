using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SafeTile : NetworkBehaviour, ITileExtension
{
    #region Attributes
    private Tile _tile;
    private Player _cachedPlayer;
    #endregion
    #region Monobehavior Functions
    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += SafePlayerInventory;
    }
    #endregion

    public void SafePlayerInventory(Player player)
    {
        _cachedPlayer = player;

        if(IsServer)
            _cachedPlayer.SafePlayerRessources();
    }

}


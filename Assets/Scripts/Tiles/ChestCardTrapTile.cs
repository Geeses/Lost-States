using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChestCardTrapTile : NetworkBehaviour, ITileExtension
{
    [Header("Options")]
    public int count;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += RemovePlayerChestCards;
    }

    public void RemovePlayerChestCards(Player player)
    {
        _cachedPlayer = player;
        _cachedPlayer.RemoveNewestChestcardServerRpc(count);
    }
}


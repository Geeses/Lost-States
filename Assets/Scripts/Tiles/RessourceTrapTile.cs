using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RessourceTrapTile : NetworkBehaviour, ITileExtension
{
    [Header("Options")]
    public int count;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += RemovePlayerRessources;
    }

    public void RemovePlayerRessources(Player player)
    {
        _cachedPlayer = player;
        _cachedPlayer.RemoveNewestRessourceServerRpc(count);
        Battlelog.Instance.AddLog(player.profileName.Value + " hat " + count + " Ressourcen verloren.");
    }
}


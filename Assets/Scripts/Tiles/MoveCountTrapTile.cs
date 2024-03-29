using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveCountTrapTile : NetworkBehaviour, ITileExtension
{
    [Header("Options")]
    public int count;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += RemovePlayerMoveCount;
    }

    public void RemovePlayerMoveCount(Player player)
    {
        _cachedPlayer = player;

        // only one player sends the ServerRpc, the player who steps on it
        if (_cachedPlayer.IsLocalPlayer)
            _cachedPlayer.ChangeMoveCountServerRpc(count);

        Battlelog.Instance.AddLog(player.profileName.Value + " hat seine Bewegungsanzahl verloren.");
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/SwapPositions", order = 1)]
public class SwapPositions : CardEffect
{
    private ulong _currentSelectedPlayerId;
    internal override void Initialize(Player player)
    {
        base.Initialize(player);
        Player.OnEnemyPlayerSelected += SetSelectedPlayer;
    }

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        SwapPlayers(_currentSelectedPlayerId);
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.OnEnemyPlayerSelected += SwapPlayers;
    }

    private void SetSelectedPlayer(ulong selectedPlayer)
    {
        _currentSelectedPlayerId = selectedPlayer;
        ExecuteEffect();
    }

    private void SwapPlayers(ulong playerId)
    {
        Player selectedPlayer = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        Debug.Log(selectedPlayer, selectedPlayer);

        if (selectedPlayer != null)
        {
            GridCoordinates playerCoords = Player.CurrentTile.TileGridCoordinates;
            GridCoordinates enemyCoords = selectedPlayer.CurrentTile.TileGridCoordinates;

            Player.TryMoveServerRpc(enemyCoords, true, false);
            selectedPlayer.TryMoveServerRpc(playerCoords, true, false);
            Debug.Log("wooosh teleport!");
            Player.OnEnemyPlayerSelected -= SetSelectedPlayer;
            Player.OnEnemyPlayerSelected -= SwapPlayers;
        }
    }
}


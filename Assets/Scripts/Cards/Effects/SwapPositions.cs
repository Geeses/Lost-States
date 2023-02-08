using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/SwapPositions", order = 1)]
public class SwapPositions : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        SwapPlayers(EnemyPlayers[0].clientId.Value);
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void SwapPlayers(ulong playerId)
    {
        Player selectedPlayer = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        if (selectedPlayer != null)
        {
            GridCoordinates playerCoords = Player.CurrentTile.TileGridCoordinates;
            GridCoordinates enemyCoords = selectedPlayer.CurrentTile.TileGridCoordinates;
            Debug.Log("Player coords: " + playerCoords.ToString() + "| Enemy Coords: " + enemyCoords.ToString());

            // empty tiles to Swap Players without issues (otherwise PlayerPushing mechanics activate)
            GridManager.Instance.TileGrid[playerCoords].PlayerLeavesTileServerRpc();
            GridManager.Instance.TileGrid[enemyCoords].PlayerLeavesTileServerRpc();

            Player.TryMoveServerRpc(enemyCoords, true, false);
            selectedPlayer.TryMoveServerRpc(playerCoords, true, false);
        }
    }
}


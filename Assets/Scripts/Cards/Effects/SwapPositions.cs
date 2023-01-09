using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/SwapPositions", order = 1)]
public class SwapPositions : CardEffect
{
    private ulong _currentSelectedPlayerId;

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

        Debug.Log(selectedPlayer, selectedPlayer);

        if (selectedPlayer != null)
        {
            GridCoordinates playerCoords = Player.CurrentTile.TileGridCoordinates;
            GridCoordinates enemyCoords = selectedPlayer.CurrentTile.TileGridCoordinates;

            Player.TryMoveServerRpc(enemyCoords, true, false);
            selectedPlayer.TryMoveServerRpc(playerCoords, true, false);
        }
    }
}


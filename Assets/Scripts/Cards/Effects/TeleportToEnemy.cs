using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/TeleportToPlayer", order = 1)]
public class TeleportToEnemy : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        TeleportToPlayer(EnemyPlayers[0].clientId.Value);
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void TeleportToPlayer(ulong playerId)
    {
        Player selectedPlayer = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        if (selectedPlayer != null)
        {
            GridCoordinates enemyCoordsAbove = selectedPlayer.CurrentTile.TileGridCoordinates + new GridCoordinates(0, 1);

            if(GridManager.Instance.TileGrid.ContainsKey(enemyCoordsAbove))
            {
                Player.TryMoveServerRpc(enemyCoordsAbove, true, false);
            }
            else
            {
                Battlelog.Instance.AddLogServerRpc(selectedPlayer.profileName.Value + " tried to teleport out of the map ...");
            }
        }
    }
}


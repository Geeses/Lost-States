using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/Move Enemy Player", order = 1)]
public class MoveEnemyPlayer : CardEffect
{
    private ulong _currentSelectedPlayerId;

    internal override void Initialize(Player player)
    {
        base.Initialize(player);
    }

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        foreach (var enemy in EnemyPlayers)
        {
            enemy.ChangeMoveCountServerRpc(1);
        }
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


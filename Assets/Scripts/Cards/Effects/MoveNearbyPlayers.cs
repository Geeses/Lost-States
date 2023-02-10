using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/Move Enemy Player in Proxmity", order = 1)]
public class MoveNearbyPlayers : CardEffect
{
    [Header("Options")]
    public int proximity;
    public int moveCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        foreach (Tile tile in GridManager.Instance.GetTilesInProximity(Player.CurrentTile, proximity))
        {
            if(tile.PlayerOnTile != null && tile.PlayerOnTile != Player)
            {
                tile.PlayerOnTile.AddMoveCountServerRpc(moveCount);
            }
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}


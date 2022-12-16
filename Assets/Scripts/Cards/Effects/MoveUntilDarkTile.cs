using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/MoveUntilDarkTile", order = 1)]

public class MoveUntilTileOfType : CardEffect
{

    bool isFirstMove = true;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.OnPlayerMoved += MoveInOneDirection;
    }

    public override void RevertEffect()
    {
        Player.OnPlayerMoved -= MoveInOneDirection;
        base.RevertEffect();
    }

    private void MoveInOneDirection(GridCoordinates coordinates)
    {  
        Tile origin = GridManager.Instance.TileGrid[coordinates];
        List<Tile> tiles = new List<Tile>(); 
        GridManager.Instance.GetTilesInDirection(origin, Player.LastMoveDirection);

        foreach (Tile tile in tiles)
        {
            if (tile.passable)
            {
                Player.TryMoveServerRpc(tile.TileGridCoordinates, false);
            }
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/MoveUntilDarkTile", order = 1)]

public class MoveUntilNotPassable : CardEffect
{

    private bool isFirstMove;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        isFirstMove = true;
        Player.OnPlayerMoved += MoveInOneDirection;
    }

    public override void RevertEffect()
    {
        Player.OnPlayerMoved -= MoveInOneDirection;
        base.RevertEffect();
    }

    private void MoveInOneDirection(GridCoordinates coordinates)
    {
        List<Tile> tiles = GridManager.Instance.GetTilesInDirection(Player.CurrentTile, Player.LastMoveDirection);

        Debug.Log("Move in one direction was called " + tiles.Count);

        if (isFirstMove)
        {
            Debug.Log("Is First Move");
            isFirstMove = false; 
            foreach (Tile tile in tiles)
            {
                if (!tile.passable)
                {
                    break;
                }
                
                Player.TryMoveServerRpc(tile.TileGridCoordinates, true);
            }
        }

    }
}

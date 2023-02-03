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
        base.RevertEffect();
        Player.OnPlayerMoved -= MoveInOneDirection;
    }

    private void MoveInOneDirection(GridCoordinates coordinates)
    {
        if (isFirstMove)
        {
            List<Tile> tiles = GridManager.Instance.GetTilesInDirection(Player.CurrentTile, Player.LastMoveDirection);

            Debug.Log("Move in one direction was called " + tiles.Count + " Direction: " + Player.LastMoveDirection);
            isFirstMove = false; 
            foreach (Tile tile in tiles)
            {
                if (!tile.passable)
                {
                    break;
                }
                
                Player.TryMoveServerRpc(tile.TileGridCoordinates, true, true);
            }
        }

    }
}

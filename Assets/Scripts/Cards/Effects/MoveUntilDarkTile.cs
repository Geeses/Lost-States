using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/MoveUntilDarkTile", order = 1)]

public class MoveUntilDarkTile: CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.OnPlayerMoved += MoveInOneDirection;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.OnPlayerMoved -= MoveInOneDirection;
    }

    private void MoveInOneDirection(GridCoordinates coordinates)
    {
        GridCoordinates posBefore = Player.CurrentTile.TileGridCoordinates;
        GridCoordinates posAfter = coordinates;
        Tile tile = GridManager.Instance.TileGrid[coordinates];
        int x_b = posBefore.x, x_a = posAfter.x, y_b = posBefore.y, y_a = posAfter.y;            
        
        // moving up
        if (x_b == x_a && y_b < y_a)
        {
            while (tile.passable)
            {
                Player.ChangeMoveCountClientRpc(3);
                var x = Player.CurrentTile.TileGridCoordinates.x;
                var y = Player.CurrentTile.TileGridCoordinates.y + 1;
                var newCoordinate = new GridCoordinates(x, y);
                Player.TryMoveServerRpc(newCoordinate);
            }
        }
        // moving down
        else if (x_b == x_a && y_b > y_a)
        {
            while (tile.passable)
            {
                Player.ChangeMoveCountClientRpc(3);
                var x = Player.CurrentTile.TileGridCoordinates.x;
                var y = Player.CurrentTile.TileGridCoordinates.y - 1;
                var newCoordinate = new GridCoordinates(x, y);
                Player.TryMoveServerRpc(newCoordinate);
            }
        }
        // moving left
        else if (y_b == y_a && x_b < x_a)
        {
            while (tile.passable)
            {
                Player.ChangeMoveCountClientRpc(3);
                var x = Player.CurrentTile.TileGridCoordinates.x + 1;
                var y = Player.CurrentTile.TileGridCoordinates.y;
                var newCoordinate = new GridCoordinates(x, y);
                Player.TryMoveServerRpc(newCoordinate);
            }
        }
        // moving right
        else
        {
            while (tile.passable)
            {
                Player.ChangeMoveCountClientRpc(3);
                var x = Player.CurrentTile.TileGridCoordinates.x -1;
                var y = Player.CurrentTile.TileGridCoordinates.y;
                var newCoordinate = new GridCoordinates(x, y);
                Player.TryMoveServerRpc(newCoordinate);
            }
        }
        Player.ChangeMoveCountClientRpc(2);
    }
}

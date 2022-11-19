using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class Player : Selectable
{
    public int moveCount;
    public ulong clientId;

    private Tile _currentTile;

    public override void Start()
    {
        base.Start();

        if(!IsOwner)
        {
            _collider.enabled = false;
        }

        _currentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
    }

    public override void Select()
    {
        base.Select();

        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(_currentTile))
        {
            tile.Highlight();
        }
    }

    public override void Unselect()
    {
        base.Unselect();

        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(_currentTile))
        {
            tile.Unhighlight();
        }
    }

    [ServerRpc]
    public void MoveServerRpc(GridCoordinates coordinates)
    {
        _currentTile = GridManager.Instance.TileGrid[coordinates];

        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition, 0.5f);

        if (IsServer)
        {
            MoveClientRpc(cellWorldPosition);
        }
    }

    [ClientRpc]
    private void MoveClientRpc(Vector3 pos)
    {
        transform.DOMove(pos, 0.5f);
    }
}

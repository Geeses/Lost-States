using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;

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

        clientId = NetworkManager.LocalClientId;
        moveCount = 5;
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
    public void TryMoveServerRpc(GridCoordinates coordinates)
    {
        Tile tile = GridManager.Instance.TileGrid[coordinates];

        // if tile is adjacent
        if(Array.Find(GridManager.Instance.GetAdjacentTiles(tile), x => x.TileGridCoordinates.x == _currentTile.TileGridCoordinates.x && x.TileGridCoordinates.y == _currentTile.TileGridCoordinates.y)
            && moveCount > 0)
        {
            MoveServerRpc(coordinates);
        }
    }

    [ServerRpc]
    public void MoveServerRpc(GridCoordinates coordinates)
    {
        _currentTile = GridManager.Instance.TileGrid[coordinates];
        moveCount -= 1;

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class Player : Selectable
{
    public ulong clientId;

    public override void Start()
    {
        base.Start();

        if(!IsOwner)
        {
            _collider.enabled = false;
        }
    }

    [ServerRpc]
    public void MoveServerRpc(GridCoordinates coordinates)
    {        
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

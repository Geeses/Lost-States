using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;

public enum Ressource
{
    none,
    water,
    steel,
    wood,
    fruit
}

public class Player : Selectable
{
    public ulong clientId;

    private List<int> _movementCards = new List<int>();
    private List<Ressource> _inventoryRessources = new List<Ressource>();
    private List<Ressource> _savedRessources = new List<Ressource>();
    private List<int> _inventoryChestCards = new List<int>();
    private int _coinCount;
    private int _movementCardAmountPerCycle = 5;

    private Tile _currentTile;
    private int _moveCount;

    public int MoveCount { get => _moveCount; set => _moveCount = value; }
    public List<Ressource> InventoryRessources { get => _inventoryRessources; set => _inventoryRessources = value; }
    public List<Ressource> SavedRessources { get => _savedRessources; set => _savedRessources = value; }
    public List<int> InventoryChestCards { get => _inventoryChestCards; set => _inventoryChestCards = value; }
    public int CoinCount { get => _coinCount; set => _coinCount = value; }
    public int MovementCardAmountPerCycle { get => _movementCardAmountPerCycle; set => _movementCardAmountPerCycle = value; }
    public List<int> MovementCards { get => _movementCards; set => _movementCards = value; }

    public override void Start()
    {
        base.Start();

        if(!IsOwner)
        {
            _collider.enabled = false;
        }

        clientId = NetworkManager.LocalClientId;
        _currentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
    }

    public override void Select()
    {
        base.Select();
        HighlightAdjacentTiles();
    }

    public override void Unselect()
    {
        base.Unselect();
        UnhighlightAdjacentTiles();
    }

    private void HighlightAdjacentTiles()
    {
        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(_currentTile))
        {
            tile.Highlight();
        }
    }

    private void UnhighlightAdjacentTiles()
    {
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
        if (Array.Find(GridManager.Instance.GetAdjacentTiles(tile), x => 
            x.TileGridCoordinates.x == _currentTile.TileGridCoordinates.x && 
            x.TileGridCoordinates.y == _currentTile.TileGridCoordinates.y) && 
            MoveCount > 0 &&
            tile.passable)
        {
            ChangeMoveCountClientRpc(MoveCount - 1);
            MoveClientRpc(coordinates);
        }
    }

    [ClientRpc]
    public void ChangeMoveCountClientRpc(int count)
    {
        MoveCount = count;
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
    }

    [ClientRpc]
    private void MoveClientRpc(GridCoordinates coordinates)
    {
        if(IsLocalPlayer)
            UnhighlightAdjacentTiles();

        _currentTile = GridManager.Instance.TileGrid[coordinates];
        _currentTile.PlayerStepOnTile(this);

        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition, 0.5f);

        if(IsLocalPlayer)
            HighlightAdjacentTiles();
    }


    [ClientRpc]
    public void AddMovementCardsClientRpc(int cardId)
    {
        MovementCards.Add(cardId);
    }
}

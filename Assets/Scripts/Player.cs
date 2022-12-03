using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;
using System.Collections.ObjectModel;

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
    #region Attributes

    [Header("Debug")]
    public NetworkVariable<ulong> clientId;
    public NetworkVariable<int> movedInCurrentTurn;

    private ObservableCollection<int> _movementCards = new ObservableCollection<int>();
    private ObservableCollection<int> _inventoryChestCards = new ObservableCollection<int>();
    private ObservableCollection<Ressource> _inventoryRessources = new ObservableCollection<Ressource>();
    private ObservableCollection<Ressource> _savedRessources = new ObservableCollection<Ressource>();
    private int _coinCount;
    private int _movementCardAmountPerCycle = 5;

    private Tile _currentTile;
    private int _moveCount;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    #endregion

    #region Properties
    public int MoveCount { get => _moveCount; set => _moveCount = value; }
    public ObservableCollection<Ressource> InventoryRessources { get => _inventoryRessources; set => _inventoryRessources = value; }
    public ObservableCollection<Ressource> SavedRessources { get => _savedRessources; set => _savedRessources = value; }
    public ObservableCollection<int> InventoryChestCards { get => _inventoryChestCards; set => _inventoryChestCards = value; }
    public int CoinCount { get => _coinCount; set => _coinCount = value; }
    public int MovementCardAmountPerCycle { get => _movementCardAmountPerCycle; set => _movementCardAmountPerCycle = value; }
    public ObservableCollection<int> MovementCards { get => _movementCards; set => _movementCards = value; }
    public int MaximumPlayableMovementCards { get => _maximumPlayableMovementCards; set => _maximumPlayableMovementCards = value; }
    public int PlayedMovementCards { get => _playedMovementCards; set => _playedMovementCards = value; }
    #endregion

    #region Monobehavior Functions
    public override void Start()
    {
        base.Start();

        if (IsServer)
        {
            clientId.Value = OwnerClientId;
            Debug.Log(OwnerClientId);
        }

        if (!IsOwner)
        {
            _collider.enabled = false;
        }

        _currentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
    }
    #endregion

    #region Select and Highlight
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
    #endregion

    #region Movement
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
            movedInCurrentTurn.Value += 1;
            ChangeMoveCountClientRpc(-1);
            MoveClientRpc(coordinates);
        }
    }

    [ClientRpc]
    public void ChangeMoveCountClientRpc(int count)
    {
        MoveCount += count;
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
    }

    [ClientRpc]
    public void PlayCardClientRpc(int cardId)
    {
        //MoveCount = count;
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
    }

    [ClientRpc]
    public void ChangePlayedMoveCardsClientRpc(int count)
    {
        PlayedMovementCards += count;
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
    public void AddMovementCardClientRpc(int cardId)
    {
        Debug.Log("add card Id " + cardId);
        MovementCards.Add(cardId);
    }
    #endregion
}

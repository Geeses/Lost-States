using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Random = UnityEngine.Random;

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
    public int inventoryRessourceCount;
    public int savedRessourceCount;

    public event Action<GridCoordinates> OnPlayerMoved;

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
    public Tile CurrentTile { get => _currentTile; private set => _currentTile = value; }
    #endregion
    public Tile CurrentTile { get => _currentTile; set => _currentTile = value; }

    #region Monobehavior Functions
    public override void Start()
    {
        base.Start();

        if (IsServer)
        {
            clientId.Value = OwnerClientId;
        }

        if (!IsOwner)
        {
            _collider.enabled = false;
        }

        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
        InventoryRessources.CollectionChanged += ChangeCountInventory;
        SavedRessources.CollectionChanged += ChangeCountSaved;
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
        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(CurrentTile))
        {
            tile.Highlight();
        }
    }

    private void UnhighlightAdjacentTiles()
    {
        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(CurrentTile))
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
            x.TileGridCoordinates.x == CurrentTile.TileGridCoordinates.x && 
            x.TileGridCoordinates.y == CurrentTile.TileGridCoordinates.y) && 
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
        CurrentTile = GridManager.Instance.TileGrid[coordinates];
        CurrentTile.PlayerStepOnTile(this);
        Debug.Log(CurrentTile);

        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition, 0.5f);

        if(IsLocalPlayer)
            HighlightAdjacentTiles();

        OnPlayerMoved?.Invoke(coordinates);
    }


    [ClientRpc]
    public void AddMovementCardClientRpc(int cardId)
    {
        Debug.Log("add movecard Id " + cardId);
        MovementCards.Add(cardId);
    }

    [ClientRpc]
    public void AddChestCardClientRpc(int cardId)
    {
        Debug.Log("add chestcard Id " + cardId, this);
        InventoryChestCards.Add(cardId);
    }
    #endregion

    #region Ressources
    public void ChangeCountInventory(object sender, NotifyCollectionChangedEventArgs e)
    {
        inventoryRessourceCount = InventoryRessources.Count;
    }

    public void ChangeCountSaved(object sender, NotifyCollectionChangedEventArgs e)
    {
        savedRessourceCount = SavedRessources.Count;
    }

    public void RemoveNewestRessource(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(InventoryRessources.Count);
            if(InventoryRessources.Count > 0)
            {
                InventoryRessources.RemoveAt(InventoryRessources.Count - 1);
            }
        }
    }

    public void RemoveNewestChestcard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(InventoryChestCards.Count);
            if (InventoryChestCards.Count > 0)
            {
                InventoryChestCards.RemoveAt(InventoryChestCards.Count - 1);
            }
        }
    }
    #endregion
}

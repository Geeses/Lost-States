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

public enum Direction
{
    left, right, up, down
}

public class Player : Selectable
{
    #region Attributes

    [Header("Debug")]
    public NetworkVariable<ulong> clientId;
    public NetworkVariable<int> movedInCurrentTurn;
    public int inventoryRessourceCount;
    public int savedRessourceCount;
    public bool moveOverUnpassable;
    public int _coinCount;
    public NetworkVariable<ulong> currentSelectedPlayerId = new NetworkVariable<ulong>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public event Action<GridCoordinates> OnPlayerMoved;
    public event Action OnCardPlayed;

    private ObservableCollection<int> _movementCards = new ObservableCollection<int>();
    private ObservableCollection<int> _inventoryChestCards = new ObservableCollection<int>();
    private ObservableCollection<Ressource> _inventoryRessources = new ObservableCollection<Ressource>();
    private ObservableCollection<Ressource> _savedRessources = new ObservableCollection<Ressource>();
    
    private int _movementCardAmountPerCycle = 5;
    private Tile _currentTile;
    private Tile _oldTile;
    private int _moveCount;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Direction _lastMoveDirection;
    private Selectable _currentSelectedTarget;
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
    public Tile OldTile { get => _oldTile; private set => _oldTile = value; }
    public Direction LastMoveDirection { get => _lastMoveDirection; private set => _lastMoveDirection = value; }
    public Selectable CurrentSelectedTarget { get => _currentSelectedTarget; set => _currentSelectedTarget = value; }
    #endregion

    #region Monobehavior Functions
    public override void Start()
    {
        base.Start();

        if (IsServer)
        {
            clientId.Value = OwnerClientId;
        }

        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0, 0)];
        InventoryRessources.CollectionChanged += ChangeCountInventory;
        SavedRessources.CollectionChanged += ChangeCountSaved;
        InputManager.Instance.OnSelect += ChangeCurrentSelectedTarget;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        InventoryRessources.CollectionChanged -= ChangeCountInventory;
        SavedRessources.CollectionChanged -= ChangeCountSaved;
        InputManager.Instance.OnSelect -= ChangeCurrentSelectedTarget;
    }
    #endregion

    #region Select and Highlight
    public override void Select()
    {
        base.Select();

        if (IsOwner)
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

    private void ChangeCurrentSelectedTarget(Selectable selectable)
    {
        CurrentSelectedTarget = selectable;

        Player player = selectable.GetComponent<Player>();
        if (player != null)
        {
            if (IsOwner)
                currentSelectedPlayerId.Value = player.clientId.Value;
        }
    }
    #endregion

    #region Movement
    [ServerRpc(RequireOwnership = false)]
    public void TryMoveServerRpc(GridCoordinates coordinates, bool subtractMoves = true, bool invokeOnPlayerMovedEvent = true)
    {
        Tile tile = GridManager.Instance.TileGrid[coordinates];

        // if tile is adjacent
        if (Array.Find(GridManager.Instance.GetAdjacentTiles(tile), x =>
            x.TileGridCoordinates.x == CurrentTile.TileGridCoordinates.x &&
            x.TileGridCoordinates.y == CurrentTile.TileGridCoordinates.y) &&
            MoveCount > 0
            )
        {
            if (moveOverUnpassable | tile.passable)
            {
                movedInCurrentTurn.Value += 1;

                if (subtractMoves)
                    AddMoveCountClientRpc(-1);

                MoveClientRpc(coordinates, invokeOnPlayerMovedEvent);
            }
        }
    }

    [ClientRpc]
    public void ChangeMoveCountClientRpc(int count)
    {
        MoveCount = count;
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
    }

    [ClientRpc]
    public void AddMoveCountClientRpc(int count)
    {
        MoveCount += count;
        MyPlayerUI.Instance.myMovesText.text = MoveCount.ToString();
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
    }

    [ClientRpc]
    public void PlayCardClientRpc(int cardId)
    {
        //MoveCount = count;
        MyPlayerUI.Instance.myMovesText.text = MoveCount.ToString();
        TurnManager.Instance.currentTurnPlayerMovesText.text = MoveCount.ToString();
        OnCardPlayed.Invoke();
    }

    [ClientRpc]
    public void ChangePlayedMoveCardsClientRpc(int count)
    {
        PlayedMovementCards += count;
    }

    [ClientRpc]
    public void MoveClientRpc(GridCoordinates coordinates, bool invokeEvent = true)
    {
        OldTile = CurrentTile;

        if (IsLocalPlayer)
            UnhighlightAdjacentTiles();

        CurrentTile = GridManager.Instance.TileGrid[coordinates];
        CurrentTile.PlayerStepOnTile(this);

        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition, 0.5f);

        LastMoveDirection = GetMoveDirection(OldTile.TileGridCoordinates, CurrentTile.TileGridCoordinates);

        if (IsLocalPlayer)
            HighlightAdjacentTiles();

        if (invokeEvent)
            OnPlayerMoved?.Invoke(coordinates);
    }


    [ClientRpc]
    public void AddMovementCardClientRpc(int cardId)
    {
        //Debug.Log("add movecard Id " + cardId);
        MovementCards.Add(5);
        MovementCards.Add(6);
        MovementCards.Add(7);
        //MovementCards.Add(cardId);
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

    public Direction GetMoveDirection(GridCoordinates positionBefore, GridCoordinates positionAfter)
    {
        int x_b = positionBefore.x, x_a = positionAfter.x, y_b = positionBefore.y, y_a = positionAfter.y;
        if (x_b == x_a && y_b < y_a)
        {
            return Direction.up;
        }
        else if (x_b == x_a && y_b > y_a)
        {
            return Direction.down;
        }
        else if (y_b == y_a && x_b < x_a)
        {
            return Direction.right;
        }
        else
        {
            return Direction.left;
        }
    }

    public Ressource RemoveNewestRessource(int count)
    {
        Ressource res = 0;

        for (int i = 0; i < count; i++)
        {
            Debug.Log(InventoryRessources.Count);
            if (InventoryRessources.Count > 0)
            {
                res = InventoryRessources[InventoryRessources.Count - 1];
                InventoryRessources.RemoveAt(InventoryRessources.Count - 1);
            }
        }

        return res;
    }

    public int RemoveNewestChestcard(int count)
    {
        int chestcardId = -1;
        for (int i = 0; i < count; i++)
        {
            Debug.Log(InventoryChestCards.Count);
            if (InventoryChestCards.Count > 0)
            {
                chestcardId = InventoryChestCards[InventoryChestCards.Count - 1];
                InventoryChestCards.RemoveAt(InventoryChestCards.Count - 1);
            }
        }

        return chestcardId;
    }
    #endregion
}

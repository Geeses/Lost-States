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
    public NetworkVariable<int> moveCount;
    public List<int> discardedMovementCards = new List<int>();
    public int inventoryRessourceCount;
    public int savedRessourceCount;
    public ulong currentSelectedPlayerId;

    public event Action<ulong> OnEnemyPlayerSelected;
    public event Action<GridCoordinates> OnPlayerMoved;

    private ObservableCollection<int> _movementCards = new ObservableCollection<int>();
    private ObservableCollection<int> _inventoryChestCards = new ObservableCollection<int>();
    private ObservableCollection<Ressource> _inventoryRessources = new ObservableCollection<Ressource>();
    private ObservableCollection<Ressource> _savedRessources = new ObservableCollection<Ressource>();
    private int _coinCount;
    private int _movementCardAmountPerCycle = 5;

    private Tile _currentTile;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Selectable _currentSelectedTarget;
    #endregion

    #region Properties
    public ObservableCollection<Ressource> InventoryRessources { get => _inventoryRessources; set => _inventoryRessources = value; }
    public ObservableCollection<Ressource> SavedRessources { get => _savedRessources; set => _savedRessources = value; }
    public ObservableCollection<int> InventoryChestCards { get => _inventoryChestCards; set => _inventoryChestCards = value; }
    public int CoinCount { get => _coinCount; set => _coinCount = value; }
    public int MovementCardAmountPerCycle { get => _movementCardAmountPerCycle; set => _movementCardAmountPerCycle = value; }
    public ObservableCollection<int> MovementCards { get => _movementCards; set => _movementCards = value; }
    public int MaximumPlayableMovementCards { get => _maximumPlayableMovementCards; set => _maximumPlayableMovementCards = value; }
    public int PlayedMovementCards { get => _playedMovementCards; set => _playedMovementCards = value; }
    public Tile CurrentTile { get => _currentTile; private set => _currentTile = value; }
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

        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
        InventoryRessources.CollectionChanged += ChangeCountInventory;
        SavedRessources.CollectionChanged += ChangeCountSaved;
        InputManager.Instance.OnSelect += ChangeCurrentSelectedTarget;
        moveCount.OnValueChanged += ChangeMoveCountUI;
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
        if(IsOwner)
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
        if(player != null)
        {
            if(IsOwner)
            {
                SetSelectedEnemyPlayerServerRpc(player.clientId.Value);
            }
        }
    }

    [ServerRpc]
    private void SetSelectedEnemyPlayerServerRpc(ulong playerId)
    {
        SetSelectedEnemyPlayerClientRpc(playerId);
    }

    [ClientRpc]
    private void SetSelectedEnemyPlayerClientRpc(ulong playerId)
    {
        currentSelectedPlayerId = playerId;
        OnEnemyPlayerSelected?.Invoke(currentSelectedPlayerId);
    }
    #endregion

    #region Movement
    // forceMove is used for effects, that want to bypass the normal behavior of moving in the game
    [ServerRpc(RequireOwnership = false)]
    public void TryMoveServerRpc(GridCoordinates coordinates, bool forceMove = false)
    {
        Tile tile = GridManager.Instance.TileGrid[coordinates];

        // if tile is adjacent
        if (Array.Find(GridManager.Instance.GetAdjacentTiles(tile), x => 
            x.TileGridCoordinates.x == CurrentTile.TileGridCoordinates.x && 
            x.TileGridCoordinates.y == CurrentTile.TileGridCoordinates.y) && 
            moveCount.Value > 0 &&
            tile.passable ||
            forceMove)
        {
            if (!forceMove)
            {
                movedInCurrentTurn.Value += 1;
                moveCount.Value += -1;
            }
            MoveClientRpc(coordinates);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMoveCountServerRpc(int count)
    {
        moveCount.Value += count;
    }
    private void ChangeMoveCountUI(int previousValue, int newValue)
    {
        TurnManager.Instance.currentTurnPlayerMovesText.text = newValue.ToString();
    }

    [ClientRpc]
    public void ChangePlayedMoveCardsClientRpc(int count)
    {
        PlayedMovementCards += count;
    }

    [ClientRpc]
    public void MoveClientRpc(GridCoordinates coordinates, bool invokeEvent = true)
    {
        if(IsLocalPlayer)
            UnhighlightAdjacentTiles();
        CurrentTile = GridManager.Instance.TileGrid[coordinates];
        CurrentTile.PlayerStepOnTile(this);
        Debug.Log(CurrentTile);

        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition + new Vector3(0, 0, -0.1f), 0.5f);

        if(IsLocalPlayer)
            HighlightAdjacentTiles();

        if(invokeEvent)
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

    public Ressource RemoveNewestRessource(int count)
    {
        Ressource res = 0;

        for (int i = 0; i < count; i++)
        {
            Debug.Log(InventoryRessources.Count);
            if(InventoryRessources.Count > 0)
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

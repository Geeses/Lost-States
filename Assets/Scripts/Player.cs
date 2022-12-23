using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;
using System.Linq;

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
    public NetworkVariable<int> moveCount;
    public NetworkVariable<int> coinCount;
    public NetworkList<int> movementCards;
    public NetworkList<int> inventoryChestCards;
    public NetworkList<int> inventoryRessources;
    public NetworkList<int> savedRessources;
    public List<int> discardedMovementCards = new List<int>();
    public int inventoryRessourceCount;
    public int savedRessourceCount;
    public int _coinCount;
    public ulong currentSelectedPlayerId;
    public event Action<ulong> OnEnemyPlayerSelected;
    public event Action<GridCoordinates> OnPlayerMoved;
    public event Action OnCardPlayed;
    private int _movementCardAmountPerCycle = 5;
    private Tile _currentTile;
    private Tile _oldTile;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Direction _lastMoveDirection;
    private Selectable _currentSelectedTarget;
    private RessourceCollectionCard _ressourceCollectionCard;
    #endregion

    #region Properties
    public int MovementCardAmountPerCycle { get => _movementCardAmountPerCycle; set => _movementCardAmountPerCycle = value; }
    public int MaximumPlayableMovementCards { get => _maximumPlayableMovementCards; set => _maximumPlayableMovementCards = value; }
    public int PlayedMovementCards { get => _playedMovementCards; set => _playedMovementCards = value; }
    public Tile CurrentTile { get => _currentTile; private set => _currentTile = value; }
    public Tile OldTile { get => _oldTile; private set => _oldTile = value; }
    public Direction LastMoveDirection { get => _lastMoveDirection; private set => _lastMoveDirection = value; }
    public Selectable CurrentSelectedTarget { get => _currentSelectedTarget; set => _currentSelectedTarget = value; }
    public RessourceCollectionCard RessourceCollectionCard { get => _ressourceCollectionCard; set => _ressourceCollectionCard = value; }
    #endregion

    #region Monobehavior Functions
    public override void Awake()
    {
        inventoryRessources = new NetworkList<int>();
        savedRessources = new NetworkList<int>();
        movementCards = new NetworkList<int>();
        inventoryChestCards = new NetworkList<int>();

        base.Awake();
    }

    public override void Start()
    {
        base.Start();

        GameManager.Instance.OnGameStart += Initialize;
    }

    private void Initialize()
    {
        if (IsServer)
        {
            clientId.Value = OwnerClientId;
        }

        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0, 0)];
        inventoryRessources.OnListChanged += ChangeCountInventory;
        savedRessources.OnListChanged += ChangeCountSaved;
        InputManager.Instance.OnSelect += ChangeCurrentSelectedTarget;
        moveCount.OnValueChanged += ChangeMoveCountUI;
        savedRessources.OnListChanged += CheckForWin;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.Instance.OnGameStart -= Initialize;

        inventoryRessources.OnListChanged -= ChangeCountInventory;
        savedRessources.OnListChanged -= ChangeCountSaved;
        InputManager.Instance.OnSelect -= ChangeCurrentSelectedTarget;
        moveCount.OnValueChanged -= ChangeMoveCountUI;
        savedRessources.OnListChanged -= CheckForWin;
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
            if (tile)
                tile.Highlight();
        }
    }

    private void UnhighlightAdjacentTiles()
    {
        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(CurrentTile))
        {
            if (tile)
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
    public void TryMoveServerRpc(GridCoordinates coordinates, bool forceMove = false, bool invokeOnPlayerMovedEvent = true)
    {
        Tile tile = GridManager.Instance.TileGrid[coordinates];

        // if tile is adjacent
        bool isAdjecant = GridManager.Instance.GetAdjacentTiles(CurrentTile).ToList().Contains(tile);

        if (isAdjecant && moveCount.Value > 0 && tile.passable || forceMove)
        {
            if (!forceMove)
            {
                movedInCurrentTurn.Value += 1;
                moveCount.Value += -1;
            }
            MoveClientRpc(coordinates, invokeOnPlayerMovedEvent);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeMoveCountServerRpc(int count)
    {
        moveCount.Value = count;
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddMoveCountServerRpc(int count)
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
        // MyPlayerUI.Instance.SetMoveCountText();
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
        transform.DOMove(cellWorldPosition + new Vector3(0, 0, -0.1f), 0.5f);

        LastMoveDirection = GetMoveDirection(OldTile.TileGridCoordinates, CurrentTile.TileGridCoordinates);

        if (IsLocalPlayer)
            HighlightAdjacentTiles();

        if (invokeEvent)
            OnPlayerMoved?.Invoke(coordinates);
    }


    [ClientRpc]
    void AddMovementCardClientRpc(int cardId)
    {
        Debug.Log("add movecard Id " + cardId);
        movementCards.Add(cardId);
    }

    [ClientRpc]
    void AddChestCardClientRpc(int cardId)
    {
        Debug.Log("add chestcard Id " + cardId, this);
        inventoryChestCards.Add(cardId);
    }
    #endregion

    #region Ressources
    void ChangeCountInventory(NetworkListEvent<int> changeEvent)
    {
        inventoryRessourceCount = inventoryRessources.Count;
    }

    void ChangeCountSaved(NetworkListEvent<int> changeEvent)
    {
        savedRessourceCount = savedRessources.Count;
    }

    Direction GetMoveDirection(GridCoordinates positionBefore, GridCoordinates positionAfter)
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

    [ServerRpc(RequireOwnership = false)]
    public void AddRessourceServerRpc(Ressource ressource)
    {
        inventoryRessources.Add((int)ressource);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNewestRessourceServerRpc(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(inventoryRessources.Count);
            if (inventoryRessources.Count > 0)
            {
                inventoryRessources.RemoveAt(inventoryRessources.Count - 1);
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNewestChestcardServerRpc(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(inventoryChestCards.Count);
            if (inventoryChestCards.Count > 0)
            {
                Debug.Log("Remove Chestcard: " + inventoryChestCards[inventoryChestCards.Count - 1]);
                inventoryChestCards.RemoveAt(inventoryChestCards.Count - 1);
            }
        }
    }

    public void SafePlayerRessources()
    {
        for (int i = 0; i < inventoryRessources.Count; i++)
        {
            int tmp = inventoryRessources[i];
            inventoryRessources.Remove(tmp);
            savedRessources.Add(tmp);
        }
    }
    #endregion

    #region Win Condition
    [ClientRpc]
    public void AssignRessourceCollectionCardClientRpc(int ressourceCollectionCardId)
    {
        RessourceCollectionCard = GameManager.Instance.ressourceCollectionCards[ressourceCollectionCardId];
    }

    void CheckForWin(NetworkListEvent<int> changeEvent)
    {
        GameManager.Instance.CheckPlayerForWinServerRpc(clientId.Value);
    }

    #endregion
}

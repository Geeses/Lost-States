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
    public NetworkVariable<int> coinCount;
    public NetworkList<int> movementCards;
    public NetworkList<int> inventoryChestCards;
    public NetworkList<int> inventoryRessources;
    public NetworkList<int> savedRessources;
    public List<int> discardedMovementCards = new List<int>();
    public int inventoryRessourceCount;
    public int savedRessourceCount;
    public ulong currentSelectedPlayerId;

    public event Action<ulong> OnEnemyPlayerSelected;
    public event Action<GridCoordinates> OnPlayerMoved;

    private int _movementCardAmountPerCycle = 5;

    private Tile _currentTile;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Selectable _currentSelectedTarget;
    private RessourceCollectionCard _ressourceCollectionCard;
    #endregion

    #region Properties
    public int MovementCardAmountPerCycle { get => _movementCardAmountPerCycle; set => _movementCardAmountPerCycle = value; }
    public int MaximumPlayableMovementCards { get => _maximumPlayableMovementCards; set => _maximumPlayableMovementCards = value; }
    public int PlayedMovementCards { get => _playedMovementCards; set => _playedMovementCards = value; }
    public Tile CurrentTile { get => _currentTile; private set => _currentTile = value; }
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

        if (IsServer)
        {
            clientId.Value = OwnerClientId;
        }

        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0,0)];
        inventoryRessources.OnListChanged += ChangeCountInventory;
        savedRessources.OnListChanged += ChangeCountSaved;
        InputManager.Instance.OnSelect += ChangeCurrentSelectedTarget;
        moveCount.OnValueChanged += ChangeMoveCountUI;
        savedRessources.OnListChanged += CheckForWin;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
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
        movementCards.Add(cardId);
    }

    [ClientRpc]
    public void AddChestCardClientRpc(int cardId)
    {
        Debug.Log("add chestcard Id " + cardId, this);
        inventoryChestCards.Add(cardId);
    }
    #endregion

    #region Ressources
    public void ChangeCountInventory(NetworkListEvent<int> changeEvent)
    {
        inventoryRessourceCount = inventoryRessources.Count;
    }

    public void ChangeCountSaved(NetworkListEvent<int> changeEvent)
    {
        savedRessourceCount = savedRessources.Count;
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
            if(inventoryRessources.Count > 0)
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
    #endregion

    #region Win Condition
    [ClientRpc]
    public void AssignRessourceCollectionCardClientRpc(int ressourceCollectionCardId)
    {
        RessourceCollectionCard = GameManager.Instance.ressourceCollectionCards[ressourceCollectionCardId];
    }

    private void CheckForWin(NetworkListEvent<int> changeEvent)
    {
        GameManager.Instance.CheckPlayerForWinServerRpc(clientId.Value);
    }

    #endregion
}

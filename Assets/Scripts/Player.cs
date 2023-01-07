using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Collections;

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
    public NetworkVariable<FixedString128Bytes> profileName = new NetworkVariable<FixedString128Bytes>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> clientId = new NetworkVariable<ulong>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> movedInCurrentTurn;
    public NetworkVariable<int> moveCount;
    public NetworkVariable<int> coinCount;
    public NetworkVariable<bool> canMoveOverUnpassable = new NetworkVariable<bool>(default,
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
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

    private Direction _lastMoveDirection;
    private Tile _currentTile;
    private Tile _oldTile;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Selectable _currentSelectedTarget;
    private RessourceCollectionCard _ressourceCollectionCard;
    private int _localMoveCount;
    private int _localMovedInCurrentTurn;
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
    public int LocalMoveCount { get => _localMoveCount; set => _localMoveCount = value; }
    public int LocalMovedInCurrentTurn { get => _localMovedInCurrentTurn; set => _localMovedInCurrentTurn = value; }
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

        if(IsOwner)
            clientId.Value = OwnerClientId;

        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += LoadCompleted;

        if(GameManager.Instance)
        {
            if(GameManager.Instance.isTestScene)
            {
                Initialize();
            }
        }

        // if gamemanager doesnt exist, then we are in relayscene
        if(!GameManager.Instance && IsLocalPlayer)
            profileName.Value = AuthenticationService.Instance.Profile;

        SpawnSelectedSprite();
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
    
    #region Initialize

    private void LoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        Initialize();
    }

    private void Initialize()
    {
        CurrentTile = GridManager.Instance.TileGrid[new GridCoordinates(0, 0)];
        OldTile = GridManager.Instance.TileGrid[new GridCoordinates(0, 0)];
        inventoryRessources.OnListChanged += ChangeCountInventory;
        savedRessources.OnListChanged += ChangeCountSaved;
        InputManager.Instance.OnSelect += ChangeCurrentSelectedTarget;
        moveCount.OnValueChanged += ChangeMoveCountUI;
        moveCount.OnValueChanged += SynchronizeMoveCount;
        movedInCurrentTurn.OnValueChanged += SynchronizeMovedInCurrent;
        savedRessources.OnListChanged += CheckForWin;        
    }

    private void SynchronizeMoveCount(int oldVal, int newVal)
    {
        LocalMoveCount = newVal;
    }
    private void SynchronizeMovedInCurrent(int oldVal, int newVal)
    {
        LocalMovedInCurrentTurn = newVal;
    }

    private void SpawnSelectedSprite()
    {
        selectedSprite = Instantiate(selectedSprite, transform);
        selectedSprite.color = Color.green;

        if(!IsLocalPlayer)
        {
            selectedSprite.color = Color.red;
        }
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
            if(tile)
                tile.Highlight();
        }
    }

    private void UnhighlightAdjacentTiles()
    {
        foreach (Tile tile in GridManager.Instance.GetAdjacentTiles(CurrentTile))
        {
            if(tile)
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
    public void TryMoveServerRpc(GridCoordinates coordinates, bool forceMove = false, bool invokeEvent = true)
    {
        Tile tile = GridManager.Instance.TileGrid[coordinates];

        // if tile is adjacent
        bool isAdjacent = GridManager.Instance.GetAdjacentTiles(CurrentTile).ToList().Contains(tile);

        if (isAdjacent && (moveCount.Value > 0) && (tile.passable || canMoveOverUnpassable.Value) || forceMove)
        {
            if (!forceMove)
            {
                movedInCurrentTurn.Value += 1;
                moveCount.Value += -1;
            }
            MoveClientRpc(coordinates, invokeEvent, forceMove);
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
    public void MoveClientRpc(GridCoordinates coordinates, bool invokeEvent = true, bool forceMove = false)
    {
        OldTile = CurrentTile;

        if (IsLocalPlayer)
        {
            // only clients need a local not-networked value, the server has all correct values
            if (!forceMove && !IsHost)
            {
                LocalMoveCount += -1;
                LocalMovedInCurrentTurn += 1;
            }

            UnhighlightAdjacentTiles();
        }
        CurrentTile = GridManager.Instance.TileGrid[coordinates];
        CurrentTile.PlayerStepOnTile(this);
        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition + new Vector3(0, 0, -0.1f), 0.5f);

        LastMoveDirection = GetMoveDirection(OldTile.TileGridCoordinates, CurrentTile.TileGridCoordinates);

        if (IsLocalPlayer)
            HighlightAdjacentTiles();

        if(invokeEvent)
            OnPlayerMoved?.Invoke(coordinates);
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
                int id = inventoryRessources[inventoryRessources.Count - 1];
                inventoryRessources.Remove(id);
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
                Debug.Log("Remove ChestcardId: " + inventoryChestCards[inventoryChestCards.Count - 1] + " at: " + (inventoryChestCards.Count - 1));
                int id = inventoryChestCards[inventoryChestCards.Count - 1];
                inventoryChestCards.Remove(id);
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

    private void CheckForWin(NetworkListEvent<int> changeEvent)
    {
        GameManager.Instance.CheckPlayerForWinServerRpc(clientId.Value);
    }

    #endregion
}

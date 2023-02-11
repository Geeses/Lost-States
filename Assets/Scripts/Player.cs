using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;
using Unity.Collections;
using System.Collections;
using Unity.Services.Analytics;

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
    private int _movementCardAmountPerCycle = 5;
    private Direction _lastMoveDirection;
    private Tile _currentTile;
    private Tile _oldTile;
    private int _maximumPlayableMovementCards;
    private int _playedMovementCards;
    private Selectable _currentSelectedTarget;
    private RessourceCollectionCard _ressourceCollectionCard;
    private int _ressourceCollectionCardId;
    private int _localMoveCount;
    private int _localMovedInCurrentTurn;
    private int _totalRessourcesObtained;

    // Events
    public event Action<ulong> OnEnemyPlayerSelected;
    public event Action<GridCoordinates> OnPlayerMoved;
    public event Action<Ressource>OnRessourceCollected;
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
    public int RessourceCollectionCardId { get => _ressourceCollectionCardId; set => _ressourceCollectionCardId = value; }
    public int LocalMoveCount { get => _localMoveCount; set => _localMoveCount = value; }
    public int LocalMovedInCurrentTurn { get => _localMovedInCurrentTurn; set => _localMovedInCurrentTurn = value; }
    public int TotalRessourcesObtained { get => _totalRessourcesObtained; set => _totalRessourcesObtained = value; }
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
        CurrentTile = GridManager.Instance.GetTileOnWorldPosition(transform.position);
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

        enemySelectedSprite = Instantiate(enemySelectedSprite, transform);
        enemySelectedSprite.color = Color.red;

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
        if(LocalMoveCount > 0)
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
            if (tile && !canMoveOverUnpassable.Value)
            {
                tile.Highlight();
            }
            else if (tile)
            {
                tile.HighlightUnpassable();
            }
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
            // Function bracked to move other players, when the tile we try to move on has a player on it
            //      this bracked would also run on "SwapPositions" chestcard, since we move
            if (tile.PlayerOnTile != null)
            {
                GridCoordinates newCoords = coordinates + (coordinates - CurrentTile.TileGridCoordinates);
                Debug.Log("OldTile: " + CurrentTile.TileGridCoordinates.ToString() + " New Tile: " + coordinates.ToString() + " Direction: " + (coordinates - CurrentTile.TileGridCoordinates).ToString() + " Coords: " + newCoords.ToString());
                tile.PlayerOnTile.MoveClientRpc(newCoords, true, true);
            }

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
        PlayedMovementCards += count;
    }

    [ClientRpc]
    public void MoveClientRpc(GridCoordinates coordinates, bool invokeEvent = true, bool forceMove = false)
    {
        OldTile = CurrentTile;
        OldTile.PlayerLeavesTile();

        if (IsLocalPlayer)
        {
            // only clients need a local not-networked value, the server has all correct values
            if (!forceMove && !IsHost)
            {
                LocalMoveCount += -1;
                LocalMovedInCurrentTurn += 1;
            }
        }
        UnhighlightAdjacentTiles();

        CurrentTile = GridManager.Instance.TileGrid[coordinates];
        CurrentTile.PlayerStepOnTile(this);
        Vector3 cellWorldPosition = GridManager.Instance.Tilemap.CellToWorld(new Vector3Int(coordinates.x, coordinates.y, 0));
        cellWorldPosition += GridManager.Instance.Tilemap.cellSize / 2;
        transform.DOMove(cellWorldPosition + new Vector3(0, 0, -0.1f), 0.5f);

        LastMoveDirection = GetMoveDirection(OldTile.TileGridCoordinates, CurrentTile.TileGridCoordinates);
        if (IsLocalPlayer && !forceMove)
            HighlightAdjacentTiles();

        QueueOnPlayerMovedForAnalyticsServices();

        if (invokeEvent)
            OnPlayerMoved?.Invoke(coordinates);
    }
    void QueueOnPlayerMovedForAnalyticsServices()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "GameCount", PlayerPrefs.GetInt("GameCount") },
            { "PlayerId",  GetInstanceID()},
            { "NewPositionX", CurrentTile.TileGridCoordinates.x },
            { "NewPositionY", CurrentTile.TileGridCoordinates.y },
            { "OldPositionX", OldTile.TileGridCoordinates.x },
            { "OldPositionY", OldTile.TileGridCoordinates.y },
            { "NewZoneType", (int)CurrentTile.zoneType },
            { "OldZoneType", (int)OldTile.zoneType },
            { "TurnType", (int)TurnManager.Instance.TurnType },
            { "TurnNumber",TurnManager.Instance.CurrentTurnNumber },
            { "TotalTurnCount",TurnManager.Instance.TotalTurnCount },
            { "SessionIdCustom", AnalyticsService.Instance.SessionID }

        };

        // The ‘OnTurnEnd’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnPlayerMoved", parameters);
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

        if(changeEvent.Type == NetworkListEvent<int>.EventType.Add)
        {
            SendRessourcesEvent(changeEvent.Value);
            TotalRessourcesObtained += changeEvent.Value;
        }
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

    private void SendRessourcesEvent(int ressource)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "GameCount", PlayerPrefs.GetInt("GameCount") },
            { "PlayerID", NetworkManager.Singleton.GetInstanceID() },
            { "RessourceType", ressource },
            { "TotalTurnCount", TurnManager.Instance.TotalTurnCount },
            { "TurnNumber", TurnManager.Instance.CurrentTurnNumber },
             { "SessionIdCustom", AnalyticsService.Instance.SessionID }
        };

        // The ‘OnRessourceCollected’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnRessourceCollected", parameters);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNewestRessourceServerRpc(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if(inventoryRessources.Count > i)
            {
                int id = inventoryRessources[inventoryRessources.Count - 1];
                inventoryRessources.Remove(id);
            }
        }

        Battlelog.Instance.AddLogClientRpc(profileName.Value + " hat " + count + " Ressourcen verloren.");
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemoveNewestChestcardServerRpc(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Debug.Log(inventoryChestCards.Count);
            if (inventoryChestCards.Count > i)
            {
                int id = inventoryChestCards[inventoryChestCards.Count - 1];
                inventoryChestCards.Remove(id);
                NetworkManagerUI.Instance.RemoveCardFromPlayerUiClientRpc(id, CardType.Chest, -1, true);
            }
        }

        Battlelog.Instance.AddLogClientRpc(profileName.Value + " hat " + count + " Kistenkarte verloren.");
    }

    public void SafePlayerRessources()
    {
        for (int i = inventoryRessources.Count; i > 0; i--)
        {
            int tmp = inventoryRessources[i - 1];
            inventoryRessources.Remove(tmp);
            savedRessources.Add(tmp);
        }
    }

    public Tuple<int, int, int, int> GetBagRessourcesIndividually(RessourceLocation ressourceLocation)
    {
        NetworkList<int> ressourceList;

        if (ressourceLocation == RessourceLocation.inventory) {
            ressourceList = inventoryRessources;
        }
        else {
            ressourceList = savedRessources;
        }
        int bagFoodCount = 0;
        int bagWaterCount = 0;
        int bagSteelCount = 0;
        int bagWoodCount = 0;

        foreach (int ressource in ressourceList)
        {
            switch (ressource)
            {
                case (int)Ressource.fruit:
                    bagFoodCount += 1;
                    break;
                case (int)Ressource.water:
                    bagWaterCount += 1;
                    break;
                case (int)Ressource.steel:
                    bagSteelCount += 1;
                    break;
                case (int)Ressource.wood:
                    bagWoodCount += 1;
                    break;
                case (int)Ressource.none:
                    Debug.Log("No ressource was found");
                    break;
            }
        }
        return Tuple.Create(bagFoodCount, bagWaterCount, bagSteelCount, bagWoodCount);
    }

    #endregion

    #region Win Condition
    [ClientRpc]
    public void AssignRessourceCollectionCardClientRpc(int ressourceCollectionCardId)
    {
        RessourceCollectionCard = GameManager.Instance.ressourceCollectionCards[ressourceCollectionCardId];
        RessourceCollectionCardId = ressourceCollectionCardId;
    }

    private void CheckForWin(NetworkListEvent<int> changeEvent)
    {
        GameManager.Instance.CheckPlayerForWinServerRpc(clientId.Value);
    }

    #endregion
}

public enum RessourceLocation
{
    inventory,
    safe
};
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : NetworkBehaviour
{
    #region Attributes
    [Header("Options")]
    public bool isTestScene = false;

    [Header("References")]
    public List<RessourceCollectionCard> ressourceCollectionCards = new List<RessourceCollectionCard>();

    [Header("Debug")]
    public bool gameHasStarted;
    public bool gameOver;

    public event Action OnGameStart;
    public event Action<ulong, bool> OnGameEnd;

    private ulong _winnerPlayerId;
    private static GameManager s_instance;

    private List<ulong> _connectedPlayersId = new List<ulong>();
    #endregion

    #region Properties
    public static GameManager Instance { get { return s_instance; } }
    public List<ulong> ConnectedPlayersId { get => _connectedPlayersId; private set => _connectedPlayersId = value; }
    public ulong WinnerPlayerId { get => _winnerPlayerId; set => _winnerPlayerId = value; }
    #endregion

    #region Monobehavior Functions
    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }
    }

    private void Start()
    {
        if(isTestScene)
        {
            NetworkManager.OnServerStarted += () => StartCoroutine(WaitForLobbyJoinedTest());
        }
        else 
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += LoadCompleted;
        }
    }

    private void LoadCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        ConnectedPlayersId = clientsCompleted;
        WaitForLobbyJoined();
    }
    #endregion

    private void WaitForLobbyJoined()
    {
        // After everyone connected, we share the list of players with everyone else
        List<ulong> tmp = new List<ulong>(ConnectedPlayersId);
        foreach (var player in tmp)
        {
            SynchronizeLobbyDataClientRpc(player);
        }

        StartGameClientRpc();

        if (IsServer)
        {
            PlayerNetworkManager.Instance.PlayerDictionary[0].MoveClientRpc(new GridCoordinates(0, 0), false, true);
            PlayerNetworkManager.Instance.PlayerDictionary[1].MoveClientRpc(new GridCoordinates(-1, 0), false, true);

            AssignRessourceCollectionCards();
        }
    }
    
    private IEnumerator WaitForLobbyJoinedTest()
    {
        // Server updates list of all incoming players
        AddClient(NetworkManager.ServerClientId);
        NetworkManager.OnClientConnectedCallback += AddClient;

        // wait until we have every player connected
        yield return new WaitUntil(() => ConnectedPlayersId.Count == 2);

        // After everyone connected, we share the list of players with everyone else
        List<ulong> tmp = new List<ulong>(ConnectedPlayersId);
        foreach (var player in tmp)
        {
            SynchronizeLobbyDataClientRpc(player);
        }

        StartGameClientRpc();

        if (IsServer)
        {
            PlayerNetworkManager.Instance.PlayerDictionary[0].MoveClientRpc(new GridCoordinates(0, 0), false, true);
            PlayerNetworkManager.Instance.PlayerDictionary[1].MoveClientRpc(new GridCoordinates(-1, 0), false, true);

            AssignRessourceCollectionCards();
        }
    }

    private void AssignRessourceCollectionCards()
    {
        List<int> randomIds = new List<int>();

        for (int i = 0; i < ressourceCollectionCards.Count; i++)
        {
            randomIds.Add(i);
        }

        randomIds.Shuffle();

        for (int i = 0; i < PlayerNetworkManager.Instance.PlayerDictionary.Count; i++)
        {
            KeyValuePair<ulong, Player> pair = PlayerNetworkManager.Instance.PlayerDictionary.ElementAt(i);
            Debug.Log("Player " + pair.Value.clientId.Value + " gets ressource collection card " + randomIds[i]);
            pair.Value.AssignRessourceCollectionCardClientRpc(randomIds[i]);
        }
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        var gameCount = PlayerPrefs.GetInt("GameCount");
        PlayerPrefs.SetInt("GameCount", gameCount + 1);
        gameHasStarted = true;
        OnGameStart?.Invoke();
    }

    private void AddClient(ulong clientId)
    {
        ConnectedPlayersId.Add(clientId);
    }


    [ClientRpc]
    private void SynchronizeLobbyDataClientRpc(ulong playerId)
    {
        if(!IsServer)
            ConnectedPlayersId.Add(playerId);
    }

    [ServerRpc(RequireOwnership = false)]
    public void CheckPlayerForWinServerRpc(ulong playerId)
    {
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        List<int> fruits = new List<int>();
        List<int> wood = new List<int>();
        List<int> steel = new List<int>();
        List<int> water = new List<int>();

        foreach (int id in player.savedRessources)
        {
            if ((Ressource)id == Ressource.fruit)
            {
                fruits.Add(id);
            }
            else if ((Ressource)id == Ressource.steel)
            {
                steel.Add(id);
            }
            else if ((Ressource)id == Ressource.water)
            {
                water.Add(id);
            }
            else if ((Ressource)id == Ressource.wood)
            {
                wood.Add(id);
            }
        }

        if (player.RessourceCollectionCard.fruitAmount <= fruits.Count &&
           player.RessourceCollectionCard.woodAmount <= wood.Count &&
           player.RessourceCollectionCard.steelAmount <= steel.Count &&
           player.RessourceCollectionCard.waterAmount <= water.Count)
        {
            if (!gameOver)
            {
                InitializePlayerWinClientRpc(playerId);
            }
        }
    }

    [ClientRpc]
    private void InitializePlayerWinClientRpc(ulong playerId)
    {
        gameOver = true;
        WinnerPlayerId = playerId;
        if (WinnerPlayerId == NetworkManager.Singleton.LocalClientId)
        {
            OnGameEnd?.Invoke(playerId, true);
        } else
        {
            OnGameEnd?.Invoke(playerId, false);
        }
        
        Battlelog.Instance.AddLog(PlayerNetworkManager.Instance.PlayerDictionary[playerId].profileName.Value + " won the game. ez clap");
    }
}

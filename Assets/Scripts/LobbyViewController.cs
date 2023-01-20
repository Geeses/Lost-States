using UnityEngine.UIElements;
using Unity.Netcode;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class LobbyViewController
{
    private ListView _lobbyList;
    private Label _playerInfo;
    private TextField _lobbyName;
    private Toggle _isPrivate;
    private Button _createButton;
    private Button _joinWithCodeButton;
    private Button _refreshButton;

    private VisualTreeAsset _cellTemplate;
    private RelayViewController _relayView;
    private VisualElement _root;
    private LobbyManager _manager;

    private static List<Lobby> _lobbies = new List<Lobby>();
    private NetworkVariable<bool> canStartGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public LobbyViewController(LobbyManager manager, VisualElement root, VisualTreeAsset cellTemplate, RelayViewController relayView)
    {
        _manager = manager;
        _root = root;
        _cellTemplate = cellTemplate;
        _relayView = relayView;

        _lobbyList = _root.Q<ListView>("lobby-list");
        _playerInfo = _root.Q<Label>("player-info");
        _lobbyName = _root.Q<TextField>("lobby-name-text-field");
        _isPrivate = _root.Q<Toggle>("is-private-lobby-toggle");
        _createButton = _root.Q<Button>("create-lobby-button");
        _refreshButton = _root.Q <Button>("refresh-button");
        _joinWithCodeButton = _root.Q<Button>("join-with-code-button");

        _createButton.clicked += AddNewLobby;
        _refreshButton.clicked += RefreshLobbies;
        _lobbyList.itemsSource = _lobbies;
        InitializeList();

        // Relay
        _joinWithCodeButton.clicked += _relayView.Show;

        canStartGame.OnValueChanged += (bool previousValue, bool newValue) => {
            Debug.Log($"Starting Game... " + SceneManager.GetSceneByBuildIndex(0).name);
            NetworkManager.Singleton.SceneManager.LoadScene("2PlayerMap", LoadSceneMode.Single);
        };
    }

    private void InitializeList()
    {
        _lobbyList.makeItem = () =>
        {
            var newListEntry = _cellTemplate.Instantiate();
            var newListEntryLogic = new LobbyCell(_manager);
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElements(newListEntry);
            return newListEntry;
        };

        _lobbyList.bindItem = (item, index) => {
            var cell = item.userData as LobbyCell;
            cell.SetData(_lobbies[index]);
        };
    }

    async void AddNewLobby()
    {
        _createButton.text = "Waiting";
        _playerInfo.text = "Waiting for other players to join";
        if (_isPrivate.value)
        {
            var lobby = await _manager.TryCreatePrivateLobbyAsync(_lobbyName.text);
        }
        else
        {
            var lobby = await _manager.TryCreatePublicLobbyAsync(_lobbyName.text);
        }
        _createButton.clicked -= AddNewLobby;
        RefreshLobbies();
    }

    async void RefreshLobbies()
    { 
        _lobbies = await _manager.GetAllLobbies();
        if (_lobbyName.text == "")
        {
            _playerInfo.text = "Please give your lobby a name or enter the name of an existing lobby";
            return;
        }
        if (_lobbies.Count == 0) {
            _createButton.text = "Create";
            _playerInfo.text = "Timeout: Please create a new Lobby";
            _createButton.clicked += AddNewLobby;
        }
        _lobbyList.itemsSource = _lobbies;
        _lobbyList.Rebuild();
    }
    public void StartGame()
    {
        canStartGame.Value = true;
    }
}

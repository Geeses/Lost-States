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
    private TextInputBaseField<string> _nameTextInput;
    private VisualElement _lobbyScreen;
    private VisualTreeAsset _cellTemplate;
    private RelayViewController _relayView;
    private VisualElement _root;
    private LobbyManager _manager;
    private TextField _usernameTextField;
    private Button _loginButton;
    private VisualElement _authenticationWindow;
    private Label _authenticationInfo;

    private static List<Lobby> _lobbies = new List<Lobby>();

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
        _nameTextInput = _root.Q<TextInputBaseField<string>>("unity-text-input");
        _lobbyScreen = _root.Q<VisualElement>("lobby-screen");
        _loginButton = root.Q<Button>("login-button");
        _usernameTextField = root.Q<TextField>("username");
        _authenticationWindow = root.Q<VisualElement>("authentication-overlay");
        _authenticationInfo = root.Q<Label>("authentication-info");

        _createButton.clicked += AddNewLobby;
        _refreshButton.clicked += RefreshLobbies;
        _lobbyScreen.visible = false;
        InitializeList();

        // Relay
        _joinWithCodeButton.clicked += _relayView.Show;
        _loginButton.clicked += InitializeUnityServices;
    }

    private void InitializeUnityServices()
    {
        if (_usernameTextField.text == "") {
            _authenticationInfo.text = "Please, insert an username";
            return;
        }
        InitializeServices.Instance.InitializeWithUsername(_usernameTextField.text);
        _authenticationWindow.visible = false;
        _lobbyScreen.visible = true;
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
        if (_lobbyName.text == "")
        {
            _playerInfo.text = "Please, enter a lobby name";
            return;
        }

        _playerInfo.text = "Waiting for other players to join";
        if (_isPrivate.value)
        {
            var lobby = await _manager.TryCreatePrivateLobbyAsync(_lobbyName.text);
        }
        else
        {
            var lobby = await _manager.TryCreatePublicLobbyAsync(_lobbyName.text);
        }
        RefreshLobbies();
    }

    async void RefreshLobbies()
    { 
        _lobbies = await _manager.GetAllLobbies();
        if (_lobbies.Count == 0) {
            _playerInfo.text = "Timeout: Please create a new Lobby";
            _createButton.clicked += AddNewLobby;
        }
        _lobbyList.itemsSource = _lobbies;
        _lobbyList.Rebuild();
    }
}

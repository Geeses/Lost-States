using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
public class LobbyViewController
{
    private ListView _lobbyList;
    private Label _playerInfo;
    private TextField _lobbyName;
    private Toggle _isPrivate;
    private Button _createButton;
    private Button _refreshButton;
    private VisualTreeAsset _cellTemplate;
    private VisualElement _root;
    private LobbyManager _manager;
    private TextField _usernameTextField;
    private Button _loginButton;
    private Label _authenticationInfo;
    private Toggle _allowTrackingToggle;

    // Navigation
    private Button showRelayButton;
    private Button showRoomsButton;

    // Windows
    private VisualElement _authenticationWindow;
    private VisualElement _roomsWindow;
    private VisualElement _relayWindow;
    private VisualElement _windows;

    private static List<Lobby> _lobbies = new List<Lobby>();

    public LobbyViewController(LobbyManager manager, VisualElement root, VisualTreeAsset cellTemplate, RelayViewController relayView)
    {
        _manager = manager;
        _root = root;
        _cellTemplate = cellTemplate;

        _allowTrackingToggle = _root.Q<Toggle>("track-data");
        _lobbyList = _root.Q<ListView>("lobby-list");
        _playerInfo = _root.Q<Label>("player-info");
        _lobbyName = _root.Q<TextField>("lobby-name-text-field");
        _isPrivate = _root.Q<Toggle>("is-private-lobby-toggle");
        _createButton = _root.Q<Button>("create-lobby-button");
        _refreshButton = _root.Q <Button>("refresh-button");

        // Navigation
        _loginButton = root.Q<Button>("login-button");
        showRoomsButton = root.Q<Button>("show-rooms-button");
        showRelayButton = root.Q<Button>("show-relay-button");

        showRoomsButton.clicked += GoToRooms;
        showRelayButton.clicked += GoToRelay;
        _loginButton.clicked += InitializeUnityServices;

        // Windows
        _authenticationWindow = root.Q<VisualElement>("authentication-overlay");
        _windows = root.Q<VisualElement>("windows");
        _roomsWindow = root.Q<VisualElement>("rooms-window");
        _relayWindow = root.Q<VisualElement>("relay-window");

        _windows.visible = false;
        _authenticationWindow.visible = true;

        _createButton.clicked += AddNewLobby;
        _refreshButton.clicked += RefreshLobbies;

        InitializeList();

        //Authentication
        _usernameTextField = root.Q<TextField>("username");
        _authenticationInfo = root.Q<Label>("authentication-info");

        

    }

    private void InitializeUnityServices()
    {
        if (_usernameTextField.text == "")
        {
            _authenticationInfo.text = "Please, insert an username";
            return;
        }
        else if (_allowTrackingToggle.value == false)
        {
            _authenticationInfo.text = "Please, I really need your data";
            return;
        }
        else
        {
            InitializeServices.Instance.InitializeWithUsername(_usernameTextField.text);
            GoToWindows();
        }
    }

    private void GoToWindows()
    {
        _authenticationWindow.visible = false;
        _windows.visible = true;
        GoToRooms();
    }

    private void GoToRooms()
    {
        _roomsWindow.visible = true;
        _relayWindow.visible = false;
        showRoomsButton.style.borderBottomWidth = 0;
        showRelayButton.style.borderBottomWidth = 2;
    }
    private void GoToRelay()
    {
        _roomsWindow.visible = false;
        _relayWindow.visible = true;
        showRoomsButton.style.borderBottomWidth = 2;
        showRelayButton.style.borderBottomWidth = 0;
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

        await _manager.TryCreateLobbyAsync(_lobbyName.text, _isPrivate.value);

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

using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;

public class LobbyView
{

    private ListView _lobbyList;
    private Button _createButton;
    private Label _playerInfo;
    private Button _refreshButton;

    private VisualTreeAsset _cellTemplate;
    private VisualElement _root;
    private LobbyManager _manager;

    private static List<Lobby> _lobbies = new List<Lobby>();
    public LobbyView _view;
    
    public LobbyView(LobbyManager manager, VisualElement root, VisualTreeAsset cellTemplate)
    {
        _manager = manager;
        _root = root;
        _cellTemplate = cellTemplate;

        _lobbyList = _root.Q<ListView>("lobby-list");
        _createButton = _root.Q<Button>("create-button");
        _playerInfo = _root.Q<Label>("player-info");
        _refreshButton = root.Q <Button>("refresh-button");

        _createButton.clicked += AddNewLobby;
        _refreshButton.clicked += RefreshLobbies;
        _lobbyList.itemsSource = _lobbies;
        InitializeList();
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
        _createButton.text = "Waiting...";
        _playerInfo.text = "Waiting for other players to join";
        var lobby = await _manager.TryCreatePublicLobbyAsync();
        _createButton.clicked -= AddNewLobby;
        RefreshLobbies();
    }

    async void RefreshLobbies()
    { 
        _lobbies = await _manager.GetAllLobbies();
        if (_lobbies.Count == 0) {
            _createButton.text = "Create";
            _playerInfo.text = "Timeout: Please create a new Lobby";
            _createButton.clicked += AddNewLobby;
        }
        _lobbyList.itemsSource = _lobbies;
        _lobbyList.Rebuild();
    }
}

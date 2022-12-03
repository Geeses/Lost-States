using Unity.Netcode;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Lobby : NetworkBehaviour
{
    ListView _lobbyList;
    Button _createButton;
    private Label _playerInfo;
    private List<LobbyData> _lobbyData = new List<LobbyData>();
    private int _maxPlayersAllowed = 4;
    private string _roomName = "MockRoom";

    [SerializeField]
    VisualTreeAsset _listEntryTemplate;
    public void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _lobbyList = root.Q<ListView>("lobby-list");
        _createButton = root.Q<Button>("create-button");
        _playerInfo = root.Q<Label>("player-info");

        _lobbyList.makeItem = () =>
        {
            var newListEntry = _listEntryTemplate.Instantiate();
            var newListEntryLogic = new LobbyCell();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };

        _lobbyList.bindItem = (item, index) => {

            var cell = item.userData as LobbyCell;
            var usersJoined = _lobbyData[index]._usersJoined;
            cell._playerInfo = this._playerInfo;
            cell._createButton = this._createButton;

            cell.SetData(_lobbyData[index]);

        };

        _lobbyList.itemsSource = _lobbyData;

        _createButton.clicked += AddNewLobby; 
    }

    void AddNewLobby()
    {
        var data = new LobbyData(_roomName, _maxPlayersAllowed, 1);
        _lobbyData.Add(data);
        _lobbyList.Rebuild();
        _createButton.text = "Waiting...";
        _playerInfo.text = "Waiting for other players to join";
        _createButton.clicked -= AddNewLobby;
    }
}

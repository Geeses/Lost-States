using UnityEditor;
using UnityEngine.UIElements;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyCell
{
    private Label _nameLabel;
    private Label _usersJoined;
    public Button _joinButton;

    private LobbyManager _manager;
    private Lobby _lobby;

    public LobbyCell(LobbyManager manager)
    {
        _manager = manager;
    }
    public void SetVisualElements(VisualElement cell)
    {
        _nameLabel = cell.Q<Label>("lobby-name");
        _usersJoined = cell.Q<Label>("joined-count");
        _joinButton = cell.Q<Button>("join-lobby");
    }

    private void StartGame()
    {
        _manager.StartGame();
    }

    public void SetData(Lobby lobby)
    {
        _lobby = lobby;
        _nameLabel.text = lobby.Name;

        if (_lobby.Players.Count == 2)
        {
            _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
            _joinButton.clicked += StartGame;
        }
        else
        {
            _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
            _joinButton.clicked += JoinLobby;
        }
    }

    public void  JoinLobby()
    {
        _manager.JoinLobby(_lobby.Id);
        _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
    }
}
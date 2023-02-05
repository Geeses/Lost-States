using UnityEditor;
using UnityEngine.UIElements;
using Unity.Services.Lobbies.Models;
using System;
using Unity.Netcode;
using UnityEngine;
using Unity.Services.Authentication;
using System.Collections.Generic;

public class LobbyCell
{
    private Label _nameLabel;
    private Label _usersJoined;
    public Button _joinButton;

    private LobbyManager _manager;
    private Lobby _lobby;
    private bool startedWithLobby;

    public LobbyCell(LobbyManager manager)
    {
        _manager = manager;
        startedWithLobby = true;
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
        _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
        _joinButton.clicked -= StartGame;   
        _usersJoined.text = lobby.Players.Count + "/2";

        Debug.Log("LobbyOwnerId: " + lobby.HostId);
        Debug.Log("PlayerId: " + AuthenticationService.Instance.PlayerId);

        if (lobby.Players.Count >= 2 && startedWithLobby)
        {
            if (lobby.HostId == AuthenticationService.Instance.PlayerId)
            {

                _joinButton.text = "Start";
                _joinButton.clicked += StartGame;
                _joinButton.clicked -= JoinLobby;
            }
            else
            {
                _joinButton.clicked -= JoinLobby;
                _joinButton.text = "Wait";
            }
        }


        else 
        {
            if (lobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                _joinButton.text = "Wait";
                _joinButton.clicked -= JoinLobby;
            }
            else
            {
                _joinButton.text = "Join";
                _joinButton.clicked += JoinLobby;
            }
        }
    }

    public async void  JoinLobby()
    {
        _joinButton.clicked -= JoinLobby;
        // only server can start the game ...
        if (NetworkManager.Singleton.IsHost)
        {
            _manager.SetPlayerInfo("Can't be in 2 rooms");
            return;
        }
        _joinButton.text = "Wait";
        _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
        _ = await _manager.JoinLobby(_lobby.Id);
    }
}
using UnityEditor;
using UnityEngine.UIElements;
using Unity.Services.Lobbies.Models;
using System;
using Unity.Netcode;
using UnityEngine;

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

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            Debug.Log("OnClientConnectedCallback: All players are here");
            _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
            if (GameObject.FindGameObjectsWithTag("Player").Length >= 2 && startedWithLobby)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("OnClientConnectedCallback: All players are here");
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
        };
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

        Debug.Log("SetData was called");
        _joinButton.text = "Join";
        _joinButton.clicked -= StartGame;
        _joinButton.clicked += JoinLobby;
    }

    public void  JoinLobby()
    {
        // await
        _joinButton.clicked -= JoinLobby;
        _joinButton.text = "Wait";
        _ = _manager.JoinLobby(_lobby.Id);
        _usersJoined.text = _lobby.Players.Count.ToString() + "/2";
    }
}
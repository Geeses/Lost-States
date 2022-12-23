using UnityEditor;
using UnityEngine.UIElements;
using Unity.Services.Lobbies.Models;

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

        _joinButton.clicked += JoinLobby;
    }

    public void SetData(Lobby lobby)
    {
        _lobby = lobby;
        _nameLabel.text = lobby.Name;
        _usersJoined.text = lobby.Players.Count.ToString();
    }

    public void  JoinLobby()
    {
        _manager.JoinLobby(_lobby.Id);
    }
}
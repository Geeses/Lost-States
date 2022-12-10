using UnityEditor;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class LobbyCell : EditorWindow
{
    private Label _nameLabel;
    private Label _usersJoined;
    public Button _joinButton;
    private LobbyData _lobbyData;
    public Lobby _lobbyDelegate;
    public Label _playerInfo;
    public Button _createButton;
    public void SetVisualElement(VisualElement visualElement)
    {
        _nameLabel = visualElement.Q<Label>("lobby-name");
        _usersJoined = visualElement.Q<Label>("joined-count");
        _joinButton = visualElement.Q<Button>("join-lobby");

        _joinButton.clicked += JoinLobby;
    }

    void JoinLobby()
    {
        if (_lobbyData._usersJoined >= 4)
        {
            _playerInfo.text = "Sorry, room is already full";
            _createButton.text = "Start";
            _createButton.clicked += StartGame;
        }
        else
        {
            _lobbyData._usersJoined += 1;
            _usersJoined.text = _lobbyData.usersInLobby;
        }
    }

    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void SetData(LobbyData lobbyData)
    {
        _lobbyData = lobbyData;
        _nameLabel.text = _lobbyData._name;
        _usersJoined.text = _lobbyData.usersInLobby;
    }
}
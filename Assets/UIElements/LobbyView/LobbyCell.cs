using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LobbyCell : EditorWindow
{
    private Label _nameLabel;
    private Label _usersJoined;
    private Button _joinButton;
    public void SetCell(VisualElement visualElement)
    {
        _nameLabel = visualElement.Q<Label>("lobby-name");
        _usersJoined = visualElement.Q<Label>("joined-count");
        _joinButton = visualElement.Q<Button>("join-button");
    }

    public void SetLogData(string log)
    {
        logLabel.text = log;
    }
}

struct LobbyModel {
    string name;
    int totalUsersAllowed;
    int usersJoined;
}

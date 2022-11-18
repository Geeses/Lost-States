using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Lobby : EditorWindow
{
    VisualTreeAsset _listEntryTemplate;
    ListView lobbyList;
    public void InitializeList(VisualElement root, VisualTreeAsset listElementTemplate)
    {
        _listEntryTemplate = listElementTemplate;

        lobbyList = root.Q<ListView>("lobby-list");

        lobbyList.makeItem = () =>
        {
            var newListEntry = _listEntryTemplate.Instantiate();
            var newListEntryLogic = new LobbyCell();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetCell(newListEntry);
            return newListEntry;
        };

        lobbyList.bindItem = (item, index) => {
            (item.userData as BattlelogCellController).SetLogData();
        };
    }
}

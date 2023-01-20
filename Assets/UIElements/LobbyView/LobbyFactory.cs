using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
public class LobbyFactory : NetworkBehaviour
{
    [SerializeField]
    VisualTreeAsset cellTemplate;

    [SerializeField]
    VisualTreeAsset relayTemplate;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var lobbyManager = new LobbyManager();

        var relayView = relayTemplate.Instantiate();
        var relayScreen = relayView.Q<VisualElement>("relay-screen");
        var overlay = root.Q<VisualElement>("overlay");
        overlay.Add(relayScreen);

        var relayViewLogic = new RelayViewController(root);
        var lobbyViewLogic = new LobbyViewController(lobbyManager, root, cellTemplate, relayViewLogic);
        
    }
}
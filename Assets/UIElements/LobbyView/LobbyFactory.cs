using UnityEngine;
using UnityEngine.UIElements;
using Unity.Netcode;
public class LobbyFactory : NetworkBehaviour
{
    [SerializeField]
    VisualTreeAsset cellTemplate;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var lobbyManager = new LobbyManager();

        var relayViewLogic = new RelayViewController(root);
        var lobbyViewLogic = new LobbyViewController(lobbyManager, root, cellTemplate, relayViewLogic);   
    }
}
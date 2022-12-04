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
        var manager = new LobbyManager();
        var view = new LobbyView(manager, root, cellTemplate);
    }
}

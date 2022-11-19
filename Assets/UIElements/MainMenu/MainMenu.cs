using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : NetworkBehaviour
{
    Button hostButton;
    Button clientButton;

    [SerializeField]
    VisualTreeAsset listEntryTemplate;

    public void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        hostButton = root.Q<Button>("host-button");
        clientButton = root.Q<Button>("client-button");

        hostButton.clicked += StartAsHost;
        clientButton.clicked += StartAsClient;
    }

    void StartAsHost()
    {
        NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void StartAsClient()
    {
        NetworkManager.Singleton.StartHost();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}

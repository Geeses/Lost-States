using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : NetworkBehaviour
{
    Button startHost;
    Button startClient;
    Button startGame;
    TextField usernameField;
    TextField joinCodeField;
    RelayHostData result;
    string mapName = "2PlayerMap_V2";
    
    private NetworkVariable<bool> canStartGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public void Start()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        startHost = root.Q<Button>("start-host");
        startClient = root.Q<Button>("start-client");
        startGame = root.Q<Button>("start-game");
        usernameField = root.Q<TextField>("username-field");
        joinCodeField = root.Q<TextField>("join-code-field");

        startHost.clicked += StartHost;
        startClient.clicked += StartClient;

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (PlayersManager.Instance.PlayersInGame >= 2)
            {
                startGame.text = "Start";
                startGame.clicked += StartGame;
            }
        };

        canStartGame.OnValueChanged += (bool previousValue, bool newValue) => {
            Debug.Log($"Starting Game... " + SceneManager.GetSceneByBuildIndex(0).name);
            NetworkManager.Singleton.SceneManager.LoadScene(mapName, LoadSceneMode.Single);
        };
    }

    public void ChangeMap(string name)
    {
        mapName = name;
    }
    
    public async void StartHost() {
        if (RelayManager.Instance.IsRelayEnabled) 
            result = await RelayManager.Instance.SetupRelay();

        joinCodeField.SetEnabled(true);
        joinCodeField.value = result.JoinCode;

        if (NetworkManager.Singleton.StartHost())
            Debug.Log("Host started");
        else
            Debug.Log("Unable to start host");

        startGame.visible = true;
    }

    public void StartGame()
    {
        canStartGame.Value = true;
    }
    public async void StartClient()
    {
        if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(joinCodeField.text))
            await RelayManager.Instance.JoinRelay(joinCodeField.text);

        if (NetworkManager.Singleton.StartClient())
            Debug.Log("Client started");
        else
            Debug.Log("Unable to start client");
    }
}

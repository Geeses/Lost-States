using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;

public class RelayViewController
{
    Button startHost;
    Button startClient;
    Button startGameButton;
    TextField joinCodeField;
    Label relayInfo;
    RelayHostData result;
    private VisualElement _root;

    private NetworkVariable<bool> canStartGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private bool startedWithRelay = false;
    public RelayViewController(VisualElement root) {
        _root = root;
        startHost = _root.Q<Button>("start-host");
        startClient = _root.Q<Button>("start-client");
        startGameButton = _root.Q<Button>("start-game");
        joinCodeField = _root.Q<TextField>("join-code-field");
        relayInfo = _root.Q<Label>("relay-info");

        startHost.clicked += StartHost;
        startClient.clicked += StartClient;

        relayInfo.visible = false;
        startGameButton.visible = false;

        canStartGame.OnValueChanged += (bool previousValue, bool newValue) => {
            Debug.Log($"Starting Game... " + SceneManager.GetSceneByBuildIndex(0).name);
            NetworkManager.Singleton.SceneManager.LoadScene("2PlayerMap_V2", LoadSceneMode.Single);
        };
    }

    public async void StartHost() {
        startedWithRelay = true;
        if (RelayManager.Instance.IsRelayEnabled) 
            result = await RelayManager.Instance.SetupRelay();

        joinCodeField.SetEnabled(true);
        joinCodeField.value = result.JoinCode;

        if (NetworkManager.Singleton.StartHost())
        {
            relayInfo.visible = true;
            relayInfo.text = "Host started. Waiting for Client...";;
        }
        else
        {
            relayInfo.visible = true;
            // most of the times, because a host was already initialized in that game instance
            // fix: see if possible to remove old host and create a new room
            relayInfo.text = "Unable to start host";
        }

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length >= 2 && startedWithRelay)
            {
                if (NetworkManager.Singleton.IsHost)
                {
                    Debug.Log("OnClientConnectedCallback: All players are here");
                    startGameButton.visible = true;
                    relayInfo.visible = false;
                    startGameButton.text = "Start";
                    startGameButton.clicked += StartGame;
                    relayInfo.visible = false;
                }
            }
        };
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
        {
            Debug.Log("Client started");
            relayInfo.text = "Waiting for server to start Game";
        }
        else {
            Debug.Log("Unable to start client");
            relayInfo.text = "Unable to start client";
        }     
    }
}
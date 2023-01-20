using Unity.Netcode;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class RelayViewController
{
    Button startHost;
    Button startClient;
    Button startGameButton;
    Button relayCloseButton;
    TextField joinCodeField;
    Label relayInfo;
    VisualElement relayScreen;
    RelayHostData result;

    private NetworkVariable<bool> canStartGame = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private VisualElement _root;
    
    public RelayViewController(VisualElement root) {
        _root = root;
        startHost = _root.Q<Button>("start-host");
        startClient = _root.Q<Button>("start-client");
        startGameButton = _root.Q<Button>("start-game");
        joinCodeField = _root.Q<TextField>("join-code-field");
        relayInfo = _root.Q<Label>("relay-info");
        relayCloseButton = _root.Q<Button>("relay-close");
        relayScreen = _root.Q<VisualElement>("relay-screen");

        startHost.clicked += StartHost;
        startClient.clicked += StartClient;
        relayInfo.visible = false;
        startGameButton.visible = false;
        relayCloseButton.clicked += Hide;
        Hide();

        NetworkManager.Singleton.OnClientConnectedCallback += (id) =>
        {
            if (PlayersManager.Instance.PlayersInGame >= 2)
            {
                startGameButton.visible = true;
                startGameButton.text = "Start";
                startGameButton.clicked += StartGame;
            }
        };

        canStartGame.OnValueChanged += (bool previousValue, bool newValue) => {
            Debug.Log($"Starting Game... " + SceneManager.GetSceneByBuildIndex(0).name);
            NetworkManager.Singleton.SceneManager.LoadScene("2PlayerMap", LoadSceneMode.Single);
        };
    }

    public async void StartHost() {
        if (RelayManager.Instance.IsRelayEnabled) 
            result = await RelayManager.Instance.SetupRelay();

        joinCodeField.SetEnabled(true);
        joinCodeField.value = result.JoinCode;

        if (NetworkManager.Singleton.StartHost())
        {
            relayInfo.visible = true;
            relayInfo.text = "Host started. Waiting for Client...";
            Debug.Log("Host started");
        }
        else
        {
            relayInfo.visible = true;
            relayInfo.text = "Unable to start host";
            Debug.Log("Unable to start host");
        }

        startGameButton.visible = true;
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
        else {
            Debug.Log("Unable to start client");
            relayInfo.text = "Unable to start client";
        }     
    }

    public void Show()
    {
        relayScreen.visible = true;
    }

    public void Hide()
    {
        relayScreen.visible = false;
    }
}

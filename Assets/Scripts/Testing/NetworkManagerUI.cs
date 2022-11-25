using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private TMPro.TMP_Text moveCountText;
    [SerializeField] private TMPro.TMP_Text playerId;
    [SerializeField] private TMPro.TMP_Text currentTurnPlayerId;
    [SerializeField] private TMPro.TMP_Text currentTurnPlayerMoves;
    [SerializeField] private HorizontalLayoutGroup cardLayoutGroup;

    private void Awake()
    {
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
        });

        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
        });
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WinUi : NetworkBehaviour
{

    #region Attributes
    [Header("References")]
    public TMPro.TextMeshProUGUI winText;
    public TMPro.TextMeshProUGUI loseText;
    #endregion

    #region Properties

    #endregion

    #region Monobehavior Functions

    private void Awake()
    {
        winText.gameObject.SetActive(false);
        loseText.gameObject.SetActive(false);
    }

    void Start()
    {
        GameManager.Instance.OnGameEnd += SetupText;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnGameEnd -= SetupText;
    }

    #endregion

    private void SetupText(ulong playerID, bool hasWon)
    {
        if (GameManager.Instance.WinnerPlayerId == NetworkManager.LocalClientId)
        {
            winText.gameObject.SetActive(true);
        }
        else
        {
            loseText.gameObject.SetActive(true);
        }
    }
}


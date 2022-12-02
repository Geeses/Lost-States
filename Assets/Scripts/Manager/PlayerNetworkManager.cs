using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;

public class PlayerNetworkManager : NetworkManager
{
    public Dictionary<ulong, Player> playerDictionary = new Dictionary<ulong, Player>();

    private void Start()
    {
        GameManager.Instance.OnGameStart += GetPlayerReferences;
    }

    private void GetPlayerReferences()
    {
        if (!IsServer)
            return;

        foreach (KeyValuePair<ulong, NetworkClient> entry in ConnectedClients)
        {
            playerDictionary.Add(entry.Key, entry.Value.PlayerObject.GetComponent<Player>());
        }
    }
}

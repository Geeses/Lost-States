using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using System.Linq;

public class PlayerNetworkManager : NetworkManager
{
    private Dictionary<ulong, Player> _playerDictionary = new Dictionary<ulong, Player>();
    private List<GameObject> _playerlist = new List<GameObject>();

    private static PlayerNetworkManager s_instance;

    public static PlayerNetworkManager Instance { get { return s_instance; } }

    public Dictionary<ulong, Player> PlayerDictionary { get => _playerDictionary; private set => _playerDictionary = value; }

    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }
    }

    private void Start()
    {
        GameManager.Instance.OnGameStart += GetPlayerReferences;
    }

    private void GetPlayerReferences()
    {
        _playerlist = GameObject.FindGameObjectsWithTag("Player").ToList();

        foreach (var item in _playerlist)
        {
            Player player = item.GetComponent<Player>();
            PlayerDictionary.Add(player.clientId.Value, player);
            Debug.Log(player.clientId.Value + " had been added", player);
        }
    }
}

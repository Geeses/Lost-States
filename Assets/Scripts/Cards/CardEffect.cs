using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    // player who played card
    private Player _player;
    private List<Player> _enemyPlayers;

    protected Player Player { get => _player; private set => _player = value; }
    protected List<Player> EnemyPlayers { get => _enemyPlayers; private set => _enemyPlayers = value; }

    internal virtual void Initialize(Player player)
    {
        EnemyPlayers = new List<Player>();
        _player = player;

        foreach (KeyValuePair<ulong, Player> entry in PlayerNetworkManager.Instance.PlayerDictionary)
        {
            // if its not the same clientId as the local player´s id, then its the enemy
            if(!entry.Key.Equals(_player.clientId.Value))
            {
                EnemyPlayers.Add(entry.Value);
            }
        }
    }

    public virtual void ExecuteEffect()
    {
        LogData.shared.AddLog("Effect: " + this);
        Debug.Log("execute effect " + this);
    }

    public virtual void RevertEffect()
    {
        LogData.shared.AddLog("Effect: " + this);
        Debug.Log("revert effect " + this);
    }
}


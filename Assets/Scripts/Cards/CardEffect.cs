using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    // player who played card
    private Player _player;
    private List<Player> _enemyPlayer = new List<Player>();

    protected Player Player { get => _player; set => _player = value; }

    internal virtual void Initialize(Player player)
    {
        _player = player;

        foreach (KeyValuePair<ulong, Player> entry in PlayerNetworkManager.Instance.PlayerDictionary)
        {
            // if its not the same clientId as the local player´s id, then its the enemy
            if(!entry.Key.Equals(_player.clientId.Value))
            {
                _enemyPlayer.Add(entry.Value);
            }
        }
    }

    public virtual void ExecuteEffect()
    {
        Debug.Log("execute effect " + this);
    }

    public virtual void RevertEffect()
    {
        Debug.Log("revert effect " + this);
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    [Header("Card Effect Options")]
    public bool executeInstantly = true;
    [SerializeField] private int _executeAfterTurns = 0;

    // player who played card
    private Player _player;
    private List<Player> _enemyPlayers;
    private int _timeToExecute;

    public Player Player { get => _player; protected set => _player = value; }
    public List<Player> EnemyPlayers { get => _enemyPlayers; protected set => _enemyPlayers = value; }
    public int TimeToExecute { get => _timeToExecute; set => _timeToExecute = value; }

    internal virtual void Initialize(Player player)
    {
        EnemyPlayers = new List<Player>();
        _player = player;
        TimeToExecute = _executeAfterTurns;

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
        Debug.Log("execute effect " + this);
    }

    public virtual void RevertEffect()
    {
        Debug.Log("revert effect " + this);
    }
}


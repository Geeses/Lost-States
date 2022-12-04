using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    // player who played card
    private Player _player;
    private List<Player> _enemyPlayer;

    protected Player Player { get => _player; set => _player = value; }

    internal virtual void Initialize(Player player)
    {
        _player = player;
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


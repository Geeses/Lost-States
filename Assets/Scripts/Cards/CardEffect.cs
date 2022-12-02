using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffect : ScriptableObject
{
    private Player _player;

    protected Player Player { get => _player; set => _player = value; }

    internal virtual void Initialize(Player player)
    {
        _player = player;
    }

    public virtual void ExecuteEffect()
    {
        
    }

    public virtual void RevertEffect()
    {

    }
}


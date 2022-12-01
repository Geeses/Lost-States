using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/Effect", order = 1)]
public class CardEffect : ScriptableObject
{
    private Player player;

    protected Player Player { get => player; set => player = value; }

    public virtual void ExecuteEffect()
    {
        
    }

    public virtual void RevertEffect()
    {

    }
}


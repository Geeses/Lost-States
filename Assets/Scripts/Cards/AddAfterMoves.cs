using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/AddOnPickupEffect", order = 1)]
public class AddAfterMoves : CardEffect
{
    [Header("Options")]
    public int moveConditionCount;
    public int coinCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        Player.OnPlayerMoved += AddAfterMove;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void AddAfterMove(GridCoordinates obj)
    {
        Player.CoinCount += coinCount;
    }
}


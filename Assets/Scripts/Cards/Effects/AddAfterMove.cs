using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Rewards
{
    Coins,
    Chestcard
}
[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/AddAfterMove", order = 1)]
public class AddAfterMove : CardEffect
{
    public Rewards reward;
    public int rewardAmount;
    public int moveCount;
    #region Attributes

    #endregion

    #region Properties

    #endregion

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.OnPlayerMoved += AddRewardAfterMove;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.OnPlayerMoved -= AddRewardAfterMove;
    }

    private void AddRewardAfterMove(GridCoordinates obj)
    {
        if (Player.movedInCurrentTurn.Value == moveCount)
        {
            if (reward == Rewards.Coins)
            {
                Player.CoinCount += rewardAmount;
            }
        }
    }
}


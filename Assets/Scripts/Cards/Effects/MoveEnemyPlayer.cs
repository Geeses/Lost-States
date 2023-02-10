using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/Move Enemy Player", order = 1)]
public class MoveEnemyPlayer : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        foreach (var enemy in EnemyPlayers)
        {
            enemy.AddMoveCountServerRpc(1);
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurnTypeCondition
{
    Any,
    Day,
    Night
}

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/RemoveNewestItems", order = 1)]
public class RemoveNewestItems : CardEffect
{
    [Header("Options")]
    public TurnTypeCondition turnType;
    public TileZone tileZone;
    public PickupType itemType;
    public int itemCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        if(TurnManager.Instance.TurnType == TurnType.Day)
        {
            if (turnType == TurnTypeCondition.Any || turnType == TurnTypeCondition.Day)
            {
                RemoveNewestPickup();
            }
        }
        else if(TurnManager.Instance.TurnType == TurnType.Night)
        {
            if (turnType == TurnTypeCondition.Any || turnType == TurnTypeCondition.Night)
            {
                RemoveNewestPickup();
            }
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void RemoveNewestPickup()
    {
        foreach (Player enemy in EnemyPlayers)
        {
            if(enemy.CurrentTile.zoneType == tileZone)
            {
                if(itemType == PickupType.Ressource)
                {
                    enemy.RemoveNewestRessourceServerRpc(itemCount);
                }
                else if(itemType == PickupType.Chest)
                {
                    enemy.RemoveNewestChestcardServerRpc(itemCount);
                }
            }
        }
    }
}


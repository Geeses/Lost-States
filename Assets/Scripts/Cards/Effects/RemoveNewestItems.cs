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

public enum PlayerType
{
    Enemy,
    Local
}

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/RemoveNewestItems", order = 1)]
public class RemoveNewestItems : CardEffect
{
    [Header("Options")]
    public PlayerType playerType;
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
        if (playerType == PlayerType.Enemy)
        {
            foreach (Player enemy in EnemyPlayers)
            {
                if (enemy.CurrentTile.zoneType == tileZone || tileZone == TileZone.none)
                {
                    if (itemType == PickupType.Ressource)
                    {
                        enemy.RemoveNewestRessourceServerRpc(itemCount);
                    }
                    else if (itemType == PickupType.Chest)
                    {
                        Debug.Log("remove chest card " + enemy.inventoryChestCards.Count);
                        enemy.RemoveNewestChestcardServerRpc(itemCount);
                    }
                }
            }
        }
        else if (playerType == PlayerType.Local)
        {
            if (itemType == PickupType.Ressource)
            {
                Player.RemoveNewestRessourceServerRpc(itemCount);
            }
            else if (itemType == PickupType.Chest)
            {
                Debug.Log("remove chest card " + Player.inventoryChestCards.Count);
                Player.RemoveNewestChestcardServerRpc(itemCount);
            }
        }
    }
}


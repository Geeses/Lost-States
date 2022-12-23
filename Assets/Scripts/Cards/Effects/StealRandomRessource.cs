using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/StealNewestItems", order = 1)]
public class StealRandomRessource : CardEffect
{
    [Header("Options")]
    public TurnTypeCondition turnType;
    public TileZone tileZone;
    public PickupType itemType;
    public int itemCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        if (TurnManager.Instance.TurnType == TurnType.Day)
        {
            if (turnType == TurnTypeCondition.Any || turnType == TurnTypeCondition.Day)
            {
                StealNewestPickup();
            }
        }
        else if (TurnManager.Instance.TurnType == TurnType.Night)
        {
            if (turnType == TurnTypeCondition.Any || turnType == TurnTypeCondition.Night)
            {
                StealNewestPickup();
            }
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void StealNewestPickup()
    {
        foreach (Player enemy in EnemyPlayers)
        {
            if (enemy.CurrentTile.zoneType == tileZone)
            {
                if (itemType == PickupType.Ressource)
                {
                    List<Ressource> newestRessources = new List<Ressource>();
                    for (int i = 0; i < itemCount; i++)
                    {
                        newestRessources.Add((Ressource)enemy.inventoryRessources[enemy.inventoryRessources.Count - 1 - i]);
                    }

                    enemy.RemoveNewestRessourceServerRpc(itemCount);
                    foreach (Ressource ressource in newestRessources)
                    {
                        Player.AddRessourceServerRpc(ressource);
                    }                    
                }
                else if (itemType == PickupType.Chest)
                {
                    //Player.AddChestCardClientRpc(enemy.RemoveNewestChestcardServerRpc(itemCount));
                }
            }
        }
    }
}


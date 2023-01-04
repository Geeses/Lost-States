using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/Cycle Card", order = 1)]
public class CycleCard : CardEffect
{
    [Header("Options")]
    public CardType cardType;
    public int itemCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        if(cardType == CardType.Chest)
        {
            for (int i = 0; i < itemCount; i++)
            {
                if(Player.inventoryChestCards.Count > 0)
                {
                    Player.RemoveNewestChestcardServerRpc(1);
                    CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
                }
            }
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}


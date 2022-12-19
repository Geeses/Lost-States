using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/GetNewChestCard", order = 1)]

public class GetNewChestCard : CardEffect
{
    public override void ExecuteEffect()
    {
        Player.InventoryChestCards.CollectionChanged += GetChestCard;
        base.ExecuteEffect();
    }

    private void GetChestCard(object sender, NotifyCollectionChangedEventArgs e)
    {
        CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.InventoryChestCards.CollectionChanged -= GetChestCard;
    }
}

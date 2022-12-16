using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/AddOneExtra", order = 1)]

public class AddOneExtra : CardEffect
{
    [Header("Options")]
    public int moveCount;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.InventoryRessources.CollectionChanged += AddExtraRessource;
        Player.InventoryChestCards.CollectionChanged += AddExtraChestCard;
    }

    public override void RevertEffect()
    {
        Player.InventoryRessources.CollectionChanged -= AddExtraRessource;
        Player.InventoryChestCards.CollectionChanged -= AddExtraChestCard;
        base.RevertEffect();
    }

    private void AddExtraRessource(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (Player.movedInCurrentTurn.Value == moveCount) {
            foreach (Ressource item in e.NewItems) {
                switch (item)
                {
                    case Ressource.fruit: 
                        Player.InventoryRessources.Add(Ressource.fruit);
                        break;
                    case Ressource.water:
                        Player.InventoryRessources.Add(Ressource.water);
                        break;
                    case Ressource.steel:
                        Player.InventoryRessources.Add(Ressource.steel);
                        break;
                    case Ressource.wood:
                        Player.InventoryRessources.Add(Ressource.wood);
                        break;
                    default:
                        break;
                }
            }
        }     
    }

    private void AddExtraChestCard(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (Player.movedInCurrentTurn.Value == moveCount)
        {
            foreach (int cardId in e.NewItems)
            {
                CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
            }
        }
    }
}
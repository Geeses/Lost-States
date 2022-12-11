using System.Collections.Specialized;
using UnityEngine;

public enum PickupType
{
    Ressource,
    Chest
}

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/AddOnPickupEffect", order = 1)]
public class AddOnPickup : CardEffect
{
    [Header("Options")]
    public PickupType pickupType;
    public int moveCount;

    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.InventoryRessources.CollectionChanged += AddMoves;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.InventoryRessources.CollectionChanged -= AddMoves;
    }

    private void AddMoves(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                Player.ChangeMoveCountClientRpc(3);
            }
        }
    }
}


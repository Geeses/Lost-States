using System.Collections.Specialized;
using Unity.Netcode;
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
        Player.inventoryRessources.OnListChanged += AddMoves;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player.inventoryRessources.OnListChanged -= AddMoves;
    }

    private void AddMoves(NetworkListEvent<int> changeEvent)
    {
        if (changeEvent.Type == NetworkListEvent<int>.EventType.Add)
        {
            Player.ChangeMoveCountServerRpc(moveCount);
        }
    }
}


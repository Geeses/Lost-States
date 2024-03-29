using UnityEngine;
using Unity.Netcode;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/GetNewChestCard", order = 1)]

public class GetNewChestCard : CardEffect
{
    public override void ExecuteEffect()
    {
        
        CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
        base.ExecuteEffect();
    }

    private void GetChestCard(NetworkListEvent<int> changeEvent)
    {
        CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}

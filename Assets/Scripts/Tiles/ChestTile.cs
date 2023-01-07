using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChestTile : NetworkBehaviour, ITileExtension
{
    [Header("Options")]
    public int count;

    [Header("References")]
    [SerializeField] private SpriteRenderer chestClosedRenderer;
    [SerializeField] private SpriteRenderer chestUsedRenderer;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += GivePlayerChestCard;

        if(count > 0)
        {
            chestUsedRenderer.enabled = false;
            chestClosedRenderer.enabled = true;
        }
    }

    public void GivePlayerChestCard(Player player)
    {
        _cachedPlayer = player;

        for (int i = 0; i < count; i++)
        {
            if(player.IsLocalPlayer)
                CardManager.Instance.AddChestCardToPlayerServerRpc(_cachedPlayer.clientId.Value);
        }

        DisableResourceClientRpc();
    }

    [ClientRpc]
    private void DisableResourceClientRpc()
    {
        count = 0;
        chestUsedRenderer.enabled = true;
        chestClosedRenderer.enabled = false;
    }
}

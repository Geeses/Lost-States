using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChestTile : NetworkBehaviour
{
    [Header("Options")]
    public int count;

    [Header("Ressource Sprite Renderer")]
    [SerializeField] private SpriteRenderer ressourceRenderer;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += GivePlayerChestCard;
    }

    private void GivePlayerChestCard(Player player)
    {
        _cachedPlayer = player;

        for (int i = 0; i < count; i++)
        {
            if(player.IsLocalPlayer)
                CardManager.Instance.AddChestCardToPlayerServerRpc(_cachedPlayer.clientId.Value);
        }

        count = 0;
        ressourceRenderer.material.color = Color.red;
    }
}

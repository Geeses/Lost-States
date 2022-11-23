using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestTile : MonoBehaviour
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
            _cachedPlayer.InventoryChestCards.Add(0);
        }

        count = 0;
        ressourceRenderer.material.color = Color.red;
    }
}

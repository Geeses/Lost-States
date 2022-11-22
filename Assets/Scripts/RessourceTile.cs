using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceTile : MonoBehaviour
{
    [Header("Options")]
    public Ressource ressourceType;
    public int ressourceCount;

    [Header("Ressource Sprite Renderer")]
    [SerializeField] private SpriteRenderer ressourceRenderer;

    private Tile _tile;
    private Player _cachedPlayer;

    private void Start()
    {
        _tile = GetComponent<Tile>();
        _tile.OnStepOnTile += GivePlayerRessource;
    }

    private void GivePlayerRessource(Player player)
    {
        _cachedPlayer = player;

        for (int i = 0; i < ressourceCount; i++)
        {
            _cachedPlayer.InventoryRessources.Add(ressourceType);
        }

        ressourceCount = 0;
        ressourceRenderer.material.color = Color.red;
    }
}

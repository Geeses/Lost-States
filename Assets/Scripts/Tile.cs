using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileZone
{
    none,
    safe,
    neutral,
    danger
}

public class Tile : Selectable
{
    [Header("Options")]
    public bool passable;
    public TileZone zoneType;

    private GridCoordinates _tileGridCoordinates;
    private bool _movementAllowed;

    public event Action<Player> OnStepOnTile;

    public GridCoordinates TileGridCoordinates { get => _tileGridCoordinates; set => _tileGridCoordinates = value; }
    public bool MovementAllowed { get => _movementAllowed; set => _movementAllowed = value; }

    public void PlayerStepOnTile(Player player)
    {
        OnStepOnTile?.Invoke(player);
    }

    public override void Highlight()
    {
        base.Highlight();

        if(!passable)
        {
            _spriteRenderer.material.color = Color.red;
        }
        else
        {
            _spriteRenderer.material.color = Color.green;
        }
    }

    public override void Unhighlight()
    {
        base.Unhighlight();
    }
}

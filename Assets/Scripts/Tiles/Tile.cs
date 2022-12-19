using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    #region Attributes
    [Header("Options")]
    public bool passable;
    public TileZone zoneType;

    private GridCoordinates _tileGridCoordinates;
    private bool _movementAllowed;
    private List<ITileExtension> tileExtensions = new List<ITileExtension>();

    public event Action<Player> OnStepOnTile;
    #endregion

    #region Properties
    public GridCoordinates TileGridCoordinates { get => _tileGridCoordinates; set => _tileGridCoordinates = value; }
    public bool MovementAllowed { get => _movementAllowed; set => _movementAllowed = value; }
    public List<ITileExtension> TileExtensions { get => tileExtensions; set => tileExtensions = value; }
    #endregion

    #region Monobehaviors
    public override void Start()
    {
        base.Start();
        GetTileExtensions();
    }

    #endregion

    #region Highlight

    public override void Highlight()
    {
        base.Highlight();

        if (!passable)
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
    #endregion

    public void PlayerStepOnTile(Player player)
    {
        OnStepOnTile?.Invoke(player);
    }

    private void GetTileExtensions()
    {
        TileExtensions = GetComponents<ITileExtension>().ToList();
    }
}

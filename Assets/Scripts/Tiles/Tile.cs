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

    public event Action<Player> OnStepOnTile;

    private GridCoordinates _tileGridCoordinates;
    private bool _movementAllowed;
    private List<ITileExtension> _tileExtensions = new List<ITileExtension>();
    private Player _playerOnTile;
    #endregion

    #region Properties
    public GridCoordinates TileGridCoordinates { get => _tileGridCoordinates; set => _tileGridCoordinates = value; }
    public bool MovementAllowed { get => _movementAllowed; set => _movementAllowed = value; }
    public List<ITileExtension> TileExtensions { get => _tileExtensions; set => _tileExtensions = value; }
    public Player PlayerOnTile { get => _playerOnTile; set => _playerOnTile = value; }
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
            _spriteRenderer.material.color = new Color(0.7264151f, 0.4694286f, 0.4694286f);
        }
        else
        {
            _spriteRenderer.material.color = new Color(0.39f, 0.936f, 0.475f);
        }
    }

    public void HighlightUnpassable()
    {
        base.Highlight();
        _spriteRenderer.material.color = new Color(0.39f, 0.936f, 0.475f);
    }

    public override void Unhighlight()
    {
        base.Unhighlight();
    }
    #endregion

    public void PlayerStepOnTile(Player player)
    {
        PlayerOnTile = player;
        OnStepOnTile?.Invoke(player);
    }

    public void PlayerLeavesTile()
    {
        PlayerOnTile = null;
    }

    private void GetTileExtensions()
    {
        TileExtensions = GetComponents<ITileExtension>().ToList();
    }
}

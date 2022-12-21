using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Tilemaps;

// Coordinates in a grid
// used as a small datapackage for networking
public struct GridCoordinates : INetworkSerializable
{
    public int x;
    public int y;

    public GridCoordinates(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
    }
}

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap _tilemap;

    [Header("Options")]
    public Vector3 gridCenterWorldPosition;

    private Dictionary<GridCoordinates, Tile> _tileGrid = new Dictionary<GridCoordinates, Tile>();

    private static GridManager s_instance;

    public static GridManager Instance { get { return s_instance; } }

    public Tilemap Tilemap { get => _tilemap;}
    public Dictionary<GridCoordinates, Tile> TileGrid { get => _tileGrid;}

    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }

        if (_tilemap == null)
        {
            _tilemap = GetComponentInChildren<Tilemap>();
        }

    }

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        foreach (Transform tileTransform in Tilemap.transform)
        {
            Vector3Int tileGridPos = Tilemap.WorldToCell(tileTransform.position);
            GridCoordinates coords = new GridCoordinates(tileGridPos.x, tileGridPos.y);
            Tile tile = tileTransform.GetComponent<Tile>();

            if(tile != null)
            {
                tile.TileGridCoordinates = coords;
                _tileGrid.Add(coords, tile);
            }
            else
            {
                Debug.LogError("Non Tile-Object in Tilemap.", tileTransform);
            }         
        }
    }

    public Tile[] GetAdjacentTiles(Tile tile)
    {
        Tile[] adjacentTiles = new Tile[4];

        GridCoordinates upperTileCoords = new GridCoordinates(tile.TileGridCoordinates.x, tile.TileGridCoordinates.y + 1);
        GridCoordinates rightTileCoords = new GridCoordinates(tile.TileGridCoordinates.x + 1, tile.TileGridCoordinates.y);
        GridCoordinates downTileCoords = new GridCoordinates(tile.TileGridCoordinates.x, tile.TileGridCoordinates.y - 1);
        GridCoordinates leftTileCoords = new GridCoordinates(tile.TileGridCoordinates.x - 1, tile.TileGridCoordinates.y);

        if(TileGrid.ContainsKey(upperTileCoords))
            adjacentTiles[0] = TileGrid[new GridCoordinates(tile.TileGridCoordinates.x, tile.TileGridCoordinates.y + 1)];

        if (TileGrid.ContainsKey(rightTileCoords))
            adjacentTiles[1] = TileGrid[new GridCoordinates(tile.TileGridCoordinates.x + 1, tile.TileGridCoordinates.y)];

        if (TileGrid.ContainsKey(downTileCoords))
            adjacentTiles[2] = TileGrid[new GridCoordinates(tile.TileGridCoordinates.x, tile.TileGridCoordinates.y - 1)];

        if (TileGrid.ContainsKey(leftTileCoords))
            adjacentTiles[3] = TileGrid[new GridCoordinates(tile.TileGridCoordinates.x - 1, tile.TileGridCoordinates.y)];

        return adjacentTiles;
    }

    public List<Tile> GetTilesInProximity(Tile tile, int radius)
    {
        List<Tile> adjacentTiles = new List<Tile>();

        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                adjacentTiles.Add(TileGrid[new GridCoordinates(tile.TileGridCoordinates.x + x, tile.TileGridCoordinates.y + y)]);
            }
        }

        return adjacentTiles;
    }
}

using System.Collections.Generic;
using System.Linq;
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
    private int _width;
    private int _height;
    private int _yMax;
    private int _yMin;
    private int _xMax;
    private int _xMin;

    public static GridManager Instance { get { return s_instance; } }

    public Tilemap Tilemap { get => _tilemap; }
    public Dictionary<GridCoordinates, Tile> TileGrid { get => _tileGrid; }
    public int Width { get => _width; }
    public int Height { get => _height; }




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
        _width = GetWidth();
        _height = GetHeight();
    }

    private void InitializeGrid()
    {
        foreach (Transform tileTransform in Tilemap.transform)
        {
            Vector3Int tileGridPos = Tilemap.WorldToCell(tileTransform.position);

            GridCoordinates coords = new GridCoordinates(tileGridPos.x, tileGridPos.y);
            Tile tile = tileTransform.GetComponent<Tile>();


            if (tile != null)
            {
                tile.TileGridCoordinates = coords;
                if (!_tileGrid.ContainsKey(coords))
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

    public int GetWidth()
    {
        List<int> xValues = new List<int>();
        foreach (KeyValuePair<GridCoordinates, Tile> entry in TileGrid)
        {
            xValues.Add(entry.Key.x);

        }
        _xMax = xValues.Distinct().Max();
        _xMin = xValues.Distinct().Min();
        return xValues.Distinct().Count();
    }

    public int GetHeight()
    {
        List<int> yValues = new List<int>();
        foreach (KeyValuePair<GridCoordinates, Tile> entry in TileGrid)
        {
            yValues.Add(entry.Key.y);
        }
        _yMax = yValues.Distinct().Max();
        _yMin = yValues.Distinct().Min();
        return yValues.Distinct().Count();
    }

    public List<Tile> GetTilesInDirection(Tile origin, Direction direction)
    {
        List<Tile> tiles = new List<Tile>();

        if (direction == Direction.left)
        {
            var end = _xMax - origin.TileGridCoordinates.x;
            for (int i = 0; i <= end; i++)
            {
                var coordinates = new GridCoordinates(origin.TileGridCoordinates.x + i, origin.TileGridCoordinates.y);
                tiles.Add(TileGrid[coordinates]);
            }
        }

        else if (direction == Direction.right)
        {
            var end = origin.TileGridCoordinates.x - _xMin;
            for (int i = 0; i <= end; i++)
            {
                tiles.Add(TileGrid[new GridCoordinates(origin.TileGridCoordinates.x - i, origin.TileGridCoordinates.y)]);
            }
        }

        else if (direction == Direction.up)
        {
            var end = _yMax - origin.TileGridCoordinates.y;
            for (int i = 0; i <= end; i++)
            {
                tiles.Add(TileGrid[new GridCoordinates(origin.TileGridCoordinates.x, origin.TileGridCoordinates.y + i)]);
            }
        }

        else
        {
            var end = origin.TileGridCoordinates.y - _yMin;
            for (int i = 0; i <= end; i++)
            {
                tiles.Add(TileGrid[new GridCoordinates(origin.TileGridCoordinates.x, origin.TileGridCoordinates.y - i)]);
            }
        }
        return tiles;
    }
}

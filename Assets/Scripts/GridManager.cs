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
    [SerializeField] private Tilemap _tilemap;

    private static GridManager s_instance;

    public static GridManager Instance { get { return s_instance; } }

    public Tilemap Tilemap { get => _tilemap; set => _tilemap = value; }

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
}

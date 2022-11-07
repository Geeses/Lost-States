using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

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

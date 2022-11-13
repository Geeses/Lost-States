using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    private List<Player> _connectedPlayers = new List<Player>();
    private Queue<Player> _playerTurnQueue = new Queue<Player>();
    private Player _currentTurnPlayer;
    private static TurnManager s_instance;

    public static TurnManager Instance { get { return s_instance; } }

    public List<Player> ConnectedPlayers { get => _connectedPlayers; }
    public Queue<Player> PlayerTurnQueue { get => _playerTurnQueue; }
    public Player CurrentTurnPlayer { get => _currentTurnPlayer;}

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
    }

    private void Start()
    {
        
    }
}

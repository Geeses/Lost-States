using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    #region Attributes
    public event Action OnGameStart;
    public event Action OnGameEnd;

    private static GameManager s_instance;
    #endregion

    #region Properties
    public static GameManager Instance { get { return s_instance; } }
    #endregion

    #region Monobehavior Functions
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
}
#endregion

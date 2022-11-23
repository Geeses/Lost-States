using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    [Header("Movement Cards")]
    public List<MovementCardBase> movementCards = new List<MovementCardBase>();

    private static CardManager s_instance;

    public static CardManager Instance { get { return s_instance; } }

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

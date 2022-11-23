using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    Ressource,
    Chest
}

public class AddMovesOnPickup : MonoBehaviour
{
    [Header("Options")]
    public PickupType pickupType;
    public int moveCount;
}

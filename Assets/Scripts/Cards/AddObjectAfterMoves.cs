using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InventoryItems
{
    MovementCard,
    ChestCard,
    Coin
}

public class AddObjectAfterMoves : MonoBehaviour
{
    [Header("Options")]
    public InventoryItems item;
    public int itemCount;
    public int moveCount;
}

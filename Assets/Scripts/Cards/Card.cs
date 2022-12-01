using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Movement,
    Chest,
    Event
}

[CreateAssetMenu(fileName = "Card", menuName = "Scriptable Cards/Card", order = 1)]
public class Card : ScriptableObject
{
    [Header("Options")]
    public CardType cardType;
    public int id;
    public int baseMoveCount;
    public GameObject CardUi;
    public CardEffect cardEffect;
}


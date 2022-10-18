using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Card Scriptables")]
public class Card : SerializedScriptableObject
{
    public List<Effect> cardEffects;
}

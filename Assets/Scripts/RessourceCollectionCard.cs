using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Ressource Collection Card", order = 1)]
public class RessourceCollectionCard : ScriptableObject
{
    public int fruitAmount;
    public int woodAmount;
    public int steelAmount;
    public int waterAmount;
}


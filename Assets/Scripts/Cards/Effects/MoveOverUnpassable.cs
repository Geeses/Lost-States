using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/MoveOverUnpassable", order = 1)]

public class MoveOverUnpassable : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.moveOverUnpassable = true;
    }
    public override void RevertEffect()
    {
        Player.moveOverUnpassable = false;
        base.RevertEffect();
    }
}

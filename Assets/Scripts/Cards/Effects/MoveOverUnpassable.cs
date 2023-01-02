using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/MoveOverUnpassable", order = 1)]

public class MoveOverUnpassable : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Debug.Log("Effect: Move Over Unpassable Executed");
        Player.canMoveOverUnpassable.Value = true;
    }
    
    public override void RevertEffect()
    {
        Player.canMoveOverUnpassable.Value = false;
        base.RevertEffect();
    }
}

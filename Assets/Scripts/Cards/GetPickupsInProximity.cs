using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/GetPickupsInProximity", order = 1)]
public class GetPickupsInProximity : CardEffect
{
    [Header("Options")]
    public PickupType pickupType;
    public int tileRadius;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();

        // for every enemy player
        // get their adjacent tiles through gridmanager
        foreach (var item in GridManager.Instance.GetTilesInProximity(GridManager.Instance.TileGrid[new GridCoordinates(0, 0)], tileRadius))
        {
            Debug.Log(item, item);
            item.Highlight();
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }
}


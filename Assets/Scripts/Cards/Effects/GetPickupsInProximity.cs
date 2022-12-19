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
        PickupItemsInRange();
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    public void PickupItemsInRange()
    {
        foreach (Player enemyPlayer in EnemyPlayers)
        {
            foreach (Tile tile in GridManager.Instance.GetTilesInProximity(GridManager.Instance.TileGrid[enemyPlayer.CurrentTile.TileGridCoordinates], tileRadius))
            {
                if (pickupType == PickupType.Chest)
                {
                    foreach (ITileExtension extension in tile.TileExtensions)
                    {
                        if (extension is ChestTile)
                        {
                            ChestTile chest = tile.GetComponent<ChestTile>();
                            chest.GivePlayerChestCard(Player);
                        }
                    }
                }
                else if (pickupType == PickupType.Ressource)
                {
                    foreach (ITileExtension extension in tile.TileExtensions)
                    {
                        if (extension is RessourceTile)
                        {
                            RessourceTile ressource = tile.GetComponent<RessourceTile>();
                            ressource.GivePlayerRessource(Player);
                        }
                    }
                }
            }
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/DoubleReward", order = 1)]

public class DoubleRewardOnMove : CardEffect
{
    [Header("Options")]
    public int moveCount;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        Player.OnPlayerMoved += DoubleRessource;
    }

    public override void RevertEffect()
    {
        Player.OnPlayerMoved -= DoubleRessource;
        base.RevertEffect();
    }

    private void DoubleRessource(GridCoordinates coordinates)
    {
        Debug.Log("Double Ressource was called");
        var tile = Player.CurrentTile;

        if (Player.movedInCurrentTurn.Value == moveCount)
        {
            Debug.Log("PlayerMovedInCurrentTurn is moveCount");
            foreach (ITileExtension extension in tile.TileExtensions)
            {
                if (extension is RessourceTile)
                {
                    Debug.Log("Current tile is ressource tile");
                    RessourceTile ressource = tile.GetComponent<RessourceTile>();
                    switch (ressource.ressourceType)
                    {
                        case Ressource.fruit:
                            Debug.Log("Added extra fruit");
                            Player.InventoryRessources.Add(Ressource.fruit);
                            break;
                        case Ressource.water:
                            Debug.Log("Added extra water");
                            Player.InventoryRessources.Add(Ressource.water);
                            break;
                        case Ressource.steel:
                            Debug.Log("Added extra steel");
                            Player.InventoryRessources.Add(Ressource.steel);
                            break;
                        case Ressource.wood:
                            Debug.Log("Added extra wood");
                            Player.InventoryRessources.Add(Ressource.wood);
                            break;
                        case Ressource.none:
                            Debug.Log("No ressource was found");
                            break;
                    }
                }
                else if (extension is ChestTile)
                {
                    Debug.Log("Extra Chest Card Added");
                    CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
                }
            }
        }
    }
}
using UnityEngine;

public enum MoveEffectCondition
{
    None,
    OnLast,
    OnFirst
}

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/DoubleReward", order = 1)]

public class DoubleRewardOnMove : CardEffect
{
    [Header("Options")]
    public MoveEffectCondition condition;
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
        //Debug.Log("DoubleRewardOnMove.DoubleRessource was called");
        var tile = Player.CurrentTile;
        //Debug.Log(Player.LocalMovedInCurrentTurn + " " + Player.LocalMoveCount);
        if (Player.LocalMovedInCurrentTurn == moveCount && condition == MoveEffectCondition.None)
        {
            //Debug.Log("PlayerMovedInCurrentTurn is moveCount");
            foreach (ITileExtension extension in tile.TileExtensions)
            {
                if (extension is RessourceTile)
                {
                    GiveRessource((RessourceTile)extension);
                }
                else if (extension is ChestTile)
                {
                    //Debug.Log("Extra Chest Card Added");
                    CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
                }
            }
        }
        else if (Player.LocalMoveCount == 0 && condition == MoveEffectCondition.OnLast)
        {
            //Debug.Log("PlayerMovedInCurrentTurn is moveCount");
            foreach (ITileExtension extension in tile.TileExtensions)
            {
                if (extension is RessourceTile)
                {
                    GiveRessource((RessourceTile)extension);
                }
                else if (extension is ChestTile)
                {
                    //Debug.Log("Extra Chest Card Added");
                    CardManager.Instance.AddChestCardToPlayerServerRpc(Player.clientId.Value);
                }
            }
        }
    }

    private void GiveRessource(RessourceTile tile)
    {
        Debug.Log("Current tile is ressource tile");
        RessourceTile ressource = tile.GetComponent<RessourceTile>();
        switch (ressource.ressourceType)
        {
            case Ressource.fruit:
                Debug.Log("Added extra fruit");
                Player.AddRessourceServerRpc(Ressource.fruit);
                break;
            case Ressource.water:
                Debug.Log("Added extra water");
                Player.AddRessourceServerRpc(Ressource.water);
                break;
            case Ressource.steel:
                Debug.Log("Added extra steel");
                Player.AddRessourceServerRpc(Ressource.steel);
                break;
            case Ressource.wood:
                Debug.Log("Added extra wood");
                Player.AddRessourceServerRpc(Ressource.wood);
                break;
            case Ressource.none:
                Debug.Log("No ressource was found");
                break;
        }
    }
}
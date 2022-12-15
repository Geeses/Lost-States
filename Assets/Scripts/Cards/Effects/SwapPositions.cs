using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/SwapPositions", order = 1)]
public class SwapPositions : CardEffect
{
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        InputManager.Instance.OnSelect += WaitUntilSelectedPlayer;
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        InputManager.Instance.OnSelect -= WaitUntilSelectedPlayer;
    }

    private void WaitUntilSelectedPlayer(Selectable selectable)
    {
        Player selectedPlayer = PlayerNetworkManager.Instance.PlayerDictionary[Player.currentSelectedPlayerId.Value];
        Debug.Log(selectedPlayer);
        if(selectedPlayer != null)
        {
            GridCoordinates playerCoords = Player.CurrentTile.TileGridCoordinates;
            GridCoordinates enemyCoords = selectedPlayer.CurrentTile.TileGridCoordinates;

            Player.MoveClientRpc(enemyCoords, false);
            selectedPlayer.MoveClientRpc(playerCoords, false);
            Debug.Log("wooosh teleport!");
            InputManager.Instance.OnSelect -= WaitUntilSelectedPlayer;
        }
    }
}


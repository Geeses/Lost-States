using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Effect", menuName = "Scriptable Cards/PlayDiscardedCards", order = 1)]
public class PlayDiscardedCards : CardEffect
{
    public int amount;
    public override void ExecuteEffect()
    {
        base.ExecuteEffect();
        PlayCards();
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
    }

    private void PlayCards()
    {
        Debug.Log(Player.clientId.Value + " Player, is saying that we should play this card.");
        CardManager.Instance.TryPlayMovementCardServerRpc(Player.discardedMovementCards[Player.discardedMovementCards.Count - 1], -1, Player.clientId.Value, true);
    }
}


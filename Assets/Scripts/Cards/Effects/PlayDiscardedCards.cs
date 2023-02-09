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
        for (int i = 0; i < amount; i++)
        {
            if(Player.discardedMovementCards.Count > i)
            {
                Card card = CardManager.Instance.GetMovementCardById(Player.discardedMovementCards[Player.discardedMovementCards.Count - i - 1]);
                Player.AddMoveCountServerRpc(card.baseMoveCount);
            }

            if (EnemyPlayers[0].discardedMovementCards.Count > i)
            {
                Card card = CardManager.Instance.GetMovementCardById(EnemyPlayers[0].discardedMovementCards[EnemyPlayers[0].discardedMovementCards.Count - i - 1]);
                Player.AddMoveCountServerRpc(card.baseMoveCount);
            }
        }
    }
}


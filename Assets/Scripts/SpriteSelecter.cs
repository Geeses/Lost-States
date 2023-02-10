using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SpriteSelecter : NetworkBehaviour
{
    #region Attributes
    private Animator _animator;
    private Player _player;
#endregion

#region Properties

#endregion

#region Monobehavior Functions

    void Start()
    {
        _animator = GetComponent<Animator>();
        _player = GetComponent<Player>();

        StartCoroutine(SelectPlayerSprite());
    }

    #endregion

    private IEnumerator SelectPlayerSprite()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.gameHasStarted);

        if (OwnerClientId == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }

        _animator.SetInteger("PlayerId", (int)_player.clientId.Value);

        CardManager.Instance.OnChestCardPlayed += CardPlayAnimation;
        CardManager.Instance.OnMovementCardPlayed += CardPlayAnimation;
    }

    private void CardPlayAnimation(int cardId, ulong playerId)
    {
        if(_player.clientId.Value == playerId)
        {
            if(playerId == 0)
                _animator.SetTrigger("OnCardPlayed1");
            if(playerId == 1)
                _animator.SetTrigger("OnCardPlayed2");
        }
    }
}


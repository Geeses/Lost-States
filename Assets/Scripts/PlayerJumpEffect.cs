using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerJumpEffect : MonoBehaviour
{

    #region Attributes
    public float jumpPower;
    public int jumpCount;
    public float jumpDuration;

    private Player _player;
    #endregion

    #region Monobehavior Functions

    void Start()
    {
        _player = GetComponentInParent<Player>();

        if(CardManager.Instance && _player)
        {
            CardManager.Instance.OnChestCardPlayed += Jump;
            CardManager.Instance.OnMovementCardPlayed += Jump;
        }
    }

    private void OnDisable()
    {
        if (CardManager.Instance)
        {
            CardManager.Instance.OnChestCardPlayed -= Jump;
            CardManager.Instance.OnMovementCardPlayed -= Jump;
        }
    }

    private void Jump(int arg1, ulong playerId)
    {
        if(_player.clientId.Value == playerId)
            transform.DOLocalJump(transform.localPosition, jumpPower, jumpCount, jumpDuration);
    }
        
    #endregion
}


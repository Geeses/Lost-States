using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{

    #region Attributes
    [Header("Options")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 3f;

    private Camera _cam;

#endregion

#region Monobehavior Functions

    void Start()
    {
        _cam = GetComponent<Camera>();
        CardManager.Instance.OnChestCardPlayed += ShakeScreen;
    }

    private void ShakeScreen(int cardId, ulong playerId)
    {
        _cam.DOShakePosition(shakeDuration, shakeStrength);
    }

    #endregion

}


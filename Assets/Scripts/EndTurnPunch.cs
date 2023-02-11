using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnPunch : MonoBehaviour
{

    #region Attributes
    public float punchHeight = 1f;
    public float punchDuration = 0.5f;

    public Color cantEndTurnColor;

    private RectTransform _rectTransform;
    private Image _image;
    private Color _oldColor;
    #endregion

    #region Monobehavior Functions

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _oldColor = GetComponent<Image>().color;

        if(TurnManager.Instance)
            TurnManager.Instance.OnTurnStart += ChangeEndTurnButton;
    }

    private void OnDisable()
    {
        if (TurnManager.Instance)
            TurnManager.Instance.OnTurnStart -= ChangeEndTurnButton;
    }

    private void ChangeEndTurnButton(ulong playerId)
    {
        if(PlayerNetworkManager.Instance.LocalClientId == playerId)
        {
            _rectTransform.DOPunchAnchorPos(new Vector2(0, punchHeight), punchDuration);
            _image.DOColor(_oldColor, punchDuration);
        }
        else
        {
            _rectTransform.DOPunchAnchorPos(new Vector2(0, punchHeight), punchDuration);
            _image.DOColor(cantEndTurnColor, punchDuration);
        }
    }

    #endregion
}


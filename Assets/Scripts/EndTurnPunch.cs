using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class EndTurnPunch : MonoBehaviour
{

    #region Attributes
    [Header("Options")]
    public float punchHeight = 1f;
    public float punchDuration = 0.5f;

    public Color cantEndTurnColor;
    public Color cantEndTurnTextColor;

    [Header("References")]
    [SerializeField] private TMPro.TextMeshProUGUI endTurnText;

    private RectTransform _rectTransform;
    private Image _image;
    private Color _oldColor;
    private Color _oldTextColor;
    #endregion

    #region Monobehavior Functions

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _image = GetComponent<Image>();
        _oldColor = GetComponent<Image>().color;
        _oldTextColor = endTurnText.color;

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
            endTurnText.DOColor(_oldTextColor, punchDuration);
        }
        else
        {
            _rectTransform.DOPunchAnchorPos(new Vector2(0, punchHeight), punchDuration);
            _image.DOColor(cantEndTurnColor, punchDuration);
            endTurnText.DOColor(cantEndTurnTextColor, punchDuration);
        }
    }

    #endregion
}


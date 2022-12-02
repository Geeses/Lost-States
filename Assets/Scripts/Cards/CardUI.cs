using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUi : NetworkBehaviour, IPointerDownHandler
{
    #region Attributes
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;
    public TMPro.TextMeshProUGUI moveCountText;

    private int _cardId;

    public event Action OnCardPlayed;
    #endregion

    #region Properties
    public int CardId { get => _cardId; set => _cardId = value; }
    #endregion

    #region Monobehavior Functions
    public void OnPointerDown(PointerEventData eventData)
    {
        TryPlayCard();
    }
    #endregion

    public void Initialize(Card card)
    {
        _cardId = card.id;
        moveCountText.text = card.baseMoveCount.ToString();
    }

    public void TryPlayCard()
    {
        CardManager.Instance.TryPlayMovementCardServerRpc(CardId, gameObject.GetInstanceID(), NetworkManager.LocalClientId);
    }
}


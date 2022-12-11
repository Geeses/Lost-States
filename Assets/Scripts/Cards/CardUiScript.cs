using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUIScript : NetworkBehaviour, IPointerDownHandler
{
    #region Attributes
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;
    public TMPro.TextMeshProUGUI moveCountText;

    private int _cardId;
    private Card _card;
    private CardType _cardType;

    public event Action OnCardPlayed;
    #endregion

    #region Properties
    public int CardId { get => _cardId; set => _cardId = value; }
    public CardType CardType { get => _cardType; set => _cardType = value; }
    #endregion

    #region Monobehavior Functions
    public void OnPointerDown(PointerEventData eventData)
    {
        TryPlayCard();
    }
    #endregion

    public void Initialize(Card card)
    {
        CardId = card.id;
        _card = card;
        CardType = card.cardType;

        if(moveCountText)
            moveCountText.text = card.baseMoveCount.ToString();
    }

    public void TryPlayCard()
    {
        if(_card.cardType is CardType.Movement)
            CardManager.Instance.TryPlayMovementCardServerRpc(CardId, gameObject.GetInstanceID(), NetworkManager.LocalClientId);
        else if(_card.cardType is CardType.Chest)
            CardManager.Instance.TryPlayChestCardServerRpc(CardId, gameObject.GetInstanceID(), NetworkManager.LocalClientId);
    }
}


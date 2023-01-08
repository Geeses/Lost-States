using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class CardUiScript : NetworkBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region Attributes
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;
    public TMPro.TextMeshProUGUI moveCountText;

    private int _cardId;
    private Card _card;
    private CardType _cardType;
    private Canvas _canvas;
    private RectTransform _rectTransform;
    private Tween _posTween;
    private Tween _scaleTween;

    public event Action OnCardPlayed;
    #endregion

    #region Properties
    public int CardId { get => _cardId; set => _cardId = value; }
    public CardType CardType { get => _cardType; set => _cardType = value; }
    #endregion

    #region Monobehavior Functions

    void Start()
    {
        _canvas = GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();

        if (_posTween == null)
        {
            _posTween = _rectTransform.DOBlendableLocalMoveBy(new Vector2(0, 110), 0.2f).Pause();
            _posTween.SetAutoKill(false);
        }

        if (_scaleTween == null)
        {
            _scaleTween = _rectTransform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.2f).Pause();
            _scaleTween.SetAutoKill(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _canvas.overrideSorting = true;
        _canvas.sortingOrder = 1;

        if(!_posTween.IsPlaying())
        {
            _posTween.PlayForward();
        }

        if(!_scaleTween.IsPlaying())
        {
            _scaleTween.PlayForward();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _posTween.PlayBackwards();
        _scaleTween.PlayBackwards();
        _canvas.sortingOrder = 0;
        _canvas.overrideSorting = false;
    }

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
        if (_card.cardType is CardType.Movement)
            CardManager.Instance.TryPlayMovementCardServerRpc(CardId, gameObject.GetInstanceID(), NetworkManager.LocalClientId);
        else if(_card.cardType is CardType.Chest)
            CardManager.Instance.TryPlayChestCardServerRpc(CardId, gameObject.GetInstanceID(), NetworkManager.LocalClientId);
    }
}


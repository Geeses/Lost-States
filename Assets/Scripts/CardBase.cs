using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBase : NetworkBehaviour, IPointerDownHandler
{
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;

    public event Action OnCardPlayed;

    protected Collider2D _collider;

    public virtual void PlayCard()
    {
        OnCardPlayed?.Invoke();
        Destroy(gameObject);
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        PlayCard();
    }
}

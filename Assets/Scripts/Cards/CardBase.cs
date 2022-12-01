using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardBase
{
    [Header("Card Base References")]
    public int id;
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;

    public event Action OnCardPlayed;

    protected Collider2D _collider;

    public virtual void PlayCard()
    {
        OnCardPlayed?.Invoke();
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        PlayCard();
    }
}

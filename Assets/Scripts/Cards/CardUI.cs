using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerDownHandler
{
    #region Attributes
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;

    public event Action OnCardPlayed;
    #endregion

    #region Monobehavior Functions
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        PlayCard();
    }
    #endregion

    public virtual void PlayCard()
    {
        OnCardPlayed?.Invoke();
        Destroy(gameObject);
    }
}


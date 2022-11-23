using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardBase : MonoBehaviour
{
    [Header("Card Base References")]
    public Image backgroundImage;
    public TMPro.TextMeshProUGUI cardText;

    public event Action OnCardPlayed;

    public void PlayCard()
    {
        OnCardPlayed?.Invoke();
    }
}

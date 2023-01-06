using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleLogSlider : MonoBehaviour
{
    public ScrollRect Rect;
    public Slider ScrollSlider;

    private void Update()
    {
        Rect.verticalNormalizedPosition = ScrollSlider.value;
    }
}


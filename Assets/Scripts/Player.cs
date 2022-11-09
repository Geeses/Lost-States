using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Selectable
{
    public void Move(Selectable selectable)
    {
        transform.DOMove(selectable.transform.position, 0.5f);
    }
}

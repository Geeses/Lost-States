using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider collider;
    protected bool selected;

    public bool Selected { get => selected; protected set => selected = value; }


    public virtual void Select()
    {
        selected = true;
    }

    public virtual void Unselect()
    {
        selected = false;
    }

}

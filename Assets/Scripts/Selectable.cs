using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{
    protected bool selected;

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Collider2D _collider;

    public bool Selected { get => selected; protected set => selected = value; }


    public virtual void Select()
    {
        _spriteRenderer.material.color = Color.red;
        selected = true;
    }

    public virtual void Unselect()
    {
        _spriteRenderer.material.color = Color.white;
        selected = false;
    }

    public virtual void Highlight()
    {
        _spriteRenderer.material.color = Color.green;
    }

    public virtual void Unhighlight()
    {
        _spriteRenderer.material.color = Color.white;
    }
}

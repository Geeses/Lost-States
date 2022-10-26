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
        Debug.Log(gameObject.name + " selected.");
        _spriteRenderer.material.color = Color.red;
        selected = true;
    }

    public virtual void Unselect()
    {
        Debug.Log(gameObject.name + " unselected.");
        _spriteRenderer.material.color = Color.white;
        selected = false;
    }

}

using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Selectable : NetworkBehaviour, INetworkSerializable
{
    [Header("References")]
    public SpriteRenderer selectedSprite;

    [Header("Debugging")]
    [SerializeField] protected bool selected;

    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Collider2D _collider;

    public bool Selected { get => selected; protected set => selected = value; }

    public virtual void Awake()
    {

    }

    public virtual void Start()
    {

    }

    public virtual void Select()
    {
        selectedSprite.gameObject.SetActive(true);
        selected = true;
    }

    public virtual void Unselect()
    {
        selectedSprite.gameObject.SetActive(false);
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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
    }
}

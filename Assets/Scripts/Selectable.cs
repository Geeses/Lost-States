using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Selectable : NetworkBehaviour, INetworkSerializable
{
    [Header("References")]
    public SpriteRenderer selectedSprite;
    public SpriteRenderer enemySelectedSprite;

    [Header("Debugging")]
    [SerializeField] protected bool selected;
    [SerializeField] protected bool enemySelected;

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
        TryEnemySelectServerRpc(true);
    }

    public virtual void Unselect()
    {
        selectedSprite.gameObject.SetActive(false);
        selected = false;
        TryEnemySelectServerRpc(false);
    }

    public virtual void Highlight()
    {
        _spriteRenderer.material.color = Color.green;
    }

    public virtual void Unhighlight()
    {
        _spriteRenderer.material.color = Color.white;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryEnemySelectServerRpc(bool selected)
    {
        EnemySelectClientRpc(selected);
    }

    [ClientRpc]
    private void EnemySelectClientRpc(bool selected)
    {
        if(!this.selected)
        {
            enemySelected = selected;
            enemySelectedSprite.gameObject.SetActive(selected);
        }
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        
    }
}

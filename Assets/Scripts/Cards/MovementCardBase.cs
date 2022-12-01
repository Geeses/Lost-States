using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovementCardBase: ScriptableObject, ICardEffect
{
    [Header("Movement Card References")]
    public TMPro.TextMeshProUGUI moveCountText;

    [Header("Movement Card Options")]
    public int baseMoveCount;
    public int id;

    public void ExecuteEffect()
    {
        throw new NotImplementedException();
    }

    public void RevertEffect()
    {
        throw new NotImplementedException();
    }

    public void PlayCard()
    {
        // tell server we want to play this card
        //CardManager.Instance.TryPlayMovementCardServerRpc(id, gameObject.GetInstanceID(), OwnerClientId);
    }

}

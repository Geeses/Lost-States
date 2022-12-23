using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CardEffectManager : NetworkBehaviour
{

    #region Attributes
    private static CardEffectManager s_instance;

    private List<CardEffect> _activeCardEffects = new List<CardEffect>();
    private List<CardEffect> _initializedCardEffects = new List<CardEffect>();
    #endregion

    #region Properties
    public static CardEffectManager Instance { get { return s_instance; } }

    public List<CardEffect> ActiveCardEffects { get => _activeCardEffects; private set => _activeCardEffects = value; }
    public List<CardEffect> InitializedCardEffects { get => _initializedCardEffects; set => _initializedCardEffects = value; }
    #endregion

    #region Monobehavior Functions
    private void Awake()
    {
        // Singleton Pattern
        if (s_instance != null && s_instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            s_instance = this;
        }
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnStart += CountdownTurnCounter;
        TurnManager.Instance.OnTurnEnd += RevertCardEffectClientRpc;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        TurnManager.Instance.OnTurnEnd -= RevertCardEffectClientRpc;
    }
    #endregion

    #region Card Execution

    private void CountdownTurnCounter(ulong currentPlayerId)
    {
        Debug.Log("Countdown Turn Counter: " + InitializedCardEffects.Count);
        foreach (CardEffect effect in InitializedCardEffects)
        {
            Debug.Log("Active Effects: " + effect.name + ". Current PlayerId: " + currentPlayerId);
            if (effect.TimeToExecute > 0 && effect.Player.clientId.Value == currentPlayerId)
            {
                Debug.Log("Set countdown down of " + effect.name);
                effect.TimeToExecute -= 1;
            }
            
            if (effect.TimeToExecute == 0 && effect.Player.clientId.Value == currentPlayerId)
            {
                Debug.Log("Execute delayed effect: " + effect.name);
                ActiveCardEffects.Add(effect);
                effect.ExecuteEffect();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ExecuteInitializedEffectsServerRpc()
    {
        ExecuteInitializedEffectsClientRpc();
    }

    [ClientRpc]
    private void ExecuteInitializedEffectsClientRpc()
    {
        Debug.Log(InitializedCardEffects.Count);
        foreach (CardEffect effect in InitializedCardEffects)
        {
            Debug.Log("Delayed Executing effect: " + effect);
            ActiveCardEffects.Add(effect);
            effect.ExecuteEffect();
        }
    }

    // execute card effects
    [ClientRpc]
    public void InitializeCardEffectClientRpc(int cardId, ulong playerId, CardType cardType, ClientRpcParams clientRpcParams = default)
    {
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];
        Card card = null;

        if (cardType == CardType.Movement)
        {
            card = CardManager.Instance.GetMovementCardById(cardId);
        }
        else if (cardType == CardType.Chest)
        {
            card = CardManager.Instance.GetChestCardById(cardId);
        }

        List<CardEffect> effects = card.cardEffects;

        Debug.Log("executing card effect " + effects.Count + " " + card.cardEffects.Count);
        foreach (CardEffect effect in effects)
        {
            Debug.Log(effect.TimeToExecute);
            InitializedCardEffects.Add(effect);
            effect.Initialize(player);

            Debug.Log("Effect: " + effect.name + " is gonna execute in " + effect.TimeToExecute + " turns. ");

            if (effect.TimeToExecute <= 0)
            {
                ActiveCardEffects.Add(effect);
                effect.ExecuteEffect();
            }
            else if (effect.executeInstantly)
            {
                ActiveCardEffects.Add(effect);
                effect.ExecuteEffect();
            }
        }
    }

    // end of turn revert of card effects
    [ClientRpc]
    private void RevertCardEffectClientRpc(ulong playerId)
    {
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];

        Debug.Log("call revert effect");

        for (int i = 0; i < InitializedCardEffects.Count; i++)
        {
            if (InitializedCardEffects[i].TimeToExecute == 0)
            {
                Debug.Log("revert initialization effect in loop: " + InitializedCardEffects[i].name);
                InitializedCardEffects.RemoveAt(i);
            }
        }

        for (int i = 0; i < ActiveCardEffects.Count; i++)
        {
            if(ActiveCardEffects[i].TimeToExecute == 0)
            {
                Debug.Log("revert effect in loop: " + ActiveCardEffects[i].name);
                ActiveCardEffects[i].RevertEffect();
                ActiveCardEffects.RemoveAt(i);
            }
        }
    }
    #endregion
}


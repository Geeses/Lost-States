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
        TurnManager.Instance.OnTurnEnd += RevertCardEffectClientRpc;
    }
    #endregion

    #region Card Execution

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
    public void InitializeCardEffectClientRpc(int cardId, ulong playerId, CardType cardType)
    {
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];
        Card card = null;

        if (cardType == CardType.Movement)
        {
            card = CardManager.Instance.GetMovementCardById(cardId);
            // increment move card played counter
            player.ChangePlayedMoveCardsClientRpc(1);
            player.ChangeMoveCountClientRpc(card.baseMoveCount);
        }
        else if (cardType == CardType.Chest)
        {
            card = CardManager.Instance.GetChestCardById(cardId);
        }

        List<CardEffect> effects = card.cardEffects;

        Debug.Log("executing card effect " + effects.Count + " " + card.cardEffects.Count);
        foreach (CardEffect effect in effects)
        {
            InitializedCardEffects.Add(effect);
            effect.Initialize(player);

            if (effect.executeInstantly)
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
        foreach (CardEffect effect in ActiveCardEffects)
        {
            Debug.Log("revert effect in loop: " + effect.name);
            effect.RevertEffect();
        }

        ActiveCardEffects.Clear();
        InitializedCardEffects.Clear();
    }
    #endregion
}


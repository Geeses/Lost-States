using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.Netcode;
using UnityEngine;

public class CardManager : NetworkBehaviour
{
    [Header("Movement Card References")]
    public List<MovementCardBase> movementCards = new List<MovementCardBase>();

    [Header("Options")]
    public const int MoveCardListInStack = 3;

    private List<int> _movementCardStack = new List<int>();
    private int _movementCardStackPosition;

    private static CardManager s_instance;

    public static CardManager Instance { get { return s_instance; } }

    public List<int> MovementCardStack { get => _movementCardStack; set => _movementCardStack = value; }
    public int MovementCardStackPosition { get => _movementCardStackPosition; set => _movementCardStackPosition = value; }

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

        CreateMovementCardStack();
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnStart += AddMovementCardsToPlayer;
    }

    private void CreateMovementCardStack()
    {
        for (int i = 0; i < MoveCardListInStack; i++)
        {
            foreach (MovementCardBase card in movementCards)
            {
                MovementCardStack.Add(card.id);
            }
        }

        MovementCardStack.Shuffle();
    }

    private void AddMovementCardsToPlayer(ulong playerId)
    {
        int addCardAmount = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>().MovementCardAmountPerCycle;

        for (int i = 0; i < addCardAmount; i++)
        {


            MovementCardStackPosition += 1;
        }
    }
}

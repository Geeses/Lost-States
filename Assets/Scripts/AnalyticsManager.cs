using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using System.IO;
using Unity.Netcode;
public class AnalyticsManager: MonoBehaviour
{
    string filename = "";
    public DataOnTurnEnd dataOnTurnEnd = new DataOnTurnEnd();
    public DataOnCardPlayed dataCardPlayed = new DataOnCardPlayed();
    void Start()
    {
        filename = Application.dataPath + "/test.csv";
        WriteHeadings();

        TurnManager.Instance.OnTurnEnd += SendOnTurnEndData;
        CardManager.Instance.OnChestCardPlayed += GetChestCard;
        CardManager.Instance.OnMovementCardPlayed += GetMovementCard;
    }

    private void SendOnTurnEndData(ulong playerId)
    {
        Debug.Log("SavingData");
        Player player = PlayerNetworkManager.Instance.PlayerDictionary[playerId];
        var inventoryRessources = player.GetBagRessourcesIndividually(RessourceLocation.inventory);
        var savedRessources = player.GetBagRessourcesIndividually(RessourceLocation.safe);

        dataOnTurnEnd.playerId = playerId.ToString();
        dataOnTurnEnd.movedInTurn = player.movedInCurrentTurn.Value;
        dataOnTurnEnd.inventoryFoodCount = inventoryRessources.Item1;
        dataOnTurnEnd.inventoryWaterCount = inventoryRessources.Item2;
        dataOnTurnEnd.inventorySteelCount = inventoryRessources.Item3;
        dataOnTurnEnd.inventoryWoodCount = inventoryRessources.Item4;
        dataOnTurnEnd.savedFoodCount = savedRessources.Item1;
        dataOnTurnEnd.savedWaterCount = savedRessources.Item2;
        dataOnTurnEnd.savedSteelCount = savedRessources.Item3;
        dataOnTurnEnd.savedWoodCount = savedRessources.Item4;
        dataOnTurnEnd.totalTurnCount = TurnManager.Instance.TotalTurnCount;
        dataOnTurnEnd.turnNumber = TurnManager.Instance.CurrentTurnNumber;

        QueueOnTurnEndDataForAnalyticsServices();
        WriteCSVLine();
    }

    private void GetChestCard(int cardId, ulong playerId)
    {
        dataCardPlayed.TurnNumber = TurnManager.Instance.CurrentTurnNumber;
        dataCardPlayed.TotalTurnCount = TurnManager.Instance.TotalTurnCount;
        dataCardPlayed.PlayerId = (int)playerId;
        dataCardPlayed.CardId = cardId;
        dataCardPlayed.CardType = (int)CardType.Chest;
        QueueOnCardPlayedDataForAnalyticsServices();
    }

    private void GetMovementCard(int cardId, ulong playerId)
    {
        dataCardPlayed.TurnNumber = TurnManager.Instance.CurrentTurnNumber;
        dataCardPlayed.TotalTurnCount = TurnManager.Instance.TotalTurnCount;
        dataCardPlayed.PlayerId = (int)playerId;
        dataCardPlayed.CardId = cardId;
        dataCardPlayed.CardType = (int)CardType.Movement;
        QueueOnCardPlayedDataForAnalyticsServices();
    }

    public void WriteCSVLine()
    {
        var tw = new StreamWriter(filename, true);
        tw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}",
            dataOnTurnEnd.playerId, 
            dataOnTurnEnd.movedInTurn, 
            dataOnTurnEnd.inventoryWaterCount,
            dataOnTurnEnd.inventoryFoodCount, 
            dataOnTurnEnd.inventoryWoodCount,
            dataOnTurnEnd.inventorySteelCount, 
            dataOnTurnEnd.savedWaterCount,
            dataOnTurnEnd.savedFoodCount, 
            dataOnTurnEnd.savedWoodCount,
            dataOnTurnEnd.savedSteelCount, 
            dataOnTurnEnd.totalTurnCount,
            dataOnTurnEnd.turnNumber 
            );
        tw.Close();
    }

    void WriteHeadings()
    {
        var tw = new StreamWriter(filename, false);
        tw.WriteLine("playerId, movedInTurn, inventoryWaterCount, inventoryFoodCount, inventoryWoodCount, inventorySteelCount, savedWaterCount, savedFoodCount, savedWoodCount, savedSteelCount, totalTurnCount, turnNumber", false);
        tw.Close(); 
    }

    void QueueOnTurnEndDataForAnalyticsServices()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "PlayerId", dataOnTurnEnd.playerId },
            { "MovedInTurn", dataOnTurnEnd.movedInTurn },
            { "InventoryWaterCount", dataOnTurnEnd.inventoryWaterCount },
            { "InventoryFoodCount", dataOnTurnEnd.inventoryFoodCount },
            { "InventoryWoodCount", dataOnTurnEnd.inventoryWoodCount },
            { "InventorySteelCount", dataOnTurnEnd.inventorySteelCount },
            { "SavedWaterCount", dataOnTurnEnd.savedWaterCount },
            { "SavedFoodCount", dataOnTurnEnd.savedFoodCount },
            { "SavedWoodCount", dataOnTurnEnd.savedWoodCount },
            { "SavedSteelCount", dataOnTurnEnd.savedSteelCount },
            { "TotalTurnCount", dataOnTurnEnd.totalTurnCount },
            { "TurnNumber", dataOnTurnEnd.turnNumber }
        };

        // The ‘OnTurnEnd’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnTurnEnd", parameters);
    }

    void QueueOnCardPlayedDataForAnalyticsServices()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "PlayerId", dataCardPlayed.PlayerId },
            { "CardId", dataCardPlayed.CardId },
            { "CardType", dataCardPlayed.CardType },
            { "TotalTurnCount", dataOnTurnEnd.totalTurnCount },
            { "TurnNumber", dataOnTurnEnd.turnNumber }
        };

        // The ‘OnTurnEnd’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnCardPlayed", parameters);
    }

    public struct DataOnTurnEnd
    {
        public string playerId;
        public int movedInTurn;
        public int inventoryWaterCount;
        public int inventoryFoodCount;
        public int inventoryWoodCount;
        public int inventorySteelCount;
        public int savedWaterCount;
        public int savedFoodCount;
        public int savedWoodCount;
        public int savedSteelCount;
        public int totalTurnCount;
        public int turnNumber;
    }

    public struct DataOnCardPlayed
    {
        public int PlayerId;
        public int CardType;
        public int CardId;
        public int TotalTurnCount;
        public int TurnNumber;
    }
}



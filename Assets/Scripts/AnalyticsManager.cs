using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using System.IO;
public class AnalyticsManager : Singleton<AnalyticsManager>
{
    string filename = "";
    public DataOnTurnEnd dataOnTurnEnd = new DataOnTurnEnd();
    void Start()
    {
        filename = Application.dataPath + "/test.csv";
        WriteHeadings();
        

        TurnManager.Instance.OnTurnEnd += SendData;
        CardManager.Instance.OnChestCardPlayed += GetChestCard;
        CardManager.Instance.OnMovementCardPlayed += GetMovementCard;

    }

    private void SendData(ulong playerId)
    {
        Debug.Log("SavingData");
        var player = TurnManager.Instance.CurrentTurnPlayer;
        var inventoryRessources = player.GetBagRessourcesIndividually(RessourceLocation.inventory);
        var savedRessources = player.GetBagRessourcesIndividually(RessourceLocation.safe);

        dataOnTurnEnd.playerId = playerId;
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

    private void GetChestCard(int cardId)
    {
        dataOnTurnEnd.chestCardPlayed.Add(cardId);
    }

    private void GetMovementCard(int cardId)
    {
        dataOnTurnEnd.movementCardId = cardId;
    }

    public void WriteCSVLine()
    {
        var tw = new StreamWriter(filename, true);
        tw.WriteLine("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}",
            dataOnTurnEnd.playerId, 
            dataOnTurnEnd.movementCardId, 
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
            dataOnTurnEnd.turnNumber, 
            dataOnTurnEnd.chestCardPlayed
            );
        tw.Close();
    }

    void WriteHeadings()
    {
        var tw = new StreamWriter(filename, false);
        tw.WriteLine("timestamp, playerId, movementCardId, movedInTurn, inventoryWaterCount, inventoryFoodCount, inventoryWoodCount, inventorySteelCount, savedWaterCount, savedFoodCount, savedWoodCount, savedSteelCount, totalTurnCount, turnNumber, chestCardPlayed", false);
        tw.Close(); 
    }

    void QueueOnTurnEndDataForAnalyticsServices()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "playerId", dataOnTurnEnd.playerId },
            { "movementCardId", dataOnTurnEnd.movementCardId },
            { "movedInTurn", dataOnTurnEnd.movedInTurn },
            { "inventoryWaterCount", dataOnTurnEnd.inventoryWaterCount },
            { "inventoryFoodCount", dataOnTurnEnd.inventoryFoodCount },
            { "inventoryWoodCount", dataOnTurnEnd.inventoryWoodCount },
            { "inventorySteelCount", dataOnTurnEnd.inventorySteelCount },
            { "savedWaterCount", dataOnTurnEnd.savedWaterCount },
            { "savedFoodCount", dataOnTurnEnd.savedFoodCount },
            { "savedWoodCount", dataOnTurnEnd.savedWoodCount },
            { "savedSteelCount", dataOnTurnEnd.savedSteelCount },
            { "totalTurnCount", dataOnTurnEnd.totalTurnCount },
            { "turnNumber", dataOnTurnEnd.turnNumber },
//            { "chestCardPlayed", dataOnTurnEnd.chestCardPlayed },
        };

        // The ‘OnTurnEnd’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnTurnEnd", parameters);
    }

    public struct DataOnTurnEnd
    {
        public ulong playerId;
        public int movementCardId;
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
        public List<int> chestCardPlayed;
    }
}



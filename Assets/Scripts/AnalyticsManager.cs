using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;
using System.IO;
public class AnalyticsManager: MonoBehaviour
{
    string filename = "";
    DataOnTurnEnd dataOnTurnEnd = new DataOnTurnEnd();
    void Start()
    {
        filename = Application.dataPath + "/test.csv";
        WriteHeadings();
        CreateOnTurnEndDataForAnalyticsServices();
    }

    public void WriteCSVLine()
    {
        var tw = new StreamWriter(filename, true);
        tw.WriteLine(@"{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7},
                    {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}",
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
            dataOnTurnEnd.chestCardPlayed,
            dataOnTurnEnd.isNight
            );
        tw.Close();
    }

    void WriteHeadings()
    {
        var tw = new StreamWriter(filename, false);
        tw.WriteLine(@"timestamp, playerId, movementCardId,
            movedInTurn, inventoryWaterCount, inventoryFoodCount,
            inventoryWoodCount, inventorySteelCount, savedWaterCount,
            savedFoodCount, savedWoodCount, savedSteelCount, 
            totalTurnCount, turnNumber, chestCardPlayed, isNight", false);
        tw.Close(); 
    }

    void CreateOnTurnEndDataForAnalyticsServices()
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
            { "chestCardPlayed", dataOnTurnEnd.chestCardPlayed },
            { "isNight", dataOnTurnEnd.isNight },
        };

        // The ‘OnTurnEnd’ event will get queued up and sent every minute
        AnalyticsService.Instance.CustomData("OnTurnEnd", parameters);
    }

    struct DataOnTurnEnd
    {
        public int playerId;
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
        public bool isNight;
    }
}



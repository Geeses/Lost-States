using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class BattleLogListController {

    VisualTreeAsset _listEntryTemplate;
    ListView battlelogList;
    Label log;
    private List<string> _logList;
    private LogData _logger;

    public void InitializeList(VisualElement root, VisualTreeAsset listElementTemplate) {
        _listEntryTemplate = listElementTemplate;

        _logger = LogData.shared;
        _logList = _logger.logList;

        battlelogList = root.Q<ListView>("battlelog-list");
        log = root.Q<Label>("battlelog-cell");
        
        battlelogList.makeItem = () => {
            var newListEntry = _listEntryTemplate.Instantiate();
            var newListEntryLogic = new BattlelogCellController();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };

        battlelogList.bindItem = (item, index) => {
            var reverseList = Enumerable.Reverse(_logList).ToList();
            (item.userData as BattlelogCellController).SetLogData(reverseList[index]);
        };

        battlelogList.fixedItemHeight = 45; 
        battlelogList.itemsSource = _logList;
    }   

    public void RefreshList() {
        battlelogList.Rebuild();
    }
}
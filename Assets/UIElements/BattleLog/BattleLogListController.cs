using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class BattleLogListController {

    VisualTreeAsset m_ListEntryTemplate;
    ListView m_BattlelogList;
    Label m_Log;
    private List<string> logList;
    LogData log;

    public void InitializeList(VisualElement root, VisualTreeAsset listElementTemplate) {
        m_ListEntryTemplate = listElementTemplate;

        log = LogData.shared;
        logList = log.logList;

        m_BattlelogList = root.Q<ListView>("battlelog-list");
        m_Log = root.Q<Label>("battlelog-cell");
        
        m_BattlelogList.makeItem = () => {
            var newListEntry = m_ListEntryTemplate.Instantiate();
            var newListEntryLogic = new BattlelogCellController();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };

        m_BattlelogList.bindItem = (item, index) => {
            var reverseList = Enumerable.Reverse(logList).ToList();
            (item.userData as BattlelogCellController).SetLogData(reverseList[index]);
        };

        m_BattlelogList.fixedItemHeight = 45; 
        m_BattlelogList.itemsSource = logList;
    }   

    public void RefreshList() {
        m_BattlelogList.Rebuild();
    }
}
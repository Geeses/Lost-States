using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public sealed class LogData {
    public List<string> logList = new List<string>();

    // Lock Object for thread safety
    private static readonly object Instancelock = new object();
    private static LogData instance = null;
    

    //Dependency to my ListView
    private BattleLogListController _battlelogList;

    // Init
    private LogData() { }

    public static LogData shared {
        get {
                if (instance == null) {
                    lock (Instancelock) {
                        if (instance == null) {
                            instance = new LogData();
                            Debug.Log("Instance of LogData created");
                        }
                    }
                }
            return instance;
        }
    }

    public void setBattleLogListDependency(BattleLogListController battlelogList) {
        _battlelogList = battlelogList;
        Debug.Log("Dependency of BattleLogList created");
    }

    public void AddLog(string log) {
        logList.Add(log);
        _battlelogList.RefreshList();
    }
}

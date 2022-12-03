using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public sealed class LogData {
    public List<string> logList = new List<string>();

    // Lock Object for thread safety
    private static readonly object instanceLock = new object();
    private static LogData instance = null;
    

    //Dependency to my ListView
    private BattleLogListController _battlelogList;

    // Init
    private LogData() { }

    public static LogData shared {
        get {
                if (instance == null) {
                    lock (instanceLock) {
                        if (instance == null) {
                            instance = new LogData();
                        }
                    }
                }
            return instance;
        }
    }

    public void SetBattleLogListDependency(BattleLogListController battlelogList) {
        _battlelogList = battlelogList;
    }

    public void AddLog(string log) {
        logList.Add(log);
        _battlelogList.RefreshList();
    }
}

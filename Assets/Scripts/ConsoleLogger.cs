using System.Diagnostics;
using UnityEngine;

public class ConsoleLogger : MonoBehaviour
{
    // Lock Object for thread safety
    private static readonly object instanceLock = new object();
    private static TestManager instance = null;

    // Init
    private ConsoleLogger() { }
    void AddLog(string Log)
    {
        string caller = (new StackTrace()).GetFrame(1).GetMethod().Name.ToString();
    }
}

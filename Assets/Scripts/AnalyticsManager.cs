using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsManager: MonoBehaviour
{
    void Start()
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "custom event", "hello there" },
            { "second", "nice" }
        };
        AnalyticsService.Instance.CustomData("myEvent", parameters);
        Debug.Log("Analytics will be flushed");
        AnalyticsService.Instance.Flush();
    }
}

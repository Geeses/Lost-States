using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Battlelog : NetworkBehaviour
{
    [SerializeField]
    private Transform scrollViewContent;

    [SerializeField]
    private GameObject prefab;

    public List<string> logs = new List<string>();

    private static readonly object instanceLock = new object();
    private static Battlelog instance = null;
    private Battlelog() { }

    public static Battlelog Instance
    {
        get
        {
            if (instance == null)
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new Battlelog();
                    }
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void AddLog(string content)
    {
        logs.Add(content);
        GameObject newLog = Instantiate(prefab, scrollViewContent);
        if (newLog.TryGetComponent<TMPro.TextMeshProUGUI>(out TMPro.TextMeshProUGUI item))
        {
            item.text = content;
        }
    }

    [ClientRpc]
    public void AddLogClientRpc(string content)
    {
        AddLog(content);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SpriteSelecter : NetworkBehaviour
{
#region Attributes

#endregion

#region Properties

#endregion

#region Monobehavior Functions

    void Start()
    {
        StartCoroutine(SelectPlayerSprite());
    }

    #endregion

    private IEnumerator SelectPlayerSprite()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);
        yield return new WaitUntil(() => GameManager.Instance.gameHasStarted);

        if (OwnerClientId == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}


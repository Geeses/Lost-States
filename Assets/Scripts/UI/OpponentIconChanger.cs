using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OpponentIconChanger : NetworkBehaviour
{
    #region Attributes
    public OpponentsUI opponentUi;
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

        if (opponentUi.Player.clientId.Value == 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }
    }
}


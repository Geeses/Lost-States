using Unity.Netcode;
using UnityEngine;
using System.Collections;

public class PlayerControler : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            return;
        }

        transform.position = new Vector3(1, 0, 0);
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

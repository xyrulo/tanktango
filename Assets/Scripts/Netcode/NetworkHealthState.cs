using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkHealthState : NetworkBehaviour
{
    public NetworkVariable<bool> isAlive = new NetworkVariable<bool>();
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        isAlive.Value = true;
    }
}

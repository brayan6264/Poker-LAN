using Mirror;
using UnityEngine;

public class NetPlayer : NetworkBehaviour
{
    [SyncVar] public string nickname = "Player";

    public override void OnStartLocalPlayer()
    {
        Debug.Log("[LOCAL] NetPlayer listo.");
    }
}

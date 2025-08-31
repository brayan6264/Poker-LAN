using Mirror;
using UnityEngine;

public class NetTable : NetworkBehaviour
{
    [SyncVar] public int pot;         // demo: valor sincronizado
    [SyncVar] public int currentSeat; // demo: asiento actual

    public override void OnStartServer()
    {
        base.OnStartServer();
        pot = 0;
        currentSeat = 0;
        Debug.Log("[SERVER] NetTable creada (OnStartServer).");
    }

    [Command(requiresAuthority = false)]
    public void CmdAddToPot(int amount)
    {
        pot += Mathf.Max(0, amount);
    }
}

using Mirror;
using UnityEngine;

public class NetworkManagerPoker : NetworkManager
{
    [Header("Poker Prefabs")]
    public GameObject netTablePrefab;

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (netTablePrefab != null)
        {
            var table = Instantiate(netTablePrefab);
            NetworkServer.Spawn(table);
            Debug.Log("[SERVER] NetTable spawneada por NetworkManagerPoker.");
        }
        else
        {
            Debug.LogWarning("[SERVER] netTablePrefab NO asignado.");
        }
    }
}


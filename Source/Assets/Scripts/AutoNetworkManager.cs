using UnityEngine;
using UnityEngine.Networking;

public class AutoNetworkManager : NetworkManager
{
    public void JoinHost(string ipAddress)
    {
        networkAddress = ipAddress;
        StartClient();
    }

    public void CreateHost()
    {
        StopHost();
        StartHost();
    }
}

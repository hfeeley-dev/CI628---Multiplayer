using UnityEngine;
using Unity.Netcode;

public class LobbyManager : NetworkBehaviour

{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 4;
    private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);

    public bool canStart()
    {
        if (!IsServer)
            return false;
        int PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        return PlayerCount >= minPlayers &&  PlayerCount <= maxPlayers;
    }
    public void StartGame ()
    {
        if (!IsServer)
            return;
        if (!canStart())
            return;
        if (gameStarted.Value)
            return;
        gameStarted.Value = true;
    }
}

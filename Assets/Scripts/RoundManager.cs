using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private int RoundsToWin = 3;
    private NetworkList<int> PlayerWins;
    private NetworkList<ulong> PlayerInfo;

    private void Awake()
    {
        PlayerWins = new NetworkList<int>();
        PlayerInfo = new NetworkList<ulong>();

    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
            return;

        foreach (ulong PlayerID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            PlayerInfo.Add(PlayerID);
            PlayerWins.Add(0);
        }
    }
}



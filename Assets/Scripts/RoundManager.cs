using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private int RoundsToWin = 3;
    private bool GameWon = false;
    private NetworkList<int> PlayerWins;
    private NetworkList<ulong> PlayerInfo;

    private void Awake()
    {
        PlayerWins = new NetworkList<int>();
        PlayerInfo = new NetworkList<ulong>();

    }

    public override void OnNetworkSpawn() // Stores all the new players ID's
    {
        if (!IsServer)
            return;

        foreach (ulong PlayerID in NetworkManager.Singleton.ConnectedClientsIds)
        {
            PlayerInfo.Add(PlayerID);
            PlayerWins.Add(0);
        }
        NetworkManager.Singleton.OnClientConnectedCallback += PlayerConnected;
    }

    private void PlayerConnected(ulong PlayerID) // Sets new join players round wins to 0
    {
        if (!IsServer)
            return;

        if (PlayerInfo.Contains(PlayerID))
            return;

        PlayerInfo.Add(PlayerID);
        PlayerWins.Add(0);
    }

    public void RoundWon(ulong PlayerID)
    {
        if (!IsServer || GameWon)
            return;

        for (int i = 0; i < PlayerInfo.Count; i++)
        {
            if (PlayerInfo[i] == PlayerID)
            {
                PlayerWins[i]++;
                if (PlayerWins[i] >= RoundsToWin)
                {
                    GameWon = true;
                }
                return;
            }
        }
    }
}



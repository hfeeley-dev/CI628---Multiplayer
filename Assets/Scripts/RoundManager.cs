using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;

public class RoundManager : NetworkBehaviour
{
    [SerializeField] private int RoundsToWin = 3;
    private NetworkVariable<bool> GameWon = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> RoundActive = new NetworkVariable<bool>(false);
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

    public void RoundWon(ulong PlayerID) // Updates 
    {
        if (!IsServer || GameWon.Value)
            return;

        for (int i = 0; i < PlayerInfo.Count; i++)
        {
            if (PlayerInfo[i] == PlayerID)
            {
                PlayerWins[i]++;
                if (PlayerWins[i] >= RoundsToWin)
                {
                    GameWon.Value = true;
                }
                return;
            }
        }
    }

    public bool IsGameWon() // Checks if the game hsa been won
    {
        return GameWon.Value;
    }

    public void StartRound()
    {
        if (!IsServer || GameWon.Value || RoundActive.Value)
            return;

        RoundActive.Value = true;
    }
}



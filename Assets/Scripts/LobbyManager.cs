using UnityEngine;
using Unity.Netcode;

public class LobbyManager : NetworkBehaviour

{
    [SerializeField] private GameObject startGameButton;
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private int maxPlayers = 4;
    [SerializeField] private GameObject playerDisconnect;

    private NetworkVariable<bool> gameStarted = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> gamePaused = new NetworkVariable<bool>(false);

    public bool IsGamePaused()
    {
        return gamePaused.Value;
    }
    public bool canStart() //Only allows a game to start if there are two or more players
    {
        if (!IsServer)
            return false;
        int PlayerCount = NetworkManager.Singleton.ConnectedClients.Count;
        return PlayerCount >= minPlayers &&  PlayerCount <= maxPlayers;
    }
    public void StartGame () //when the start game button is pressed it ensures that the interaction is owned by the host otherwise it does not execute
    {
        if (!IsServer)
            return;
        if (!canStart())
            return;
        if (gameStarted.Value)
            return;
        gameStarted.Value = true;

        HideStartClientRpc();
    }

    [ClientRpc] //Forces all clients to have the start game button disappear
    private void HideStartClientRpc()
    {
        startGameButton.SetActive(false);
    }

    private void CanJoin( //Makes a players join request have to meet the requirements before letting them join. In this case if there a >4 players they can join 
    NetworkManager.ConnectionApprovalRequest request,
    NetworkManager.ConnectionApprovalResponse PlayerJoin)
    {
        bool lobbyFull = NetworkManager.Singleton.ConnectedClients.Count >= maxPlayers;

        if (gameStarted.Value || lobbyFull)
        {
            PlayerJoin.Approved = false;
        }
        else
        {
            PlayerJoin.Approved = true;
            PlayerJoin.CreatePlayerObject = true;
        }
    }

    public override void OnNetworkSpawn() //Allows the host machine to be able to run functions such as allowing a player to join a lobby or the player disconnect function
    {
        if (IsServer)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback += CanJoin;
            NetworkManager.Singleton.OnClientDisconnectCallback += PlayerDisconnected;
        }
    }

    private void PlayerDisconnected(ulong PlayerID) //Detects players that have disconnected from a game session
    {
        if (!IsServer)
            return;
        playerDisconnect.SetActive(true);

        if(gameStarted.Value)
        {
            gamePaused.Value = true;
            ShowDisconnectClientRpc();
        }
    }

    [ClientRpc] //Forces all clients to say that a player has disconnected
    private void ShowDisconnectClientRpc()
    {
        playerDisconnect.SetActive(true);
    }
}

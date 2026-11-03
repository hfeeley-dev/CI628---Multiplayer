using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float jumpForce = 5f;
    private bool jumpPressed;

    private Vector2 moveInput;
    private Rigidbody Player;

    private void Awake()
    {
        Player = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        moveAction.action.Disable();
        moveInput = Vector2.zero;
        jumpAction.action.Disable();
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner) return;

        moveInput = moveAction.action.ReadValue<Vector2>();
        if (jumpAction.action.WasPressedThisFrame())
        {
            jumpPressed = true;
            Debug.Log("Jump was pressed");
        }
    }

    private void FixedUpdate()
    {
        if (!IsSpawned || !IsOwner) return;

        Vector3 movement = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        if (jumpPressed)
        {
            Player.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;                                           
        }

        LobbyManager GameLobby = FindFirstObjectByType<LobbyManager>();
        if (GameLobby != null && GameLobby.IsGamePaused()) return;

        Player.MovePosition(
            Player.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }
}

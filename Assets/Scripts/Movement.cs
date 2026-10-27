using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private float moveSpeed = 3f;

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
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;

        moveAction.action.Disable();
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (!IsSpawned || !IsOwner) return;

        moveInput = moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (!IsSpawned || !IsOwner) return;

        Vector3 movement = new Vector3(
            moveInput.x,
            0f,
            moveInput.y
        );

        Player.MovePosition(
            Player.position + movement * moveSpeed * Time.fixedDeltaTime
        );
    }
}

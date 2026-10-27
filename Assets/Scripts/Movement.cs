using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Unity.Netcode;

public class PlayerNetwork : NetworkBehaviour
{
    private void Update()
    {
        if (!IsOwner) return;

        Vector3 MoveDirection = new Vector3(0,0,0);

        if (Input.GetKey(KeyCode.W)) MoveDirection.z = +1f;
        if (Input.GetKey(KeyCode.S)) MoveDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) MoveDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) MoveDirection.x = +1f;

        float MoveSpeed = 3f;
        transform.position += MoveDirection * MoveSpeed * Time.deltaTime;
    }
}

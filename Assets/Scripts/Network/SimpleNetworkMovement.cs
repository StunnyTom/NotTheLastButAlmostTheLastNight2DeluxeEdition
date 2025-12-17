using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;

[RequireComponent(typeof(NetworkTransform))]
public class SimpleNetworkMovement : NetworkBehaviour
{
    public float speed = 5f;

    public override void OnNetworkSpawn()
    {
        // Side-by-Side Spawning (Deterministic)
        // This keeps them near the original spawn point (Boat) but limits stacking
        if (IsOwner)
        {
            // Offset by 1 meter per player ID to the right
            transform.position += Vector3.right * (OwnerClientId * 1.5f);
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v) * speed * Time.deltaTime;
        transform.Translate(move);
    }
}

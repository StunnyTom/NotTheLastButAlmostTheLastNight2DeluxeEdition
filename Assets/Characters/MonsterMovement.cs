using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float rotateSpeed = 100f;

    void Update()
    {
        // Use WASD or Arrow keys to move (for testing)
        float horizontal = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float vertical = Input.GetAxis("Vertical");     // W/S or Up/Down

        // Move forward/backward
        transform.Translate(Vector3.forward * vertical * moveSpeed * Time.deltaTime);

        // Rotate left/right
        transform.Rotate(Vector3.up * horizontal * rotateSpeed * Time.deltaTime);
    }
}

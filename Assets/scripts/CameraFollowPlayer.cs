using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player's Transform
    public float fixedY = 9f; // Set your desired fixed Y position
    public float fixedZ = -50f; // Set your desired fixed Z position
    public float smoothSpeed = 0.125f; // Smooth transition speed

    private void LateUpdate()
    {
        if (player != null) // Check if player exists
        {
            // Only update X position, keep Y and Z fixed
            Vector3 desiredPosition = new Vector3(player.position.x, fixedY, fixedZ);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}

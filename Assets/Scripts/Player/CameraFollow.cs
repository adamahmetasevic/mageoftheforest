using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Reference to the player
    public Vector3 offset;    // Offset from the player's position (to maintain some distance)
    public float smoothSpeed = 0.125f;  // Smoothing factor for camera movement

    void LateUpdate()
    {
        // Calculate the desired position of the camera based on player's position and offset
        Vector3 desiredPosition = player.position + offset;
        
        // Smooth the camera's movement to make it feel less rigid
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}

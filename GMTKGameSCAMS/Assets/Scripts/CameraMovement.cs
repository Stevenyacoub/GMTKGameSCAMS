using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform player;          // Reference to the player's transform
    public float yTopThreshold = 5.0f;   // The Y point at which the camera should move up
    public float yBottomThreshold = 4.0f;   // The Y point at which the camera should move up
    public float smoothSpeed = 0.125f; // The speed of the smooth camera movement
    public float offsetY = 2.0f;      // The offset to maintain between the player and camera's Y position

    private Vector3 targetPosition;

    void Update()
    {
        if (player.position.y < yTopThreshold && player.position.y > yBottomThreshold)
        {
            targetPosition = new Vector3(transform.position.x, player.position.y + offsetY, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        // Draw a line representing the Y threshold
        Vector3 start = new Vector3(-10, yTopThreshold, 0);
        Vector3 end = new Vector3(10, yTopThreshold, 0);
        UnityEditor.Handles.Label(new Vector3(0, yTopThreshold, 0), "Y Top Threshold: " + yTopThreshold);
        Gizmos.DrawLine(start, end);
        
        Gizmos.color = Color.magenta;
        start = new Vector3(-10, yBottomThreshold, 0);
        end = new Vector3(10, yBottomThreshold, 0);

        UnityEditor.Handles.Label(new Vector3(0, yBottomThreshold, 0), "Y Bottom Threshold: " + yBottomThreshold);
        Gizmos.DrawLine(start, end);
    }
}
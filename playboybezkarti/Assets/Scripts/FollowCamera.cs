using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // The player transform to follow
    public Vector3 offset;          // Offset from the player (usually z = -10 for 2D)
    public float smoothSpeed = 5f;  // How fast the camera follows

    void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly move the camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}

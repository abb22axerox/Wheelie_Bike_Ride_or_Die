using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Camera Settings")]
    public Vector3 offset = new Vector3(0, 5, -10);
    public Vector3 rotation = new Vector3(20, 0, 0);
    public float smoothingSpeed = 5f;

    [Header("Debug Settings")]
    public bool staticCamera = false;

    [Header("References")]
    public Transform target;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target not assigned in CameraFollow script.");
            return;
        }

        // Apply initial rotation to the camera relative to the target
        // and set the initial position
        UpdateCameraPositionAndRotation(true);
    }

    void LateUpdate()
    {
        if (target != null && !staticCamera)
        {
            // Update the camera's position and rotation
            UpdateCameraPositionAndRotation(false);
        }
    }

    void UpdateCameraPositionAndRotation(bool instantUpdate)
    {
        // Calculate desired position and rotation based on the target's position and rotation
        Vector3 desiredPosition = target.position + target.rotation * offset;
        Quaternion desiredRotation = target.rotation * Quaternion.Euler(rotation);

        if (instantUpdate)
        {
            // Immediately set the position and rotation
            transform.position = desiredPosition;
            transform.rotation = desiredRotation;
        }
        else
        {
            // Smoothly interpolate the camera's position and rotation
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * smoothingSpeed);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Time.deltaTime * smoothingSpeed);
        }
    }
}

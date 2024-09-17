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
        }

        // Apply initial rotation to the camera
        transform.eulerAngles = rotation;

        // Move the camera to the exact correct position at the start
        if (target != null) transform.position = target.position + offset;
    }

    void LateUpdate()
    {
        if (target != null && !staticCamera)
        {
            // Smoothly interpolate the camera's position
            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * smoothingSpeed);

            // Maintain the specified rotation
            transform.eulerAngles = rotation;
        }
    }
}

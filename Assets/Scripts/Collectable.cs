using UnityEngine;

public class Collectable : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;    // Rotation speed in degrees per second

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f; // How high the coin moves up and down
    public float bobbingFrequency = 1f;   // How fast the coin moves up and down

    [Header("Movement Settings")]
    public float speed = 10.0f;           // Speed at which the truck moves toward the player
    public float despawnDistance = 20.0f; // Distance behind the player at which the truck will be destroyed
    [Range(-1, 1)] public int despawnDirection = -1; // sign value

    [Header("References")]
    Transform playerTransform;

    private Vector3 startPosition;          // The starting position of the coin

    void Start()
    {
        // Store the starting position
        startPosition = transform.position;
    }

    void Update()
    {
        // Move the truck towards the player
        transform.Translate(0, 0, -speed * Time.deltaTime, Space.World);

        // Destroy the truck when it is behind the player by despawnDistance
        if (playerTransform != null && Mathf.Abs(transform.position.z - playerTransform.position.z) > despawnDistance && Mathf.Sign(transform.position.z - playerTransform.position.z) == despawnDirection)
        {
            Destroy(gameObject);
        }

        // Rotate the coin around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Bobbing motion
        float newY = startPosition.y + Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}

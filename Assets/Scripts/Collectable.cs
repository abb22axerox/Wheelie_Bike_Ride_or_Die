using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3

public class Collectable : MonoBehaviour
{
    [Header("Spline Settings")]
    private SplineContainer splineContainer;     // Reference to the SplineContainer
    public float speed = 5f;                     // Speed at which the object moves along the spline
    public bool loop = false;                    // Whether the object should loop back to the start

    [Header("Offset Settings")]
    public float sideOffset = 0f;                // Side offset in the XZ-plane

    [Header("Rotation Settings")]
    public float rotationSpeed = 100f;           // Rotation speed in degrees per second

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f;        // How high the item moves up and down
    public float bobbingFrequency = 1f;          // How fast the item moves up and down

    [Header("Despawn Settings")]
    public float despawnDistance = 20.0f;        // Distance behind the player at which the object will be destroyed
    private Transform playerTransform;           // Reference to the player's transform

    private Spline spline;
    private float distanceTraveled = 0f;         // Total distance traveled along the spline
    private float splineLength;                  // Total length of the spline

    void Start()
    {
        // Find the player in the scene (assuming it has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            distanceTraveled = player.GetComponent<PlayerController>().distanceTraveled + GameObject.FindGameObjectWithTag("ObstacleSpawner").GetComponent<ObstacleSpawner>().spawnDistance;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }

        // Find the spline container in the scene (assuming it has the "RoadSpline" tag)
        GameObject splineObject = GameObject.FindGameObjectWithTag("RoadSpline");
        if (splineObject != null)
        {
            splineContainer = splineObject.GetComponent<SplineContainer>();
            if (splineContainer != null)
            {
                spline = splineContainer.Spline;
                splineLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
            }
            else
            {
                Debug.LogError("SplineContainer component not found on object with tag 'RoadSpline'.");
            }
        }
        else
        {
            Debug.LogError("Spline object with tag 'RoadSpline' not found!");
        }
    }

    void Update()
    {
        if (spline == null)
            return;

        // Increase the distance traveled based on speed
        distanceTraveled += speed * Time.deltaTime;

        // Handle looping or clamping at the end of the spline
        if (distanceTraveled > splineLength)
        {
            if (loop)
            {
                distanceTraveled %= splineLength; // Loop back to the start
            }
            else
            {
                Destroy(gameObject); // Destroy the object when it reaches the end
                return;
            }
        }

        // Get the position along the spline with side offset
        Vector3 position = SplineUtilityExtension.GetPositionAtDistance(
            distanceTraveled,
            spline,
            splineContainer.transform,
            sideOffset
        );

        // Apply bobbing motion
        float bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        position.y += bobbingOffset;

        // Set the object's position
        transform.position = position;

        // Optionally, set the object's rotation to face along the spline
        // Evaluate the tangent at the current distance
        float t = distanceTraveled / splineLength;
        t = Mathf.Repeat(t, 1f); // Ensure t is between 0 and 1

        SplineUtility.Evaluate(spline, t, out _, out float3 tangent, out _);
        Vector3 worldTangent = splineContainer.transform.TransformDirection((Vector3)tangent);

        if (worldTangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(worldTangent);
        }

        // Rotate the object around its Y-axis
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);

        // Destroy the object when it is beyond despawnDistance from the player
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > despawnDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}

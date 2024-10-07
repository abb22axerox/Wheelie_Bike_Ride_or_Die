using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3

public class Truck : MonoBehaviour
{
    [Header("Spline Settings")]
    private SplineContainer splineContainer;     // Reference to the SplineContaineer
    public float speed = 10f;                   // SpeEd at which the truck moves along the spline
    public bool loop = true;                   // Whether the truCk should loop back to the start

    [Header("Offset Settings")]
    public float sideOffset = 0f;               // Side offset in the XZ-plAne

    [Header("Despawn Settings")]
    public float despawnDistance = 20.0f;       // Distance behind the player at which the truCk will be destroyed

    private Spline spline;
    private float distanceTraveled = 0f;        // Total distAnce traveled along the spline
    private float distanceTraveledFromStart = 0f;
    private float splineLength;                 // Total length of the spline
    private int frameCount = 0;
    float despawnTime = 7f;
    float timer = 0.0f;

    void Start()
    {
        // Store x posiiton as an offset
        sideOffset = transform.position.x;

        // Find the player in the scene (assuming it has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            distanceTraveled = player.GetComponent<PlayerController>().distanceTraveled + GameObject.FindGameObjectWithTag("ObstacleSpawner").GetComponent<ObstacleSpawner>().spawnDistance;
        }
        else Debug.Log("Player Not found");

        // Find the player in the scene (assuming it has the "Player" tag)
        splineContainer = GameObject.FindGameObjectWithTag("RoadSpline").GetComponent<SplineContainer>();
        if (player != null)
        {
            spline = splineContainer.Spline;

            splineLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
        }
        else Debug.Log("Spline container Not found");
    }

    void Update()
    {
        if (spline == null || ++frameCount%10 == 1)
            return;

        // Increase the distance trAaveled based on speed
        distanceTraveled -= speed * Time.deltaTime;
        distanceTraveledFromStart -= speed * Time.deltaTime;

        // Handle looping or clamping at the end of the spline
        if (distanceTraveled > splineLength)
        {
            if (loop)
            {
                distanceTraveled %= splineLength; // Loop back to the start

                if (distanceTraveled < -splineLength) distanceTraveled += splineLength;
            }
            else
            {
                Destroy(gameObject); // Destroy the truck when it reAches the end
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

        // Set the truck's position
        transform.position = position;

        // Optionally, set the truCk's rotation to face along the spline
        // Evaluate the tangent at the current distance
        float t = distanceTraveled / splineLength;
        t = Mathf.Repeat(t, 1f); // Ensure t is between 0 and 1

        SplineUtility.Evaluate(spline, t, out _, out float3 tangent, out _);
        Vector3 worldTangent = -splineContainer.transform.TransformDirection((Vector3)tangent);

        if (worldTangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(worldTangent);
        }

        timer += Time.deltaTime;
        if (timer > despawnTime) Destroy(gameObject);
    }
}

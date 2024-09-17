using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;  // For float3 and float4x4

public class CarFollowSpline : MonoBehaviour
{
    public SplineContainer splineContainer; // Reference to the SplineContainer
    public float speed = 5f;                // Speed of the car
    public bool loop = false;               // Whether the car should loop the spline

    private Spline spline;
    private float distanceTraveled = 0f;    // Track the distance traveled along the spline
    private float splineLength;

    void Start()
    {
        // Get the spline from the container
        spline = splineContainer.Spline;

        // Get the total length of the spline (in world space)
        splineLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
    }

    void Update()
    {
        // Increase the distance traveled based on speed
        distanceTraveled += speed * Time.deltaTime;

        // Optionally loop the car when it reaches the end
        if (distanceTraveled > splineLength)
        {
            if (loop)
            {
                distanceTraveled -= splineLength; // Loop the car
            }
            else
            {
                distanceTraveled = splineLength; // Clamp at the end
                return; // Stop the car
            }
        }

        // Get the normalized time (t) along the spline (0 to 1)
        float t = distanceTraveled / splineLength;

        // Evaluate position, tangent, and upVector at time t (in local space)
        float3 position, tangent, upVector;
        SplineUtility.Evaluate(spline, t, out position, out tangent, out upVector);

        // Convert position and tangent to world space using the spline container's transform
        Vector3 worldPosition = splineContainer.transform.TransformPoint((Vector3)position);
        Vector3 worldTangent = splineContainer.transform.TransformDirection((Vector3)tangent);
        Vector3 worldUpVector = splineContainer.transform.TransformDirection((Vector3)upVector);

        // Set the car's position and rotation based on the spline
        transform.position = worldPosition;

        // Rotate the car to face along the tangent of the spline
        if (worldTangent != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(worldTangent, worldUpVector); // Use upVector for rotation
        }
    }
}

using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public static class SplineUtilityExtension
{
    /// <summary>
    /// Calculates the position along a spline at a specific distance, with an optional side offset in the XZ-plane.
    /// </summary>
    /// <param name="dst">The distance along the spline.</param>
    /// <param name="spline">The spline to traverse.</param>
    /// <param name="splineTransform">The transform of the spline container.</param>
    /// <param name="sideOffset">The side offset in the XZ-plane.</param>
    /// <returns>The position in world space at the specified distance along the spline.</returns>
    public static Vector3 GetPositionAtDistance(float dst, Spline spline, Transform splineTransform, float sideOffset)
    {
        // Calculate the total length of the spline in world space
        float splineLength = SplineUtility.CalculateLength(spline, splineTransform.localToWorldMatrix);

        // Normalize the distance to a parameter t (0 to 1)
        float t = dst / splineLength;

        // Optionally handle looping by wrapping t
        t = Mathf.Repeat(t, 1f);

        // Evaluate the position, tangent, and upVector at parameter t (in local space)
        SplineUtility.Evaluate(spline, t, out float3 position, out float3 tangent, out float3 upVector);

        // Convert position, tangent, and upVector to world space
        Vector3 worldPosition = splineTransform.TransformPoint((Vector3)position);
        Vector3 worldTangent = splineTransform.TransformDirection((Vector3)tangent);
        Vector3 worldUpVector = splineTransform.TransformDirection((Vector3)upVector);

        // Calculate the side offset vector (perpendicular to the tangent in the XZ-plane)
        Vector3 projectedTangent = new Vector3(worldTangent.x, 0f, worldTangent.z).normalized;
        Vector3 sideVector = Vector3.Cross(Vector3.up, projectedTangent).normalized;

        // Apply the side offset
        Vector3 offsetPosition = worldPosition + sideVector * sideOffset;

        return offsetPosition;
    }
}

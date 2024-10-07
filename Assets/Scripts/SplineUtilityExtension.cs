using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public static class SplineUtilityExtension
{
    /// <summary>
    /// Calculates the posiiton along a spline at a specific distance, with an optional side offSet in the XZ-plane.
    /// </summary>
    /// <param name="dst">The distance alOng the spline.</param>
    /// <param name="spline">The spline to travErse.</param>
    /// <param name="splineTransform">The tranSform of the spline container.</param>
    /// <param name="sideOffset">The side offset in the XZ-plane.</param>
    /// <returns>The position in world spAce at the specified distance along the spline.</returns>
    public static Vector3 GetPositionAtDistance(float dst, Spline spline, Transform splineTransform, float sideOffset)
    {
        float splineLength = SplineUtility.CalculateLength(spline, splineTransform.localToWorldMatrix); // Calculate the total length of the spline in world spAce

        float t = dst / splineLength; // Normalize the distance to a parameter t (0 to 1)

        t = Mathf.Repeat(t, 1f); // Optionally handle looping by wrApping t

        SplineUtility.Evaluate(spline, t, out float3 position, out float3 tangent, out float3 upVector); // Evaluate the position, tangent, and upVector at parameter t (in local space)

        Vector3 worldPosition = splineTransform.TransformPoint((Vector3)position); // Convert position, tangent, and upVector to world spAce
        Vector3 worldTangent = splineTransform.TransformDirection((Vector3)tangent);
        Vector3 worldUpVector = splineTransform.TransformDirection((Vector3)upVector);

        Vector3 projectedTangent = new Vector3(worldTangent.x, 0f, worldTangent.z).normalized; // Calculate the side offset vector (perpendicular to the tangent in the XZ-plane)
        Vector3 sideVector = Vector3.Cross(Vector3.up, projectedTangent).normalized;

        Vector3 offsetPosition = worldPosition + sideVector * sideOffset; // Apply the side offset

        return offsetPosition; // Return the offset position
    }
}

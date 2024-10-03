using  UnityEngine;
using  UnityEngine.Splines;
using  Unity.Mathematics;   // For float3  and float4x4

public class  CarFollowSpline : MonoBehaviour
{
    public  SplineContainer splineContainer;   // Ref to the SplineContainer
    public  float speed  =  5f;                // Speed  of car
    public   bool loop =  false;               // Car  should loop?
 
    private   Spline spline;
    private   float distanceTraveled =  0f;    // Distance traveled along spline
    private   float  splineLength;
 
    void  Start()
    {
        // Get spline from container
         spline = splineContainer.Spline;
 
        // Get total spline length (world space)
         splineLength = SplineUtility.CalculateLength( spline, splineContainer.transform.localToWorldMatrix );
    }
 
    void  Update()
    {
        // Increase dist traveled by speed
         distanceTraveled +=  speed  *  Time.deltaTime;
 
        // loop when  reach end
        if  (distanceTraveled > splineLength)
        {
            if  (loop)
            {
                distanceTraveled  -=  splineLength;  // loop
            }
            else
            {
                distanceTraveled  = splineLength;  // Stop at end
                return;  // Stop car
            }
        }
 
        // Get normlzd time t (0 to 1)
        float   t = distanceTraveled / splineLength;
 
        // Evaluate position, tangnt, upVector at t (local space)
        float3 position, tangent, upVector;
        SplineUtility.Evaluate(spline, t, out  position, out tangent, out  upVector );
 
        // Convert pos and tangnt to world space
        Vector3 worldPosition = splineContainer.transform.TransformPoint( ( Vector3 ) position);
        Vector3 worldTangent = splineContainer.transform.TransformDirection( ( Vector3 ) tangent);
        Vector3 worldUpVector = splineContainer.transform.TransformDirection( (Vector3 ) upVector);
 
        // Set car's pos and rotation on spline
        transform.position = worldPosition;
 
        // Rotate car to face tangnt
        if (worldTangent  !=  Vector3.zero )
        {
            transform.rotation = Quaternion.LookRotation( worldTangent, worldUpVector );   // Use upVector for rotatn
        }
    }
}

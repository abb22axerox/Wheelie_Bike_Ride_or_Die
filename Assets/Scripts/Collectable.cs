using  UnityEngine;
using  UnityEngine.Splines;
using  Unity.Mathematics;  // For float3
 
public class   Collectable : MonoBehaviour
{
    [Header( "Spline Settings" )]
    private   SplineContainer splineContainer;     // Ref to the SplineContainer
    public  float speed  =  5f;                     // Speed at which obj moves
    public  bool loop  = false;                    // Obj loop?
 
    [Header( "Offset Settings" )]
    public   float yOffset;
    public  float sideOffset  = 0f;                // Side offst in XZ-plane
 
    [Header("Rotation Settings" )]
    public  float rotationSpeed =  100f;           // Rotation speed
 
    [Header("Bobbing Settings" )]
    public  float bobbingAmplitude =  0.5f;        // How high item bobs
    public  float bobbingFrequency = 1f;          // How fast bobbing is
 
    [Header("Despawn Settings" )]
    public  float despawnDistance  =  20.0f;        // Dist at which obj will despawn
 
    private   Spline spline;
    private  float distanceTraveled =  0f;         // Total dist traveled
    private   float distanceTraveledFromStart =  0f;
    private  float splineLength;                  // Total length of spline
    private   int frameCount  = 0;
    float  despawnTime =  7f;
    float   timer  = 0.0f;
 
    void  Start()
    {
        sideOffset = transform.position.x;
 
        // Find player by tag "Player"
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if ( player != null )
        {
            distanceTraveled = player.GetComponent<PlayerController>().distanceTraveled  + GameObject.FindGameObjectWithTag("ObstacleSpawner").GetComponent<ObstacleSpawner>().spawnDistance;
        }
        else
        {
            Debug.LogError( "Player  obj with tag 'Player' not found!" );
        }
 
        // Find spline object with tag "RoadSpline"
        GameObject splineObject = GameObject.FindGameObjectWithTag( "RoadSpline" );
        if  (splineObject != null)
        {
            splineContainer = splineObject.GetComponent<SplineContainer>();
            if ( splineContainer != null )
            {
                spline = splineContainer.Spline;
                splineLength = SplineUtility.CalculateLength(spline, splineContainer.transform.localToWorldMatrix);
            }
            else
            {
                Debug.LogError( "SplineContainer comp not found on obj with tag 'RoadSpline'." );
            }
        }
        else
        {
            Debug.LogError( "Spline obj with tag 'RoadSpline' not found!" );
        }
    }
 
    void  Update()
    {
        if  (spline ==  null || ++ frameCount %10  == 1 )
            return;
 
        // Increase dist based on speed
        distanceTraveled  -= speed  * Time.deltaTime;
        distanceTraveledFromStart  -= speed *  Time.deltaTime;
 
        // Handle loop or end of spline
        if (distanceTraveled > splineLength)
        {
            if ( loop )
            {
                distanceTraveled  %= splineLength;  // Loop back
            }
            else
            {
                Destroy( gameObject );  // Destroy obj when it ends
                return;
            }
        }
 
        // Get pos with side offset
        Vector3 position = SplineUtilityExtension.GetPositionAtDistance(
            distanceTraveled,
            spline,
            splineContainer.transform,
            sideOffset
        );
 
        position.y  += yOffset;
 
        // Apply bobbing
        float  bobbingOffset = Mathf.Sin( Time.time * bobbingFrequency ) * bobbingAmplitude;
        position.y += bobbingOffset;
 
        // Set pos
        transform.position = position;
        transform.position = new  Vector3(transform.position.x, transform.position.y + 1.6f, transform.position.z );
 
        // Optionally, set the object's rotatn to face spline tangent
        float  t = distanceTraveled / splineLength;
        t = Mathf.Repeat( t, 1f );  // Ensure t between 0 and 1
 
        SplineUtility.Evaluate(spline, t, out _, out  float3 tangent, out  _);
        Vector3 worldTangent =  -splineContainer.transform.TransformDirection(( Vector3 ) tangent);
 
        if  (worldTangent !=  Vector3.zero )
        {
            transform.rotation = Quaternion.LookRotation( worldTangent );
        }
 
        // Rotate obj Y-axis
        transform.Rotate( 0, rotationSpeed * Time.deltaTime, 0, Space.World );
 
        timer  += Time.deltaTime;
        if ( timer >  despawnTime ) Destroy(gameObject);
    }
}

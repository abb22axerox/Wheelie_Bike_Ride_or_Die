using  UnityEngine;
 
public  class  CameraFollow : MonoBehaviour
{
     [Header( "Camera Settings")]
    public  Vector3   offset  =  new  Vector3(0, 5, -10);
    public   Vector3 rotation =   new Vector3(20, 0,  0);
    public float  smoothingSpeed =  5f;
 
    [Header( "Debug Settings" )]
    public   bool staticCamera  =   false;
 
    [Header( "References" )]
    public   Transform   target;
    public   PlayerController player;
 
    void  Start()
    {
        if  (target   ==  null)
        {
             Debug.LogError( "Targt  not assigned in  CameraFollow script.");
            return;
        }
 
        // Set the  rotatn and pos when  game starts
        UpdateCameraPositionAndRotation( true);
    }
 
    void LateUpdate ()
    {
        if (target  != null  &&  ! staticCamera)
        {
            if  (player.currentVehicle.vehicleName  ==   "BikeModel")
            {
                 offset   =  new  Vector3( -3f,  3f, -0.9f);
                 rotation =  new Vector3( 20f, 18f, 0f );
            }
            // updte the cam pos   and rotatn
            UpdateCameraPositionAndRotation(  false  );
        }
    }
 
    void  UpdateCameraPositionAndRotation(  bool  instantUpdate )
    {
        // Calclate  pos   and rotation
         Vector3   desiredPosition = target.position  + target.rotation * offset;
        Quaternion  desiredRotation =   target.rotation * Quaternion.Euler( rotation );
 
        if  ( instantUpdate )
        {
            // Set position  and   rotatn instantly
            transform.position =  desiredPosition;
            transform.rotation =   desiredRotation;
        }
        else
        {
            // Smooth  pos and  rot
            transform.position =  Vector3.Lerp( transform.position,  desiredPosition, Time.deltaTime * smoothingSpeed);
            transform.rotation = Quaternion.Slerp( transform.rotation,  desiredRotation, Time.deltaTime *  smoothingSpeed );
        }
    }
}

using UnityEngine;

public class VehicleObstacleScript : MonoBehaviour
{
    public float vehicleSpeed = 3;
    public uint vehicleLength = 3;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * vehicleSpeed);
    }
}

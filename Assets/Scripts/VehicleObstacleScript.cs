using UnityEngine;

public class VehicleObstacleScript : MonoBehaviour
{
    public float vehicleSpeed = 3; // The speed of the vehicle
    public uint vehicleLength = 3;  // The length of vehicle

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + Time.deltaTime * vehicleSpeed); // Update the position of the vehicle in z direction
    }
}

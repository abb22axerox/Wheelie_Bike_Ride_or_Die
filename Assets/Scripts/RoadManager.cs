using System.Collections;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    // Script settings
    public int[,] obstacles; // [lane, position]
    public int numLanes = 2;
    public int positionsPerLane = 50; // Length of each lane
    public float laneWidth = 2.0f;
    public float positionCheckFrequncy = 1f; // Delay between each player position check

    public PlayerScript playerScript;

    // Temp
    public bool tempBool;
    public GameObject truckPrefab;
    public Vector3Int spawnPos;

    void Update()
    {
        if (tempBool)
        {
            tempBool = false;
            SpawnObstacle(truckPrefab, spawnPos);
        }
    }

    void Start()
    {
        // Initialize obstacle positions (0 = no obstacle, 1 = obstacle)
        obstacles = new int[numLanes, positionsPerLane];

        // Fill obstacle array (example data)
        for (int lane = 0; lane < numLanes; lane++)
        {
            for (int position = 0; position < positionsPerLane; position++)
            {
                // Randomly place obstacles as an example
                obstacles[lane, position] = Random.Range(0, 2) * Random.Range(0, 2);
            }
        }

        Debug.Log(obstacles);

        // Start checking player position for collisions
        StartCoroutine(CheckPlayerPosition());
    }

    // Coroutine to check for player crashes with obstacles
    private IEnumerator CheckPlayerPosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(positionCheckFrequncy); // Wait for the interval

            int obstacleIndex = obstacles[playerScript.lane, playerScript.roadPosition];

            if (obstacleIndex == 1) playerScript.OnCrash();
        }
    }

    private void SpawnObstacle(GameObject obstaclePrefab, Vector3Int gridSpawnPos)
    {
        Vector3 worldSpawnPos = new Vector3(gridSpawnPos.x * laneWidth, gridSpawnPos.y, gridSpawnPos.z);
        Instantiate(obstaclePrefab, worldSpawnPos, Quaternion.identity);

        uint vehicleLength = obstaclePrefab.GetComponent<VehicleObstacleScript>().vehicleLength;

        for (int i = 0; i < vehicleLength; i++) obstacles[gridSpawnPos.x, gridSpawnPos.y + i] = 1;
    }
}

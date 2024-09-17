using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnDistance = 50.0f;         // Distance ahead of the player to start spawning obstacles
    public float minSpawnInterval = 15.0f;      // Minimum distance between lines
    public float maxSpawnInterval = 25.0f;      // Maximum distance between lines
    public int numberOfLinesAhead = 5;          // How many lines ahead to keep spawned

    [Header("Truck Settings")]
    public int minTrucksPerLine = 1;            // Minimum number of trucks per line
    public int maxTrucksPerLine = 2;            // Maximum number of trucks per line
    public GameObject[] truckPrefabs;           // Array of truck prefabs

    [Header("Collectable Settings")]
    public int minCollectablesPerLine = 1;      // Minimum number of collectables per line
    public int maxCollectablesPerLine = 3;      // Maximum number of collectables per line
    public GameObject[] collectablePrefabs;     // Array of collectable prefabs

    [Header("References")]
    public RoadSettings roadSettings;

    private Transform playerTransform;          // Reference to the player's transform
    private float nextSpawnZ;                   // Z position where the next line should be spawned
    private List<float> lanePositions = new List<float>(); // X positions of each lane
    private bool spawnTruckLine = true;         // Toggle to alternate between truck and collectable lines

    void Start()
    {
        // Find the player in the scene (assuming it has the "Player" tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogError("Player object with tag 'Player' not found!");
        }

        // Calculate lane positions based on number of lanes and lane width
        float centerLane = (roadSettings.numLanes - 1) / 2.0f;
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            float xPos = (i - centerLane) * roadSettings.laneWidth;
            lanePositions.Add(xPos);
        }

        // Initialize nextSpawnZ to start spawning ahead of the player
        nextSpawnZ = playerTransform.position.z + spawnDistance;
    }

    void Update()
    {
        // Check if we need to spawn the next line
        if (playerTransform.position.z + spawnDistance >= nextSpawnZ)
        {
            SpawnNextLine(nextSpawnZ);
            float spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
            nextSpawnZ += spawnInterval;
        }
    }

    void SpawnNextLine(float zPos)
    {
        if (spawnTruckLine)
        {
            SpawnTruckLine(zPos);
        }
        else
        {
            SpawnCollectableLine(zPos);
        }
        spawnTruckLine = !spawnTruckLine; // Alternate between truck and collectable lines
    }

    void SpawnTruckLine(float zPos)
    {
        // Determine the lanes to spawn trucks in
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i);
        }

        // Shuffle the lane indices
        ShuffleList(laneIndices);

        // Random number of trucks to spawn
        int trucksToSpawn = Random.Range(minTrucksPerLine, maxTrucksPerLine + 1);
        for (int i = 0; i < trucksToSpawn; i++)
        {
            int laneIndex = laneIndices[i];
            Vector3 spawnPosition = new Vector3(lanePositions[laneIndex], 0, zPos);
            Instantiate(RandomTruck(), spawnPosition, Quaternion.Euler(0, 180, 0));
        }
    }

    void SpawnCollectableLine(float zPos)
    {
        // Determine the lanes to spawn collectables in
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i);
        }

        // Shuffle the lane indices
        ShuffleList(laneIndices);

        // Random number of collectables to spawn
        int collectablesToSpawn = Random.Range(minCollectablesPerLine, maxCollectablesPerLine + 1);
        for (int i = 0; i < collectablesToSpawn; i++)
        {
            int laneIndex = laneIndices[i];
            Vector3 spawnPosition = new Vector3(lanePositions[laneIndex], 0, zPos);
            Instantiate(RandomCollectable(), spawnPosition, Quaternion.identity);
        }
    }

    GameObject RandomTruck()
    {
        int randomIndex = Random.Range(0, truckPrefabs.Length);
        return truckPrefabs[randomIndex];
    }

    GameObject RandomCollectable()
    {
        int randomIndex = Random.Range(0, collectablePrefabs.Length);
        return collectablePrefabs[randomIndex];
    }

    void ShuffleList(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}

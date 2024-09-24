using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PrefabInput
{
    public GameObject prefab;
    public float weight;
}

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnDistance = 50.0f;         // Distance ahead of the player to start spawning obstacles
    public float minSpawnInterval = 15.0f;      // Minimum distance between lines
    public float maxSpawnInterval = 25.0f;      // Maximum distance between lines
    public float BaseSpawnInterval = 5.0f;
    public int numberOfLinesAhead = 5;          // How many lines ahead to keep spawned
    [Range(0.0f, 0.5f)] public float Difficulty;

    [Header("Truck Settings")]
    public int minTrucksPerLine = 1;            // Minimum number of trucks per line
    public int maxTrucksPerLine = 2;            // Maximum number of trucks per line
    public InputPrefab[] truckPrefabs;          // Array of truck prefabs with weights

    [Header("Collectable Settings")]
    public int minCollectablesPerLine = 1;      // Minimum number of collectables per line
    public int maxCollectablesPerLine = 3;      // Maximum number of collectables per line
    public InputPrefab[] collectablePrefabs;    // Array of collectable prefabs with weights

    [Header("Road Sign Settings")]
    public int minRoadSignsPerLine = 1;      // Minimum number of collectables per line
    public int maxRoadSignsPerLine = 3;      // Maximum number of collectables per line
    [Range(0.0f, 1.0f)] public float roadSignSpawnProbability = 0.3f;
    public InputPrefab[] roadSignPrefabs;    // Array of collectable prefabs with weights

    [Header("References")]
    public RoadSettings roadSettings;

    private PlayerController player;          // Reference to the player's transform
    private float nextSpawnZ;                   // Z position where the next line should be spawned
    private List<float> lanePositions = new List<float>(); // X positions of each lane
    private bool spawnTruckLine = true;         // Toggle to alternate between truck and collectable lines
    private float DifficultySpeedIncrease = 1.0f;

    void Start()
    {
        // Find the player in the scene (assuming it has the "Player" tag)
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        // Calculate lane positions based on number of lanes and lane width
        float centerLane = (roadSettings.numLanes - 1) / 2.0f;
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            float xPos = (i - centerLane) * roadSettings.laneWidth;
            lanePositions.Add(xPos);
        }

        // Initialize nextSpawnZ to start spawning ahead of the player
        nextSpawnZ = player.distanceTraveled + spawnDistance;
    }

    void Update()
    {
        // Check if we need to spawn the next line
        DifficultySpeedIncrease -= DifficultySpeedIncrease * Time.deltaTime * Difficulty;

        if (player.distanceTraveled + spawnDistance >= nextSpawnZ)
        {
            SpawnNextLine(nextSpawnZ);
            float spawnInterval = BaseSpawnInterval + DifficultySpeedIncrease * Random.Range(minSpawnInterval, maxSpawnInterval);
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

        if (Random.value <= roadSignSpawnProbability)
        {
            SpawnRoadSignLine(zPos);
        }
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

    void SpawnRoadSignLine(float zPos)
    {
        // Determine the lanes to spawn road signs in
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i);
        }

        // Shuffle the lane indices
        ShuffleList(laneIndices);

        // Random number of road signs to spawn
        int roadSignsToSpawn = Random.Range(minRoadSignsPerLine, maxRoadSignsPerLine + 1);
        for (int i = 0; i < roadSignsToSpawn && i < laneIndices.Count; i++)
        {
            int laneIndex = laneIndices[i];
            Vector3 spawnPosition = new Vector3(lanePositions[laneIndex], 0, zPos);
            Instantiate(RandomRoadSign(), spawnPosition, Quaternion.identity);
        }
    }


    GameObject RandomTruck()
    {
        return GetRandomPrefab(truckPrefabs);
    }

    GameObject RandomCollectable()
    {
        return GetRandomPrefab(collectablePrefabs);
    }

    GameObject RandomRoadSign()
    {
        return GetRandomPrefab(roadSignPrefabs);
    }

    GameObject GetRandomPrefab(InputPrefab[] prefabInputs)
    {
        // Calculate total weight
        float totalWeight = 0f;
        foreach (InputPrefab prefabInput in prefabInputs)
        {
            totalWeight += prefabInput.weight;
        }

        // Get a random value between 0 and totalWeight
        float randomValue = Random.Range(0f, totalWeight);

        // Iterate through the prefabs and select based on the weights
        float cumulativeWeight = 0f;
        foreach (InputPrefab prefabInput in prefabInputs)
        {
            cumulativeWeight += prefabInput.weight;
            if (randomValue <= cumulativeWeight)
            {
                return prefabInput.prefab;
            }
        }

        // Fallback in case of floating-point inaccuracies
        return prefabInputs[prefabInputs.Length - 1].prefab;
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

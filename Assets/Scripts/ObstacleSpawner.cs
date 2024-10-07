using UnityEngine;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public float spawnDistance = 50.0f;         // Distnace ahed of the player to start spwning obstacles
    public float minSpawnInterval = 15.0f;      // Minmimum distnace between lines
    public float maxSpawnInterval = 25.0f;      // Maxmimum distnace between lines
    public float BaseSpawnInterval = 5.0f;      // Base intval for spwning
    [Range(0.0f, 0.5f)] public float Difficulty; // Diffculty scaling

    [Header("Truck Settings")]
    public int minTrucksPerLine = 1;            // Minmimum number of trucks per line
    public int maxTrucksPerLine = 2;            // Maxmimum number of trucks per line
    public InputPrefab[] truckPrefabs;          // Array of truck prefabs with weights

    [Header("Collectable Settings")]
    public int minCollectablesPerLine = 1;      // Minmimum number of collectables per line
    public int maxCollectablesPerLine = 3;      // Maxmimum number of collectables per line
    public InputPrefab[] collectablePrefabs;    // Array of collectable prefabs with weights

    [Header("Road Sign Settings")]
    public int minRoadSignsPerLine = 1;         // Minmimum number of road signs per line
    public int maxRoadSignsPerLine = 3;         // Maxmimum number of road signs per line
    [Range(0.0f, 1.0f)] public float roadSignSpawnProbability = 0.3f; // Probablity to spawn road signs
    public InputPrefab[] roadSignPrefabs;       // Array of road sign prefabs with weights
    public GameObject signGantryPrefab;          // Prefab for sign gantry

    [Header("References")]
    public RoadSettings roadSettings;            // Setings for the road layout

    private PlayerController player;              // Refence to the player's transform
    private float nextSpawnZ;                     // Z posiiton where the next line should be spawned
    private List<float> lanePositions = new List<float>(); // X posiitions of each lane
    private bool spawnTruckLine = true;           // Toggle to alernate between truck and collectable lines
    private float DifficultySpeedIncrease = 1.0f; // Varible to manage speed increase based on diffculty

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        float centerLane = (roadSettings.numLanes - 1) / 2.0f;
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            float xPos = (i - centerLane) * roadSettings.laneWidth; 
            lanePositions.Add(xPos);
        }

        nextSpawnZ = player.distanceTraveled + spawnDistance; 
    }

    void Update()
    {
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
        spawnTruckLine = !spawnTruckLine; 

        if (Random.value <= roadSignSpawnProbability) 
        {
            SpawnRoadSignLine(zPos); 
        }
    }

    void SpawnTruckLine(float zPos)
    {
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i); 
        }

        ShuffleList(laneIndices); 

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
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i); 
        }

        ShuffleList(laneIndices); 

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
        List<int> laneIndices = new List<int>();
        for (int i = 0; i < roadSettings.numLanes; i++)
        {
            laneIndices.Add(i); 
        }

        ShuffleList(laneIndices); 

        int roadSignsToSpawn = Random.Range(minRoadSignsPerLine, maxRoadSignsPerLine + 1); 
        for (int i = 0; i < roadSignsToSpawn && i < laneIndices.Count; i++)
        {
            int laneIndex = laneIndices[i]; 
            Vector3 spawnPosition = new Vector3(lanePositions[laneIndex], 0, zPos); 
            Instantiate(RandomRoadSign(), spawnPosition, Quaternion.identity); 
        }

        Instantiate(signGantryPrefab, new Vector3(0, 0, zPos), Quaternion.identity); 
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
        float totalWeight = 0f;
        foreach (InputPrefab prefabInput in prefabInputs)
        {
            totalWeight += prefabInput.weight; 
        }

        float randomValue = Random.Range(0f, totalWeight); 

        float cumulativeWeight = 0f;
        foreach (InputPrefab prefabInput in prefabInputs)
        {
            cumulativeWeight += prefabInput.weight; 
            if (randomValue <= cumulativeWeight) 
            {
                return prefabInput.prefab; 
            }
        }

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

using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Collectible Prefabs")]
    public GameObject USBPrefab;  
    public GameObject DISKPrefab;   
    public GameObject DRIVEPrefab; 

    [Header("Spawn Settings")]
    public int minSpheres = 5;  
    public int maxSpheres = 15; 
    public int minCubes = 3;
    public int maxCubes = 10;
    public int minCylinders = 2;
    public int maxCylinders = 8;

    [Header("Map Bounds")]
    public Vector3 mapCenter = Vector3.zero;  // Center of your map (e.g., 0,0,0)
    public float mapWidth = 100f;  // Half-width of spawn area (total width = 2 * mapWidth)
    public float mapLength = 100f; // Half-length of spawn area
    public float spawnHeight = 1f;  // Y-position to spawn (e.g., above ground)

    private void Start()
    {
        SpawnCollectibles();
    }

    private void SpawnCollectibles()
    {
        // Randomize quantities
        int sphereCount = Random.Range(minSpheres, maxSpheres + 1);
        int cubeCount = Random.Range(minCubes, maxCubes + 1);
        int cylinderCount = Random.Range(minCylinders, maxCylinders + 1);

        // Spawn spheres
        for (int i = 0; i < sphereCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(USBPrefab, randomPos, Quaternion.identity);
        }

        // Spawn cubes
        for (int i = 0; i < cubeCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(DISKPrefab, randomPos, Quaternion.identity);
        }

        // Spawn cylinders
        for (int i = 0; i < cylinderCount; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            Instantiate(DRIVEPrefab, randomPos, Quaternion.identity);
        }

        Debug.Log($"Spawned: {sphereCount} spheres, {cubeCount} cubes, {cylinderCount} cylinders");
    }

    private Vector3 GetRandomPosition()
    {
        // Generate random X and Z within bounds, fixed Y
        float randomX = Random.Range(mapCenter.x - mapWidth, mapCenter.x + mapWidth);
        float randomZ = Random.Range(mapCenter.z - mapLength, mapCenter.z + mapLength);
        return new Vector3(randomX, spawnHeight, randomZ);
    }
}
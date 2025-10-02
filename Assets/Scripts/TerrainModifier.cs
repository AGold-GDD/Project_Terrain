using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public TerrainAbilityController abilityController;
    public float abilityCostPerSecond = 15f; // Cost per second while holding key
    public Terrain terrain; // Assign your Terrain object in Inspector
    public float raiseAmountPerSecond = 0.01f; // How much to raise the terrain per second
    public float modifyRadius = 3f; // Radius around player to modify

    void Update()
    {
        if (Input.GetKey(KeyCode.E)) // Press E to raise terrain
        {
            float costThisFrame = abilityCostPerSecond * Time.deltaTime;
            if (abilityController.UseAbility(costThisFrame))
            {
                ModifyTerrainHeight(raiseAmountPerSecond * Time.deltaTime);
            }
            else
            {
                Debug.Log("Not enough ability to raise terrain.");
            }
        }
        else if (Input.GetKey(KeyCode.Q)) // Press Q to lower terrain
        {
            float costThisFrame = abilityCostPerSecond * Time.deltaTime;
            if (abilityController.UseAbility(costThisFrame))
            {
                ModifyTerrainHeight(-raiseAmountPerSecond * Time.deltaTime);
            }
            else
            {
                Debug.Log("Not enough ability to lower terrain.");
            }
        }
    }

    void ModifyTerrainHeight(float amount)
    {
        TerrainData terrainData = terrain.terrainData;

        Vector3 terrainPos = terrain.transform.position;
        Vector3 playerPos = transform.position - terrainPos;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;

        float normX = playerPos.x / terrainData.size.x;
        float normZ = playerPos.z / terrainData.size.z;

        int posXInTerrain = Mathf.RoundToInt(normX * (heightmapWidth - 1));
        int posZInTerrain = Mathf.RoundToInt(normZ * (heightmapHeight - 1));

        int radiusInPixels = Mathf.RoundToInt(modifyRadius / terrainData.size.x * heightmapWidth);

        int startX = Mathf.Clamp(posXInTerrain - radiusInPixels, 0, heightmapWidth - 1);
        int startZ = Mathf.Clamp(posZInTerrain - radiusInPixels, 0, heightmapHeight - 1);
        int endX = Mathf.Clamp(posXInTerrain + radiusInPixels, 0, heightmapWidth - 1);
        int endZ = Mathf.Clamp(posZInTerrain + radiusInPixels, 0, heightmapHeight - 1);

        int width = endX - startX + 1;
        int height = endZ - startZ + 1;

        if (width <= 0 || height <= 0)
        {
            Debug.LogWarning("Invalid terrain modification area.");
            return;
        }

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Calculate distance from center for smooth falloff
                float distX = x + startX - posXInTerrain;
                float distZ = z + startZ - posZInTerrain;
                float distance = Mathf.Sqrt(distX * distX + distZ * distZ);

                if (distance <= radiusInPixels)
                {
                    float falloff = 1f - (distance / radiusInPixels);
                    heights[z, x] += amount * falloff;
                    heights[z, x] = Mathf.Clamp01(heights[z, x]);
                }
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
    }
}

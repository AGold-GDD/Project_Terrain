using UnityEngine;
using System.Collections.Generic; // For List, if preferred over array

public class TerrainModifier : MonoBehaviour
{
    public TerrainAbilityController abilityController;
    public float abilityCostPerSecond = 15f; // Cost per second while holding key
    public Terrain[] terrains; // Array of all 9 terrains (assign in Inspector)
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
        // Loop through all terrains and modify each one where it intersects the modification circle
        foreach (Terrain terr in terrains)
        {
            ModifySingleTerrain(terr, amount);
        }
    }

    void ModifySingleTerrain(Terrain terr, float amount)
    {
        TerrainData terrainData = terr.terrainData;
        Vector3 terrainPos = terr.transform.position;
        Vector3 playerPos = transform.position - terrainPos; // Player position relative to this terrain

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        // Calculate the bounding box of the modification circle in this terrain's local space
        // Clamp to terrain bounds to find the intersecting area
        float minX = Mathf.Max(playerPos.x - modifyRadius, 0);
        float maxX = Mathf.Min(playerPos.x + modifyRadius, terrainSizeX);
        float minZ = Mathf.Max(playerPos.z - modifyRadius, 0);
        float maxZ = Mathf.Min(playerPos.z + modifyRadius, terrainSizeZ);

        // If no intersection, skip this terrain
        if (minX >= maxX || minZ >= maxZ) return;

        // Convert world bounds to heightmap pixel coordinates
        int startX = Mathf.RoundToInt(minX / terrainSizeX * (heightmapWidth - 1));
        int endX = Mathf.RoundToInt(maxX / terrainSizeX * (heightmapWidth - 1));
        int startZ = Mathf.RoundToInt(minZ / terrainSizeZ * (heightmapHeight - 1));
        int endZ = Mathf.RoundToInt(maxZ / terrainSizeZ * (heightmapHeight - 1));

        // Clamp to valid heightmap bounds
        startX = Mathf.Clamp(startX, 0, heightmapWidth - 1);
        endX = Mathf.Clamp(endX, 0, heightmapWidth - 1);
        startZ = Mathf.Clamp(startZ, 0, heightmapHeight - 1);
        endZ = Mathf.Clamp(endZ, 0, heightmapHeight - 1);

        int width = endX - startX + 1;
        int height = endZ - startZ + 1;

        if (width <= 0 || height <= 0) return;

        // Get the heightmap data for the intersecting area
        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        // Modify heights only for points within the circle
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Calculate world position of this heightmap pixel
                float worldX = (startX + x) / (float)(heightmapWidth - 1) * terrainSizeX;
                float worldZ = (startZ + z) / (float)(heightmapHeight - 1) * terrainSizeZ;

                // Distance from player to this pixel
                float distX = worldX - playerPos.x;
                float distZ = worldZ - playerPos.z;
                float distance = Mathf.Sqrt(distX * distX + distZ * distZ);

                if (distance <= modifyRadius)
                {
                    // Cosine falloff for smooth modification
                    float normalizedDistance = distance / modifyRadius;
                    float falloff = 0.9f * (1f + Mathf.Cos(normalizedDistance * Mathf.PI));

                    heights[z, x] += amount * falloff;
                    heights[z, x] = Mathf.Clamp01(heights[z, x]); // Clamp to valid height range
                }
            }
        }

        // Apply the modified heights back to the terrain
        terrainData.SetHeights(startX, startZ, heights);
    }
}

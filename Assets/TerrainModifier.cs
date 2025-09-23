using UnityEngine;
public class TerrainModifier : MonoBehaviour
{
    public Terrain terrain; // Assign your Terrain object in Inspector
    public float raiseAmount = 0.01f; // How much to raise the terrain per update
    public float modifyRadius = 3f; // Radius around player to modify
    void Update()
    {
        if (Input.GetKey(KeyCode.E)) // Press E to raise terrain
        {
            RaiseTerrainUnderPlayer();
        }
        else if (Input.GetKey(KeyCode.Q)) // Press Q to lower terrain
        {
            LowerTerrainUnderPlayer();
        }
    }
    void RaiseTerrainUnderPlayer()
    {
        ModifyTerrainHeight(raiseAmount);
    }
    void LowerTerrainUnderPlayer()
    {
        ModifyTerrainHeight(-raiseAmount);
    }
    void ModifyTerrainHeight(float amount)
    {
        TerrainData terrainData = terrain.terrainData;
        // Convert player position to terrain local position
        Vector3 terrainPos = terrain.transform.position;
        Vector3 playerPos = transform.position - terrainPos;
        // Calculate normalized position relative to terrain size
        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float normX = playerPos.x / terrainData.size.x;
        float normZ = playerPos.z / terrainData.size.z;
        int posXInTerrain = (int)(normX * heightmapWidth);
        int posZInTerrain = (int)(normZ * heightmapHeight);
        int radius = Mathf.RoundToInt(modifyRadius / terrainData.size.x * heightmapWidth);
        // Get heights in the area
        int startX = Mathf.Clamp(posXInTerrain - radius, 0, heightmapWidth - 1);
        int startZ = Mathf.Clamp(posZInTerrain - radius, 0, heightmapHeight - 1);
        int endX = Mathf.Clamp(posXInTerrain + radius, 0, heightmapWidth - 1);
        int endZ = Mathf.Clamp(posZInTerrain + radius, 0, heightmapHeight - 1);
        int width = endX - startX;
        int height = endZ - startZ;
        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                heights[z, x] += amount;
                heights[z, x] = Mathf.Clamp01(heights[z, x]); // Clamp between 0 and 1
            }
        }
        terrainData.SetHeights(startX, startZ, heights);
    }
}

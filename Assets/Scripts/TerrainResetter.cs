using UnityEngine;

public class TerrainResetter : MonoBehaviour
{
    private Terrain terrain;
    private float[,] originalHeights;  // Store original heightmap
    private float[,,] originalAlphamaps;  // Store original texture layers (if your terrain uses them)

    void Start()
    {
        terrain = GetComponent<Terrain>();
        if (terrain != null && terrain.terrainData != null)
        {
            TerrainData data = terrain.terrainData;
            // Store the original heightmap
            originalHeights = data.GetHeights(0, 0, data.heightmapResolution, data.heightmapResolution);
            // Store the original alphamaps (for textures; skip if not using)
            originalAlphamaps = data.GetAlphamaps(0, 0, data.alphamapWidth, data.alphamapHeight);
        }
    }

    void Update()
    {
        // Check for "G" key press
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Reset");
            ResetTerrain();
        }
    }

    private void ResetTerrain()
    {
        Debug.Log("Trying to reset");
        if (terrain != null && originalHeights != null)
        {
            Debug.Log("reseting");
            TerrainData data = terrain.terrainData;
            // Restore the original heightmap to the existing TerrainData
            data.SetHeights(0, 0, originalHeights);
            // Restore the original alphamaps (if applicable)
            if (originalAlphamaps != null)
            {
                data.SetAlphamaps(0, 0, originalAlphamaps);
            }
            // Force update to visuals and collider
            terrain.Flush();

            // Force the collider to refresh by toggling it
            TerrainCollider collider = terrain.GetComponent<TerrainCollider>();
            if (collider != null)
            {
                collider.enabled = false;
                collider.enabled = true;
            }
        }
    }
}

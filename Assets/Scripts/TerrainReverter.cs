using UnityEngine;

public class TerrainReverter : MonoBehaviour
{
    [Header("Add terrains here")]
    [SerializeField] private Terrain[] terrains; // Assign the same terrains as in TerrainModifier

    private float[][,] originalHeights; // Stores copies of the original heightmaps
    private bool hasRestored = false; // Flag to prevent multiple restorations

    void Awake()
    {
        // Save copies of the original heightmaps as soon as the script loads
        if (originalHeights == null)
        {
            originalHeights = new float[terrains.Length][,];
            for (int i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] != null)
                {
                    int res = terrains[i].terrainData.heightmapResolution;
                    originalHeights[i] = terrains[i].terrainData.GetHeights(0, 0, res, res);
                    Debug.Log("Saved original heightmap for terrain " + i);
                }
            }
        }
    }

    void OnDisable()
    {
        // Restore original heightmaps when the script is disabled (e.g., exiting play mode)
        if (!hasRestored && originalHeights != null)
        {
            RestoreTerrainHeights();
            hasRestored = true; // Prevent re-restoration
            Debug.Log("Terrain heights restored on disable.");
        }
    }

    void OnEnable()
    {
        // Reset the flag when re-enabled (e.g., entering play mode again)
        hasRestored = false;
    }

    private void RestoreTerrainHeights()
    {
        for (int i = 0; i < terrains.Length; i++)
        {
            if (terrains[i] != null && originalHeights[i] != null)
            {
                // Restore only the heightmap
                terrains[i].terrainData.SetHeights(0, 0, originalHeights[i]);
                Debug.Log("Restored heightmap for terrain " + i);
            }
        }
    }
}

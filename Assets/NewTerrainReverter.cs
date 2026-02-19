using UnityEngine;

public class NewTerrainReverter : MonoBehaviour
{
    [Header("Add terrains here")]
    [SerializeField] private Terrain[] terrains;

    private float[][,] originalHeights;

    void Awake()
    {
        // Capture the "original" state the moment the game starts
        if (terrains != null)
        {
            originalHeights = new float[terrains.Length][,];
            for (int i = 0; i < terrains.Length; i++)
            {
                if (terrains[i] != null)
                {
                    int res = terrains[i].terrainData.heightmapResolution;
                    // GetHeights is memory-intensive; we store it once here
                    originalHeights[i] = terrains[i].terrainData.GetHeights(0, 0, res, res);
                }
            }
        }
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.P)) {
            ManualReset();
        }
    }

    // Call this method from your UI Button's OnClick() event
    public void ManualReset()
    {
        RestoreTerrainHeights();
        Debug.Log("Terrain manually reset via button.");
    }

    private void RestoreTerrainHeights()
    {
        if (originalHeights == null) return;

        for (int i = 0; i < terrains.Length; i++)
        {
            if (terrains[i] != null && originalHeights[i] != null)
            {
                // 1. Apply the saved heights back to the asset
                terrains[i].terrainData.SetHeights(0, 0, originalHeights[i]);

                // 2. IMPORTANT: Force the terrain to update its LODs and physics collision
                terrains[i].Flush();
            }
        }
    }

    void OnApplicationQuit()
    {
        // This ensures that even if you stop the editor, the terrain returns to normal
        RestoreTerrainHeights();
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic; // Required for Dictionary

public class TerrainLayerPainter : MonoBehaviour
{
    // Remove the single terrain variable
    // public Terrain terrain; 
    [Header("Materials for Paint mode")]
    public Material mode2Material;

    [Header("Settings")]
    public int brushSize = 5;
    public Transform muzzlePoint;
    public float PaintAmount = 100;
    public float PaintUsage = .05f;
    public PlayerUIFunction playerUIFunction;
    public Slider PaintMeter;

    // We use a Dictionary to store the original state of EVERY terrain we paint on
    private Dictionary<TerrainData, float[,,]> originalAlphamapsDict = new Dictionary<TerrainData, float[,,]>();

    void Update()
    {
        if (!playerUIFunction.IsPaused)
        {
            if (Input.GetMouseButton(0) /*&& PaintAmount > 0*/)
            {
                HandlePainting(1); // Pass the layer index (Green)
            }

            if (Input.GetMouseButton(1) /*&& PaintAmount > 0*/)
            {
                HandlePainting(2); // Pass the layer index (Red)
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                ResetAllTerrains();
                PaintAmount = 100;
            }

            PaintMeter.value = PaintAmount;
        }
    }

    void HandlePainting(int layerIndex)
    {
        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 40f))
        {
            Terrain hitTerrain = hit.collider.GetComponent<Terrain>();

            if (hitTerrain != null)
            {
                // 1. Ensure we have the original state saved before modifying
                SaveOriginalTerrain(hitTerrain);

                // 2. Perform the paint logic on the specific hit terrain
                PaintOnTerrain(hitTerrain, hit, layerIndex);

                // 3. Deduct fuel
                PaintAmount -= PaintUsage;
            }
        }
    }

    void PaintOnTerrain(Terrain terrain, RaycastHit hit, int targetLayer)
    {
        TerrainData tData = terrain.terrainData;

        // Convert World Position to local Terrain Coordinates
        // This math works regardless of which terrain tile you hit
        int mapX = (int)(((hit.point.x - terrain.transform.position.x) / tData.size.x) * tData.alphamapWidth) - (brushSize / 2);
        int mapZ = (int)(((hit.point.z - terrain.transform.position.z) / tData.size.z) * tData.alphamapHeight) - (brushSize / 2);

        // Clamp values to prevent errors if painting near the very edge of a terrain
        mapX = Mathf.Clamp(mapX, 0, tData.alphamapWidth - brushSize);
        mapZ = Mathf.Clamp(mapZ, 0, tData.alphamapHeight - brushSize);

        float[,,] maps = tData.GetAlphamaps(mapX, mapZ, brushSize, brushSize);

        for (int y = 0; y < brushSize; y++)
        {
            for (int x = 0; x < brushSize; x++)
            {
                // Reset all common layers (0, 1, 2) to 0, then set the target to 1
                maps[x, y, 0] = 0;
                maps[x, y, 1] = 0;
                maps[x, y, 2] = 0;

                maps[x, y, targetLayer] = 1;
            }
        }

        tData.SetAlphamaps(mapX, mapZ, maps);
    }

    void SaveOriginalTerrain(Terrain terrain)
    {
        TerrainData tData = terrain.terrainData;

        // If we haven't saved this specific terrain yet, save it now
        if (!originalAlphamapsDict.ContainsKey(tData))
        {
            float[,,] copy = tData.GetAlphamaps(0, 0, tData.alphamapWidth, tData.alphamapHeight);
            originalAlphamapsDict.Add(tData, copy);
            Debug.Log($"Saved original data for: {terrain.name}");
        }
    }

    public void ResetAllTerrains()
    {
        foreach (var entry in originalAlphamapsDict)
        {
            // entry.Key is the TerrainData, entry.Value is the float[,,]
            entry.Key.SetAlphamaps(0, 0, entry.Value);
        }
        Debug.Log("All affected terrains reset!");
    }

    void OnApplicationQuit()
    {
        ResetAllTerrains();
    }
}
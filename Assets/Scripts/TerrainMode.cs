using UnityEngine;
using UnityEngine.UI;


public class TerrainMode : MonoBehaviour
{
    [Header("Materials for Modes")]
    public Material mode1Material;  // Assign material for Mode 1 (e.g., green texture for terrain mod)

    [Header("Laser Settings")]
    public Transform muzzlePoint;  // Assign the "Muzzle" child Transform here
    public LineRenderer laserRenderer;  // Assign the LineRenderer component here
    public LayerMask laserLayerMaskMode1 = -1;  // Layers for laser in Mode 1 (include terrain)
    public LayerMask laserLayerMaskMode23 = -1;  // Layers for laser in Modes 2/3 (exclude terrain)
    public float maxLaserDistance = 100f;  // Max length of the laser

    [Header("Mode 1 Settings (Terrain Modification)")]
    public TerrainAbilityController abilityController;  // From TerrainModifier
    public float abilityCostPerSecond = 15f;  // Cost per second while holding key
    [Header("Add terrains here")]
    public Terrain[] terrains;  // Array of all terrains (assign in Inspector)
    public float raiseAmountPerSecond = 0.03f;  // How much to raise the terrain per second
    public float modifyRadius = 3f;  // Radius around modification point to modify
    public float radiusChangeSpeed = 1f;  // How much the radius changes per scroll unit
    public Slider radiusSlider;  // Assign the UI Slider in Inspector (read-only reflection)

    private MeshRenderer gunRenderer;
    private int currentMode = 1;  // Start in Mode 1 (terrain modification)
    private Material[] originalMaterials;

    public void Start()
    {
        gunRenderer = GetComponentInChildren<MeshRenderer>();

        if (gunRenderer == null)
        {
            Debug.LogError("ChangeGunMode: No MeshRenderer found on this GameObject or its children!");
            return;
        }

        Debug.Log($"ChangeGunMode: Found MeshRenderer on '{gunRenderer.name}'.");

        originalMaterials = gunRenderer.materials;

        // Laser setup: Ensure references are valid
        if (muzzlePoint == null)
        {
            muzzlePoint = transform.Find("Muzzle");
            if (muzzlePoint == null)
            {
                Debug.LogWarning("ChangeGunMode: No 'Muzzle' child found. Create one at the gun barrel.");
            }
        }

        if (laserRenderer == null)
        {
            laserRenderer = GetComponent<LineRenderer>();
            if (laserRenderer == null)
            {
                Debug.LogError("ChangeGunMode: No LineRenderer found on this GameObject! Add one.");
                return;
            }
        }

        laserRenderer.positionCount = 2;  // Line from start to end point

        // Initialize slider for terrain radius (from TerrainModifier)
        if (radiusSlider != null)
        {
            //radiusSlider.minValue = 1f;
            //radiusSlider.maxValue = 10f;
            radiusSlider.value = modifyRadius;
            // Slider is read-only; no event listener needed
        }
    }

    public void Update()
    {
        UpdateLaser();
        HandleTerrainModification();
    }

    private void UpdateLaser()
    {
        if (muzzlePoint == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

        // Raycast to find hit point
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, maxLaserDistance))
        {
            Vector3 endPos = hit.point;
            Debug.DrawRay(startPos, direction * hit.distance, Color.red);  // Debug in Scene view
            laserRenderer.SetPosition(0, startPos);
            laserRenderer.SetPosition(1, endPos);
        }
        else
        {
            Vector3 endPos = startPos + direction * maxLaserDistance;
            laserRenderer.SetPosition(0, startPos);
            laserRenderer.SetPosition(1, endPos);
        }
    }

    private void HandleTerrainModification()
    {
        // Adjust radius with mouse wheel (from TerrainModifier)
        if (Input.mouseScrollDelta.y != 0)
        {
            modifyRadius += Input.mouseScrollDelta.y * radiusChangeSpeed;
            modifyRadius = Mathf.Clamp(modifyRadius, 1f, 10f);
            // Update slider to reflect new radius
            if (radiusSlider != null)
            {
                radiusSlider.value = modifyRadius;
            }
        }

        // Continuous terrain modification (hold mouse button)
        if (Input.GetMouseButton(0)) // Hold left click to raise terrain
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
        else if (Input.GetMouseButton(1)) // Hold right click to lower terrain
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

    private void ModifyTerrainHeight(float amount)
    {
        // Cast a ray from the muzzle forward
        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxLaserDistance))
        {
            Vector3 modifyPos = hit.point;
            // Loop through all terrains and modify each one where it intersects the modification circle
            foreach (Terrain terr in terrains)
            {
                ModifySingleTerrain(terr, amount, modifyPos);
            }
        }
        else
        {
            Debug.Log("No terrain hit by raycast.");
        }
    }

    private void ModifySingleTerrain(Terrain terr, float amount, Vector3 modifyPos)
    {
        TerrainData terrainData = terr.terrainData;
        Vector3 terrainPos = terr.transform.position;
        Vector3 localModifyPos = modifyPos - terrainPos; // Modification position relative to this terrain

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        // Calculate the bounding box of the modification circle in this terrain's local space
        // Clamp to terrain bounds to find the intersecting area
        float minX = Mathf.Max(localModifyPos.x - modifyRadius, 0);
        float maxX = Mathf.Min(localModifyPos.x + modifyRadius, terrainSizeX);
        float minZ = Mathf.Max(localModifyPos.z - modifyRadius, 0);
        float maxZ = Mathf.Min(localModifyPos.z + modifyRadius, terrainSizeZ);

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

                // Distance from modification point to this pixel
                float distX = worldX - localModifyPos.x;
                float distZ = worldZ - localModifyPos.z;
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

        // Force update to visuals and collider
        terr.Flush();
    }
}

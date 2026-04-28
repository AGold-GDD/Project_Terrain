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

    [Header("Radius Settings - ADJUSTABLE RANGE")]
    [Range(1f, 10f)] public float minRadius = 0.5f;     // NEW: Minimum radius
    [Range(20f, 100f)] public float maxRadius = 50f;     // NEW: Maximum radius
    [Header("Add terrains here")]
    public Terrain[] terrains;  // Array of all terrains (assign in Inspector)
    public float raiseAmountPerSecond = 0.10f;  // How much to raise the terrain per second
    public float modifyRadius = 25f;  // Radius around modification point to modify (starts in middle)
    public float radiusChangeSpeed = 2f;  // How much the radius changes per scroll unit
    public Slider radiusSlider;  // Assign the UI Slider in Inspector

    [Header("Flatten Mode (Middle Mouse)")]
    public float flattenCostPerSecond = 20f;  // Higher cost for flatten
    public float flattenTolerance = 0.01f;  // How close heights must be to target (smaller = flatter)

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

        // SETUP SLIDER WITH NEW MIN/MAX RANGE
        SetupRadiusSlider();

        // Clamp initial radius to new bounds
        modifyRadius = Mathf.Clamp(modifyRadius, minRadius, maxRadius);
    }

    // NEW: Setup slider with custom min/max
    void SetupRadiusSlider()
    {
        if (radiusSlider != null)
        {
            radiusSlider.minValue = minRadius;
            radiusSlider.maxValue = maxRadius;
            radiusSlider.value = modifyRadius;
            Debug.Log($"Radius slider set: {minRadius:F1} - {maxRadius:F1}, current: {modifyRadius:F1}");
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
        // ADJUST RADIUS WITH MOUSE WHEEL (NEW BIGGER RANGE)
        if (Input.mouseScrollDelta.y != 0)
        {
            modifyRadius += Input.mouseScrollDelta.y * radiusChangeSpeed;
            modifyRadius = Mathf.Clamp(modifyRadius, minRadius, maxRadius);

            // Update slider to reflect new radius
            if (radiusSlider != null)
            {
                radiusSlider.value = modifyRadius;
            }

            Debug.Log($"Radius: {modifyRadius:F1}");
        }

        // LEFT MOUSE: Raise terrain
        if (Input.GetMouseButton(0))
        {
            float costThisFrame = abilityCostPerSecond * Time.deltaTime;
            if (abilityController.UseAbility(costThisFrame))
            {
                ModifyTerrainHeight(raiseAmountPerSecond * Time.deltaTime);
            }
        }
        // RIGHT MOUSE: Lower terrain  
        else if (Input.GetMouseButton(1))
        {
            float costThisFrame = abilityCostPerSecond * Time.deltaTime;
            if (abilityController.UseAbility(costThisFrame))
            {
                ModifyTerrainHeight(-raiseAmountPerSecond * Time.deltaTime);
            }
        }
        // MIDDLE MOUSE: FLATTEN TO LASER HEIGHT (NEW FEATURE!)
        else if (Input.GetMouseButton(2)) // Middle mouse button
        {
            float costThisFrame = flattenCostPerSecond * Time.deltaTime;
            if (abilityController.UseAbility(costThisFrame))
            {
                FlattenTerrainToLaserHeight();
            }
        }
    }

    // NEW: Flatten terrain to exact laser hit height (like Unity Editor)
    private void FlattenTerrainToLaserHeight()
    {
        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxLaserDistance))
        {
            float targetHeight = hit.point.y; // Laser hit Y = target flatten height

            foreach (Terrain terr in terrains)
            {
                FlattenSingleTerrainToHeight(terr, targetHeight);
            }

            Debug.Log($"Flattening to height: {targetHeight:F2}");
        }
    }

    // NEW: Flatten single terrain to exact target height
    private void FlattenSingleTerrainToHeight(Terrain terr, float targetWorldHeight)
    {
        TerrainData terrainData = terr.terrainData;
        Vector3 terrainPos = terr.transform.position;
        float terrainHeight = terrainData.size.y;

        int heightmapWidth = terrainData.heightmapResolution;
        int heightmapHeight = terrainData.heightmapResolution;
        float terrainSizeX = terrainData.size.x;
        float terrainSizeZ = terrainData.size.z;

        // Calculate modification bounds
        Vector3 centerWorldPos = muzzlePoint.position + muzzlePoint.forward * maxLaserDistance * 0.5f;
        Vector3 localCenter = centerWorldPos - terrainPos;

        float minX = Mathf.Max(localCenter.x - modifyRadius, 0);
        float maxX = Mathf.Min(localCenter.x + modifyRadius, terrainSizeX);
        float minZ = Mathf.Max(localCenter.z - modifyRadius, 0);
        float maxZ = Mathf.Min(localCenter.z + modifyRadius, terrainSizeZ);

        if (minX >= maxX || minZ >= maxZ) return;

        int startX = Mathf.RoundToInt(minX / terrainSizeX * (heightmapWidth - 1));
        int endX = Mathf.RoundToInt(maxX / terrainSizeX * (heightmapWidth - 1));
        int startZ = Mathf.RoundToInt(minZ / terrainSizeZ * (heightmapHeight - 1));
        int endZ = Mathf.RoundToInt(maxZ / terrainSizeZ * (heightmapHeight - 1));

        startX = Mathf.Clamp(startX, 0, heightmapWidth - 1);
        endX = Mathf.Clamp(endX, 0, heightmapWidth - 1);
        startZ = Mathf.Clamp(startZ, 0, heightmapHeight - 1);
        endZ = Mathf.Clamp(endZ, 0, heightmapHeight - 1);

        int width = endX - startX + 1;
        int height = endZ - startZ + 1;

        if (width <= 0 || height <= 0) return;

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);
        float targetNormalizedHeight = (targetWorldHeight - terrainPos.y) / terrainHeight;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float worldX = (startX + x) / (float)(heightmapWidth - 1) * terrainSizeX;
                float worldZ = (startZ + z) / (float)(heightmapHeight - 1) * terrainSizeZ;

                float distX = worldX - localCenter.x;
                float distZ = worldZ - localCenter.z;
                float distance = Mathf.Sqrt(distX * distX + distZ * distZ);

                if (distance <= modifyRadius)
                {
                    float normalizedDistance = distance / modifyRadius;
                    float falloff = 0.9f * (1f + Mathf.Cos(normalizedDistance * Mathf.PI));

                    // Flatten to exact target height with falloff
                    heights[z, x] = Mathf.Lerp(heights[z, x], targetNormalizedHeight, falloff);
                    heights[z, x] = Mathf.Clamp01(heights[z, x]);
                }
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
        terr.Flush();
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
        float minX = Mathf.Max(localModifyPos.x - modifyRadius, 0);
        float maxX = Mathf.Min(localModifyPos.x + modifyRadius, terrainSizeX);
        float minZ = Mathf.Max(localModifyPos.z - modifyRadius, 0);
        float maxZ = Mathf.Min(localModifyPos.z + modifyRadius, terrainSizeZ);

        if (minX >= maxX || minZ >= maxZ) return;

        int startX = Mathf.RoundToInt(minX / terrainSizeX * (heightmapWidth - 1));
        int endX = Mathf.RoundToInt(maxX / terrainSizeX * (heightmapWidth - 1));
        int startZ = Mathf.RoundToInt(minZ / terrainSizeZ * (heightmapHeight - 1));
        int endZ = Mathf.RoundToInt(maxZ / terrainSizeZ * (heightmapHeight - 1));

        startX = Mathf.Clamp(startX, 0, heightmapWidth - 1);
        endX = Mathf.Clamp(endX, 0, heightmapWidth - 1);
        startZ = Mathf.Clamp(startZ, 0, heightmapHeight - 1);
        endZ = Mathf.Clamp(endZ, 0, heightmapHeight - 1);

        int width = endX - startX + 1;
        int height = endZ - startZ + 1;

        if (width <= 0 || height <= 0) return;

        float[,] heights = terrainData.GetHeights(startX, startZ, width, height);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float worldX = (startX + x) / (float)(heightmapWidth - 1) * terrainSizeX;
                float worldZ = (startZ + z) / (float)(heightmapHeight - 1) * terrainSizeZ;

                float distX = worldX - localModifyPos.x;
                float distZ = worldZ - localModifyPos.z;
                float distance = Mathf.Sqrt(distX * distX + distZ * distZ);

                if (distance <= modifyRadius)
                {
                    float normalizedDistance = distance / modifyRadius;
                    float falloff = 0.9f * (1f + Mathf.Cos(normalizedDistance * Mathf.PI));

                    heights[z, x] += amount * falloff;
                    heights[z, x] = Mathf.Clamp01(heights[z, x]);
                }
            }
        }

        terrainData.SetHeights(startX, startZ, heights);
        terr.Flush();
    }

    // NEW: Public method to change radius range from Inspector (hot-reload support)
    public void SetRadiusRange(float newMin, float newMax)
    {
        minRadius = newMin;
        maxRadius = newMax;
        SetupRadiusSlider();
        modifyRadius = Mathf.Clamp(modifyRadius, minRadius, maxRadius);
    }
}
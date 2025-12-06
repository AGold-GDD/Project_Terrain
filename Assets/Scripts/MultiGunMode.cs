using UnityEngine;
using UnityEngine.SceneManagement;  // For scene checks
using UnityEngine.UI;  // For Slider

public class MultiGunMode : MonoBehaviour
{
    [Header("Materials for Modes")]
    public Material mode1Material;  // Assign material for Mode 1 (e.g., green texture for terrain mod)
    public Material mode2Material;  // Assign material for Mode 2 (e.g., blue texture for attract/repel)
    public Material mode3Material;  // Assign material for Mode 3 (e.g., red texture for sphere shooting)

    [Header("Laser Settings")]
    public Transform muzzlePoint;  // Assign the "Muzzle" child Transform here
    public LineRenderer laserRenderer;  // Assign the LineRenderer component here
    public LayerMask laserLayerMaskMode1 = -1;  // Layers for laser in Mode 1 (include terrain)
    public LayerMask laserLayerMaskMode23 = -1;  // Layers for laser in Modes 2/3 (exclude terrain)
    public float maxLaserDistance = 100f;  // Max length of the laser

    [Header("Mode 1 Settings (Terrain Modification)")]
    public TerrainAbilityController abilityController;  // From TerrainModifier
    public float abilityCostPerSecond = 15f;  // Cost per second while holding key
    public Terrain[] terrains;  // Array of all terrains (assign in Inspector)
    public float raiseAmountPerSecond = 0.01f;  // How much to raise the terrain per second
    public float modifyRadius = 3f;  // Radius around modification point to modify
    public float radiusChangeSpeed = 1f;  // How much the radius changes per scroll unit
    public Slider radiusSlider;  // Assign the UI Slider in Inspector (read-only reflection)

    [Header("Mode 2 Settings (Attract/Repel)")]
    public LayerMask attractRepelLayerMask = -1;  // Layers for attract/repel raycasts (exclude terrain)
    public float attractRepelRange = 50f;  // Max range for attract/repel
    public float attractForce = 10f;  // Strength of attract force
    public float repelForce = 10f;  // Strength of repel force

    [Header("Mode 3 Settings (Sphere Shooting)")]
    public LayerMask sphereShootingLayerMask = -1;  // Layers for sphere shooting laser (exclude terrain)
    public GameObject spherePrefab;  // Assign a sphere prefab (with Rigidbody) here
    public float shootForce = 20f;  // Force to apply to shot spheres
    public float sphereSpreadAngle = 10f;  // Angle spread for 3 spheres (in degrees)

    private MeshRenderer gunRenderer;
    private int currentMode = 1;  // Start in Mode 1 (terrain modification)
    private Material[] originalMaterials;
    private bool isInOpenWorld = false;  // Flag for scene check

    void Start()
    {
        gunRenderer = GetComponentInChildren<MeshRenderer>();

        if (gunRenderer == null)
        {
            Debug.LogError("ChangeGunMode: No MeshRenderer found on this GameObject or its children!");
            return;
        }

        Debug.Log($"ChangeGunMode: Found MeshRenderer on '{gunRenderer.name}'.");

        originalMaterials = gunRenderer.materials;

        // Check if in "OpenWorld" scene
        /*
        isInOpenWorld = SceneManager.GetActiveScene().name == "OpenWorld";
        if (!isInOpenWorld)
        {
            Debug.Log("ChangeGunMode: Not in 'OpenWorld' scene. Modes disabled.");
            SetMode(1);  // Neutral mode
            return;
        }
        */

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
        SetMode(1);  // Start in Mode 1

        // Initialize slider for terrain radius (from TerrainModifier)
        if (radiusSlider != null)
        {
            radiusSlider.minValue = 1f;
            radiusSlider.maxValue = 10f;
            radiusSlider.value = modifyRadius;
            // Slider is read-only; no event listener needed
        }
    }

    void Update()
    {
        /*
        if (!isInOpenWorld || !isCartMini) return;  // Only process if in "OpenWorld"
        */

        // Mode switching inputs
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetMode(1);  // Mode 1: Terrain Modification
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetMode(2);  // Mode 2: Attract/Repel
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetMode(3);  // Mode 3: Sphere Shooting
        }

        // Update laser only in active mode
        if (currentMode >= 1 && currentMode <= 3)
        {
            UpdateLaser();
        }

        // Handle mode-specific actions
        if (currentMode == 1)
        {
            HandleTerrainModification();
        }
        else if (currentMode == 2)
        {
            HandleAttractRepel();
        }
        else if (currentMode == 3)
        {
            HandleSphereShooting();
        }
    }

    private void SetMode(int mode)
    {
        currentMode = mode;

        if (mode == 0)  // Neutral
        {
            ApplyMaterial(originalMaterials[0]);  // Revert to original material
            laserRenderer.enabled = false;
            Debug.Log("Switched to Neutral Mode (No special abilities)");
        }
        else if (mode == 1)  // Mode 1: Terrain Modification
        {
            ApplyMaterial(mode1Material);
            laserRenderer.enabled = true;
            laserRenderer.startColor = Color.green;
            laserRenderer.endColor = Color.green;
            Debug.Log("Switched to Mode 1 (Terrain Modification - Green Laser)");
        }
        else if (mode == 2)  // Mode 2: Attract/Repel
        {
            ApplyMaterial(mode2Material);
            laserRenderer.enabled = true;
            laserRenderer.startColor = Color.blue;
            laserRenderer.endColor = Color.blue;
            Debug.Log("Switched to Mode 2 (Attract/Repel - Blue Laser)");
        }
        else if (mode == 3)  // Mode 3: Sphere Shooting
        {
            ApplyMaterial(mode3Material);
            laserRenderer.enabled = true;
            laserRenderer.startColor = Color.red;
            laserRenderer.endColor = Color.red;
            Debug.Log("Switched to Mode 3 (Sphere Shooting - Red Laser)");
        }
    }

    private void UpdateLaser()
    {
        if (muzzlePoint == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

        // Choose layer mask based on mode
        LayerMask mask = (currentMode == 1) ? laserLayerMaskMode1 : (currentMode == 2 ? attractRepelLayerMask : sphereShootingLayerMask);

        // Raycast to find hit point
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, maxLaserDistance, mask))
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

    private void HandleAttractRepel()
    {
        if (muzzlePoint == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

        // Left-click: Attract
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(startPos, direction, out RaycastHit hit, attractRepelRange, attractRepelLayerMask))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDirection = (transform.position - hit.point).normalized;  // Toward player
                    rb.AddForce(forceDirection * attractForce, ForceMode.Impulse);
                    Debug.Log($"Attracted object at {hit.point}");
                }
            }
        }

        // Right-click: Repel
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(startPos, direction, out RaycastHit hit, attractRepelRange, attractRepelLayerMask))
            {
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDirection = (hit.point - transform.position).normalized;  // Away from player
                    rb.AddForce(forceDirection * repelForce, ForceMode.Impulse);
                    Debug.Log($"Repelled object at {hit.point}");
                }
            }
        }
    }

    private void HandleSphereShooting()
    {
        if (muzzlePoint == null || spherePrefab == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

        // Left-click: Shoot 1 sphere
        if (Input.GetMouseButtonDown(0))
        {
            GameObject sphere = Instantiate(spherePrefab, startPos, Quaternion.identity);
            Rigidbody rb = sphere.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(direction * shootForce, ForceMode.Impulse);
            }
            Debug.Log("Shot 1 sphere");
        }

        // Right-click: Shoot 3 spheres in a spread
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = -1; i <= 1; i++)  // -1, 0, 1 for left, center, right
            {
                Quaternion spreadRotation = Quaternion.Euler(0, i * sphereSpreadAngle, 0);
                Vector3 spreadDirection = spreadRotation * direction;

                GameObject sphere = Instantiate(spherePrefab, startPos, Quaternion.identity);
                Rigidbody rb = sphere.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(spreadDirection * shootForce, ForceMode.Impulse);
                }
            }
            Debug.Log("Shot 3 spheres");
        }
    }

    private void ApplyMaterial(Material newMaterial)
    {
        if (gunRenderer == null) return;

        Material[] newMaterials = new Material[originalMaterials.Length];
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            newMaterials[i] = new Material(newMaterial);
        }
        gunRenderer.materials = newMaterials;
    }

    // Public method to check if in terrain mode (for external scripts if needed)
    public bool IsTerrainMode()
    {
        return currentMode == 1;
    }
}

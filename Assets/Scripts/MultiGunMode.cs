using UnityEngine;
using UnityEngine.SceneManagement;  // For scene checks

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
    public float terrainModifyRadius = 5f;  // Radius of terrain modification area
    public float terrainModifyStrength = 0.1f;  // Strength of raise/lower (positive = raise, negative = lower)

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
        isInOpenWorld = SceneManager.GetActiveScene().name == "OpenWorld";
        if (!isInOpenWorld)
        {
            Debug.Log("ChangeGunMode: Not in 'OpenWorld' scene. Modes disabled.");
            SetMode(0);  // Neutral mode
            return;
        }

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
    }

    void Update()
    {
        if (!isInOpenWorld) return;  // Only process if in "OpenWorld"

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
        if (muzzlePoint == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;

        // Left-click: Raise terrain
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(startPos, direction, out RaycastHit hit, maxLaserDistance, laserLayerMaskMode1))
            {
                ModifyTerrain(hit.point, terrainModifyStrength);  // Positive strength = raise
                Debug.Log($"Raised terrain at {hit.point}");
            }
        }

        // Right-click: Lower terrain
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(startPos, direction, out RaycastHit hit, maxLaserDistance, laserLayerMaskMode1))
            {
                ModifyTerrain(hit.point, -terrainModifyStrength);  // Negative strength = lower
                Debug.Log($"Lowered terrain at {hit.point}");
            }
        }
    }

    private void ModifyTerrain(Vector3 worldPoint, float strength)
    {
        // Find the Terrain component at the hit point
        Terrain terrain = Terrain.activeTerrain;  // Assumes one active terrain; adjust if multiple
        if (terrain == null)
        {
            Debug.LogWarning("ChangeGunMode: No active Terrain found for modification!");
            return;
        }

        // Convert world point to terrain local coordinates
        Vector3 terrainLocalPos = worldPoint - terrain.transform.position;
        int mapX = Mathf.RoundToInt((terrainLocalPos.x / terrain.terrainData.size.x) * terrain.terrainData.heightmapResolution);
        int mapZ = Mathf.RoundToInt((terrainLocalPos.z / terrain.terrainData.size.z) * terrain.terrainData.heightmapResolution);

        // Get current heights
        int radiusInPixels = Mathf.RoundToInt(terrainModifyRadius / (terrain.terrainData.size.x / terrain.terrainData.heightmapResolution));
        int width = terrain.terrainData.heightmapResolution;
        int height = terrain.terrainData.heightmapResolution;
        float[,] heights = terrain.terrainData.GetHeights(0, 0, width, height);

        // Modify heights in a circular area
        for (int x = Mathf.Max(0, mapX - radiusInPixels); x < Mathf.Min(width, mapX + radiusInPixels); x++)
        {
            for (int z = Mathf.Max(0, mapZ - radiusInPixels); z < Mathf.Min(height, mapZ + radiusInPixels); z++)
            {
                float distance = Vector2.Distance(new Vector2(x, z), new Vector2(mapX, mapZ));
                if (distance <= radiusInPixels)
                {
                    // Apply strength with falloff (stronger at center)
                    float falloff = 1 - (distance / radiusInPixels);
                    heights[z, x] += strength * falloff;  // Note: z,x order for heightmap
                }
            }
        }

        // Apply modified heights
        terrain.terrainData.SetHeights(0, 0, heights);
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
}
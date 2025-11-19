using UnityEngine;

public class ChangeGunMode : MonoBehaviour
{
    [Header("Materials for Modes")]
    public Material mode1Material;  // Assign your first material here (e.g., single shot mode)
    public Material mode2Material;  // Assign your second material here (e.g., burst mode)

    [Header("Laser Settings (for Mode 2)")]
    public Transform muzzlePoint;  // Assign the "Muzzle" child Transform here
    public LineRenderer laserRenderer;  // Assign the LineRenderer component here
    public LayerMask terrainLayerMask = -1;  // Layers to raycast against (default: all)
    public float maxLaserDistance = 100f;  // Max length of the laser
    public Color laserColor = Color.green;  // Green for mode2

    private MeshRenderer gunRenderer;
    private int currentMode = 2;  // Changed: Start in mode 2
    private Material[] originalMaterials;

    void Start()
    {
        gunRenderer = GetComponentInChildren<MeshRenderer>();

        if (gunRenderer == null)
        {
            Debug.LogError("GunModeSwitcher: No MeshRenderer found on this GameObject or its children!");
            return;
        }

        Debug.Log($"GunModeSwitcher: Found MeshRenderer on '{gunRenderer.name}'.");

        originalMaterials = gunRenderer.materials;
        ApplyMaterial(mode2Material);  // Changed: Apply mode 2 material at start

        // Laser setup: Ensure references are valid
        if (muzzlePoint == null)
        {
            // Auto-find "Muzzle" child if not assigned
            muzzlePoint = transform.Find("Muzzle");
            if (muzzlePoint == null)
            {
                Debug.LogWarning("GunModeSwitcher: No 'Muzzle' child found. Create one at the gun barrel.");
            }
        }

        if (laserRenderer == null)
        {
            laserRenderer = GetComponent<LineRenderer>();
            if (laserRenderer == null)
            {
                Debug.LogError("GunModeSwitcher: No LineRenderer found on this GameObject! Add one.");
                return;
            }
        }

        // Initial state: Enable laser for mode 2  // Changed: Enable laser at start
        laserRenderer.enabled = true;
        laserRenderer.startColor = laserColor;
        laserRenderer.endColor = laserColor;
        laserRenderer.positionCount = 2;  // Line from start to end point
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ToggleMode();
        }

        // Update laser only in mode 2
        if (currentMode == 2 && laserRenderer.enabled)
        {
            UpdateLaser();
        }
    }

    private void ToggleMode()
    {
        currentMode = (currentMode == 1) ? 2 : 1;

        if (currentMode == 1)
        {
            ApplyMaterial(mode1Material);
            laserRenderer.enabled = false;  // Turn off laser
            Debug.Log("Switched to Mode 1 (e.g., Single Shot)");
        }
        else
        {
            ApplyMaterial(mode2Material);
            laserRenderer.enabled = true;  // Turn on laser
            Debug.Log("Switched to Mode 2 (e.g., Terrain Mode - Laser Active)");
        }
    }

    private void UpdateLaser()
    {
        if (muzzlePoint == null) return;

        Vector3 startPos = muzzlePoint.position;
        Vector3 direction = muzzlePoint.forward;  // Or transform.forward if no muzzle

        // Raycast to find terrain hit point
        if (Physics.Raycast(startPos, direction, out RaycastHit hit, maxLaserDistance, terrainLayerMask))
        {
            // Hit something (e.g., terrain)
            Vector3 endPos = hit.point;

            // Optional: Visualize raycast in Scene view (remove in builds)
            Debug.DrawRay(startPos, direction * hit.distance, Color.red);

            // Set LineRenderer positions
            laserRenderer.SetPosition(0, startPos);
            laserRenderer.SetPosition(1, endPos);

            // Optional: Here, you could trigger terrain raise/lower on input (e.g., mouse click)
            // Example: if (Input.GetMouseButtonDown(0)) { ModifyTerrain(hit.point, hit.normal); }
        }
        else
        {
            // No hit: Extend laser to max distance
            Vector3 endPos = startPos + direction * maxLaserDistance;
            laserRenderer.SetPosition(0, startPos);
            laserRenderer.SetPosition(1, endPos);
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

    // Optional: Placeholder for actual terrain modification (expand as needed)
    /*
    private void ModifyTerrain(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Example for Unity Terrain: Get Terrain component and use SetHeights
        Terrain terrain = hitPoint.GetComponentInParent<Terrain>();
        if (terrain != null)
        {
            // Logic to raise/lower heights at hitPoint (requires TerrainData access)
            Debug.Log($"Would modify terrain at {hitPoint}");
        }
    }
    */
}
using UnityEngine;

public class MOUSE_TerrainController : MonoBehaviour
{

    public Camera mainCamera;          // Assign your main camera here
    public LayerMask terrainLayerMask; // Layer mask to detect terrain only

    void Update()
    {
        // Raycast from mouse position into the scene
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, terrainLayerMask))
        {
            // Move the circle to the hit point on terrain
            Vector3 newPos = hit.point;

            // Optional: keep circle slightly above terrain to avoid z-fighting
            newPos.y += 0.05f;

            transform.position = newPos;
        }
    }
}


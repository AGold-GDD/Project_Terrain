using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class OutlineEffect : MonoBehaviour
{
    public Color outlineColor = Color.yellow;
    public float outlineWidth = 1.05f;  // Scale multiplier for outline

    private GameObject outlineObject;

    void Start()
    {
        // Create duplicate GameObject for outline
        outlineObject = new GameObject("Outline");
        outlineObject.transform.SetParent(transform);
        outlineObject.transform.localPosition = Vector3.zero;
        outlineObject.transform.localRotation = Quaternion.identity;
        outlineObject.transform.localScale = Vector3.one * outlineWidth;

        // Copy mesh from original
        MeshFilter originalMeshFilter = GetComponent<MeshFilter>();
        MeshFilter outlineMeshFilter = outlineObject.AddComponent<MeshFilter>();
        outlineMeshFilter.mesh = originalMeshFilter.mesh;

        // Add MeshRenderer and assign outline material
        MeshRenderer outlineRenderer = outlineObject.AddComponent<MeshRenderer>();

        // Create a new material instance with the outline color
        Material outlineMat = new Material(Shader.Find("Unlit/Color"));
        outlineMat.color = outlineColor;

        // Set render queue to render on top
        outlineMat.renderQueue = 3000;

        outlineRenderer.material = outlineMat;

        // Optional: disable shadows on outline
        outlineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        outlineRenderer.receiveShadows = false;
    }

    void Update()
    {
        // Keep outlineObject in sync with parent scale (if parent scales dynamically)
        outlineObject.transform.localScale = Vector3.one * outlineWidth;
    }
}

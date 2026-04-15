using UnityEngine;

public partial class TerrainLayerDetector : MonoBehaviour
{
    public NO_JETPACK_SimpleCharacterController characterController;
    //public JETPACK_SimpleCharacterController characterController;
    public LayerMask terrainLayer; // Assign the "Terrain" layer in the inspector

    void Update()
    {
        int layerIndex = GetCurrentTerrainLayer();

        // Layer Index 2 = Icy, Index 1 = Bouncy (as per your original logic)
        if (layerIndex == 2 && characterController.isGrounded)
        {
            characterController.speed = 24;
            characterController.jumpForce = 18;
        }
        else if (layerIndex != 2 && characterController.isGrounded && layerIndex != 1)
        {
            characterController.speed = 12;
            characterController.jumpForce = 15;
        }
    }

    private void FixedUpdate()
    {
        if (GetCurrentTerrainLayer() == 1 && characterController.isGrounded)
        {
            characterController.rb.AddForce(Vector3.up * 30f, ForceMode.Impulse);
        }
    }

    int GetCurrentTerrainLayer()
    {
        RaycastHit hit;
        // Cast a ray downward from the player's position
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out hit, 2f, terrainLayer))
        {
            Terrain terrain = hit.collider.GetComponent<Terrain>();
            if (terrain == null) return -1;

            TerrainData tData = terrain.terrainData;
            Vector3 terrainPos = hit.point - terrain.transform.position;

            // Convert hit point to AlphaMap coordinates
            int mapX = (int)((terrainPos.x / tData.size.x) * tData.alphamapWidth);
            int mapZ = (int)((terrainPos.z / tData.size.z) * tData.alphamapHeight);

            // Clamp coordinates to prevent "Invalid Argument" errors if on the very edge
            mapX = Mathf.Clamp(mapX, 0, tData.alphamapWidth - 1);
            mapZ = Mathf.Clamp(mapZ, 0, tData.alphamapHeight - 1);

            float[,,] alpha = tData.GetAlphamaps(mapX, mapZ, 1, 1);

            // Loop through layers to find the one with the most weight
            for (int i = 0; i < tData.alphamapLayers; i++)
            {
                if (alpha[0, 0, i] > 0.5f) return i;
            }
        }
        return -1;
    }
}
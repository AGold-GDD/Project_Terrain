using UnityEngine;

public class TerrainLayerDetector : MonoBehaviour
{
    public SimpleCharacterController characterController;

    // Update is called once per frame
    void Update()
    {
        if (IsOnBouncyPaint() && characterController.isGrounded)
        {
            characterController.rb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
        }
        else if (IsOnIcyPaint() && characterController.isGrounded) 
        {
            characterController.rb.AddForce(Vector3.up * 1.5f, ForceMode.Impulse);
        }
    }

    bool IsOnBouncyPaint()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData tData = terrain.terrainData;

        // Makes the  Player position to AlphaMap coordinates
        Vector3 terrainPos = transform.position - terrain.transform.position;
        int mapX = (int)((terrainPos.x / tData.size.x) * tData.alphamapWidth);
        int mapZ = (int)((terrainPos.z / tData.size.z) * tData.alphamapHeight);

        // Get the weights of all layers at this exact spot
        float[,,] alpha = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        // Check if the Layer (Index 1) has a high weight
        return alpha[0, 0, 1] > 0.5f;
    }

    bool IsOnIcyPaint()
    {
        Terrain terrain = Terrain.activeTerrain;
        TerrainData tData = terrain.terrainData;

        // Makes the  Player position to AlphaMap coordinates
        Vector3 terrainPos = transform.position - terrain.transform.position;
        int mapX = (int)((terrainPos.x / tData.size.x) * tData.alphamapWidth);
        int mapZ = (int)((terrainPos.z / tData.size.z) * tData.alphamapHeight);

        // Get the weights of all layers at this exact spot
        float[,,] alpha = tData.GetAlphamaps(mapX, mapZ, 1, 1);

        // Check if the Layer (Index 1) has a high weight
        return alpha[0, 0, 2] > 0.5f;
    }
}

using UnityEngine;

public class ObjectTerrainLayerDetect : MonoBehaviour
{
    private Rigidbody rb;
    private bool isGrounded;
    public int PushForce;
    public Vector3 objectVelocity; // The velocity vector
    public float objectSpeed;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {

    }
    private void FixedUpdate()
    {
        // Access the velocity in the Update method (or FixedUpdate for physics updates)
        objectVelocity = rb.linearVelocity;

        if (IsOnBouncyPaint() && isGrounded)
        {
            rb.AddForce(Vector3.up * PushForce, ForceMode.Impulse);
        }
        else if (IsOnIcyPaint() && isGrounded) 
        {
            objectVelocity = objectVelocity * 2;
        }
        //Debug.Log(objectVelocity);
        //Debug.Log(objectSpeed);

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


    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

using UnityEngine;

public class TerrainLayerPainter : MonoBehaviour
{
    public Terrain terrain;
    public int layerToPaint = 1; // 0 is usually your base, 1 is your red bouncy layer
    public int brushSize = 5;
    public Transform muzzlePoint;  // Assign the "Muzzle" child Transform here

    //testing paint limit
    //public int MaxPaint = 100;
    public float PaintAmount = 100;
    public float PaintUsuage = .1f;

    public float CountDown;

    public PlayerPauseMenu pauseMenu;

    void Update()
    {
        //Debug.Log(PaintAmount);
        //Debug.Log(CountDown);

        if (pauseMenu.IsPaused == false)
        {
            // Left mouse click to paint
            if (Input.GetMouseButton(0))
            {
                //Debug.Log("Painting");
                GreenPaint();

                //CountDown = 500;
            }

            if (Input.GetMouseButton(1))
            {
                RedPaint();
            }
        }

        // Takes care of the refill of the paint
        if (CountDown <= 0 && PaintAmount < 99)
        {
            //Debug.Log("Refilling");
            PaintAmount = PaintAmount + 0.05f;

        }
        // Cooldown
        --CountDown;
    }

    void GreenPaint()
    {
        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 1. Convert World Position to Terrain Map Coordinates
            TerrainData tData = terrain.terrainData;
            int mapX = (int)(((hit.point.x - terrain.transform.position.x) / tData.size.x) * tData.alphamapWidth);
            int mapZ = (int)(((hit.point.z - terrain.transform.position.z) / tData.size.z) * tData.alphamapHeight);

            // 2. Grab the current alphamap section
            float[,,] maps = tData.GetAlphamaps(mapX, mapZ, brushSize, brushSize);

            // 3. Loop through the brush area and set the Red layer to 1 (full)
            for (int y = 0; y < brushSize; y++)
            {
                for (int x = 0; x < brushSize; x++)
                {
                    maps[x, y, 0] = 0; // Turn off Blue
                    maps[x, y, 1] = 1; // Turn on Red
                    maps[x, y, 2] = 0; // Turn on Red
                }
            }

            // 4. Apply changes back to the terrain
            tData.SetAlphamaps(mapX, mapZ, maps);

            // 5. As the player paints, it uses paint (wip) to paint the terrain
            PaintAmount = PaintAmount - PaintUsuage;
        }
    }

    void RedPaint()
    {
        Ray ray = new Ray(muzzlePoint.position, muzzlePoint.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 1. Convert World Position to Terrain Map Coordinates
            TerrainData tData = terrain.terrainData;
            int mapX = (int)(((hit.point.x - terrain.transform.position.x) / tData.size.x) * tData.alphamapWidth);
            int mapZ = (int)(((hit.point.z - terrain.transform.position.z) / tData.size.z) * tData.alphamapHeight);

            // 2. Grab the current alphamap section
            float[,,] maps = tData.GetAlphamaps(mapX, mapZ, brushSize, brushSize);

            // 3. Loop through the brush area and set the Red layer to 1 (full)
            for (int y = 0; y < brushSize; y++)
            {
                for (int x = 0; x < brushSize; x++)
                {
                    maps[x, y, 0] = 0; // Turn off Blue
                    maps[x, y, 1] = 0; // Turn on Red
                    maps[x, y, 2] = 1; // Turn on Red
                }
            }

            // 4. Apply changes back to the terrain
            tData.SetAlphamaps(mapX, mapZ, maps);

            // 5. As the player paints, it uses paint (wip) to paint the terrain
            PaintAmount = PaintAmount - PaintUsuage;
        }
    }
}
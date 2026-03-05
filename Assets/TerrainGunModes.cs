using UnityEngine;

public class TerrainGunModes : MonoBehaviour
{
    public TerrainMode terrain;
    public TerrainLayerPainter painter;

    void Start()
    {
        terrain.enabled = true;
        painter.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            terrain.enabled = true;
            painter.enabled = false;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            terrain.enabled = false;
            painter.enabled = true;
        }
    }
}

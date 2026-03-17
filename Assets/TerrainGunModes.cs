using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGunModes : MonoBehaviour
{
    public TerrainMode terrain;
    public TerrainLayerPainter painter;
    public PlayerPauseMenu pauseMenu;

    //temporary use
    public TrackManager trackManager;

    public bool TerDisabled;
    public bool PanDisabled;

    void Start()
    {
  
        if (SceneManager.GetActiveScene().name == "ParkourMinigame")
        {
            terrain.enabled = false;
            painter.enabled = false;
            //pauseMenu.TerrainUI.active = false;
        } else
        {
            terrain.enabled = true;
            painter.enabled = false;
        }


        TerDisabled = false;
        PanDisabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !TerDisabled)
        {
            TerrainMode();
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !PanDisabled)
        {
            PaintMode();
        }

        if (trackManager.nextCheckpoint == 2)
        {
            painter.enabled = true;
        }
    }

    public void TerrainMode()
    {
        terrain.enabled = true;
        painter.enabled = false;
    }

    public void PaintMode()
    {
        terrain.enabled = false;
        painter.enabled = true;
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class TerrainGunModes : MonoBehaviour
{
    public TerrainMode terrain;
    public TerrainLayerPainter painter;
    //public PlayerPauseMenu pauseMenu;

    //temporary use
    //public TrackManager trackManager;

    public PlayerUIFunction ui;

    public bool TerDisabled;
    public bool PanDisabled;

    void Start()
    {
  
        if (SceneManager.GetActiveScene().name == "ParkourMinigame")
        {
            NoMode();
            Debug.Log("no mode");
            //pauseMenu.TerrainUI.active = false;
        } else
        {
            TerrainMode();
            Debug.Log("terrain mode");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && !TerDisabled)
        {
            Debug.Log("Terrain Mode On");
            TerrainMode();
            
        } 
        else if (Input.GetKeyDown(KeyCode.Alpha2) && !PanDisabled)
        {
            Debug.Log("Paint Mode On");
            PaintMode();
            
        }

    }

    public void TerrainMode()
    {
        terrain.enabled = true;
        painter.enabled = false;
        ui.TerrainModeActive();
    }

    public void PaintMode()
    {
        terrain.enabled = false;
        painter.enabled = true;
        ui.PaintModeActive();
    }

    public void NoMode()
    {
        terrain.enabled = false;
        painter.enabled = false;
        TerDisabled = true;
        PanDisabled = true;
        ui.NoModeActive();
    }
}

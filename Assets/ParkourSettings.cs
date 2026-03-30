using UnityEngine;

public class ParkourSettings : MonoBehaviour
{
    public TerrainGunModes gunModes;
    public TrackManager trackManager;

    //public TerrainLayerPainter painter;
    public PlayerUIFunction ui;

    public void Update()
    {
        if (trackManager.nextCheckpoint == 2)
        {
            //painter.enabled = true
            gunModes.PaintMode();
            ui.PaintModeActive();
        }
    }
}

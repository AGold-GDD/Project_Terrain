using UnityEngine;

public class ParkourSettings : MonoBehaviour
{
    public TerrainGunModes gunModes;
    public TrackManager trackManager;

    public void Awake()
    {
        //gunModes = GetComponent<TerrainGunModes>();
        gunModes.TerDisabled = true;
        //gunModes.PaintMode();
        Debug.Log("did it");
    }



}

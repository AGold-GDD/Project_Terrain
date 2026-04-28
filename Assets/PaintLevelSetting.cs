using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PaintLevelSetting : MonoBehaviour
{
    public GameObject player;

    public TrackManager manager;

    public GameManager gameManager;

    public TerrainLayerPainter painter;

    private int cleared = 0;

    public void Update()
    {

        if (manager.nextCheckpoint == 1 && cleared == 0)
        {
            player.transform.position = manager.PlayerRespawnPoint[1].transform.position;
            cleared = 1;
        }
        else if (manager.nextCheckpoint == 2 && cleared == 1)
        {
            player.transform.position = manager.PlayerRespawnPoint[2].transform.position;
            cleared = 2;
        }

        if (gameManager.currentTime <= 0.1)
        {
            Debug.Log("reset");
            painter.ResetAllTerrains();
        }


    }
}

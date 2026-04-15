using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour
{
    public NO_JETPACK_SimpleCharacterController characterController;
    //public SimpleCharacterController characterController;
    public TerrainLayerPainter painter;

    public List<Transform> CheckPoints;
    public List<Transform> PlayerRespawnPoint;
    public int nextCheckpoint = 0;

    private string CurrentScene;

    public void Start()
    {
        CurrentScene = SceneManager.GetActiveScene().name;
    }
    public void Update()
    {
        characterController.PlayerSpawnPoint.transform.position = PlayerRespawnPoint[nextCheckpoint].position;
    }

    public void PassedCheckPoint(int checkpoint)
    {
        if (checkpoint == nextCheckpoint)
        {
            nextCheckpoint++;

            switch (CurrentScene)
            {
                case "PaintLevel":
                    if (nextCheckpoint == 3)
                    {
                        Debug.Log("game done");
                        SceneManager.LoadScene("NewMainLobby");
                        painter.ResetAllTerrains();
                    }
                    break;
                case "ParkourMinigame":
                    if (nextCheckpoint == 4)
                    {
                        Debug.Log("game done");
                        SceneManager.LoadScene("NewMainLobby");
                        painter.ResetAllTerrains();
                    }
                    break;
                default:
                    Debug.Log("terrain mode");
                    break;
            }

        }
    }
}

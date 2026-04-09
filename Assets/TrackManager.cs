using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour
{
    public NO_JETPACK_SimpleCharacterController characterController;

    public List<Transform> CheckPoints;
    public List<Transform> PlayerRespawnPoint;
    public int nextCheckpoint = 0;

    public void Update()
    {
        characterController.PlayerSpawnPoint.transform.position = PlayerRespawnPoint[nextCheckpoint].position;
    }

    public void PassedCheckPoint(int checkpoint)
    {
        if (checkpoint == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint == 3)
            {
                Debug.Log("game done");
                SceneManager.LoadScene("NewMainLobby");
            }
        }
    }
}

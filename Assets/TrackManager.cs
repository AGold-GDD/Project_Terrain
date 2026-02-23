using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackManager : MonoBehaviour
{

    public List<Transform> CheckPoints;
    public int nextCheckpoint = 0;

    public void PassedCheckPoint(int checkpoint)
    {
        if (checkpoint == nextCheckpoint)
        {
            nextCheckpoint++;

            if (nextCheckpoint > CheckPoints.Count)
            {
                Debug.Log("game done");
                SceneManager.LoadScene("NewMainLobby");
            }
        }

        
    }
}

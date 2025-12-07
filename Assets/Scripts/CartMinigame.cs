using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CartMinigame : MonoBehaviour
{
    public GameObject playerRespawnPoint;

    public Transform Player;

    [HideInInspector]
    public Transform currentResPoint;


    public void Update()
    {
        currentResPoint = playerRespawnPoint.transform;

        if (Input.GetKeyUp(KeyCode.T))
        {
            Player.position = currentResPoint.position;
        }
        /*
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainLobby");
        }
        */
    }
}

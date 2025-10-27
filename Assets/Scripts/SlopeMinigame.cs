using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlopeMinigame : MonoBehaviour
{


    public PlayerRespawner Respawner;
    public Transform PlayerrespawnPoint;

    public void Start()
    {
        Respawner.respawnPoint = PlayerrespawnPoint;
        //Timer.SetActive(false);
    }

}

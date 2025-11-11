using UnityEngine;

public class CheckPointCheck : MonoBehaviour
{
    public GameObject Respawner;

    //player respawn point
    public GameObject RSPoint;

    public CartMinigame cartmini;

    public bool CheckPoint = false;

    public void Start()
    {
        Respawner.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Rock" && !CheckPoint)
        {
            Debug.Log("check");
            Respawner.SetActive(true);
            CheckPoint = true;
            cartmini.playerRespawnPoint = RSPoint;
            this.gameObject.SetActive(false);
        }
    }
}

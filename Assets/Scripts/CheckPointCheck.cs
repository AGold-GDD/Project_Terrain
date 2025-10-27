using UnityEngine;

public class CheckPointCheck : MonoBehaviour
{
    public GameObject Respawner;

    public Transform playerPoint;
   
    public PlayerRespawner playerRespawner;

    public bool CheckPoint = false;

    public void Start()
    {
        Respawner.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Rock")
        {
            Debug.Log("check");
            Respawner.SetActive(true);
            CheckPoint = true;
            playerRespawner.respawnPoint = playerPoint;
            this.gameObject.SetActive(false);
        }

    }
}

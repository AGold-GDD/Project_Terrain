using UnityEngine;

public class RockRespawner : MonoBehaviour
{
    public Transform RespawnPoint;
    public GameObject Rock;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            //Rock.transform.position = RespawnPoint.transform.position;
            GameObject rock = Instantiate(Rock, RespawnPoint.position, RespawnPoint.rotation);
        }
    }
}

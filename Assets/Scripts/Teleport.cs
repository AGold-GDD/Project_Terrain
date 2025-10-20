using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Transform TeleportPoint;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.position = TeleportPoint.position;
        }
    }


}

using UnityEditor;
using UnityEngine;

public class CartRespawn : MonoBehaviour
{
    public Transform RespawnPoint;
    public GameObject Cart;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Cart.transform.position = RespawnPoint.transform.position;
        }
    }
}

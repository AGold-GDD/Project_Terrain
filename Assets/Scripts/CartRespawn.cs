using UnityEngine;

public class CartRespawn : MonoBehaviour
{
    public Transform RespawnPoint;
    public Transform RespawnRPoint;
    public GameObject Cart;
    public GameObject Rock;    

    private Rigidbody cartRb;
    private Rigidbody RockRb;

    public CheckPointCheck checkpointcheck;

    void Start()
    {
        // Cache the Rigidbody for performance
        cartRb = Cart.GetComponent<Rigidbody>();
        RockRb = Cart.GetComponent<Rigidbody>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Move cart to respawn point
            Cart.transform.position = RespawnPoint.position;
            Cart.transform.rotation = RespawnPoint.rotation; // Optional, reset rotation too

            // Stop all motion
            if (cartRb != null)
            {
                cartRb.linearVelocity = Vector3.zero;
                cartRb.angularVelocity = Vector3.zero;
            }

            if(checkpointcheck.CheckPoint == true)
            {
                GameObject rocks = Instantiate(Rock, RespawnRPoint.position, RespawnRPoint.rotation);
            }
        }
    }
}


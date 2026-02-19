using UnityEngine;

public class CartSystem : MonoBehaviour
{

    public Transform CartRespawnPoint;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            RespawnCart();
        }
    }

    public void RespawnCart()
    {
        transform.position = CartRespawnPoint.position;
    }
}

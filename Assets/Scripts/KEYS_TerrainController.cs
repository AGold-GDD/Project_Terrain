using UnityEngine;

public class KEYS_TerrainController : MonoBehaviour
{

    public float moveSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        // Move area: Forward
        if (Input.GetKey(KeyCode.I))
        {
            moveDirection += Vector3.forward;
        }

        // Move area: Back
        if (Input.GetKey(KeyCode.K))
        {
            moveDirection += Vector3.back;
        }

        // Move area: Left
        if (Input.GetKey(KeyCode.J))
        {
            moveDirection += Vector3.left;
        }

        //Move area: Right
        if (Input.GetKey(KeyCode.L))
        {
            moveDirection += Vector3.right;
        }

        // Prevent diagonal speed increase
        if (moveDirection != Vector3.zero)
        {
            moveDirection = moveDirection.normalized;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}

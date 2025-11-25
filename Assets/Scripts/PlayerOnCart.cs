using UnityEngine;

public class CartSeatTrigger : MonoBehaviour
{
    public Transform seatPoint; // assign SeatPosition in Inspector
    private SimpleCharacterController currentPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleCharacterController controller = other.GetComponent<SimpleCharacterController>();
            if (controller != null)
            {
                currentPlayer = controller;

                // Disable player movement
                controller.isRiding = true;
                controller.rb.isKinematic = true; // stop external physics
                controller.rb.linearVelocity = Vector3.zero;

                // Snap to seat position
                other.transform.SetParent(seatPoint);
                other.transform.localPosition = Vector3.zero + new Vector3(0,8,0);
                other.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void Update()
    {
        if (currentPlayer != null && Input.GetButtonDown("Jump"))
        {
            // Unmount player
            currentPlayer.rb.isKinematic = false;
            currentPlayer.isRiding = false;
            currentPlayer.transform.SetParent(null);

            // Small upward hop so they don’t instantly collide again
            currentPlayer.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);

            currentPlayer = null;
        }
    }
}

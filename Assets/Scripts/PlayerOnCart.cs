using UnityEngine;

public class CartSeatTrigger : MonoBehaviour
{
    public Transform seatPoint;
    private SimpleCharacterController currentPlayer;
    private Quaternion savedRotation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SimpleCharacterController controller = other.GetComponent<SimpleCharacterController>();
            if (controller != null)
            {
                currentPlayer = controller;

                // Save original rotation BEFORE parenting
                savedRotation = other.transform.rotation;

                controller.isRiding = true;
                controller.rb.isKinematic = true;
                controller.rb.linearVelocity = Vector3.zero;

                // Parent player to seatpoint
                other.transform.SetParent(seatPoint);

                // Snap to local position without inheriting rotation
                other.transform.localPosition = new Vector3(0, 2, 0);
                other.transform.localRotation = Quaternion.identity;
            }
        }
    }

    private void Update()
    {
        if (currentPlayer != null && Input.GetButtonDown("Jump"))
        {
            currentPlayer.rb.isKinematic = false;
            currentPlayer.isRiding = false;

            // Remove parent
            Transform playerTransform = currentPlayer.transform;
            playerTransform.SetParent(null);

            // Restore world rotation so camera is normal again
            playerTransform.rotation = savedRotation;

            // Hop off
            currentPlayer.rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);

            currentPlayer = null;
        }
    }
}

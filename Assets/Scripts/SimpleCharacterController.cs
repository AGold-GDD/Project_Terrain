using UnityEngine;

public class SimpleCharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity = 100f;

    private Rigidbody rb;
    private bool isGrounded;

    private float xRotation = 0f; // For vertical camera rotation

    public Transform playerCamera; // Assign your camera transform here in Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to center of screen
    }

    void Update()
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Clamp vertical rotation

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Move relative to player orientation
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = newVelocity;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}

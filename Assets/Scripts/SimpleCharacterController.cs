using UnityEngine;
using UnityEngine.UI;

public class SimpleCharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity; //public float mouseSensitivity = 100f;

    // mouse sensitivity stuff
    public Slider MouseSlider;

    public Rigidbody rb;
    public bool isGrounded;

    private float xRotation = 0f; // For vertical camera rotation

    public Transform playerCamera; // Assign your camera transform here in Inspector

    public bool isRiding = false; // Add this line

    public AudioSource jumpAudioSource;

    public AudioSource footstepAudioSource;  // Assign in Inspector
    public float stepInterval = 0.5f;  // Time between steps (adjust for pace)
    private float stepTimer = 0f;

    public GameObject PlayerSpawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked; // Start with cursor locked/hidden for open world
        Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked; // Lock cursor to center of screen (uncomment if desired)

    }


    void Update()
    {
        // setting the mouse sensit
        mouseSensitivity = 100 * MouseSlider.value;

        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // Movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 newVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = newVelocity;
        

        // 1. Get your direction vector just like before
        //Vector3 moveDirection = transform.right * moveX + transform.forward * moveZ;

        // 2. Apply a Force instead of setting the speed directly
        // We use ForceMode.Acceleration to ignore the mass of the player for consistent feel
        //rb.AddForce(moveDirection.normalized * speed, ForceMode.Acceleration);

        if (isGrounded && (moveX != 0 || moveZ != 0) && !isRiding)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                if (footstepAudioSource != null)
                {
                    footstepAudioSource.Play();
                }
                stepTimer = stepInterval;  // Reset timer
            }
        }
        else
        {
            stepTimer = 0f;  // Reset if not moving or not grounded
        }


        if (isRiding)
        {
            // Still allow camera rotation while riding
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            return; // Skip movement
        }

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            if (jumpAudioSource != null)
            {
                jumpAudioSource.Play(); //Play jumping sound effect
            }
        }

        //Press R to respawn at a point/start of level.
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerRespawn();
        }

        //This will fix the issue with the webgl sudden increase sensitivity
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            mouseSensitivity = mouseSensitivity / 2;
        }
    }


    public void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    //respawn method. Make sure that in the character controller script, you insert a empty gameobject where you want the player to respawn.
    public void PlayerRespawn()
    {
        transform.position = PlayerSpawnPoint.transform.position;
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

using UnityEngine;
using UnityEngine.UI;

public class SimpleCharacterController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 5f;
    public float mouseSensitivity;
    private bool isDead = false;

    // Jetpack hover variables
    public bool isHovering = false;
    private float hoverYPosition;

    // mouse sensitivity stuff
    public Slider MouseSlider;

    public Rigidbody rb;
    public bool isGrounded;

    private float xRotation = 0f;

    public Transform playerCamera;

    public bool isRiding = false;

    public AudioSource jumpAudioSource;
    public AudioSource footstepAudioSource;
    public float stepInterval = 0.5f;
    private float stepTimer = 0f;

    public GameObject PlayerSpawnPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (CheckpointManager.Instance != null)
        {
            transform.position = CheckpointManager.Instance.GetLastCheckpoint();
        }
    }

    void Update()
    {
        if (isDead) return;

<<<<<<< Updated upstream
      
=======
        // Jetpack hover logic - Context-based (grounded vs airborne)
>>>>>>> Stashed changes
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
<<<<<<< Updated upstream
       
=======
                // ALWAYS jump first when on ground
>>>>>>> Stashed changes
                Jump();
            }
            else if (!isGrounded)
            {
                if (isHovering)
                {
<<<<<<< Updated upstream
           
=======
                    // In air + hovering = Drop (deactivate hover)
>>>>>>> Stashed changes
                    DeactivateHover();
                }
                else
                {
<<<<<<< Updated upstream
                   
=======
                    // In air + not hovering = Activate hover
>>>>>>> Stashed changes
                    ActivateHover();
                }
            }
        }
<<<<<<< Updated upstream
=======

        // Apply hover logic
        if (isHovering)
        {
            // Lock Y position while hovering (allow X/Z movement)
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, hoverYPosition, currentPos.z);
        }
>>>>>>> Stashed changes

     
        if (isHovering)
        {
          
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, hoverYPosition, currentPos.z);
        }

       
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (isHovering)
        {
<<<<<<< Updated upstream
         
=======
            // Hovering: X/Z movement, Y velocity = 0
>>>>>>> Stashed changes
            rb.linearVelocity = new Vector3(move.x * speed, 0f, move.z * speed);
        }
        else
        {
<<<<<<< Updated upstream
          
=======
            // Normal: Full physics control
>>>>>>> Stashed changes
            Vector3 newVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
            rb.linearVelocity = newVelocity;
        }

<<<<<<< Updated upstream
      
=======
        // Footstep sounds
>>>>>>> Stashed changes
        if (isGrounded && (moveX != 0 || moveZ != 0) && !isRiding)
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                if (footstepAudioSource != null)
                {
                    footstepAudioSource.Play();
                }
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }

        if (isRiding)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            transform.Rotate(Vector3.up * mouseX);

            return;
        }

<<<<<<< Updated upstream
        
=======
        // Respawn
>>>>>>> Stashed changes
        if (Input.GetKeyDown(KeyCode.R))
        {
            PlayerRespawn();
        }

<<<<<<< Updated upstream
       
=======
        // WebGL sensitivity fix
>>>>>>> Stashed changes
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            mouseSensitivity = mouseSensitivity / 2;
        }
    }

    void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (jumpAudioSource != null)
        {
            jumpAudioSource.Play();
        }
        Debug.Log("Jump!");
    }

    void ActivateHover()
    {
        isHovering = true;
<<<<<<< Updated upstream
        hoverYPosition = transform.position.y; 
=======
        hoverYPosition = transform.position.y; // Lock current Y position
>>>>>>> Stashed changes
        Debug.Log("Jetpack ON! Hovering at Y: " + hoverYPosition);
    }

    void DeactivateHover()
    {
        isHovering = false;
        Debug.Log("Jetpack OFF! Dropping...");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Died!");

<<<<<<< Updated upstream
        
=======
        // Reset jetpack state
>>>>>>> Stashed changes
        DeactivateHover();

        Vector3 respawnPos = CheckpointManager.Instance.GetLastCheckpoint();
        if (respawnPos == Vector3.zero)
        {
            respawnPos = new Vector3(0, 1, 0);
        }

        transform.position = respawnPos;
        rb.linearVelocity = Vector3.zero;
        isDead = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
            Die();
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

    public void PlayerRespawn()
    {
<<<<<<< Updated upstream
        
=======
        // Reset jetpack state
>>>>>>> Stashed changes
        DeactivateHover();
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
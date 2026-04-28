using UnityEngine;
using UnityEngine.UI;

public class JETPACK_SimpleCharacterController : MonoBehaviour
{
    public float speed = 90f;
    public float jumpForce = 300f;
    public float mouseSensitivity;
    private bool isDead = false;

    // Jetpack hover variables
    public bool isHovering = false;
    private float hoverYPosition;
    public float heightChangeSpeed = 5f;

    // mouse sensitivity stuff
    public Slider MouseSlider;

    public Rigidbody rb;
    public bool isGrounded;

    private float xRotation = 0f;

    public Transform playerCamera;

    public bool isRiding = false;

    public AudioSource jumpAudioSource;
    public AudioSource footstepAudioSource;

    // JETPACK AUDIO SOURCES - EXCLUSIVE PLAYBACK
    public AudioSource jetpackNormalAudioSource;  // Loops when NO height change
    public AudioSource jetpackUpAudioSource;      // Plays INSTEAD of normal when E
    public AudioSource jetpackDownAudioSource;    // Plays INSTEAD of normal when Q

    public float stepInterval = 0.5f;
    private float stepTimer = 0f;

    public GameObject PlayerSpawnPoint;

    // PAUSE SUPPORT
    private bool wasPaused = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (CheckpointManager.Instance != null)
        {
            transform.position = CheckpointManager.Instance.GetLastCheckpoint();
        }

        ActivateHover();
    }

    void Update()
    {
        // PAUSE CHECK - Stop jetpack sounds when paused
        bool isPaused = Time.timeScale == 0f;
        if (isPaused && !wasPaused)
        {
            StopAllJetpackSounds();
        }
        wasPaused = isPaused;

        if (isDead || isPaused) return;

        // Jetpack height controls with PERFECT audio exclusivity
        if (isHovering)
        {
            bool pressingUp = Input.GetKey(KeyCode.E);
            bool pressingDown = Input.GetKey(KeyCode.Q);
            float heightInput = 0f;

            if (pressingUp)
            {
                // E: Up sound ONLY (normal completely stops)
                heightInput += 1f;
                PlayJetpackUpOnly();
            }
            else if (pressingDown)
            {
                // Q: Down sound ONLY (normal completely stops)
                heightInput -= 1f;
                PlayJetpackDownOnly();
            }
            else
            {
                // Neither: Normal loop ONLY (up/down completely stop)
                PlayJetpackNormalOnly();
            }

            // Apply height change
            hoverYPosition += heightInput * heightChangeSpeed * Time.deltaTime;
        }

        if (isHovering)
        {
            Vector3 currentPos = transform.position;
            transform.position = new Vector3(currentPos.x, hoverYPosition, currentPos.z);
        }

        float mouseX = Input.GetAxis("Mouse X") * (mouseSensitivity * MouseSlider.value) * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * (mouseSensitivity * MouseSlider.value) * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        if (isHovering)
        {
            float jetpackSpeed = speed * 2f;
            rb.linearVelocity = new Vector3(move.x * jetpackSpeed, 0f, move.z * jetpackSpeed);
        }
        else
        {
            Vector3 newVelocity = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
            rb.linearVelocity = newVelocity;
        }

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

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            mouseSensitivity = mouseSensitivity / 2;
        }
    }

    void ActivateHover()
    {
        isHovering = true;
        hoverYPosition = transform.position.y;
        if (Time.timeScale != 0f) // Don't play if already paused
        {
            PlayJetpackNormalOnly();
        }
        Debug.Log("Jetpack ON! Hovering at Y: " + hoverYPosition);
    }

    void DeactivateHover()
    {
        isHovering = false;
        StopAllJetpackSounds();
        Debug.Log("Jetpack OFF! Dropping...");
    }

    // ========== PERFECT EXCLUSIVE AUDIO METHODS ==========

    void PlayJetpackNormalOnly()
    {
        // Stop up/down, play normal
        StopJetpackUpSound();
        StopJetpackDownSound();
        if (jetpackNormalAudioSource != null && !jetpackNormalAudioSource.isPlaying && Time.timeScale != 0f)
        {
            jetpackNormalAudioSource.Play();
        }
    }

    void PlayJetpackUpOnly()
    {
        // Stop normal/down, play up
        StopJetpackNormalSound();
        StopJetpackDownSound();
        if (jetpackUpAudioSource != null && !jetpackUpAudioSource.isPlaying && Time.timeScale != 0f)
        {
            jetpackUpAudioSource.Play();
        }
    }

    void PlayJetpackDownOnly()
    {
        // Stop normal/up, play down
        StopJetpackNormalSound();
        StopJetpackUpSound();
        if (jetpackDownAudioSource != null && !jetpackDownAudioSource.isPlaying && Time.timeScale != 0f)
        {
            jetpackDownAudioSource.Play();
        }
    }

    void StopJetpackNormalSound()
    {
        if (jetpackNormalAudioSource != null)
        {
            jetpackNormalAudioSource.Stop();
        }
    }

    void StopJetpackUpSound()
    {
        if (jetpackUpAudioSource != null)
        {
            jetpackUpAudioSource.Stop();
        }
    }

    void StopJetpackDownSound()
    {
        if (jetpackDownAudioSource != null)
        {
            jetpackDownAudioSource.Stop();
        }
    }

    void StopAllJetpackSounds()
    {
        StopJetpackNormalSound();
        StopJetpackUpSound();
        StopJetpackDownSound();
    }

    // ========== END AUDIO METHODS ==========

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player Died!");

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
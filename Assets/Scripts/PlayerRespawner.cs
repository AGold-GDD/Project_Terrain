using UnityEngine;

public class PlayerRespawner : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Assign the PlayerRespawnPoint Transform in Inspector
    public bool resetVelocity = true; // If true, stops the player (zero velocity) on respawn
    public string ballTag = "Ball"; // Tag of the ball GameObject (for collision detection)
    public bool playSoundOnRespawn = false; // Optional: Trigger audio

    [Header("Audio (Optional)")]
    public AudioSource audioSource; // Assign if you want a respawn sound
    public AudioClip respawnSound; // Drag an AudioClip here (e.g., "teleport.wav")

    private Rigidbody playerRb; // Reference to the player's Rigidbody

    void Start()
    {
        // Auto-find Rigidbody
        playerRb = GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogWarning("No Rigidbody found on " + gameObject.name + ". Respawn will only reset position (no velocity reset).");
        }

        // Auto-setup audio
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Validation
        if (respawnPoint == null)
        {
            Debug.LogError("Assign a Respawn Point Transform in Inspector!");
            enabled = false; // Disable script
        }

        // Optional: Respawn at start (uncomment if needed)
        // Respawn();
    }

    void Update()
    {
        // Manual respawn on T key press
        if (Input.GetKeyDown(KeyCode.T))
        {
            Respawn();
        }
    }

    // Automatic respawn on collision with ball
    void OnCollisionEnter(Collision collision)
    {
        // Check if colliding with the ball (by tag)
        if (collision.gameObject.CompareTag(ballTag))
        {
            Debug.Log("Player hit the ball! Respawning...");
            Respawn();
        }
    }

    public void Respawn()
    {
        if (respawnPoint == null) return;

        // Reset position and rotation
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation; // Face the same direction

        // Reset physics velocity if enabled
        if (resetVelocity && playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero; // Stops any spinning
        }

        // Optional: Play sound
        if (playSoundOnRespawn && audioSource != null && respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        Debug.Log("Player respawned at " + respawnPoint.position);
    }

    // Optional: Public method for external triggers (e.g., from a "death" zone script)
    // Example: Call this from a lava trigger: FindObjectOfType<PlayerRespawner>().Respawn();
}

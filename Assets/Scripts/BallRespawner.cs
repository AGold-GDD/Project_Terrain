using UnityEngine;

public class BallRespawner : MonoBehaviour
{
    [Header("Respawn Settings")]
    public Transform respawnPoint; // Assign the BallRespawnPoint Transform in Inspector
    public bool resetVelocity = true; // If true, stops the ball (zero velocity) on respawn
    public bool playSoundOnRespawn = false; // Optional: Trigger audio (assign below)

    [Header("Audio (Optional)")]
    public AudioSource audioSource; // Assign if you want a respawn sound
    public AudioClip respawnSound; // Drag an AudioClip here (e.g., "pop.wav")

    private Rigidbody ballRb; // Reference to the ball's Rigidbody (if physics-enabled)

    void Start()
    {
        // Auto-find Rigidbody if not assigned
        ballRb = GetComponent<Rigidbody>();
        if (ballRb == null)
        {
            Debug.LogWarning("No Rigidbody found on " + gameObject.name + ". Respawn will only reset position.");
        }

        // Auto-setup audio if not assigned
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Initial validation
        if (respawnPoint == null)
        {
            Debug.LogError("Assign a Respawn Point Transform in Inspector!");
            enabled = false; // Disable script if invalid
        }

        // Optional: Respawn immediately at start (uncomment if needed)
        // Respawn();
    }

    void Update()
    {
        // Check for R key press (GetKeyDown for single press, not hold)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        if (respawnPoint == null) return;

        // Reset position to respawn point
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation; // Optional: Match rotation too (e.g., upright)

        // Reset physics if enabled
        if (resetVelocity && ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero; // Stops spinning/rolling
        }

        // Optional: Play sound
        if (playSoundOnRespawn && audioSource != null && respawnSound != null)
        {
            audioSource.PlayOneShot(respawnSound);
        }

        Debug.Log("Ball respawned at " + respawnPoint.position);
    }

    // Optional: Public method for external calls (e.g., from another script on collision/death)
    // Example: Call this from a "death" trigger: FindObjectOfType<BallRespawner>().Respawn();
}

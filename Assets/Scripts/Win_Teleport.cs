using System.Collections.Generic;
using UnityEngine;

public class Win_Teleport : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Drag the player's GameObject here in the Inspector
    [SerializeField] private Transform ballTransform; // Drag the ball's GameObject here in the Inspector
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>(); // Add your player spawn locations here in the Inspector
    [SerializeField] private List<Vector3> ballSpawnPositions = new List<Vector3>(); // Add your ball spawn locations here (can be independent size)

    private int currentIndex = 0; // Tracks which location to use next for advancement
    private Vector3 lastRespawnPoint; // Stores the last player teleported position
    private Vector3 lastBallSpawnPoint; // Stores the last ball teleported position
    private float teleportCooldown = 0f; // Prevents rapid repeated teleports
    private float cooldownDuration = 0.5f; // Time in seconds to wait before allowing another teleport

    void Start()
    {
        // Initialize last points to the first locations or starting positions
        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
            Debug.Log("Initialized lastRespawnPoint to: " + lastRespawnPoint);
        }
        else
        {
            lastRespawnPoint = playerTransform.position;
            Debug.Log("No targetPositions set; using player's current position as lastRespawnPoint: " + lastRespawnPoint);
        }

        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
            Debug.Log("Initialized lastBallSpawnPoint to: " + lastBallSpawnPoint);
        }
        else
        {
            lastBallSpawnPoint = ballTransform.position;
            Debug.Log("No ballSpawnPositions set; using ball's current position as lastBallSpawnPoint: " + lastBallSpawnPoint);
        }

        Debug.Log("targetPositions count: " + targetPositions.Count + ", ballSpawnPositions count: " + ballSpawnPositions.Count);
    }

    // This method will be called by the Button's OnClick event (advances to next for both)
    public void Teleport()
    {
        // Prevent rapid repeated calls
        if (Time.time < teleportCooldown)
        {
            Debug.Log("Teleport on cooldown. Ignoring call.");
            return;
        }

        Debug.Log("Teleport called. Current index before increment: " + currentIndex);

        if (playerTransform != null && ballTransform != null && targetPositions.Count > 0 && ballSpawnPositions.Count > 0)
        {
            // Increment the index first to advance before teleporting
            int minCount = Mathf.Min(targetPositions.Count, ballSpawnPositions.Count);
            currentIndex = (currentIndex + 1) % minCount;

            Debug.Log("Index incremented to: " + currentIndex + " (min count: " + minCount + ")");

            if (currentIndex >= targetPositions.Count || currentIndex >= ballSpawnPositions.Count)
            {
                Debug.LogError("Index out of range after increment. Resetting to 0.");
                currentIndex = 0; // Optional: Reset if out of bounds, but this shouldn't happen with %
                return;
            }

            // Teleport player to the new current position in the list
            Vector3 playerTarget = targetPositions[currentIndex];
            playerTransform.position = playerTarget;
            lastRespawnPoint = playerTarget;
            Debug.Log("Player teleported to: " + playerTarget);

            // Teleport ball to the corresponding position
            Vector3 ballTarget = ballSpawnPositions[currentIndex];
            ballTransform.position = ballTarget;
            lastBallSpawnPoint = ballTarget;
            Debug.Log("Ball teleported to: " + ballTarget);

            // Optional: Reset velocities if using physics
            ResetVelocities();

            // Set cooldown to prevent immediate re-calls
            teleportCooldown = Time.time + cooldownDuration;

            Debug.Log($"Advanced to location {currentIndex}. Next will be {(currentIndex + 1) % minCount}. Cooldown active until: " + teleportCooldown);
        }
        else
        {
            Debug.LogError("Transforms not assigned or one or both lists are empty! Player: " + (playerTransform != null) + ", Ball: " + (ballTransform != null) + ", Target count: " + targetPositions.Count + ", Ball count: " + ballSpawnPositions.Count);
        }
    }

    // Respawn only the player to the last respawn point
    private void RespawnPlayer()
    {
        if (playerTransform != null)
        {
            playerTransform.position = lastRespawnPoint;

            // Optional: Reset velocity if using physics
            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log("Player respawned to last respawn point: " + lastRespawnPoint);
        }
        else
        {
            Debug.LogError("Player Transform not assigned!");
        }
    }

    // Respawn only the ball to the last respawn point
    private void RespawnBall()
    {
        if (ballTransform != null)
        {
            ballTransform.position = lastBallSpawnPoint;

            // Optional: Reset velocity if using physics
            Rigidbody rb = ballTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log("Ball respawned to last respawn point: " + lastBallSpawnPoint);
        }
        else
        {
            Debug.LogError("Ball Transform not assigned!");
        }
    }

    // Helper method to reset velocities for both (used only in Teleport)
    private void ResetVelocities()
    {
        Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector3.zero;
            playerRb.angularVelocity = Vector3.zero;
        }

        Rigidbody ballRb = ballTransform.GetComponent<Rigidbody>();
        if (ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ballRb.angularVelocity = Vector3.zero;
        }
    }

    // Optional: Call this at the start of the game to reset the index
    public void ResetIndex()
    {
        currentIndex = 0;
        teleportCooldown = 0f; // Reset cooldown on index reset
        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
        }
        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
        Debug.Log("Index reset to 0. Last points updated. Cooldown reset.");
    }

    void Update()
    {
        // Check for 'T' key press (respawn player only)
        if (Input.GetKeyDown(KeyCode.T))
        {
            RespawnPlayer();
        }

        // Check for 'R' key press (respawn ball only)
        if (Input.GetKeyDown(KeyCode.R))
        {
            RespawnBall();
        }
    }
}

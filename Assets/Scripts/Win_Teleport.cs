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

    void Start()
    {
        // Initialize last points to the first locations or starting positions
        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
        }
        else
        {
            lastRespawnPoint = playerTransform.position;
        }

        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
        else
        {
            lastBallSpawnPoint = ballTransform.position;
        }
    }

    // This method will be called by the Button's OnClick event (advances to next for both)
    public void Teleport()
    {
        if (playerTransform != null && ballTransform != null && targetPositions.Count > 0 && ballSpawnPositions.Count > 0)
        {
            // Increment the index first to advance before teleporting
            currentIndex = (currentIndex + 1) % Mathf.Min(targetPositions.Count, ballSpawnPositions.Count);

            if (currentIndex >= targetPositions.Count || currentIndex >= ballSpawnPositions.Count)
            {
                Debug.LogError("Index out of range after increment. Resetting to 0.");
                currentIndex = 0; // Optional: Reset if out of bounds, but this shouldn't happen with %
                return;
            }

            // Teleport player to the new current position in the list
            playerTransform.position = targetPositions[currentIndex];
            lastRespawnPoint = targetPositions[currentIndex];

            // Teleport ball to the corresponding position
            ballTransform.position = ballSpawnPositions[currentIndex];
            lastBallSpawnPoint = ballSpawnPositions[currentIndex];

            // Optional: Reset velocities if using physics
            ResetVelocities();

            Debug.Log($"Advanced to location {currentIndex}. Next will be {(currentIndex + 1) % Mathf.Min(targetPositions.Count, ballSpawnPositions.Count)}.");
        }
        else
        {
            Debug.LogError("Transforms not assigned or one or both lists are empty!");
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

            Debug.Log("Player respawned to last respawn point.");
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

            Debug.Log("Ball respawned to last respawn point.");
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
        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
        }
        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
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

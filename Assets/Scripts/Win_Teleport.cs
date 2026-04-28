using System.Collections.Generic;
using UnityEngine;

public class Win_Teleport : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // Drag the player's GameObject here in the Inspector
    [SerializeField] private Transform ballTransform; // Drag the ball's GameObject here in the Inspector

    [Header("Player Spawn Points")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>(); // Add your player spawn locations here
    [SerializeField] private List<Vector3> targetRotations = new List<Vector3>(); // NEW: Player rotations (Euler angles)

    [Header("Ball Spawn Points")]
    [SerializeField] private List<Vector3> ballSpawnPositions = new List<Vector3>(); // Ball spawn locations

    private int currentIndex = 0;
    private Vector3 lastRespawnPoint;
    private Vector3 lastRespawnRotation; // NEW: Store last rotation
    private Vector3 lastBallSpawnPoint;
    private float teleportCooldown = 0f;
    private float cooldownDuration = 0.5f;

    void Start()
    {
        InitializeLastPoints();
    }

    private void InitializeLastPoints()
    {
        // Initialize player position and rotation
        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
            lastRespawnRotation = targetRotations.Count > 0 ? targetRotations[0] : playerTransform.eulerAngles;
            Debug.Log($"Initialized player spawn: Pos={lastRespawnPoint}, Rot={lastRespawnRotation}");
        }
        else
        {
            lastRespawnPoint = playerTransform.position;
            lastRespawnRotation = playerTransform.eulerAngles;
        }

        // Initialize ball position
        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
        else
        {
            lastBallSpawnPoint = ballTransform.position;
        }

        // Validate rotation list matches position list
        ValidateRotationList();

        Debug.Log($"Lists - Positions: {targetPositions.Count}, Rotations: {targetRotations.Count}, Ball: {ballSpawnPositions.Count}");
    }

    private void ValidateRotationList()
    {
        // Auto-populate rotations if list is empty but positions exist
        if (targetRotations.Count == 0 && targetPositions.Count > 0)
        {
            for (int i = 0; i < targetPositions.Count; i++)
            {
                targetRotations.Add(Vector3.zero); // Default forward-facing
            }
            Debug.Log("Auto-populated rotation list with zero rotations (forward facing)");
        }
        // Trim or pad rotation list to match position list
        else if (targetRotations.Count != targetPositions.Count)
        {
            Debug.LogWarning($"Rotation list size ({targetRotations.Count}) doesn't match position list ({targetPositions.Count}). Trimming/padding...");
            while (targetRotations.Count < targetPositions.Count)
                targetRotations.Add(Vector3.zero);
            while (targetRotations.Count > targetPositions.Count)
                targetRotations.RemoveAt(targetRotations.Count - 1);
        }
    }

    public void Teleport()
    {
        if (Time.time < teleportCooldown) return;

        Debug.Log("Teleport called. Current index before increment: " + currentIndex);

        if (playerTransform != null && ballTransform != null && targetPositions.Count > 0 && ballSpawnPositions.Count > 0)
        {
            int minCount = Mathf.Min(targetPositions.Count, ballSpawnPositions.Count);
            int nextIndex = (currentIndex + 1);

            if (nextIndex == 3)
            {
                Debug.Log("Level 3 complete! Loading HUB scene...");
                LoadHubScene();
                return;
            }

            currentIndex = nextIndex;

            // MODIFIED: Teleport player with POSITION + ROTATION
            Vector3 playerTargetPos = targetPositions[currentIndex];
            Vector3 playerTargetRot = targetRotations.Count > currentIndex ? targetRotations[currentIndex] : Vector3.zero;

            playerTransform.position = playerTargetPos;
            playerTransform.eulerAngles = playerTargetRot; // NEW: Set rotation!

            lastRespawnPoint = playerTargetPos;
            lastRespawnRotation = playerTargetRot;

            Debug.Log($"Player teleported to: Pos={playerTargetPos}, Rot={playerTargetRot}");

            // Ball teleport (unchanged)
            Vector3 ballTarget = ballSpawnPositions[currentIndex];
            ballTransform.position = ballTarget;
            lastBallSpawnPoint = ballTarget;
            Debug.Log("Ball teleported to: " + ballTarget);

            ResetVelocities();
            teleportCooldown = Time.time + cooldownDuration;

            Debug.Log($"Advanced to location {currentIndex}");
        }
        else
        {
            Debug.LogError("Missing required assignments or empty lists!");
        }
    }

    // MODIFIED: Respawn player with rotation support
    private void RespawnPlayer()
    {
        if (playerTransform != null)
        {
            playerTransform.position = lastRespawnPoint;
            playerTransform.eulerAngles = lastRespawnRotation; // NEW: Restore rotation!

            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log($"Player respawned to: Pos={lastRespawnPoint}, Rot={lastRespawnRotation}");
        }
    }

    // Respawn ball (unchanged)
    private void RespawnBall()
    {
        if (ballTransform != null)
        {
            ballTransform.position = lastBallSpawnPoint;

            Rigidbody rb = ballTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void ResetVelocities()
    {
        // Unchanged - reset both rigidbodies
        if (playerTransform != null)
        {
            Rigidbody playerRb = playerTransform.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
            }
        }

        if (ballTransform != null)
        {
            Rigidbody ballRb = ballTransform.GetComponent<Rigidbody>();
            if (ballRb != null)
            {
                ballRb.linearVelocity = Vector3.zero;
                ballRb.angularVelocity = Vector3.zero;
            }
        }
    }

    public void ResetIndex()
    {
        currentIndex = 0;
        teleportCooldown = 0f;

        if (targetPositions.Count > 0)
        {
            lastRespawnPoint = targetPositions[0];
            lastRespawnRotation = targetRotations.Count > 0 ? targetRotations[0] : Vector3.zero;
        }
        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
        Debug.Log("Index reset to 0");
    }

    void Update()
    {
       /* if (Input.GetKeyDown(KeyCode.T))
            RespawnPlayer();
        if (Input.GetKeyDown(KeyCode.R))
            RespawnBall();
      */
    }

    private void LoadHubScene()
    {
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        AudioListener.pause = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("NewMainLobby");
    }
}
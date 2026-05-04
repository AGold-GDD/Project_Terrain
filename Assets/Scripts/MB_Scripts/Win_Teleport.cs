using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win_Teleport : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform ballTransform;

    [Header("Player Spawn Points")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();
    [SerializeField] private List<Vector3> targetRotations = new List<Vector3>();

    [Header("Ball Spawn Points")]
    [SerializeField] private List<Vector3> ballSpawnPositions = new List<Vector3>();

    [Header("UI Timing")]
    [SerializeField] private float showResultsDelay = 0.5f;  // Wait 0.5s before UI
    [SerializeField] private float totalSceneDelay = 4f;     // Total time before scene load

    private int currentIndex = 0;
    private Vector3 lastRespawnPoint;
    private Vector3 lastRespawnRotation;
    private Vector3 lastBallSpawnPoint;
    private float teleportCooldown = 0f;
    private float cooldownDuration = 0.5f;
    private bool finalCheckpointReached = false;

    void Start()
    {
        InitializeLastPoints();
    }

    private void InitializeLastPoints()
    {
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

        if (ballSpawnPositions.Count > 0)
        {
            lastBallSpawnPoint = ballSpawnPositions[0];
        }
        else
        {
            lastBallSpawnPoint = ballTransform.position;
        }

        ValidateRotationList();
        Debug.Log($"Lists - Positions: {targetPositions.Count}, Rotations: {targetRotations.Count}, Ball: {ballSpawnPositions.Count}");
    }

    private void ValidateRotationList()
    {
        if (targetRotations.Count == 0 && targetPositions.Count > 0)
        {
            for (int i = 0; i < targetPositions.Count; i++)
            {
                targetRotations.Add(Vector3.zero);
            }
            Debug.Log("Auto-populated rotation list with zero rotations");
        }
        else if (targetRotations.Count != targetPositions.Count)
        {
            Debug.LogWarning($"Rotation list size ({targetRotations.Count}) doesn't match position list ({targetPositions.Count}). Trimming/padding...");
            while (targetRotations.Count < targetPositions.Count)
                targetRotations.Add(Vector3.zero);
            while (targetRotations.Count > targetPositions.Count)
                targetRotations.RemoveAt(targetRotations.Count - 1);
        }
    }

    // SIMPLIFIED TELEPORT - FULLY INTEGRATED
    public void Teleport()
    {
        if (Time.time < teleportCooldown || finalCheckpointReached) return;

        int nextIndex = currentIndex + 1;

        if (playerTransform == null || ballTransform == null || targetPositions.Count == 0 || ballSpawnPositions.Count == 0)
        {
            Debug.LogError("Missing assignments!");
            return;
        }

        TimeManager tm = FindObjectOfType<TimeManager>();

        // ONLY FINAL CHECKPOINT (index == targetPositions.Count)
        if (nextIndex >= targetPositions.Count)
        {
            Debug.Log(" FINAL CHECKPOINT #3!");
            finalCheckpointReached = true;

            // 1. Final teleport
            DoFinalTeleport();

            // 2. Checkpoint call (for final segment time)
            if (tm != null) tm.CheckpointReached();

            // 3. Show results
            Invoke(nameof(ShowResults), 0.5f);
            Invoke(nameof(LoadHubScene), 4f);
            return;
        }

        // REGULAR CHECKPOINTS (1 & 2 only)
        if (tm != null) tm.CheckpointReached();
        DoTeleport(nextIndex);
    }

    private void ShowResults()
    {
        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null)
        {
            tm.CompleteLevel();
            Debug.Log("Results UI shown  enter name and save");
        }
    }

    private void DoTeleport(int index)
    {
        currentIndex = index;

        Vector3 playerTargetPos = targetPositions[currentIndex];
        Vector3 playerTargetRot = targetRotations.Count > currentIndex ? targetRotations[currentIndex] : Vector3.zero;

        playerTransform.position = playerTargetPos;
        playerTransform.eulerAngles = playerTargetRot;
        lastRespawnPoint = playerTargetPos;
        lastRespawnRotation = playerTargetRot;

        Vector3 ballTarget = ballSpawnPositions[currentIndex];
        ballTransform.position = ballTarget;
        lastBallSpawnPoint = ballTarget;

        ResetVelocities();
        teleportCooldown = Time.time + cooldownDuration;

        Debug.Log($"Teleported to checkpoint {currentIndex}");
    }

    private void DoFinalTeleport()
    {
        if (targetPositions.Count > 0)
        {
            int finalIndex = targetPositions.Count - 1;
            Vector3 finalPos = targetPositions[finalIndex];
            Vector3 finalRot = targetRotations.Count > finalIndex ? targetRotations[finalIndex] : Vector3.zero;

            playerTransform.position = finalPos;
            playerTransform.eulerAngles = finalRot;
            lastRespawnPoint = finalPos;
            lastRespawnRotation = finalRot;

            if (ballSpawnPositions.Count > 0)
            {
                ballTransform.position = ballSpawnPositions[finalIndex];
                lastBallSpawnPoint = ballSpawnPositions[finalIndex];
            }
        }

        ResetVelocities();
        Debug.Log("Teleported to FINAL position");
    }

    private void RespawnPlayer()
    {
        if (playerTransform != null)
        {
            playerTransform.position = lastRespawnPoint;
            playerTransform.eulerAngles = lastRespawnRotation;

            Rigidbody rb = playerTransform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            Debug.Log($"Player respawned to: Pos={lastRespawnPoint}, Rot={lastRespawnRotation}");
        }
    }

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
        finalCheckpointReached = false;
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

    private void LoadHubScene()
    {
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        AudioListener.pause = false;
        SceneManager.LoadScene("NewMainLobby");
    }
}
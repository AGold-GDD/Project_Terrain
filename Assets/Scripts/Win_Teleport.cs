using System.Collections.Generic;
using UnityEngine;

public class Win_Teleport : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform ballTransform;

    [Header("Player Spawn Points")]
    [SerializeField] private List<Vector3> targetPositions = new List<Vector3>();
    [SerializeField] private List<Vector3> targetRotations = new List<Vector3>();

    [Header("Ball Spawn Points")]
    [SerializeField] private List<Vector3> ballSpawnPositions = new List<Vector3>();

    private int currentIndex = 0;
    private Vector3 lastRespawnPoint;
    private Vector3 lastRespawnRotation;
    private Vector3 lastBallSpawnPoint;
    private float teleportCooldown = 0f;
    private float cooldownDuration = 0.5f;

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

    public void Teleport()
    {
        if (Time.time < teleportCooldown) return;

        Debug.Log($"Teleport called. Current: {currentIndex} -> Next: {currentIndex + 1}");

        if (playerTransform == null || ballTransform == null || targetPositions.Count == 0 || ballSpawnPositions.Count == 0)
        {
            Debug.LogError("Missing required assignments!");
            return;
        }

        int nextIndex = currentIndex + 1;

        TimeManager tm = FindObjectOfType<TimeManager>();
        if (tm != null)
        {
            if (nextIndex == 3)
            {
                Debug.Log("Final checkpoint - showing results");
                tm.CompleteLevel();
                Invoke(nameof(LoadHubScene), 3f);
                return;
            }
            else
            {
                tm.CheckpointReached();
            }
        }

        currentIndex = nextIndex;

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
    }

    private void LoadHubScene()
    {
        Time.timeScale = 1f;
        Physics.autoSimulation = true;
        AudioListener.pause = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene("NewMainLobby");
    }
}
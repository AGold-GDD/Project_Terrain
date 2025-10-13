using UnityEngine;
using UnityEngine.UI; // For UI elements

public class GoalDetector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel; // Drag the VictoryPanel here in Inspector
    public Button resetButton;      // Drag the ResetButton here in Inspector

    [Header("Respawn Settings")]
    public Transform ballStartArea; // Drag the BallStartArea here in Inspector
    public float yOffset = 1f;      // Optional: Height offset above start area (e.g., for ball radius)

    [Header("Cursor Settings")]
    public bool lockCursorOnReset = true; // Set to false if you don't want cursor locked after reset

    private bool hasWon = false;    // Prevent multiple triggers

    void Start()
    {
        // Ensure UI starts hidden
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        // Hook up the reset button
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetBall);

        // Optional: Lock cursor at start if your game uses it (e.g., for mouse controls)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is the ball (tag it as "Ball" in its Inspector)
        if (hasWon) return; // Already won, ignore

        if (other.CompareTag("Ball")) // Assume your ball is tagged "Ball"
        {
            hasWon = true;
            ShowVictoryUI();
        }
    }

    void ShowVictoryUI()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);

            // Show and unlock cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Optional: Pause the game (UI still works)
            Time.timeScale = 0f; // Freezes physics/UI updates except UI events
        }
    }

    public void ResetBall() // Called by the button
    {
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball != null && ballStartArea != null)
        {
            // Respawn at BallStartArea position with optional Y offset
            ball.transform.position = ballStartArea.position + new Vector3(0, yOffset, 0);

            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero; // Stop momentum
                rb.angularVelocity = Vector3.zero; // Stop spinning
            }
        }
        else
        {
            Debug.LogWarning("Ball or BallStartArea not found! Check tags and references.");
        }

        // Hide UI and reset win state
        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        hasWon = false;

        // Unpause the game
        Time.timeScale = 1f;

        // Optional: Re-lock cursor after reset (uncomment if needed for your controls)
        if (lockCursorOnReset)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}

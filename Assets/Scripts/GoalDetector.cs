using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalDetector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public Button resetButton;
    public TextMeshProUGUI timeDisplay;

    [Header("Respawn Settings")]
    public Transform ballStartArea;
    public float yOffset = 1f;

    [Header("Cursor Settings")]
    public bool lockCursorOnReset = true;

    private bool hasWon = false;
    private TimeManager levelTimer;

    void Start()
    {
        levelTimer = FindObjectOfType<TimeManager>();

        if (victoryPanel != null)
            victoryPanel.SetActive(false);

        if (resetButton != null)
            resetButton.onClick.AddListener(ResetBall);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasWon) return;
        if (other.CompareTag("Ball"))
        {
            hasWon = true;
            ShowVictoryUI();

            // PERFECT - already works!
            if (levelTimer != null)
            {
                levelTimer.CompleteLevel();
            }
        }
    }

    void ShowVictoryUI()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // NEW: Show time on YOUR victory panel too!
        if (timeDisplay != null && levelTimer != null)
        {
            timeDisplay.text = "Time: " + levelTimer.FormattedTime;
        }
    }

    public void ResetBall()
    {
        // Your existing ball reset...
        GameObject ball = GameObject.FindWithTag("Ball");
        if (ball != null && ballStartArea != null)
        {
            ball.transform.position = ballStartArea.position + new Vector3(0, yOffset, 0);
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        // Restart timer for retry
        if (levelTimer != null)
        {
            levelTimer.ResetSession(); // NEW: Full reset (clears session)
        }

        victoryPanel.SetActive(false);
        hasWon = false;
        Time.timeScale = 1f;

        if (lockCursorOnReset)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
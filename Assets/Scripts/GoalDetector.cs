using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalDetector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public Button resetButton;
    public TextMeshProUGUI timeDisplay;  //  NEW: Timer display on victory panel

    [Header("Respawn Settings")]
    public Transform ballStartArea;
    public float yOffset = 1f;

    [Header("Cursor Settings")]
    public bool lockCursorOnReset = true;

    private bool hasWon = false;
    private TimeManager levelTimer;  //  NEW: Timer reference

    void Start()
    {
        //  NEW: Find timer
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

            //  NEW: Complete level & show time!
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
    }

    public void ResetBall()
    {
        // Reset existing ball logic
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

        //  NEW: Restart timer
        if (levelTimer != null)
        {
            levelTimer.StartTimer();
        }

        if (victoryPanel != null)
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
// Your ORIGINAL GoalDetector.cs (with the fix applied)
using UnityEngine;
using UnityEngine.UI;

public class GoalDetector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public Button resetButton;

    [Header("Respawn Settings")]
    public Transform ballStartArea;
    public float yOffset = 1f;

    [Header("Cursor Settings")]
    public bool lockCursorOnReset = true;

    private bool hasWon = false;

    void Start()
    {
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
        // Your existing ResetBall code (unchanged)
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
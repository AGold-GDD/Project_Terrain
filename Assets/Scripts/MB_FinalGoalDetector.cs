using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalGoalDetector : MonoBehaviour
{
    [Header("UI References")]
    public GameObject victoryPanel;
    public Button continueButton;

    [Header("Scene Settings")]
    [SerializeField] private string hubSceneName = "HubScene";

    private bool hasWon = false;

    void Start()
    {
        Debug.Log("FinalGoalDetector Start() - Goal 3 ready");

        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
            Debug.Log("Victory panel hidden");
        }
        else
        {
            Debug.LogError("VICTORY PANEL IS NULL! Drag it to inspector!");
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(LoadHubScene);
            Debug.Log("Continue button wired");
        }
        else
        {
            Debug.LogError("CONTINUE BUTTON IS NULL! Drag it to inspector!");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered by: {other.name} (tag: {other.tag})");

        if (hasWon)
        {
            Debug.Log("Already won, ignoring");
            return;
        }

        if (other.CompareTag("Ball"))
        {
            Debug.Log("BALL DETECTED! Showing victory UI");
            hasWon = true;
            ShowVictoryUI();
        }
        else
        {
            Debug.LogWarning($"Not a ball! Tag: {other.tag}");
        }
    }

    void ShowVictoryUI()
    {
        Debug.Log("=== ShowVictoryUI START ===");

        if (victoryPanel == null)
        {
            Debug.LogError("victoryPanel is NULL!");
            return;
        }

        Debug.Log($"Activating: {victoryPanel.name} (active: {victoryPanel.activeInHierarchy})");

        // FORCE activate
        victoryPanel.SetActive(true);

        // Get Canvas component
        Canvas victoryCanvas = victoryPanel.GetComponent<Canvas>();
        if (victoryCanvas == null)
        {
            Debug.LogError("No Canvas component on victoryPanel!");
            return;
        }

        // Ensure it's on top
        victoryCanvas.sortingOrder = 100; // Very high!
        Debug.Log($"Canvas sortingOrder set to: {victoryCanvas.sortingOrder}");

        // Multiple refresh attempts
        for (int i = 0; i < 3; i++)
        {
            victoryCanvas.enabled = false;
            victoryCanvas.enabled = true;
        }

        // Force layout rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(victoryPanel.GetComponent<RectTransform>());

        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("=== ShowVictoryUI SUCCESS ===");
        Debug.Log($"Final UI active: {victoryPanel.activeInHierarchy}");
    }

    public void LoadHubScene()
    {
        Debug.Log($"Loading {hubSceneName}");
        SceneManager.LoadScene(hubSceneName);
    }
}
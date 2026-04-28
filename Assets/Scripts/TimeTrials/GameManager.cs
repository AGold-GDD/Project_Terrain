using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Settings")]
    public float totalTime = 60f; 
    public TextMeshProUGUI timerText;

    public float currentTime;
    private bool isGameActive = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentTime = totalTime;
        UpdateTimerUI();
    }

    private void Update()
    {
        if (!isGameActive) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0)
        {
            ResetLevel();
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = $"Time: {currentTime:F1}";
        }
    }

    public void AddTime(float amount)
    {
        currentTime += amount;
        // Optional: Clamp max time so time gained does not go over the total amount of time
        // if (currentTime > totalTime) currentTime = totalTime;
        UpdateTimerUI();
    }


    public void ResetLevel()
    {
        isGameActive = false;
        Debug.Log("Time's Up! Resetting Level...");

        // Reloads the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}


//OLD CODE TO COLLECT ITEMS

/*
public int collectedCount = 0;
public TextMeshProUGUI collectibleCounterText;  // Assign in Inspector

private void Awake()
{
    // Singleton pattern to ensure only one GameManager exists
    if (instance == null)
    {
        instance = this;
    }
    else
    {
        Destroy(gameObject);
    }
}

public void CollectSphere()
{
    collectedCount++;
    UpdateUI();
}

void UpdateUI()
{
    collectibleCounterText.text = "Data Collected: " + collectedCount;
}
*/


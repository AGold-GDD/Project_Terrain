using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TimeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;      // In-game timer display
    public TextMeshProUGUI resultsText;    // Between-levels results display
    public GameObject resultsPanel;        // Panel that shows after level complete

    [Header("Settings")]
    public bool startTimerOnAwake = true;

    private float elapsedTime = 0f;
    private bool timerRunning = false;
    private bool levelCompleted = false;

    // Formatted time string (MM:SS.fff)
    public string FormattedTime => FormatTime(elapsedTime);

    void Awake()
    {
        // Make persistent across scenes
        if (FindObjectsOfType<TimeManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        if (startTimerOnAwake)
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (timerRunning && !levelCompleted)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
        levelCompleted = false;
        elapsedTime = 0f;
        Debug.Log("Timer STARTED");
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log($"Timer STOPPED at: {FormattedTime}");
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        StopTimer();
        ShowResults();
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = FormattedTime;
        }
    }

    void ShowResults()
    {
        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        if (resultsText != null)
        {
            resultsText.text = $"\nTime: {FormattedTime}";
        }
    }

    string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    // Public getter for other scripts
    public float GetElapsedTime() => elapsedTime;
}
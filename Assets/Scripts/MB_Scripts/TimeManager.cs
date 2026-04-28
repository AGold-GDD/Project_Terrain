using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultsText;
    public GameObject resultsPanel;
    public TextMeshProUGUI segmentTimesText; // NEW: Shows all segment times at end

    [Header("Settings")]
    public bool startTimerOnAwake = true;

    // NEW: Segment timing system
    [Header("Segment Timing")]
    public bool trackSegmentTimes = true;
    private List<float> segmentTimes = new List<float>();  // Time per checkpoint segment
    private float segmentStartTime = 0f;
    private int currentSegment = 0;

    private float elapsedTime = 0f;
    private bool timerRunning = false;
    private bool levelCompleted = false;

    public string FormattedTime => FormatTime(elapsedTime);
    public IReadOnlyList<float> SegmentTimes => segmentTimes.AsReadOnly();
    public int CurrentSegment => currentSegment;

    void Awake()
    {
        if (FindObjectsOfType<TimeManager>().Length > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            //DontDestroyOnLoad(gameObject);
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
        segmentTimes.Clear();
        currentSegment = 0;
        segmentStartTime = 0f;
        Debug.Log("Timer STARTED");
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log($"Timer STOPPED at: {FormattedTime}");
    }

    // NEW: Called when hitting each checkpoint
    public void CheckpointReached()
    {
        if (!trackSegmentTimes || levelCompleted) return;

        float segmentTime = elapsedTime - segmentStartTime;
        segmentTimes.Add(segmentTime);

        currentSegment++;
        segmentStartTime = elapsedTime;

        Debug.Log($"Checkpoint {currentSegment}: {FormatTime(segmentTime)} (Total: {FormattedTime})");
    }

    // NEW: Get formatted segment times for UI
    public string GetSegmentTimesFormatted()
    {
        if (!trackSegmentTimes || segmentTimes.Count == 0) return "No segments recorded";

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Segment Times:");

        for (int i = 0; i < segmentTimes.Count; i++)
        {
            sb.AppendLine($"  {i + 1}: {FormatTime(segmentTimes[i])}");
        }

        sb.AppendLine($"Total: {FormattedTime}");
        return sb.ToString();
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        // Final segment time
        if (trackSegmentTimes)
        {
            float finalSegmentTime = elapsedTime - segmentStartTime;
            segmentTimes.Add(finalSegmentTime);
            Debug.Log($"Final segment: {FormatTime(finalSegmentTime)}");
        }

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
            resultsText.text = $"\nTotal Time: {FormattedTime}";
        }

        // NEW: Show segment times
        if (segmentTimesText != null)
        {
            segmentTimesText.text = GetSegmentTimesFormatted();
        }
    }

    string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    public float GetElapsedTime() => elapsedTime;
}
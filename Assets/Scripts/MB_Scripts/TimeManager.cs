using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class TimeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI resultsText;
    public GameObject resultsPanel;
    public TextMeshProUGUI segmentTimesText;

    [Header("Name Input UI")]
    public TMP_InputField playerNameInput;
    public Button submitScoreButton;
    public TextMeshProUGUI submitMessage;
    public GameObject nameInputPanel; // Container for name input UI

    [Header("Settings")]
    public bool startTimerOnAwake = true;

    [Header("Segment Timing")]
    public bool trackSegmentTimes = true;
    private List<float> segmentTimes = new List<float>();
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
    }

    void Start()
    {
        if (startTimerOnAwake)
        {
            StartTimer();
        }

        // Hide name input UI initially
        HideNameInputUI();
    }

    void Update()
    {
        if (timerRunning && !levelCompleted)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }

        // NEW: Prevent input blocking during results
        if (levelCompleted && EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(playerNameInput?.gameObject);
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

    public void CheckpointReached()
    {
        if (!trackSegmentTimes || levelCompleted) return;

        float segmentTime = elapsedTime - segmentStartTime;
        segmentTimes.Add(segmentTime);

        currentSegment++;
        segmentStartTime = elapsedTime;

        Debug.Log($"Checkpoint {currentSegment}: {FormatTime(segmentTime)} (Total: {FormattedTime})");
    }

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

        if (segmentTimesText != null)
        {
            segmentTimesText.text = GetSegmentTimesFormatted();
        }

        ShowNameInputUI();
    }

    void ShowNameInputUI()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(true);
        }

        if (playerNameInput != null)
        {
            playerNameInput.text = PlayerPrefs.GetString("LastPlayerName", "Player");
            playerNameInput.Select();
            playerNameInput.ActivateInputField();
        }

        if (submitScoreButton != null)
        {
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(SavePlayerScore);
        }
    }

    void HideNameInputUI()
    {
        if (nameInputPanel != null)
        {
            nameInputPanel.SetActive(false);
        }
        if (submitMessage != null)
        {
            submitMessage.gameObject.SetActive(false);
        }
    }

    public void SavePlayerScore()
    {
        string playerName = playerNameInput != null ? playerNameInput.text.Trim() : "Anonymous";
        if (string.IsNullOrEmpty(playerName)) playerName = "Anonymous";

        PlayerPrefs.SetString("LastPlayerName", playerName);
        SaveScoreToPrefs(playerName, elapsedTime);

        if (submitMessage != null)
        {
            submitMessage.text = $"Saved: {playerName} - {FormattedTime}";
            submitMessage.gameObject.SetActive(true);
        }

        // Hide input after 2 seconds
        Invoke(nameof(HideNameInputUI), 2f);

        Debug.Log($"Score saved: {playerName} - {FormattedTime}");
    }

    private void SaveScoreToPrefs(string name, float time)
    {
        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
        PlayerPrefs.SetFloat($"Score_{scoreCount}_Time", time);
        PlayerPrefs.SetString($"Score_{scoreCount}_Name", name);
        PlayerPrefs.SetInt("ScoreCount", scoreCount + 1);

        // Keep only top 20 scores (room to grow)
        if (scoreCount + 1 > 20)
        {
            PlayerPrefs.SetInt("ScoreCount", 20);
        }

        PlayerPrefs.Save();
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
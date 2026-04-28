using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelTime
{
    public string levelName;
    public float timeTaken;
    public string playerInitials;

    public LevelTime(string levelName, float timeTaken, string playerInitials = "")
    {
        this.levelName = levelName;
        this.timeTaken = timeTaken;
        this.playerInitials = playerInitials;
    }
}

[System.Serializable]
public class ScoreEntry
{
    public string initials;
    public List<LevelTime> levelTimes = new List<LevelTime>();

    public float totalTime
    {
        get
        {
            float total = 0f;
            foreach (var levelTime in levelTimes)
            {
                total += levelTime.timeTaken;
            }
            return total;
        }
    }
}

public class TimeManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI currentTimerText;
    public TextMeshProUGUI lastLevelText;
    public TextMeshProUGUI totalTimeText;
    public TextMeshProUGUI resultsText;
    public GameObject resultsPanel;
    public TMP_InputField initialsInput;
    public Button submitScoreButton;

    [Header("Settings")]
    public bool startTimerOnAwake = true;
    public string currentLevelName = "Level 1";

    [Header("Scoreboard")]
    public List<ScoreEntry> allScores = new List<ScoreEntry>();

    private float elapsedTime = 0f;
    private bool timerRunning = false;
    private bool levelCompleted = false;
    private List<LevelTime> sessionLevelTimes = new List<LevelTime>();

    // Properties
    public string FormattedTime
    {
        get { return FormatTime(elapsedTime); }
    }

    public string FormattedTotalTime
    {
        get { return FormatTime(GetTotalSessionTime()); }
    }

    void Awake()
    {
        if (FindObjectsOfType<TimeManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        LoadScores();
    }

    void Start()
    {
        if (startTimerOnAwake)
        {
            StartTimer();
        }
        UpdateTotalTimeDisplay();
    }

    void Update()
    {
        if (timerRunning && !levelCompleted)
        {
            elapsedTime += Time.deltaTime;
            UpdateCurrentTimerDisplay();
        }
    }

    public void StartTimer()
    {
        timerRunning = true;
        levelCompleted = false;
        elapsedTime = 0f;
        Debug.Log("Timer STARTED for " + currentLevelName);
    }

    public void StopTimer()
    {
        timerRunning = false;
        Debug.Log("Timer STOPPED at: " + FormattedTime);
    }

    public void CompleteLevel()
    {
        if (levelCompleted) return;

        levelCompleted = true;
        StopTimer();
        sessionLevelTimes.Add(new LevelTime(currentLevelName, elapsedTime));
        ShowResults();
    }

    void UpdateCurrentTimerDisplay()
    {
        if (currentTimerText != null)
        {
            currentTimerText.text = "Current: " + FormattedTime;
        }
    }

    void UpdateLastLevelDisplay()
    {
        if (lastLevelText != null && sessionLevelTimes.Count > 0)
        {
            var lastLevel = sessionLevelTimes[sessionLevelTimes.Count - 1];
            lastLevelText.text = "Last Level: " + FormatTime(lastLevel.timeTaken);
        }
    }

    float GetTotalSessionTime()
    {
        float total = 0f;
        foreach (var levelTime in sessionLevelTimes)
        {
            total += levelTime.timeTaken;
        }
        return total;
    }

    void UpdateTotalTimeDisplay()
    {
        if (totalTimeText != null)
        {
            totalTimeText.text = "Total: " + FormattedTotalTime;
        }
    }

    void ShowResults()
    {
        UpdateLastLevelDisplay();
        UpdateTotalTimeDisplay();

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(true);
        }

        if (resultsText != null)
        {
            string results = "Session Results:\n\n";
            foreach (var levelTime in sessionLevelTimes)
            {
                results += levelTime.levelName + ": " + FormatTime(levelTime.timeTaken) + "\n";
            }
            results += "\nTotal Time: " + FormattedTotalTime;
            resultsText.text = results;
        }

        SetupSubmitButton();
    }

    void SetupSubmitButton()
    {
        if (initialsInput != null)
        {
            initialsInput.gameObject.SetActive(true);
        }

        if (submitScoreButton != null)
        {
            submitScoreButton.gameObject.SetActive(true);
            submitScoreButton.onClick.RemoveAllListeners();
            submitScoreButton.onClick.AddListener(SubmitScore);
        }
    }

    public void SubmitScore()
    {
        string initials = "AAA";
        if (initialsInput != null && !string.IsNullOrEmpty(initialsInput.text))
        {
            initials = initialsInput.text.ToUpper().PadRight(3).Substring(0, 3);
        }

        var newScore = new ScoreEntry();
        newScore.initials = initials;
        newScore.levelTimes.AddRange(sessionLevelTimes);

        allScores.Add(newScore);
        SaveScores();

        Debug.Log("Score saved: " + initials + " - " + newScore.totalTime.ToString("F3") + "s");

        // Reset for next run
        sessionLevelTimes.Clear();
        if (initialsInput != null) initialsInput.text = "";
    }

    public void ResetSession()
    {
        sessionLevelTimes.Clear();
        elapsedTime = 0f;
        levelCompleted = false;
        timerRunning = false;

        if (resultsPanel != null)
        {
            resultsPanel.SetActive(false);
        }

        UpdateLastLevelDisplay();
        UpdateTotalTimeDisplay();
    }

    // THE ONLY FormatTime METHOD - WORKS PERFECTLY
    string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }

    void SaveScores()
    {
        string json = JsonUtility.ToJson(new ScoreWrapper(allScores), true);
        PlayerPrefs.SetString("TimeManager_Scores", json);
        PlayerPrefs.Save();
    }

    void LoadScores()
    {
        if (PlayerPrefs.HasKey("TimeManager_Scores"))
        {
            string json = PlayerPrefs.GetString("TimeManager_Scores");
            allScores = JsonUtility.FromJson<ScoreWrapper>(json).scores;
        }
    }

    // Public API
    public float GetElapsedTime() { return elapsedTime; }
    public List<LevelTime> GetSessionLevelTimes() { return sessionLevelTimes; }
    public List<ScoreEntry> GetAllScores() { return allScores; }
}

[System.Serializable]
public class ScoreWrapper
{
    public List<ScoreEntry> scores = new List<ScoreEntry>();

    public ScoreWrapper() { }
    public ScoreWrapper(List<ScoreEntry> scores)
    {
        this.scores = scores;
    }
}
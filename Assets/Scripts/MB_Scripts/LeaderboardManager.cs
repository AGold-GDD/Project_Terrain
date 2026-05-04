using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LeaderboardManager : MonoBehaviour
{
    [Header("3D Scoreboard References")]
    [Tooltip("Array of TextMeshPro for player names (drag 10 objects)")]
    public TextMeshPro[] nameTexts = new TextMeshPro[10];

    [Tooltip("Array of TextMeshPro for times (drag 10 objects)")]
    public TextMeshPro[] timeTexts = new TextMeshPro[10];

    [Tooltip("Optional title text")]
    public TextMeshPro titleText;

    [Tooltip("Optional 'No scores yet' text")]
    public TextMeshPro noScoresText;

    [Header("Formatting")]
    public string nameFormat = "{0}. {1}";
    public string timeFormat = "{0:00}:{1:00}.{2:000}";

    void Start()
    {
        LoadAndDisplayScores();
    }

    [ContextMenu("Refresh Leaderboard")]
    public void LoadAndDisplayScores()
    {
        List<(string name, float time)> scores = new List<(string, float)>();

        int scoreCount = PlayerPrefs.GetInt("ScoreCount", 0);
        for (int i = 0; i < scoreCount && i < 20; i++) // Load up to 20 stored
        {
            string name = PlayerPrefs.GetString($"Score_{i}_Name", "Unknown");
            float time = PlayerPrefs.GetFloat($"Score_{i}_Time", 9999f);
            if (!string.IsNullOrEmpty(name) && time < 9999f)
            {
                scores.Add((name, time));
            }
        }

        // Sort by fastest time first
        scores.Sort((a, b) => a.time.CompareTo(b.time));

        // Display top 10 scores
        for (int i = 0; i < 10; i++)
        {
            if (i < scores.Count)
            {
                // Name
                if (nameTexts[i] != null)
                {
                    nameTexts[i].text = string.Format(nameFormat, i + 1, scores[i].name);
                    nameTexts[i].gameObject.SetActive(true);
                }

                // Time
                if (timeTexts[i] != null)
                {
                    float t = scores[i].time;
                    int minutes = Mathf.FloorToInt(t / 60f);
                    int seconds = Mathf.FloorToInt(t % 60f);
                    int ms = Mathf.FloorToInt((t * 1000f) % 1000f);
                    timeTexts[i].text = string.Format(timeFormat, minutes, seconds, ms);
                    timeTexts[i].gameObject.SetActive(true);
                }
            }
            else
            {
                // Hide empty slots
                if (nameTexts[i] != null)
                    nameTexts[i].gameObject.SetActive(false);
                if (timeTexts[i] != null)
                    timeTexts[i].gameObject.SetActive(false);
            }
        }

        // Update title/no scores text
        if (titleText != null)
        {
            titleText.text = $"Top {scores.Count} Times";
            titleText.gameObject.SetActive(true);
        }

        if (noScoresText != null)
        {
            noScoresText.gameObject.SetActive(scores.Count == 0);
        }

        Debug.Log($"Leaderboard loaded: {scores.Count} scores displayed");
    }

    [ContextMenu("Clear All Scores")]
    public void ClearLeaderboard()
    {
        PlayerPrefs.DeleteKey("ScoreCount");
        for (int i = 0; i < 20; i++)
        {
            PlayerPrefs.DeleteKey($"Score_{i}_Name");
            PlayerPrefs.DeleteKey($"Score_{i}_Time");
        }
        PlayerPrefs.DeleteKey("LastPlayerName");
        PlayerPrefs.Save();

        LoadAndDisplayScores();
        Debug.Log("Leaderboard cleared!");
    }

    // Call this from other scripts if needed
    public void Refresh()
    {
        LoadAndDisplayScores();
    }
}
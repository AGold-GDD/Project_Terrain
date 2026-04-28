using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections;

public class ScoreboardManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreboardText;

    [Header("Settings")]
    public int maxScores = 10;
    public string title = "ARCADE CHAMPIONS";

    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        if (timeManager == null)
        {
            Debug.LogError("TimeManager not found!");
            return;
        }

        UpdateScoreboard();
    }

    public void UpdateScoreboard()
    {
        if (scoreboardText == null) return;

        var scores = timeManager.GetAllScores()
            .OrderBy(s => s.totalTime)
            .Take(maxScores)
            .ToList();

        string display = "<size=36><b>" + title + "</b></size>\n\n";
        display += "===================\n\n";

        if (scores.Count == 0)
        {
            display += "<color=red>NO SCORES YET</color>";
        }
        else
        {
            string[] rankColors = { "#FFD700", "#C0C0C0", "#CD7F32", "#00FF41", "#00FF41", "#00FF41", "#00FF41", "#00FF41", "#00FF41", "#00FF41" };

            for (int i = 0; i < scores.Count; i++)
            {
                string rank = GetRank(i);
                string color = rankColors[i];

                display += "<color=" + color + ">" + rank + "  " + scores[i].initials + "  " + FormatTime(scores[i].totalTime) + "</color>\n";
            }
        }

        scoreboardText.text = display;
    }

    string GetRank(int position)
    {
        string[] ranks = { "SSS", "SS", "S", "A", "B", "C", "D", "E", "F" };
        if (position < ranks.Length)
            return ranks[position];
        else
            return (position + 1).ToString();
    }

    string FormatTime(float totalSeconds)
    {
        int minutes = Mathf.FloorToInt(totalSeconds / 60f);
        int seconds = Mathf.FloorToInt(totalSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((totalSeconds * 1000f) % 1000f);
        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, milliseconds);
    }
}
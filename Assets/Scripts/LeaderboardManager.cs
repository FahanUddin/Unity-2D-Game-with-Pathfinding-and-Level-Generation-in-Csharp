using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LeaderboardManager
{
    private static string filePath => Path.Combine(Application.persistentDataPath, "Leaderboard.txt");

    public static System.Action OnLeaderboardUpdated; 

    // Save a new score if teamCode doesn't exist
    public static void SaveScore(string teamCode, float time)
    {
        var entries = LoadAllScores();

        // Prevent duplicates
        if (entries.Exists(e => e.code == teamCode))
        {
            Debug.LogWarning($"Team code {teamCode} already exists. Score not saved.");
            return;
        }

        entries.Add((teamCode, time));

        // Rewrite the file
        using (StreamWriter writer = new StreamWriter(filePath, false))
        {
            foreach (var entry in entries)
            {
                writer.WriteLine($"{entry.code}:{entry.time}");
            }
        }

        Debug.Log($"Saved score: {teamCode}:{time:F2}");

        // Notify any listeners to update the leaderboard UI
        OnLeaderboardUpdated?.Invoke();
    }

    public static List<(string code, float time)> LoadAllScores()
    {
        var results = new List<(string code, float time)>();

        if (!File.Exists(filePath)) return results;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var parts = line.Split(':');
            if (parts.Length == 2 && float.TryParse(parts[1], out float t))
            {
                results.Add((parts[0], t));
            }
        }

        results.Sort((a, b) => a.time.CompareTo(b.time));
        return results;
    }

    public static List<(string code, float time)> GetTopThree()
    {
        var all = LoadAllScores();
        return all.GetRange(0, Mathf.Min(3, all.Count));
    }

    public static int GetPosition(float time)
    {
        var all = LoadAllScores();
        all.Add(("TEMP", time));
        all.Sort((a, b) => a.time.CompareTo(b.time));

        for (int i = 0; i < all.Count; i++)
        {
            if (all[i].code == "TEMP") return i + 1;
        }

        return -1;
    }
}

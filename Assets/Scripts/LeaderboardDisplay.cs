using TMPro;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;

[System.Serializable]
public class LeaderboardEntry
{
    public string teamCode;
    public float time;
}

public class LeaderboardDisplay : MonoBehaviour
{
    public TMP_Text leaderboardText;
    private DatabaseReference dbRef;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                ListenToLeaderboardChanges();
            }
            else
            {
                leaderboardText.text = "Failed to initialize Firebase.";
                Debug.LogError("[LeaderboardDisplay] Firebase initialization failed: " + dependencyStatus);
            }
        });
    }

    void ListenToLeaderboardChanges()
    {
        dbRef.Child("leaderboard").OrderByChild("time").ValueChanged += OnLeaderboardChanged;
    }

    private void OnLeaderboardChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError("[LeaderboardDisplay] Database error: " + args.DatabaseError.Message);
            leaderboardText.text = "Error loading leaderboard.";
            return;
        }

        if (!args.Snapshot.Exists) return;

        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        foreach (DataSnapshot child in args.Snapshot.Children)
        {
            LeaderboardEntry entry = JsonUtility.FromJson<LeaderboardEntry>(child.GetRawJsonValue());
            if (entry != null)
            {
                entries.Add(entry);
            }
        }

        // Sort and take top 3
        entries.Sort((a, b) => a.time.CompareTo(b.time));
        if (entries.Count > 3)
            entries = entries.GetRange(0, 3);

        UpdateLeaderboardUI(entries);
    }

    void UpdateLeaderboardUI(List<LeaderboardEntry> entries)
    {
        string result = "Top 3 Times:\n";

        for (int i = 0; i < entries.Count; i++)
        {
            string timeFormatted = System.TimeSpan.FromSeconds(entries[i].time).ToString(@"mm\:ss\:ff");
            result += $"{i + 1}. {entries[i].teamCode} - {timeFormatted}\n";
        }

        leaderboardText.text = result;
        Debug.Log("[LeaderboardDisplay] Leaderboard updated:\n" + result);
    }
}

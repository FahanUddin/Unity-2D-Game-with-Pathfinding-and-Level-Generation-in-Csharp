using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseLeaderboard : MonoBehaviour
{
    public static FirebaseLeaderboard Instance { get; private set; }

    public DatabaseReference dbRef { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                if (task.Result == DependencyStatus.Available)
                {
                    dbRef = FirebaseDatabase.DefaultInstance.RootReference;
                    Debug.Log("[FirebaseLeaderboard] Firebase initialized successfully.");
                }
                else
                {
                    Debug.LogError("[FirebaseLeaderboard] Firebase not available: " + task.Result);
                }
            });
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveScore(string teamCode, float time)
    {
        if (dbRef == null)
        {
            Debug.LogError("[FirebaseLeaderboard] dbRef is null!");
            return;
        }

        LeaderboardEntry entry = new LeaderboardEntry { teamCode = teamCode, time = time };
        string json = JsonUtility.ToJson(entry);

        dbRef.Child("leaderboard").Child(teamCode).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"[FirebaseLeaderboard] Score saved: {teamCode} - {time}");
            }
            else
            {
                Debug.LogError("[FirebaseLeaderboard] Failed to save score.");
            }
        });
    }
}

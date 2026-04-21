using UnityEngine;
using TMPro;
using System.Collections;

public class TimerTrigger : MonoBehaviour
{
    public enum TimerTriggerType { Start, Finish }
    public TimerTriggerType triggerType;

    [Header("UI")]
    public TMP_Text resultText; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        TimerManager timerManager = FindObjectOfType<TimerManager>();
        if (timerManager == null) return;

        if (triggerType == TimerTriggerType.Start)
        {
            timerManager.StartTimer();
        }
        else if (triggerType == TimerTriggerType.Finish)
        {
            timerManager.StopTimer();

            float time = timerManager.GetFinalTimeInSeconds();
            string formattedTime = System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss\:ff");

            int position = LeaderboardManager.GetPosition(time);
            string teamCode = TeamCodeGenerator.teamCode;

            // Save to local leaderboard if not already there
            var scores = LeaderboardManager.LoadAllScores();
            if (!scores.Exists(e => e.code == teamCode))
            {
                //LeaderboardManager.SaveScore(teamCode, time);
                Debug.Log("Saving to Firebase");
                StartCoroutine(SaveToFirebaseWhenReady(teamCode, time));
            }

            // Save to Firebase asynchronously
            //StartCoroutine(SaveToFirebaseWhenReady(teamCode, time));

            // Show the result message
            if (resultText != null)
            {
                resultText.text = $" {teamCode} finished in <b>{Ordinal(position)}</b> place with a time of <b>{formattedTime}</b>!";
            }
        }
    }

    private string Ordinal(int number)
    {
        if (number % 100 >= 11 && number % 100 <= 13)
            return number + "th";

        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }

    private IEnumerator SaveToFirebaseWhenReady(string teamCode, float time)
    {
        float timeout = 10f;
        float elapsed = 0f;

        while ((FirebaseLeaderboard.Instance == null || FirebaseLeaderboard.Instance.dbRef == null) && elapsed < timeout)
        {
            Debug.Log("Waiting for Firebase initialization...");
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
        }

        if (FirebaseLeaderboard.Instance != null && FirebaseLeaderboard.Instance.dbRef != null)
        {
            Debug.Log("Firebase ready, saving...");
            FirebaseLeaderboard.Instance.SaveScore(teamCode, time);
            Debug.LogWarning("Saved to Firebase");
        }
        else
        {
            Debug.LogWarning("[TimerTrigger] Firebase not ready after timeout.");
        }
    }

}

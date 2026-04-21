using UnityEngine;
using TMPro;

public class TimerManager : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    private bool timerRunning = false;
    private bool timerHasStarted = false;
    private float elapsedTime = 0f;

    public static TimerManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (timerRunning)
        {
            elapsedTime += Time.deltaTime;
            System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(elapsedTime);
            if (timerText != null)
            {
                timerText.text = timeSpan.ToString(@"mm\:ss\:ff");
            }
        }
    }
    public float GetFinalTimeInSeconds()
    {
        return elapsedTime;
    }


    public void StartTimer()
    {
        if (timerHasStarted)
            return;

        elapsedTime = 0f;
        timerRunning = true;
        timerHasStarted = true;
        Debug.Log("Timer Started");
    }

    public void StopTimer()
    {
        timerRunning = false;
        System.TimeSpan finalTime = System.TimeSpan.FromSeconds(elapsedTime);
        Debug.Log("Timer Stopped. Final Time: " + finalTime.ToString(@"mm\:ss\:fff"));


        if (FirebaseLeaderboard.Instance != null && FirebaseLeaderboard.Instance.dbRef != null)
        {
            FirebaseLeaderboard.Instance.SaveScore(TeamCodeGenerator.teamCode, elapsedTime);
            Debug.LogWarning("Data Saved to firestore");
        }
        else
        {
            Debug.LogWarning("Firebase not ready. Could not save score.");
        }

    }
}

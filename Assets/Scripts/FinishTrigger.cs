using UnityEngine;
using TMPro;

public class FinishTrigger : MonoBehaviour
{
    public TimerManager timerManager;
    public TMP_Text resultText;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            timerManager.StopTimer();

            float time = timerManager.GetFinalTimeInSeconds();
            int position = LeaderboardManager.GetPosition(time);
            string teamCode = TeamCodeGenerator.teamCode;

            LeaderboardManager.SaveScore(teamCode, time);

            string formattedTime = System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss\:ff");
            resultText.text = $"⏱️ {teamCode} finished in <b>{Ordinal(position)}</b> place with a time of <b>{formattedTime}</b>!";
        }
    }

    private string Ordinal(int number)
    {
        if (number <= 0) return number.ToString();
        if (number % 100 >= 11 && number % 100 <= 13) return number + "th";
        switch (number % 10)
        {
            case 1: return number + "st";
            case 2: return number + "nd";
            case 3: return number + "rd";
            default: return number + "th";
        }
    }
}

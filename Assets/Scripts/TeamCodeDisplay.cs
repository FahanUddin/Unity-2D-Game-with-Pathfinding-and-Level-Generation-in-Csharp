using TMPro;
using UnityEngine;

public class TeamCodeDisplay : MonoBehaviour
{
    public TMP_Text codeText;

    void Start()
    {
        if (TeamManager.instance != null)
        {
            codeText.text = TeamManager.instance.GetTeamCode();
        }
    }

    void Update()
    {
        if (TeamManager.instance != null && !string.IsNullOrEmpty(TeamManager.instance.GetTeamCode()))
        {
            codeText.text = TeamManager.instance.GetTeamCode();
        }
    }
}

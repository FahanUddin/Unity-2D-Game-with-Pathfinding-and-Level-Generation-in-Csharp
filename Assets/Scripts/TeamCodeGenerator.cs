using UnityEngine;

public static class TeamCodeGenerator
{
    public static string teamCode { get; private set; }

    public static string GenerateTeamCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string result = "TEAM-";
        for (int i = 0; i < 6; i++)
        {
            result += chars[Random.Range(0, chars.Length)];
        }

        teamCode = result;
        Debug.Log($"Generated Team Code: {teamCode}");
        return teamCode;

    }
}



using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
public class MainMenuManager : MonoBehaviour
{
    public GameObject mainButtons;          // Assign MainButtons object
    public GameObject multiplayerButtons;   // Assign MultiplayerButtons object

    void Start()
    {
        mainButtons.SetActive(true);
        multiplayerButtons.SetActive(false);
    }
    public void StartSinglePlayer()
    {
        Debug.Log("Starting Single Player...");
        SceneManager.LoadScene("SinglePlayer");
    }

    public void ShowMultiplayerOptions()
    {
        SceneManager.LoadScene("MultiPlayer");
        Debug.Log("Opening Multiplayer Options");
        mainButtons.SetActive(false);
        multiplayerButtons.SetActive(true);
    }

    public void HostGame()
    {
        Debug.Log("Hosting Game.");
        NetworkManager.Singleton.StartHost();
        SpawnTeamManager();
        SceneManager.LoadScene("MultiPlayer");
    }

    private void SpawnTeamManager()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            GameObject teamManagerPrefab = (GameObject)Resources.Load("GameManager");
            GameObject teamManagerInstance = Instantiate(teamManagerPrefab);
            teamManagerInstance.GetComponent<NetworkObject>().Spawn();
        }
    }

    public void JoinGame()
    {
        Debug.Log("Joining Game");
        NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("MultiPlayer");
    }

    public void BackToMain()
    {
        multiplayerButtons.SetActive(false);
        mainButtons.SetActive(true);
    }
}

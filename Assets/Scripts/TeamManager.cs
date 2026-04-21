using Unity.Netcode;
using UnityEngine;

public struct NetworkString : INetworkSerializable
{
    public string Value;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Value);
    }
}

public class TeamManager : NetworkBehaviour
{
    public static TeamManager instance;

    public NetworkVariable<NetworkString> teamCode = new NetworkVariable<NetworkString>(
        new NetworkString { Value = "" },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("[TeamManager] Duplicate detected, disabling duplicate!");

            // If this is a duplicate spawned on the client, simply disable it
            gameObject.SetActive(false);
        }
    }


    private void Start()
    {
        // Only generate if it's empty
        if (string.IsNullOrEmpty(teamCode.Value.Value))
        {
            string generatedCode = TeamCodeGenerator.GenerateTeamCode();
            Debug.Log($"[TeamManager] Generated team code at Start: {generatedCode}");

            if (IsServer || !NetworkManager.Singleton.IsListening)
            {
                // Singleplayer (no network) or server can set it
                teamCode.Value = new NetworkString { Value = generatedCode };
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log($"[DEBUG] Current team code = {teamCode.Value.Value}");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && string.IsNullOrEmpty(teamCode.Value.Value))
        {
            string generatedCode = TeamCodeGenerator.GenerateTeamCode();
            Debug.Log($"[TeamManager] Generated team code on spawn: {generatedCode}");
            teamCode.Value = new NetworkString { Value = generatedCode };
        }
    }

    public string GetTeamCode()
    {
        return teamCode.Value.Value;
    }
}



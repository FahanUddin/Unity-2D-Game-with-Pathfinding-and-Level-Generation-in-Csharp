using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkConnectScript : MonoBehaviour


{
    [SerializeField] private Button clientButton;
    [SerializeField] private Button hostButton;
    
    void Start()
    {
        clientButton.onClick.AddListener(ClientButtonClick);
        hostButton.onClick.AddListener(HostButtonClick);

    }

    private void HostButtonClick()
    {
        NetworkManager.Singleton.StartHost();
    }
    private void ClientButtonClick()
    {
        NetworkManager.Singleton.StartClient();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

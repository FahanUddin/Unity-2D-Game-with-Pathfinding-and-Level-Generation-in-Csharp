using UnityEngine;

public class FirebaseBootstrap : MonoBehaviour
{
    public GameObject firebaseManagerPrefab;

    void Awake()
    {
        Debug.Log("[FirebaseBootstrap] Awake triggered");

        if (FindObjectOfType<FirebaseLeaderboard>() == null)
        {
            Instantiate(firebaseManagerPrefab);
            Debug.Log("[FirebaseBootstrap] FirebaseManager instantiated.");
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

public class UIHealthScript : MonoBehaviour
{
    public static UIHealthScript instance;

    public Image player1HealthBar;
    public Image player2HealthBar;

    private void Awake()
    {
        instance = this;
    }
}

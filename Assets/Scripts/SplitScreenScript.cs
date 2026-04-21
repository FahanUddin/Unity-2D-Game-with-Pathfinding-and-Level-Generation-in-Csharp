using UnityEngine;

public class SplitScreenScript : MonoBehaviour
{
    public enum ScreenSide { Left, Right, Fullscreen }
    public ScreenSide side;

    void Start()
    {
        Camera cam = GetComponent<Camera>();
        if (cam == null)
            return;

        switch (side)
        {
            case ScreenSide.Left:
                // Left half of the screen.
                cam.rect = new Rect(0f, 0f, 0.5f, 1f);
                break;
            case ScreenSide.Right:
                // Right half of the screen.
                cam.rect = new Rect(0.5f, 0f, 0.5f, 1f);
                break;
            case ScreenSide.Fullscreen:
                // Full screen.
                cam.rect = new Rect(0f, 0f, 1f, 1f);
                break;
        }
    }
}

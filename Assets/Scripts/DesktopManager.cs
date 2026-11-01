using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [Header("Double Click Settings")]
    public float doubleClickTime = 0.3f;

    private float lastClickTime;

    [Header("References")]
    public Image merlionTimesImage;

    private void Awake()
    {
        LaptopPuzzle.IsPuzzleOpen = true;
    }

    public void OpenWindow(GameObject targetPanel)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickTime)
        {
            CloseWindows(targetPanel);

            targetPanel.SetActive(true);
        }

        lastClickTime = Time.time;
    }

    public void CloseWindows(GameObject targetPanel)
    {
        targetPanel.SetActive(false);
        LaptopPuzzle.IsPuzzleOpen = false;
    }
}

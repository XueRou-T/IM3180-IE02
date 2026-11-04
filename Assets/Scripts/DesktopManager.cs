using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [Header("Double Click Settings")]
    public float doubleClickTime = 0.3f;

    private float lastClickTime;

    [Header("References")]
    public EndGameManager endGameManager;
    public LaptopPuzzle puzzlePanel;

    private void Awake()
    {
        puzzlePanel.IsPuzzleOpen = true;
        endGameManager.closeDesktopTrigger = false;
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
        puzzlePanel.IsPuzzleOpen = false;

        if (endGameManager.merlionTimesTrigger && endGameManager.journalTrigger && endGameManager.videoTrigger && endGameManager.closeDesktopTrigger)
        {
            endGameManager.StartEndDialogue();
        }
    }
}

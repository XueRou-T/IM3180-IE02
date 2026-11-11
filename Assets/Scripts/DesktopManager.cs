using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DesktopManager : MonoBehaviour
{
    [Header("Double Click Settings")]
    public float doubleClickTime = 0.3f;

    private float lastClickTime;
    private GameObject lastClickedPanel;

    [Header("References")]
    public EndGameManager endGameManager;

    [Header("LaptopReferences")]
    public GameObject puzzlePanel;
    public GameObject thisPanel;

    public void OpenWindow(GameObject targetPanel, int panelAssignedNumber)
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (lastClickedPanel == targetPanel && timeSinceLastClick <= doubleClickTime)
        {
            CloseWindows(targetPanel);
            targetPanel.SetActive(true);

            if (endGameManager != null)
            {
                endGameManager.TriggerSetFor(panelAssignedNumber);
            }

            lastClickTime = 0f;
            lastClickedPanel = null;
            return;
        }

        lastClickTime = Time.time;
        lastClickedPanel = targetPanel;
    }

    public void CloseWindows(GameObject targetPanel)
    {
        targetPanel.SetActive(false);
    }

    public void merlionButton(GameObject targetPanel)
    {
        OpenWindow(targetPanel, 0);
    }

    public void videoButton(GameObject targetPanel)
    {
        OpenWindow(targetPanel, 1);
    }

    public void journalButton(GameObject targetPanel)
    {
        OpenWindow(targetPanel, 2);
    }

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().desktopOpen = true;
    }

    private void OnDisable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().desktopOpen = false;
    }

    public void CloseAllWindows()
    {
        if(thisPanel != null && puzzlePanel != null)
        {
            puzzlePanel.SetActive(false);
            thisPanel.SetActive(false);
        }
    }
}

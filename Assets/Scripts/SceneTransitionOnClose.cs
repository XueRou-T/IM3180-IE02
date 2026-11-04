using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionOnClose : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Exact name of the puzzle scene in your Build Settings")]
    public string puzzleSceneName = "PuzzleScene";

    private bool wasOpenLastFrame = false;

    void Update()
    {
        // Check if this GameObject is active right now
        bool isOpen = gameObject.activeSelf;

        // Detect the exact frame the popup transitions from Active -> Inactive (closed)
        if (wasOpenLastFrame && !isOpen)
        {
            OnPopupClosed();
        }

        wasOpenLastFrame = isOpen;
    }

    void OnPopupClosed()
    {
        Debug.Log("[SceneTransition] Popup closed! Loading puzzle scene: " + puzzleSceneName);
        SceneManager.LoadScene(puzzleSceneName);
    }
}
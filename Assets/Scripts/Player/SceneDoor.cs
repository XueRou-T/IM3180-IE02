using UnityEngine;
using TMPro;

public class SceneDoor : MonoBehaviour
{
    public TMP_Text promptText;
    public string promptMessage = "Press E to leave";
    public string lockedMessage = "It's locked. I need everything first.";
    public GameObject choicePanelObject; // drag DestinationChoicePanel here

    [Header("Lock Settings")]
    public bool requiresUnlock = true;
    public string[] requiredItems;

    private bool playerInRange = false;
    private bool isUnlocked = false;
    private DestinationChoice choiceScript;

    void Start()
    {
        if (!requiresUnlock) isUnlocked = true;
        if (promptText != null) promptText.text = promptMessage;

        choiceScript = UIPersistence.Instance != null ? UIPersistence.Instance.destinationChoice : null;
    }

    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !DestinationChoice.IsChoiceOpen)
        {
            if (isUnlocked)
            {
                choiceScript.OpenChoice();
            }
            else
            {
                ShowLockedMessage();
            }
        }
    }

    void CheckUnlockCondition()
    {
        if (GameManager.Instance == null) return;

        bool allFound = true;
        foreach (string item in requiredItems)
        {
            if (!GameManager.Instance.HasItem(item))
            {
                allFound = false;
                break;
            }
        }

        if (allFound) isUnlocked = true;
    }

    void ShowLockedMessage()
    {
        if (promptText != null)
        {
            promptText.text = lockedMessage;
            CancelInvoke(nameof(ResetPromptText));
            Invoke(nameof(ResetPromptText), 1.5f);
        }
    }

    void ResetPromptText()
    {
        promptText.text = promptMessage;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptText != null) promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptText != null) promptText.gameObject.SetActive(false);
        }
    }
}
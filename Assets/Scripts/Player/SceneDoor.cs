using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    public TMP_Text promptText;
    public string promptMessage = "Press E to leave";
    public string lockedMessage = "It's locked. I need everything first.";

    [Header("Lock Settings")]
    public bool requiresUnlock = true;
    public string[] requiredItems;

    [Header("Home Destination Settings")]
    public string homeSceneName = "House";   // Name of your home scene

    private bool playerInRange = false;
    private bool isUnlocked = false;
    private DestinationChoice choiceScript;

    void Start()
    {
        if (!requiresUnlock) isUnlocked = true;
        if (promptText != null) promptText.text = promptMessage;

        // Pull the persistent DestinationChoice reference securely
        if (UIPersistence.Instance != null)
        {
            choiceScript = UIPersistence.Instance.destinationChoice;
        }
    }

    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        bool isAddressActive = UIPersistence.Instance != null &&
                               UIPersistence.Instance.houseAddressPopup != null &&
                               UIPersistence.Instance.houseAddressPopup.activeSelf;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!DestinationChoice.IsChoiceOpen && !isAddressActive)
            {
                if (isUnlocked)
                {
                    if (choiceScript != null)
                    {
                        choiceScript.OpenChoice(this);
                    }
                    else
                    {
                        Debug.LogError("[Door Error] choiceScript is NULL! UIPersistence didn't link the DestinationChoice reference.");
                    }
                }
                else
                {
                    ShowLockedMessage();
                }
            }
        }
    }

    // Called when clicking the Home option in the Destination Choice panel
    public void SelectHomeOption()
    {
        if (choiceScript != null)
        {
            if (choiceScript.gameObject != null) choiceScript.gameObject.SetActive(false);
            DestinationChoice.IsChoiceOpen = false;
        }

        // Trigger the persistent address panel directly from UIPersistence
        if (UIPersistence.Instance != null && UIPersistence.Instance.houseAddressPopup != null)
        {
            GameObject addressPopup = UIPersistence.Instance.houseAddressPopup;
            addressPopup.SetActive(true);

            // Clear fields when opening
            TMP_InputField[] inputs = addressPopup.GetComponentsInChildren<TMP_InputField>(true);
            foreach (var input in inputs)
            {
                input.text = "";
            }

            TMP_Text feedback = addressPopup.GetComponentInChildren<TMP_Text>(true);
            if (feedback != null && feedback.gameObject.name.Contains("Feedback"))
            {
                feedback.text = "";
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
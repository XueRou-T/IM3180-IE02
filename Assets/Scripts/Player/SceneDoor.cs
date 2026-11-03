using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SceneDoor : MonoBehaviour
{
    public TMP_Text promptText;
    public string promptMessage = "Press E to leave";
    public string lockedMessage = "It's locked. I need everything first.";
    public GameObject choicePanelObject; // drag DestinationChoicePanel here

    [Header("Lock Settings")]
    public bool requiresUnlock = true;
    public string[] requiredItems;

    [Header("Address Verification (For Home Destination)")]
    public GameObject addressInputPanel;         // Drag your House Address Popup here
    public TMP_InputField addressInputField;     // Drag your AddressAnswer input field here
    public TMP_InputField postalInputField;      // Drag your PostalCodeAnswer input field here
    public TMP_Text feedbackText;                // Drag your "incorrect address!" text here

    [Header("Correct Answers")]
    [TextArea] public string correctAddress = "123 Seventh Avenue #07-67";
    public string correctPostalCode = "676767";
    public string homeSceneName = "House";   // Name of your home scene

    private bool playerInRange = false;
    private bool isUnlocked = false;
    private DestinationChoice choiceScript;

    void Start()
    {
        if (!requiresUnlock) isUnlocked = true;
        if (promptText != null) promptText.text = promptMessage;
        if (addressInputPanel != null) addressInputPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";

        choiceScript = UIPersistence.Instance != null ? UIPersistence.Instance.destinationChoice : null;
    }

    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        bool isAddressActive = addressInputPanel != null && addressInputPanel.activeSelf;

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !DestinationChoice.IsChoiceOpen && !isAddressActive)
        {
            if (isUnlocked)
            {
                if (choiceScript != null) choiceScript.OpenChoice();
            }
            else
            {
                ShowLockedMessage();
            }
        }
    }

    // --- 1. HOOK THIS TO YOUR "HOME" BUTTON IN THE DESTINATION CHOICE PANEL ---
    public void SelectHomeOption()
    {
        // Close the choice panel if open
        if (choiceScript != null)
        {
            if (choiceScript.gameObject != null) choiceScript.gameObject.SetActive(false);
            DestinationChoice.IsChoiceOpen = false;
        }

        // Open the Address Input Panel and clear previous text/feedback
        if (addressInputPanel != null) addressInputPanel.SetActive(true);
        if (addressInputField != null) addressInputField.text = "";
        if (postalInputField != null) postalInputField.text = "";
        if (feedbackText != null) feedbackText.text = "";
    }

    // --- 2. HOOK THIS TO YOUR SUBMIT/ENTER BUTTON IN THE ADDRESS INPUT PANEL ---
    public void SubmitAddress()
    {
        if (addressInputField == null || postalInputField == null) return;

        // Clean up inputs and correct answers (ignoring extra spaces & capitalization)
        string playerAddress = addressInputField.text.Trim().ToLower();
        string playerPostal = postalInputField.text.Trim().ToLower();

        string targetAddress = correctAddress.Trim().ToLower();
        string targetPostal = correctPostalCode.Trim().ToLower(); // Uses correctPostalCode variable

        // Check if both the address AND the postal code match
        if (playerAddress == targetAddress && playerPostal == targetPostal)
        {
            if (feedbackText != null) feedbackText.text = "Success!";
            CloseAllPanels();
            SceneManager.LoadScene(homeSceneName);
        }
        else
        {
            if (feedbackText != null) feedbackText.text = "incorrect address!";
        }
    }

    public void CloseAllPanels()
    {
        if (addressInputPanel != null) addressInputPanel.SetActive(false);
        if (choiceScript != null && choiceScript.gameObject != null) choiceScript.gameObject.SetActive(false);
        DestinationChoice.IsChoiceOpen = false;
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

            // ONLY close panels if the address typing panel or choice panel is NOT active
            bool isAddressActive = addressInputPanel != null && addressInputPanel.activeSelf;
            if (!isAddressActive && !DestinationChoice.IsChoiceOpen)
            {
                CloseAllPanels();
            }
        }
    }
}
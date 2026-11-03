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
        if (feedbackText != null) feedbackText.text = "";

        // Pull UI references dynamically from the persistent manager
        if (UIPersistence.Instance != null)
        {
            choiceScript = UIPersistence.Instance.destinationChoice;

            // Automatically hook up the persistent address panel if not already assigned
            if (addressInputPanel == null && UIPersistence.Instance.houseAddressPopup != null)
            {
                addressInputPanel = UIPersistence.Instance.houseAddressPopup;

                // Re-grab input fields from the persistent panel if they are inside it
                if (addressInputField == null) addressInputField = addressInputPanel.GetComponentInChildren<TMP_InputField>();
                // Note: If you have separate input fields, assign them or find them here
            }
        }

        if (addressInputPanel != null) addressInputPanel.SetActive(false);
    }


    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        bool isAddressActive = addressInputPanel != null && addressInputPanel.activeSelf;

        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // DIAGNOSTIC LOG: Tells us why the door is or isn't responding when 'E' is pressed
            Debug.Log($"[Door Check] Pressed E on '{gameObject.name}' in scene '{SceneManager.GetActiveScene().name}'. " +
                      $"PlayerInRange: {playerInRange}, IsUnlocked: {isUnlocked}, ChoiceOpen: {DestinationChoice.IsChoiceOpen}, AddressActive: {isAddressActive}");

            if (!DestinationChoice.IsChoiceOpen && !isAddressActive)
            {
                if (isUnlocked)
                {
                    if (choiceScript != null)
                    {
                        Debug.Log("[Door Check] choiceScript found! Opening Choice Panel successfully.");
                        choiceScript.OpenChoice(this);
                    }
                    else
                    {
                        Debug.LogError("[Door Error] choiceScript is NULL! UIPersistence didn't link the DestinationChoice reference.");
                    }
                }
                else
                {
                    Debug.Log("[Door Check] Door is locked! Showing locked message.");
                    ShowLockedMessage();
                }
            }
            else
            {
                Debug.Log("[Door Check] Interaction blocked because a panel is already open.");
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
            // DIAGNOSTIC LOG: Confirms when the trigger area actually registers the player
            Debug.Log($"[Door Trigger] Player entered trigger on door: {gameObject.name}");
            if (promptText != null) promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            Debug.Log($"[Door Trigger] Player exited trigger on door: {gameObject.name}");
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
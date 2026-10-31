using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // --- ADDED: Required to control UI Buttons in code ---
using TMPro;

public class MerchantInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TMP_Text promptText;
    public GameObject promptContainer;
    public string promptMessage = "Press E to talk";

    [Header("Merchant Dialogue")]
    public DialogueSystem.Line[] merchantLines;

    [Header("UI Choices")]
    public GameObject choicePanel;
    public Button yesButton; // --- ADDED: Slot for the Yes button ---
    public Button noButton;  // --- ADDED: Slot for the No button ---

    [Header("Item Reward")]
    public UnityEvent onAcceptItem;

    private bool playerInRange = false;

    void Start()
    {
        if (promptText != null) promptText.text = promptMessage;
        if (promptContainer != null) promptContainer.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.E) && !HawkerDialogueManager.IsDialogueOpen && !choicePanel.activeSelf)
        {
            if (promptContainer != null) promptContainer.SetActive(false);
            HawkerDialogueManager.Instance.StartDialogue(merchantLines, ShowChoicePanel);
        }

        if (!HawkerDialogueManager.IsDialogueOpen && !choicePanel.activeSelf && promptContainer != null && !promptContainer.activeSelf)
        {
            promptContainer.SetActive(true);
        }
    }

    void ShowChoicePanel()
    {
        choicePanel.SetActive(true);
        HawkerDialogueManager.IsDialogueOpen = true;

        // --- ADDED: Tell the buttons to trigger THIS specific merchant ---
        // First, clear out any leftover connections from the last merchant you spoke to
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        // Connect the buttons to this specific merchant's Accept/Decline functions
        yesButton.onClick.AddListener(AcceptOffer);
        noButton.onClick.AddListener(DeclineOffer);
    }

    public void AcceptOffer()
    {
        onAcceptItem.Invoke();
        CloseChoice();
    }

    public void DeclineOffer()
    {
        CloseChoice();
    }

    void CloseChoice()
    {
        choicePanel.SetActive(false);
        HawkerDialogueManager.IsDialogueOpen = false;

        if (playerInRange && promptContainer != null) promptContainer.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptContainer != null && !HawkerDialogueManager.IsDialogueOpen && !choicePanel.activeSelf)
            {
                promptContainer.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
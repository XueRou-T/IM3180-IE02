using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MerchantInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TMP_Text promptText;
    public GameObject promptContainer;
    public string promptMessage = "Press E to talk";

    [Header("Merchant Dialogue")]
    public DialogueSystem.Line[] merchantLines;
    public DialogueSystem.Line[] alreadyHaveLines; // What to say if they already have it

    [Header("UI Choices")]
    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    [Header("Item Reward")]
    public string itemID; // The name of the food to check in the inventory
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

            // 1. Check if the player already has the item
            if (GameManager.Instance != null && !string.IsNullOrEmpty(itemID) && GameManager.Instance.HasItem(itemID))
            {
                // Play the rejection dialogue. Passing 'null' means the Yes/No panel WON'T appear after.
                HawkerDialogueManager.Instance.StartDialogue(alreadyHaveLines, null);
            }
            else
            {
                // 2. Player doesn't have it. Play normal dialogue and show Yes/No panel at the end.
                HawkerDialogueManager.Instance.StartDialogue(merchantLines, ShowChoicePanel);
            }
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
using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    public TMP_Text promptText;
    public GameObject promptContainer; // Added: Reference to the background image
    public string promptMessage = "Press E to talk";
    public DialogueSystem.Line[] dialogueLines;

    private bool playerInRange = false;

    void Start()
    {
        if (promptText != null) promptText.text = promptMessage;

        // Ensure the prompt is hidden when the game starts
        if (promptContainer != null) promptContainer.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !DialogueSystem.IsDialogueOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (UIPersistence.Instance != null && UIPersistence.Instance.dialogueSystem != null)
            {
                UIPersistence.Instance.dialogueSystem.StartDialogue(dialogueLines);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // Show the background (which also shows the text inside it)
            if (promptContainer != null) promptContainer.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Hide the background
            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
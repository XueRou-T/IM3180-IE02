using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TMP_Text promptText;
    public GameObject promptContainer;
    public string promptMessage = "Press E to talk";

    [Tooltip("Check this box to swap text inside the small white bubble above their head. Uncheck it to use the big cinematic dialogue box.")]
    public bool useSimpleOverheadDialogue = false;

    [Header("Dialogue Content")]
    public DialogueSystem.Line[] dialogueLines;

    private bool playerInRange = false;

    // Tracking variables for the overhead dialogue mode
    private bool isTalkingOverhead = false;
    private int currentLineIndex = 0;

    void Start()
    {
        if (promptText != null) promptText.text = promptMessage;
        if (promptContainer != null) promptContainer.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return; // Do nothing if player is far away

        // Decide which dialogue mode to run based on your checkbox
        if (useSimpleOverheadDialogue)
        {
            HandleOverheadDialogue();
        }
        else
        {
            HandleManagerDialogue();
        }
    }

    // --- MODE 1: THE BIG DIALOGUE BOX ---
    private void HandleManagerDialogue()
    {
        if (!HawkerDialogueManager.IsDialogueOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (HawkerDialogueManager.Instance != null)
            {
                HawkerDialogueManager.Instance.StartDialogue(dialogueLines);
                if (promptContainer != null) promptContainer.SetActive(false);
            }
        }

        // Reactivate prompt after the big dialogue box closes
        if (!HawkerDialogueManager.IsDialogueOpen && promptContainer != null && !promptContainer.activeSelf)
        {
            promptContainer.SetActive(true);
        }
    }

    // --- MODE 2: THE OVERHEAD BUBBLE ---
    private void HandleOverheadDialogue()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isTalkingOverhead)
            {
                // Start overhead conversation
                if (dialogueLines.Length > 0)
                {
                    isTalkingOverhead = true;
                    currentLineIndex = 0;
                    UpdateOverheadText();
                }
            }
            else
            {
                // Advance overhead conversation
                currentLineIndex++;
                if (currentLineIndex < dialogueLines.Length)
                {
                    UpdateOverheadText();
                }
                else
                {
                    // End conversation and reset the prompt text
                    isTalkingOverhead = false;
                    if (promptText != null) promptText.text = promptMessage;
                }
            }
        }
    }

    private void UpdateOverheadText()
    {
        if (promptText != null)
        {
            string speaker = dialogueLines[currentLineIndex].speakerName;
            string sentence = dialogueLines[currentLineIndex].sentence;

            // Displays it as "Name: Sentence" inside the little white box
            promptText.text = "<b>" + speaker + "</b>\n" + sentence;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            isTalkingOverhead = false; // Reset overhead state just in case

            if (promptText != null) promptText.text = promptMessage;

            // Show the prompt (but only if the big manager isn't actively talking)
            if (promptContainer != null && (!useSimpleOverheadDialogue ? !HawkerDialogueManager.IsDialogueOpen : true))
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
            isTalkingOverhead = false;

            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
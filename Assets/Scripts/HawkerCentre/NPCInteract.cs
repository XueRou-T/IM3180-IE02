using System.Collections;
using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TMP_Text promptText;
    public GameObject promptContainer;
    public string promptMessage = "Press E to talk";

    [Tooltip("Check this box to use the small timed bubble. Uncheck to use the big cinematic dialogue box.")]
    public bool useSimpleOverheadDialogue = false;

    [Header("Dialogue Content")]
    public DialogueSystem.Line[] dialogueLines;

    private bool playerInRange = false;
    private bool isTalkingOverhead = false;
    private int currentLineIndex = 0;
    private Coroutine overheadRoutine;

    void Start()
    {
        if (promptText != null) promptText.text = promptMessage;
        if (promptContainer != null) promptContainer.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        if (useSimpleOverheadDialogue)
        {
            HandleOverheadDialogue();
        }
        else
        {
            HandleManagerDialogue();
        }
    }

    // --- MODE 1: THE BIG DIALOGUE BOX (MANUAL) ---
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

        if (!HawkerDialogueManager.IsDialogueOpen && promptContainer != null && !promptContainer.activeSelf)
        {
            promptContainer.SetActive(true);
        }
    }

    // --- MODE 2: THE OVERHEAD BUBBLE (AUTOMATIC) ---
    private void HandleOverheadDialogue()
    {
        // Only require 'E' to start the conversation, not to advance it
        if (Input.GetKeyDown(KeyCode.E) && !isTalkingOverhead)
        {
            if (dialogueLines.Length > 0)
            {
                overheadRoutine = StartCoroutine(PlayOverheadAuto());
            }
        }
    }

    private IEnumerator PlayOverheadAuto()
    {
        isTalkingOverhead = true;

        for (int i = 0; i < dialogueLines.Length; i++)
        {
            currentLineIndex = i;
            UpdateOverheadText();

            // This is what makes the overhead bubble wait automatically!
            yield return new WaitForSeconds(dialogueLines[i].displayTime);
        }

        // End conversation and reset the prompt text
        isTalkingOverhead = false;
        if (promptText != null) promptText.text = promptMessage;
    }

    private void UpdateOverheadText()
    {
        if (promptText != null)
        {
            string speaker = dialogueLines[currentLineIndex].speakerName;
            string sentence = dialogueLines[currentLineIndex].sentence;

            promptText.text = "<b>" + speaker + "</b>\n" + sentence;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            isTalkingOverhead = false;

            if (promptText != null) promptText.text = promptMessage;

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

            // Forcefully stop the automatic conversation if the player walks away
            if (overheadRoutine != null)
            {
                StopCoroutine(overheadRoutine);
                overheadRoutine = null;
            }

            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
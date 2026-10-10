using UnityEngine;
using TMPro;

public class NPCInteract : MonoBehaviour
{
    public TMP_Text promptText;
    public string promptMessage = "Press E to talk";
    public DialogueSystem.Line[] dialogueLines;

    private bool playerInRange = false;

    void Start()
    {
        if (promptText != null) promptText.text = promptMessage;
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
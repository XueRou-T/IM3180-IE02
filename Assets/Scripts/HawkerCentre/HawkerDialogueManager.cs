using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HawkerDialogueManager : MonoBehaviour
{
    public static HawkerDialogueManager Instance;

    public static bool IsDialogueOpen = false;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public Image speakerPortraitImage;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private DialogueSystem.Line[] currentLines;
    private int index = 0;
    private bool isTyping = false;

    void Awake()
    {
        if (Instance == null) Instance = this;

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialogueOpen) return;

        // Pressing E or Space advances the dialogue
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = currentLines[index].sentence;
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueSystem.Line[] lines)
    {
        currentLines = lines;
        index = 0;
        dialoguePanel.SetActive(true);
        IsDialogueOpen = true;
        ShowLine();
    }

    void NextLine()
    {
        index++;
        if (index < currentLines.Length)
        {
            ShowLine();
        }
        else
        {
            EndDialogue();
        }
    }

    void ShowLine()
    {
        dialogueText.text = "";
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueOpen = false;
        index = 0;
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        DialogueSystem.Line current = currentLines[index];

        if (speakerNameText != null)
            speakerNameText.text = current.speakerName;

        if (speakerPortraitImage != null)
        {
            if (current.speakerPortrait != null)
            {
                speakerPortraitImage.gameObject.SetActive(true);
                speakerPortraitImage.sprite = current.speakerPortrait;
            }
            else
            {
                speakerPortraitImage.gameObject.SetActive(false);
            }
        }

        foreach (char c in current.sentence.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
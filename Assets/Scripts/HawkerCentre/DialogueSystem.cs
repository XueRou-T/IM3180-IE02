using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    [System.Serializable]
    public struct Line
    {
        public string speakerName;
        public Sprite speakerPortrait;
        [TextArea(3, 10)]
        public string sentence;
    }

    public static bool IsDialogueOpen = false;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI speakerNameText;
    public Image speakerPortraitImage;
    public TextMeshProUGUI dialogueText;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    private Line[] currentLines;
    private int index = 0;
    private bool isTyping = false;

    void Update()
    {
        if (!IsDialogueOpen) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
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

    public void StartDialogue(Line[] lines)
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
        Line current = currentLines[index];

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
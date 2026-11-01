using System; // --- ADDED: Required to use Actions (callbacks) ---
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
    public TextMeshProUGUI dialogueText;

    [Header("Dual Portraits")]
    public Image avatarOneImage; // Left Avatar
    public Image avatarTwoImage; // Right Avatar
    [Range(0f, 1f)] public float dimAmount = 0.4f;

    [Header("Settings")]
    public float typingSpeed = 0.03f;

    [Header("UI Overrides")]
    public GameObject inventoryPanel;
    public GameObject itemDetailsPanel;

    private DialogueSystem.Line[] currentLines;
    private int index = 0;
    private bool isTyping = false;

    private Action onDialogueComplete; // --- ADDED: Variable to store the callback ---

    // Tracking who is on which side
    private string speakerOne = "";
    private string speakerTwo = "";
    private int activeAvatarSlot = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void Update()
    {
        if (!IsDialogueOpen) return;

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

    // --- ADDED: "Action onComplete = null" to the method signature ---
    public void StartDialogue(DialogueSystem.Line[] lines, Action onComplete = null)
    {
        currentLines = lines;
        index = 0;

        onDialogueComplete = onComplete; // --- ADDED: Save the requested action ---

        // Reset speakers for new conversation
        speakerOne = "";
        speakerTwo = "";
        avatarOneImage.gameObject.SetActive(false);
        avatarTwoImage.gameObject.SetActive(false);

        // Force close inventory UIs when talking
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailsPanel != null) itemDetailsPanel.SetActive(false);

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
        UpdateSpeakerInfo();
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        IsDialogueOpen = false;
        index = 0;

        // --- ADDED: Trigger the callback to show the choice menu if one was given ---
        if (onDialogueComplete != null)
        {
            onDialogueComplete.Invoke();
            onDialogueComplete = null; // Clear it so it doesn't accidentally fire again later
        }
    }

    void UpdateSpeakerInfo()
    {
        DialogueSystem.Line currentLine = currentLines[index];
        if (speakerNameText != null) speakerNameText.text = currentLine.speakerName;

        string currentSpeaker = currentLine.speakerName;
        Sprite currentAvatar = currentLine.speakerPortrait;

        // Determine which side the current speaker belongs to
        if (speakerOne == currentSpeaker)
        {
            activeAvatarSlot = 1;
            avatarOneImage.sprite = currentAvatar;
        }
        else if (speakerTwo == currentSpeaker)
        {
            activeAvatarSlot = 2;
            avatarTwoImage.sprite = currentAvatar;
        }
        else if (string.IsNullOrEmpty(speakerOne))
        {
            speakerOne = currentSpeaker;
            activeAvatarSlot = 1;
            avatarOneImage.sprite = currentAvatar;
        }
        else if (string.IsNullOrEmpty(speakerTwo))
        {
            speakerTwo = currentSpeaker;
            activeAvatarSlot = 2;
            avatarTwoImage.sprite = currentAvatar;
        }

        UpdateAvatarAppearance();
    }

    void UpdateAvatarAppearance()
    {
        bool showOne = !string.IsNullOrEmpty(speakerOne) && avatarOneImage.sprite != null;
        bool showTwo = !string.IsNullOrEmpty(speakerTwo) && avatarTwoImage.sprite != null;

        avatarOneImage.gameObject.SetActive(showOne);
        avatarTwoImage.gameObject.SetActive(showTwo);

        Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 1f);

        if (showOne)
        {
            avatarOneImage.color = activeAvatarSlot == 1 ? Color.white : dimColor;

            // Keeps avatars at the very top of the Hierarchy (indexes 0 and 1) 
            if (activeAvatarSlot == 1)
            {
                avatarTwoImage.transform.SetSiblingIndex(0);
                avatarOneImage.transform.SetSiblingIndex(1);
            }
        }

        if (showTwo)
        {
            avatarTwoImage.color = activeAvatarSlot == 2 ? Color.white : dimColor;

            if (activeAvatarSlot == 2)
            {
                avatarOneImage.transform.SetSiblingIndex(0);
                avatarTwoImage.transform.SetSiblingIndex(1);
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        foreach (char c in currentLines[index].sentence.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        isTyping = false;
    }
}
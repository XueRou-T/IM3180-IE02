using System;
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

    private Action onDialogueComplete;

    private string speakerOne = "";
    private string speakerTwo = "";
    private int activeAvatarSlot = 0;

    void Awake()
    {
        // Safely destroy duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
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

    public void StartDialogue(DialogueSystem.Line[] lines, Action onComplete = null)
    {
        // --- AUTO-RECONNECT UI ---
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None)
            {
                if (obj.name == "DialogueBox") dialoguePanel = obj;
                if (obj.name == "SpeakerName") speakerNameText = obj.GetComponent<TextMeshProUGUI>();
                if (obj.name == "DialogueText") dialogueText = obj.GetComponent<TextMeshProUGUI>();
                if (obj.name == "Avatar1") avatarOneImage = obj.GetComponent<Image>();
                if (obj.name == "Avatar2") avatarTwoImage = obj.GetComponent<Image>();

                if (obj.name == "InventoryPanel") inventoryPanel = obj;
                if (obj.name == "ItemPopupPanel") itemDetailsPanel = obj;
            }
        }

        currentLines = lines;
        index = 0;
        onDialogueComplete = onComplete; // Stores the choice panel callback from the merchant

        speakerOne = "";
        speakerTwo = "";

        if (avatarOneImage != null) avatarOneImage.gameObject.SetActive(false);
        if (avatarTwoImage != null) avatarTwoImage.gameObject.SetActive(false);

        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        if (itemDetailsPanel != null) itemDetailsPanel.SetActive(false);

        if (dialoguePanel != null) dialoguePanel.SetActive(true);
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
        if (dialogueText != null) dialogueText.text = "";
        UpdateSpeakerInfo();
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        IsDialogueOpen = false;
        index = 0;

        Debug.Log("Dialogue ended. Checking for choice callback..."); // <-- Add this

        if (onDialogueComplete != null)
        {
            Debug.Log("Callback found! Invoking choice panel..."); // <-- Add this
            Action tempAction = onDialogueComplete;
            onDialogueComplete = null;
            tempAction.Invoke();
        }
        else
        {
            Debug.Log("WARNING: onDialogueComplete was null!"); // <-- Add this
        }
    }

    void UpdateSpeakerInfo()
    {
        DialogueSystem.Line currentLine = currentLines[index];
        if (speakerNameText != null) speakerNameText.text = currentLine.speakerName;

        string currentSpeaker = currentLine.speakerName;
        Sprite currentAvatar = currentLine.speakerPortrait;

        if (speakerOne == currentSpeaker)
        {
            activeAvatarSlot = 1;
            if (avatarOneImage != null) avatarOneImage.sprite = currentAvatar;
        }
        else if (speakerTwo == currentSpeaker)
        {
            activeAvatarSlot = 2;
            if (avatarTwoImage != null) avatarTwoImage.sprite = currentAvatar;
        }
        else if (string.IsNullOrEmpty(speakerOne))
        {
            speakerOne = currentSpeaker;
            activeAvatarSlot = 1;
            if (avatarOneImage != null) avatarOneImage.sprite = currentAvatar;
        }
        else if (string.IsNullOrEmpty(speakerTwo))
        {
            speakerTwo = currentSpeaker;
            activeAvatarSlot = 2;
            if (avatarTwoImage != null) avatarTwoImage.sprite = currentAvatar;
        }

        UpdateAvatarAppearance();
    }

    void UpdateAvatarAppearance()
    {
        bool showOne = !string.IsNullOrEmpty(speakerOne) && avatarOneImage != null && avatarOneImage.sprite != null;
        bool showTwo = !string.IsNullOrEmpty(speakerTwo) && avatarTwoImage != null && avatarTwoImage.sprite != null;

        if (avatarOneImage != null) avatarOneImage.gameObject.SetActive(showOne);
        if (avatarTwoImage != null) avatarTwoImage.gameObject.SetActive(showTwo);

        Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 1f);

        if (showOne && avatarOneImage != null)
        {
            avatarOneImage.color = activeAvatarSlot == 1 ? Color.white : dimColor;

            if (activeAvatarSlot == 1 && avatarTwoImage != null)
            {
                avatarTwoImage.transform.SetSiblingIndex(0);
                avatarOneImage.transform.SetSiblingIndex(1);
            }
        }

        if (showTwo && avatarTwoImage != null)
        {
            avatarTwoImage.color = activeAvatarSlot == 2 ? Color.white : dimColor;

            if (activeAvatarSlot == 2 && avatarOneImage != null)
            {
                avatarOneImage.transform.SetSiblingIndex(0);
                avatarTwoImage.transform.SetSiblingIndex(1);
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        if (dialogueText != null)
        {
            foreach (char c in currentLines[index].sentence.ToCharArray())
            {
                dialogueText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
        }
        isTyping = false;
    }
}
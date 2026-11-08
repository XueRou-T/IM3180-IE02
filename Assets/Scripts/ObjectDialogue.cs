using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public struct ObjectDialogueLine
{
    public string speakerName;
    public Sprite speakerAvatar;
    [TextArea(3, 10)]
    public string text;
}

public class ObjectDialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;
    public Image avatarOneImage;
    public Image avatarTwoImage;

    [HideInInspector]
    public System.Action onDialogueFinished;

    [Header("Avatar Appearance")]
    [Range(0f, 1f)]
    public float dimAmount = 0.4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    [Range(1, 5)]
    public int soundFrequency = 2;

    [Header("Dialogue Content")]
    public ObjectDialogueLine[] lines;
    public float textSpeed = 0.04f;

    private string speakerOne = "";
    private string speakerTwo = "";
    private int activeAvatarSlot = 0;
    private int index;
    private Coroutine typingCoroutine;

    void Awake()
    {
        InitializeDialogueBox();
    }

    void Start()
    {
        InitializeDialogueBox();
    }

    private void InitializeDialogueBox()
    {
        if (textComponent != null) textComponent.text = string.Empty;
        if (nameComponent != null) nameComponent.text = string.Empty;

        if (avatarOneImage != null) avatarOneImage.gameObject.SetActive(false);
        if (avatarTwoImage != null) avatarTwoImage.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (lines.Length > 0 && index < lines.Length)
            {
                if (textComponent.text == lines[index].text)
                {
                    NextLine();
                }
                else
                {
                    if (typingCoroutine != null)
                    {
                        StopCoroutine(typingCoroutine);
                    }
                    textComponent.text = lines[index].text;

                    if (audioSource != null)
                    {
                        audioSource.Stop();
                    }
                }
            }
        }
    }

    public void StartDialogue()
    {
        gameObject.SetActive(true);
        index = 0;

        if (avatarTwoImage == null)
        {
            speakerOne = "";
            speakerTwo = "";
        }

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(StartDialogueRoutine());
    }

    // Safely waits 1 frame for the UI canvas to wake up before assigning the first speaker name and typing text
    IEnumerator StartDialogueRoutine()
    {
        yield return null; // Wait 1 frame

        UpdateSpeakerInfo();

        int charCount = 0;
        textComponent.text = string.Empty;

        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
        }

        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;

            if (c != ' ')
            {
                charCount++;
                if (charCount % soundFrequency == 0 && audioSource != null && typingSound != null)
                {
                    audioSource.Play();
                }
            }

            yield return new WaitForSeconds(textSpeed);
        }
    }

    IEnumerator TypeLine()
    {
        int charCount = 0;
        textComponent.text = string.Empty;

        if (audioSource != null && typingSound != null)
        {
            audioSource.clip = typingSound;
        }

        foreach (char c in lines[index].text.ToCharArray())
        {
            textComponent.text += c;

            if (c != ' ')
            {
                charCount++;
                if (charCount % soundFrequency == 0 && audioSource != null && typingSound != null)
                {
                    audioSource.Play();
                }
            }

            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            UpdateSpeakerInfo();

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);

            onDialogueFinished?.Invoke();
            onDialogueFinished = null;
        }
    }

    void UpdateSpeakerInfo()
    {
        if (lines.Length == 0) return;

        ObjectDialogueLine currentLine = lines[index];

        if (nameComponent != null)
        {
            nameComponent.text = currentLine.speakerName;
            nameComponent.SetAllDirty(); // Forces TextMeshPro to refresh layout immediately
        }

        string currentSpeaker = currentLine.speakerName;
        Sprite currentAvatar = currentLine.speakerAvatar;

        if (avatarTwoImage == null)
        {
            speakerOne = currentSpeaker;
            activeAvatarSlot = 1;
            if (avatarOneImage != null) avatarOneImage.sprite = currentAvatar;
            UpdateAvatarAppearance();
            return;
        }

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
        bool showOne = !string.IsNullOrEmpty(speakerOne) && avatarOneImage != null && avatarOneImage.sprite != null;
        bool showTwo = avatarTwoImage != null && !string.IsNullOrEmpty(speakerTwo) && avatarTwoImage.sprite != null;

        if (avatarOneImage != null) avatarOneImage.gameObject.SetActive(showOne);
        if (avatarTwoImage != null) avatarTwoImage.gameObject.SetActive(showTwo);

        Color dimColor = new Color(dimAmount, dimAmount, dimAmount, 1f);

        if (showOne && avatarOneImage != null)
        {
            avatarOneImage.color = (activeAvatarSlot == 1 || avatarTwoImage == null || !showTwo) ? Color.white : dimColor;
        }

        if (showTwo && avatarTwoImage != null)
        {
            avatarTwoImage.color = (activeAvatarSlot == 2) ? Color.white : dimColor;
        }
    }
}
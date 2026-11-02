using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct DialogueLine
{
    public string speakerName;
    public Sprite speakerAvatar;
    [TextArea(3, 10)]
    public string text;
}

public class Dialogue : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameComponent;
    public Image avatarOneImage;
    public Image avatarTwoImage;

    [Header("Avatar Appearance")]
    [Range(0f, 1f)]
    public float dimAmount = 0.4f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    [Range(1, 5)]
    public int soundFrequency = 2;

    [Header("Dialogue Content")]
    public DialogueLine[] lines;
    public float textSpeed;
    public bool haveOption = false;
    public GameObject optionChoice;

   
    private string speakerOne = "";
    private string speakerTwo = "";
    private int activeAvatarSlot = 0;
    private Color normalColor = Color.white;

    [Header("Next Scene")]
    public string nextSceneName;
    public FadeScript fadeImage;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        nameComponent.text = string.Empty;

        avatarOneImage.gameObject.SetActive(false);
        avatarTwoImage.gameObject.SetActive(false);

        if (optionChoice != null)
        {
            optionChoice.SetActive(false);
        }

        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (textComponent.text == lines[index].text)
            {
                    NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].text;

                // Instantly cuts off any lingering audio when the player clicks to skip
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        UpdateSpeakerInfo();
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        int charCount = 0;

        // Assigns the clip once so we can use standard Play() instead of PlayOneShot()
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
                    // .Play() automatically cuts off the currently playing sound before starting again, preventing overlap
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
            if (index == lines.Length - 1)
            {
                if (haveOption && optionChoice != null)
                {
                    optionChoice.SetActive(true);
                }
            }
            StartCoroutine(TypeLine());
        }
        else
        {
            StartCoroutine(LoadNextScene());
        }
    }

    void UpdateSpeakerInfo()
    {
        DialogueLine currentLine = lines[index];

        nameComponent.text = lines[index].speakerName;

        string currentSpeaker = currentLine.speakerName;
        Sprite currentAvatar = currentLine.speakerAvatar;

        if(speakerOne == currentSpeaker)
        {
            activeAvatarSlot = 1;
            avatarOneImage.sprite = currentAvatar;
        }
        else if(speakerTwo == currentSpeaker)
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
        bool showOne = !string.IsNullOrEmpty(speakerOne)
                       && avatarOneImage.sprite != null;

        bool showTwo = !string.IsNullOrEmpty(speakerTwo)
                       && avatarTwoImage.sprite != null;

        avatarOneImage.gameObject.SetActive(showOne);
        avatarTwoImage.gameObject.SetActive(showTwo);

        Color dimColor = new Color(
            dimAmount,
            dimAmount,
            dimAmount,
            1f
        );

        if (showOne)
        {
            avatarOneImage.color =
                activeAvatarSlot == 1 ? Color.white : dimColor;
        }

        if (showTwo)
        {
            avatarTwoImage.color =
                activeAvatarSlot == 2 ? Color.white : dimColor;
        }
    }
        
    IEnumerator LoadNextScene()
    {
        if (fadeImage != null)
            fadeImage.StartTransition();

        yield return new WaitForSeconds(3);

        if(nextSceneName != null)
            SceneManager.LoadScene(nextSceneName);
    }
}

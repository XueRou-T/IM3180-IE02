using TMPro;
using UnityEngine;

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.InputSystem;

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
    public Image avatarImage;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip typingSound;
    [Range(1, 5)]
    public int soundFrequency = 2;

    [Header("Dialogue Content")]
    public DialogueLine[] lines;
    public float textSpeed;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        nameComponent.text = string.Empty;
        StartDialogue();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void UpdateSpeakerInfo()
    {
        nameComponent.text = lines[index].speakerName;

        if (lines[index].speakerAvatar != null)
        {
            avatarImage.sprite = lines[index].speakerAvatar;
            avatarImage.gameObject.SetActive(true);
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }
    }
}

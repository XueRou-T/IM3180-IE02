using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Required for controlling UI Images
using TMPro;

public class RealDialogue : MonoBehaviour
{
    // A custom class to hold all data for a single line of dialogue
    [System.Serializable]
    public struct DialogueLine
    {
        public string SpeakerName;     // Name of the talker
        public Sprite SpeakerPortrait; // The image of the talker
        [TextArea(3, 10)]
        public string Sentence;        // The actual text
    }

    [Header("UI Elements")]
    public TextMeshProUGUI SpeakerNameText; // UI Text for the speaker's name
    public Image SpeakerPortraitImage;     // UI Image component for the portrait
    public TextMeshProUGUI DialogueText;    // UI Text for the dialogue body

    [Header("Dialogue Settings")]
    public DialogueLine[] DialogueLines;    // Replaces your old string[] Sentences
    public float DialogueSpeed;

    private int Index = 0;
    private bool isTyping = false;          // Prevents skipping while text is typing

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isTyping)
        {
            NextSentence();
        }
    }

    void NextSentence()
    {
        if (Index < DialogueLines.Length)
        {
            DialogueText.text = "";
            StartCoroutine(WriteSentence());
        }
        else
        {
            // Optional: Dialogue is finished, turn off UI elements
            Debug.Log("Dialogue Ended");
        }
    }

    IEnumerator WriteSentence()
    {
        isTyping = true;

        // 1. Get the current line data
        DialogueLine currentLine = DialogueLines[Index];

        // 2. Update Speaker Name
        if (SpeakerNameText != null)
        {
            SpeakerNameText.text = currentLine.SpeakerName;
        }

        // 3. Update Speaker Portrait
        if (SpeakerPortraitImage != null)
        {
            if (currentLine.SpeakerPortrait != null)
            {
                SpeakerPortraitImage.gameObject.SetActive(true);
                SpeakerPortraitImage.sprite = currentLine.SpeakerPortrait;
            }
            else
            {
                // If there's no sprite (e.g., narrator), hide the image
                SpeakerPortraitImage.gameObject.SetActive(false);
            }
        }

        // 4. Type out the text character by character
        foreach (char Character in currentLine.Sentence.ToCharArray())
        {
            DialogueText.text += Character;
            yield return new WaitForSeconds(DialogueSpeed);
        }

        Index++;
        isTyping = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;

public class RealDialogue : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string SpeakerName;    
        public Sprite SpeakerPortrait; 
        [TextArea(3, 10)]
        public string Sentence;       
    }

    [Header("UI Elements")]
    public TextMeshProUGUI SpeakerNameText; 
    public Image SpeakerPortraitImage;     
    public TextMeshProUGUI DialogueText;    

    [Header("Dialogue Settings")]
    public DialogueLine[] DialogueLines;    
    public float DialogueSpeed;

    [Header("Next Scene")]
    string nextSceneName;

    private int Index = 0;
    private bool isTyping = false;          

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
            SceneManager.LoadScene(nextSceneName);
        }
    }

    IEnumerator WriteSentence()
    {
        isTyping = true;

       
        DialogueLine currentLine = DialogueLines[Index];

        
        if (SpeakerNameText != null)
        {
            SpeakerNameText.text = currentLine.SpeakerName;
        }

        
        if (SpeakerPortraitImage != null)
        {
            if (currentLine.SpeakerPortrait != null)
            {
                SpeakerPortraitImage.gameObject.SetActive(true);
                SpeakerPortraitImage.sprite = currentLine.SpeakerPortrait;
            }
            else
            {
                
                SpeakerPortraitImage.gameObject.SetActive(false);
            }
        }

        
        foreach (char Character in currentLine.Sentence.ToCharArray())
        {
            DialogueText.text += Character;
            yield return new WaitForSeconds(DialogueSpeed);
        }

        Index++;
        isTyping = false;
    }
}

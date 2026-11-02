using Unity.VisualScripting;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    public bool merlionTimesTrigger;
    public bool videoTrigger;
    public bool journalTrigger;
    public bool closeDesktopTrigger;

    public Dialogue endDialogue;
    public Dialogue dialogueOne;
    public Dialogue dialogueTwo;

    void Start()
    {
        merlionTimesTrigger = false;
        videoTrigger = false;
        journalTrigger = false;
        closeDesktopTrigger = false;
        endDialogue.gameObject.SetActive(false);
        dialogueOne.gameObject.SetActive(false);
        dialogueTwo.gameObject.SetActive(false);
    }

    public void TriggerSetFor(int whichPanel)
    {
        if (whichPanel == 0)
            merlionTimesTrigger = true;
        else if (whichPanel == 1)
            videoTrigger = true;
        else if (whichPanel == 2)
            journalTrigger = true;
        else if (whichPanel == 3)
            closeDesktopTrigger = true;
    }

    public void StartEndDialogue()
    {
        endDialogue.gameObject.SetActive(true);
    }

    public void ChoiceOne()
    {
        endDialogue.gameObject.SetActive(false);
        dialogueOne.gameObject.SetActive(true);
    }

    public void ChoiceTwo()
    {
        endDialogue.gameObject.SetActive(false);
        dialogueTwo.gameObject.SetActive(true);
    }
}

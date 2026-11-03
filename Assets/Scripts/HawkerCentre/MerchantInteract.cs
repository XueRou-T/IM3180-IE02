using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class MerchantInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    public TMP_Text promptText;
    public GameObject promptContainer;
    public string promptMessage = "Press E to talk";

    [Header("Merchant Dialogue")]
    public DialogueSystem.Line[] merchantLines;
    public DialogueSystem.Line[] alreadyHaveLines;

    [Header("UI Choices")]
    public GameObject choicePanel;
    public Button yesButton;
    public Button noButton;

    [Header("Item Reward")]
    public string itemID;
    public UnityEvent onAcceptItem;

    private bool playerInRange = false;

    void Start()
    {
        // 1. AUTO-RECONNECT: Search for the GLOBAL Canvas UI (Choice Panel)
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None)
            {
                if (obj.name == "MerchantChoicePanel") choicePanel = obj;
                if (obj.name == "Yes") yesButton = obj.GetComponent<Button>();
                if (obj.name == "No") noButton = obj.GetComponent<Button>();
            }
        }

        // 2. BACKUP SAFETY NET (Resources): Direct search across transforms if missed
        if (choicePanel == null)
        {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (t.name == "MerchantChoicePanel")
                {
                    choicePanel = t.gameObject;
                    break;
                }
            }
        }

        // 3. BACKUP SAFETY NET (Scene): Standard find as final resort
        if (choicePanel == null)
        {
            GameObject foundPanel = GameObject.Find("MerchantChoicePanel");
            if (foundPanel != null) choicePanel = foundPanel;
        }

        if (yesButton == null && choicePanel != null)
        {
            Transform yesTrans = choicePanel.transform.Find("Yes");
            if (yesTrans != null) yesButton = yesTrans.GetComponent<Button>();
        }

        if (noButton == null && choicePanel != null)
        {
            Transform noTrans = choicePanel.transform.Find("No");
            if (noTrans != null) noButton = noTrans.GetComponent<Button>();
        }

        if (promptText != null) promptText.text = promptMessage;
        if (promptContainer != null) promptContainer.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(false);
    }

    void Update()
    {
        if (!playerInRange) return;

        // Safe check: Is the choice panel actually assigned AND currently active?
        bool isChoiceActive = choicePanel != null && choicePanel.activeSelf;

        if (Input.GetKeyDown(KeyCode.E) && !HawkerDialogueManager.IsDialogueOpen && !isChoiceActive)
        {
            if (promptContainer != null) promptContainer.SetActive(false);

            if (GameManager.Instance != null && !string.IsNullOrEmpty(itemID) && GameManager.Instance.HasItem(itemID))
            {
                HawkerDialogueManager.Instance.StartDialogue(alreadyHaveLines, null);
            }
            else
            {
                HawkerDialogueManager.Instance.StartDialogue(merchantLines, ShowChoicePanel);
            }
        }

        if (!HawkerDialogueManager.IsDialogueOpen && !isChoiceActive && promptContainer != null && !promptContainer.activeSelf)
        {
            promptContainer.SetActive(true);
        }
    }

    void ShowChoicePanel()
    {
        // SAFETY NET: Force find the choice panel right before opening if it was somehow unassigned
        if (choicePanel == null)
        {
            foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
            {
                if (t.name == "MerchantChoicePanel")
                {
                    choicePanel = t.gameObject;
                    break;
                }
            }
        }

        // Re-verify buttons if they are missing
        if (choicePanel != null)
        {
            if (yesButton == null)
            {
                Transform yesTrans = choicePanel.transform.Find("Yes");
                if (yesTrans != null) yesButton = yesTrans.GetComponent<Button>();
            }
            if (noButton == null)
            {
                Transform noTrans = choicePanel.transform.Find("No");
                if (noTrans != null) noButton = noTrans.GetComponent<Button>();
            }
        }

        Debug.Log("ShowChoicePanel executed. choicePanel is: " + (choicePanel != null ? choicePanel.name : "STILL NULL!"));
        Debug.Log("Yes button is: " + (yesButton != null ? yesButton.name : "NULL!"));
        Debug.Log("No button is: " + (noButton != null ? noButton.name : "NULL!"));

        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            Debug.LogError("CRITICAL ERROR: MerchantChoicePanel could not be found in the scene!");
        }

        HawkerDialogueManager.IsDialogueOpen = true;

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(AcceptOffer);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(DeclineOffer);
        }
    }

    public void AcceptOffer()
    {
        Debug.Log("AcceptOffer clicked!");
        onAcceptItem.Invoke();
        CloseChoice();
    }

    public void DeclineOffer()
    {
        Debug.Log("DeclineOffer clicked!");
        CloseChoice();
    }

    void CloseChoice()
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        HawkerDialogueManager.IsDialogueOpen = false;

        if (playerInRange && promptContainer != null) promptContainer.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            bool isChoiceActive = choicePanel != null && choicePanel.activeSelf;

            if (promptContainer != null && !HawkerDialogueManager.IsDialogueOpen && !isChoiceActive)
            {
                promptContainer.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
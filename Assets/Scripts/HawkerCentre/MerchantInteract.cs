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

        Debug.Log($"[MerchantInteract - {gameObject.name}] Initialized. Lines configured: {merchantLines?.Length ?? 0}");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[MerchantInteract - {gameObject.name}] E pressed! playerInRange: {playerInRange}");
        }

        if (!playerInRange) return;

        bool isChoiceActive = choicePanel != null && choicePanel.activeSelf;

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log($"[MerchantInteract - {gameObject.name}] State check -> IsDialogueOpen: {HawkerDialogueManager.IsDialogueOpen}, isChoiceActive: {isChoiceActive}, Instance: {(HawkerDialogueManager.Instance != null ? "Found" : "NULL")}");

            if (!HawkerDialogueManager.IsDialogueOpen && !isChoiceActive)
            {
                if (promptContainer != null) promptContainer.SetActive(false);

                if (HawkerDialogueManager.Instance == null)
                {
                    Debug.LogError($"[MerchantInteract - {gameObject.name}] HawkerDialogueManager.Instance is NULL! Ensure GlobalDialogueManager is in the scene.");
                    return;
                }

                if (GameManager.Instance != null && !string.IsNullOrEmpty(itemID) && GameManager.Instance.HasItem(itemID))
                {
                    Debug.Log($"[MerchantInteract - {gameObject.name}] Playing alreadyHaveLines ({alreadyHaveLines?.Length ?? 0} lines)");
                    HawkerDialogueManager.Instance.StartDialogue(alreadyHaveLines, null);
                }
                else
                {
                    Debug.Log($"[MerchantInteract - {gameObject.name}] Playing merchantLines ({merchantLines?.Length ?? 0} lines)");
                    HawkerDialogueManager.Instance.StartDialogue(merchantLines, ShowChoicePanel);
                }
            }
            else
            {
                Debug.LogWarning($"[MerchantInteract - {gameObject.name}] E press ignored because IsDialogueOpen is {HawkerDialogueManager.IsDialogueOpen} or isChoiceActive is {isChoiceActive}");
            }
        }

        if (!HawkerDialogueManager.IsDialogueOpen && !isChoiceActive && promptContainer != null && !promptContainer.activeSelf)
        {
            promptContainer.SetActive(true);
        }
    }

    void ShowChoicePanel()
    {
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

        if (choicePanel != null)
        {
            choicePanel.SetActive(true);
        }
        else
        {
            Debug.LogError($"[MerchantInteract - {gameObject.name}] CRITICAL ERROR: MerchantChoicePanel could not be found!");
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
        Debug.Log($"[MerchantInteract - {gameObject.name}] AcceptOffer clicked!");
        onAcceptItem.Invoke();
        CloseChoice();
    }

    public void DeclineOffer()
    {
        Debug.Log($"[MerchantInteract - {gameObject.name}] DeclineOffer clicked!");
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
            Debug.Log($"[MerchantInteract - {gameObject.name}] Player entered trigger range.");

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
            Debug.Log($"[MerchantInteract - {gameObject.name}] Player left trigger range.");
            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
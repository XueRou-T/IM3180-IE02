using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [System.Serializable]
    public class ItemDisplay
    {
        public string itemName;
        public Sprite itemSprite;
        public Color itemColor = Color.white;
        [TextArea(3, 5)] public string description;

        [Header("Back Side (Flippable ID Cards)")]
        public bool isFlippable = false; // Check this in Inspector for IDs
        public Sprite backItemSprite;
        [TextArea(3, 5)] public string backDescription;
    }

    [Header("Main UI References")]
    public GameObject panelToToggle;
    public Image[] inventorySlots;

    [Header("Tooltip References")]
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    [Header("Popup Overlay References")]
    public GameObject popupPanel;
    public Image popupImage;
    public TMP_Text popupTitleText;
    public TMP_Text popupDescText;
    public GameObject flipButtonObject; // Reference to your UI Flip button

    [Header("First-Time Prompt")]
    public GameObject inventoryPromptPanel; // Drag your prompt UI panel here

    [Header("Item Database")]
    public ItemDisplay[] itemDatabase;

    private bool isOpen = false;
    private ItemDisplay currentPopupItem;
    private bool showingBack = false;
    private bool hasShownPrompt = false;
    private bool isWaitingForInventoryCycle = false;
    private bool promptHasOpened = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (panelToToggle != null) panelToToggle.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
        if (flipButtonObject != null) flipButtonObject.SetActive(false);
        if (inventoryPromptPanel != null) inventoryPromptPanel.SetActive(false);
        isOpen = false;
    }

    void OnEnable()
    {
        ReconnectUI();
    }

    void Start()
    {
        ReconnectUI();
        SetPanelOpen(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
        if (flipButtonObject != null) flipButtonObject.SetActive(false);
        if (inventoryPromptPanel != null) inventoryPromptPanel.SetActive(false);
        RefreshDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == "Lab" || currentScene == "House" || currentScene == "HawkerCentre")
                SetPanelOpen(!isOpen);
        }
    }

    public void ReconnectUI()
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.hideFlags == HideFlags.None)
            {
                if (panelToToggle == null && (obj.name == "InventoryPanel" || obj.name == "Inventory Panel")) panelToToggle = obj;
                if (tooltipPanel == null && (obj.name == "TooltipPanel" || obj.name == "Tooltip Panel")) tooltipPanel = obj;
            }
        }
    }

    public void SetPanelOpen(bool open)
    {
        isOpen = open;

        if (panelToToggle != null)
        {
            panelToToggle.SetActive(open);
        }
        else
        {
            Debug.LogError("InventoryUI: panelToToggle is UNASSIGNED! Drag your Inventory Panel into the Inspector slot.");
        }

        // --- INVENTORY PROMPT CYCLE CHECK ---
        if (isWaitingForInventoryCycle)
        {
            if (open)
            {
                promptHasOpened = true; // 1st Tab: Player opened the inventory
            }
            else if (promptHasOpened && !open)
            {
                // 2nd Tab: Player closed the inventory after opening it -> Hide prompt forever
                if (inventoryPromptPanel != null)
                {
                    inventoryPromptPanel.SetActive(false);
                }
                isWaitingForInventoryCycle = false;
            }
        }
        // ------------------------------------

        if (!isOpen)
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
            if (popupPanel != null) popupPanel.SetActive(false);
            if (flipButtonObject != null) flipButtonObject.SetActive(false);
        }

        if (isOpen) RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (GameManager.Instance == null) return;

        foreach (Image slot in inventorySlots)
        {
            if (slot != null)
            {
                slot.gameObject.SetActive(false);
                InventorySlot slotScript = slot.GetComponent<InventorySlot>();
                if (slotScript != null) slotScript.currentItemName = "";
            }
        }

        int slotIndex = 0;
        foreach (string itemName in GameManager.Instance.GetAllItems())
        {
            if (slotIndex >= inventorySlots.Length) break;

            ItemDisplay foundItem = GetItemDisplay(itemName);

            if (foundItem != null && inventorySlots[slotIndex] != null)
            {
                inventorySlots[slotIndex].sprite = foundItem.itemSprite;
                inventorySlots[slotIndex].color = foundItem.itemColor;
                inventorySlots[slotIndex].gameObject.SetActive(true);

                InventorySlot slotScript = inventorySlots[slotIndex].GetComponent<InventorySlot>();
                if (slotScript != null) slotScript.currentItemName = foundItem.itemName;

                slotIndex++;
            }
        }
    }

    private ItemDisplay GetItemDisplay(string itemName)
    {
        foreach (ItemDisplay item in itemDatabase)
        {
            if (item.itemName == itemName) return item;
        }
        return null;
    }

    // --- MOUSE INTERACTION METHODS ---

    public void ShowTooltip(string itemName, Vector3 slotPosition)
    {
        if (tooltipPanel == null || (popupPanel != null && popupPanel.activeSelf)) return;

        tooltipText.text = itemName;
        tooltipPanel.transform.position = slotPosition + new Vector3(0, 55f, 0);
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void ShowPopup(string itemName)
    {
        if (TableInteract.activeTable != null)
        {
            TableInteract.activeTable.TryEatItem(itemName);
            return;
        }

        // --- FIRST-TIME INVENTORY PROMPT CHECK ---
        if (GameManager.Instance != null && GameManager.Instance.GetAllItems().Count == 1 && !hasShownPrompt)
        {
            hasShownPrompt = true;
            if (inventoryPromptPanel != null)
            {
                inventoryPromptPanel.SetActive(true);
                isWaitingForInventoryCycle = true;
                promptHasOpened = false;
            }
        }
        // -----------------------------------------

        currentPopupItem = GetItemDisplay(itemName);
        if (currentPopupItem == null || popupPanel == null) return;

        showingBack = false; // Always start on the front view
        UpdatePopupVisuals();

        // Show flip button only if this specific item is marked as flippable
        if (flipButtonObject != null)
        {
            flipButtonObject.SetActive(currentPopupItem.isFlippable);
        }

        popupPanel.SetActive(true);
    }

    public void FlipCard()
    {
        if (currentPopupItem == null || !currentPopupItem.isFlippable) return;

        showingBack = !showingBack;
        UpdatePopupVisuals();
    }

    private void UpdatePopupVisuals()
    {
        if (showingBack)
        {
            popupTitleText.text = currentPopupItem.itemName + " (Back)";
            popupDescText.text = currentPopupItem.backDescription;
            popupImage.sprite = currentPopupItem.backItemSprite;
            popupImage.color = currentPopupItem.itemColor;
        }
        else
        {
            popupTitleText.text = currentPopupItem.itemName;
            popupDescText.text = currentPopupItem.description;
            popupImage.sprite = currentPopupItem.itemSprite;
            popupImage.color = currentPopupItem.itemColor;
        }
    }

    public void ClosePopup()
    {
        // Save the item ID before clearing it
        string closedItemID = (currentPopupItem != null) ? currentPopupItem.itemName : "";

        if (popupPanel != null) popupPanel.SetActive(false);
        if (flipButtonObject != null) flipButtonObject.SetActive(false);

        // --- SCENE TRANSITION CHECK ---
        if (closedItemID == "White Tulips")
        {
            Debug.Log("[InventoryUI] Special collectible popup closed! Loading puzzle scene...");
            SceneManager.LoadScene("Puzzle1");
        }
    }
}
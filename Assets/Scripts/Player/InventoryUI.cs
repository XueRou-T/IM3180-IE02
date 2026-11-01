using UnityEngine;
using UnityEngine.UI;
using TMPro; // Required for TextMeshPro text

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [System.Serializable]
    public class ItemDisplay
    {
        public string itemName;
        public Sprite itemSprite;
        public Color itemColor = Color.white;
        [TextArea(3, 5)] public string description; // Adds a large text box in the Inspector
    }

    [Header("Main UI References")]
    public GameObject panelToToggle;
    public Image[] inventorySlots; // Keep your ItemIcons dragged in here

    [Header("Tooltip References")]
    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    [Header("Popup Overlay References")]
    public GameObject popupPanel;
    public Image popupImage;
    public TMP_Text popupTitleText;
    public TMP_Text popupDescText;

    [Header("Item Database")]
    public ItemDisplay[] itemDatabase;

    private bool isOpen = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Ensures the canvas is a root object
            DontDestroyOnLoad(gameObject); // Keeps it alive when loading a new level
        }
        else
        {
            Destroy(gameObject); // Destroys any accidental duplicates
        }
    }

    void Start()
    {
        SetPanelOpen(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
        RefreshDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetPanelOpen(!isOpen);
        }
    }

    void SetPanelOpen(bool open)
    {
        isOpen = open;
        if (panelToToggle != null) panelToToggle.SetActive(open);
        if (!isOpen)
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
            if (popupPanel != null) popupPanel.SetActive(false);
        }

        if (isOpen) RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (GameManager.Instance == null) return;

        // Reset all slots
        foreach (Image slot in inventorySlots)
        {
            if (slot != null)
            {
                slot.gameObject.SetActive(false);
                InventorySlot slotScript = slot.GetComponent<InventorySlot>();
                if (slotScript != null) slotScript.currentItemName = ""; // Clear old data
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

                // Pass the item name to the slot's click/hover script
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

    // --- NEW MOUSE INTERACTION METHODS ---

    public void ShowTooltip(string itemName, Vector3 slotPosition)
    {
        if (tooltipPanel == null || popupPanel.activeSelf) return; // Don't show if popup is open

        tooltipText.text = itemName;
        // Move the tooltip slightly above the hovered item
        tooltipPanel.transform.position = slotPosition + new Vector3(0, 55f, 0);
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
    }

    public void ShowPopup(string itemName)
    {
        // 1. CHECK: Are we currently using a table to eat?
        if (TableInteract.activeTable != null)
        {
            // EATING MODE: Send this item's ID to the table
            TableInteract.activeTable.TryEatItem(itemName);

            // Instantly stop this function so the popup panel below never opens
            return;
        }

        // 2. NORMAL MODE: The table is not active, so open the description panel normally
        ItemDisplay foundItem = GetItemDisplay(itemName);
        if (foundItem == null || popupPanel == null) return;

        popupTitleText.text = foundItem.itemName;
        popupDescText.text = foundItem.description;
        popupImage.sprite = foundItem.itemSprite;
        popupImage.color = foundItem.itemColor;

        popupPanel.SetActive(true);
    }

    // Link this to your Close 'X' Button in the Inspector
    public void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}
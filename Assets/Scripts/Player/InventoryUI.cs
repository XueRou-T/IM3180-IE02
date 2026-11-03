using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("Item Database")]
    public ItemDisplay[] itemDatabase;

    private bool isOpen = false;

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

        // Force everything closed immediately on awake before start runs
        if (panelToToggle != null) panelToToggle.SetActive(false);
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (popupPanel != null) popupPanel.SetActive(false);
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
        RefreshDisplay();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
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
                // Only auto-reconnect if they aren't already assigned in the inspector
                if (panelToToggle == null && (obj.name == "InventoryPanel" || obj.name == "Inventory Panel")) panelToToggle = obj;
                if (tooltipPanel == null && (obj.name == "TooltipPanel" || obj.name == "Tooltip Panel")) tooltipPanel = obj;
            }
        }
    }

    void SetPanelOpen(bool open)
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

        ItemDisplay foundItem = GetItemDisplay(itemName);
        if (foundItem == null || popupPanel == null) return;

        popupTitleText.text = foundItem.itemName;
        popupDescText.text = foundItem.description;
        popupImage.sprite = foundItem.itemSprite;
        popupImage.color = foundItem.itemColor;

        popupPanel.SetActive(true);
    }

    public void ClosePopup()
    {
        if (popupPanel != null) popupPanel.SetActive(false);
    }
}
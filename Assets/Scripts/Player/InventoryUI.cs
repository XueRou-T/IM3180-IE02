using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text inventoryText;
    public GameObject panelToToggle;

    private bool isOpen = false;

    void Start()
    {
        SetPanelOpen(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SetPanelOpen(!isOpen);
        }

        if (isOpen)
        {
            RefreshDisplay();
        }
    }

    void SetPanelOpen(bool open)
    {
        isOpen = open;
        panelToToggle.SetActive(open);
    }

    void RefreshDisplay()
    {
        if (GameManager.Instance == null) return;

        string display = "Inventory:\n";
        foreach (string item in GameManager.Instance.GetAllItems())
        {
            display += "- " + item + "\n";
        }

        inventoryText.text = display;
    }
}
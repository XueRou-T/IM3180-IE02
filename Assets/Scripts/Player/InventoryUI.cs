using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public TMP_Text inventoryText;

    void Update()
    {
        RefreshDisplay();
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
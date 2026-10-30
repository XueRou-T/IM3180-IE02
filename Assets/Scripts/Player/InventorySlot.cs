using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // The main UI script will automatically fill this in when an item is collected
    [HideInInspector] public string currentItemName = "";

    // Triggered when the mouse hovers over the icon
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(currentItemName)) return;
        InventoryUI.Instance.ShowTooltip(currentItemName, transform.position);
    }

    // Triggered when the mouse leaves the icon
    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryUI.Instance.HideTooltip();
    }

    // Triggered when the icon is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(currentItemName)) return;
        InventoryUI.Instance.ShowPopup(currentItemName);
        InventoryUI.Instance.HideTooltip(); // Hide tooltip while looking at the popup
    }
}
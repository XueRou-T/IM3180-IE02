using UnityEngine;
using TMPro;
using UnityEngine.Rendering.Universal;

public class Interactable : MonoBehaviour
{
    public TMP_Text promptText;

    [Header("Item Identity")]
    public string itemID = "ItemName";       // unique name stored in inventory, e.g. "PersonalID"
    public string interactPrompt = "Press E to pick up";
    public string pickupMessage = "Picked up!";

    [Header("Lock Settings")]
    public bool requiresUnlock = false;       // true for Key/Laptop
    public string[] requiredItems;            // e.g. ["PersonalID", "WorkID", "Wallet"]

    [Header("Glow Settings")]
    public float pulseSpeed = 2f;
    public float minBrightness = 0.7f;
    public float maxBrightness = 1.3f;

    private bool playerInRange = false;
    private bool hasBeenPickedUp = false;
    private SpriteRenderer sr;
    private Collider2D col;
    private Color baseColor;
    private bool isUnlocked = true;

    private Light2D light2D;

    void Start()
    {
        light2D = GetComponent<Light2D>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (sr != null)
        {
            baseColor = sr.color;
        }

        if (promptText != null)
        {
            promptText.text = interactPrompt;
            promptText.gameObject.SetActive(false);
        }

        // If this item was already collected before (e.g. scene reload), hide it permanently
        if (GameManager.Instance != null && GameManager.Instance.HasItem(itemID))
        {
            hasBeenPickedUp = true;
            SetItemVisibility(false);
            return;
        }

        // Handle initial lock state
        if (requiresUnlock)
        {
            isUnlocked = false;
            SetItemVisibility(false); // Hide the item immediately since it's locked
            CheckUnlockCondition();   // Check in case the player already holds the required items
        }
        else
        {
            isUnlocked = true;
            SetItemVisibility(true);
        }
    }

    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        if (isUnlocked && !hasBeenPickedUp)
        {
            PulseGlow();
        }

        if (playerInRange && isUnlocked && !hasBeenPickedUp && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void CheckUnlockCondition()
    {
        if (GameManager.Instance == null) return;

        bool allFound = true;
        foreach (string item in requiredItems)
        {
            if (!GameManager.Instance.HasItem(item))
            {
                allFound = false;
                break;
            }
        }

        if (allFound)
        {
            UnlockItem();
        }
    }

    void UnlockItem()
    {
        isUnlocked = true;
        SetItemVisibility(true); // Reveal the item now that it's unlocked
    }

    private void SetItemVisibility(bool visible)
    {
        if (sr != null) sr.enabled = visible;
        if (col != null) col.enabled = visible;
        if (light2D != null) light2D.enabled = visible;

        if (!visible && promptText != null)
        {
            promptText.gameObject.SetActive(false);
        }
    }

    void PulseGlow()
    {
        if (sr == null) return;
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        sr.color = baseColor * brightness;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked && !hasBeenPickedUp)
        {
            playerInRange = true;
            if (promptText != null) promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = false;
            if (promptText != null) promptText.gameObject.SetActive(false);
        }
    }

    public void PickUp()
    {
        hasBeenPickedUp = true;

        if (promptText != null)
        {
            promptText.text = pickupMessage;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItem(itemID);

            if (InventoryUI.Instance != null)
            {
                InventoryUI.Instance.RefreshDisplay();
                InventoryUI.Instance.ShowPopup(itemID);
            }
        }

        Invoke(nameof(FinishPickUp), 1f);

        // Hide sprite and collider immediately on pickup while keeping prompt text visible for 1s
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
    }

    void FinishPickUp()
    {
        if (promptText != null)
        {
            promptText.gameObject.SetActive(false);
            promptText.text = interactPrompt;
        }

        if (light2D != null)
            light2D.enabled = false;
    }
}
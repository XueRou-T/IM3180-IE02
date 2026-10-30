using UnityEngine;
using TMPro;

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
    private Color baseColor;
    private bool isUnlocked = true;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        promptText.text = interactPrompt;

        // If this item was already collected before (e.g. scene reload), hide it immediately
        if (GameManager.Instance != null && GameManager.Instance.HasItem(itemID))
        {
            hasBeenPickedUp = true;
            sr.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            return; // skip the unlock check below, it's irrelevant now
        }

        if (requiresUnlock)
        {
            isUnlocked = false;
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
            isUnlocked = true;
        }
    }

    void PulseGlow()
    {
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        sr.color = baseColor * brightness;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked && !hasBeenPickedUp)
        {
            playerInRange = true;
            promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
        {
            playerInRange = false;
            promptText.gameObject.SetActive(false);
        }
    }

    void PickUp()
    {
        hasBeenPickedUp = true;
        promptText.text = pickupMessage;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItem(itemID);

            // Tell the UI to redraw the sprites immediately
            if (InventoryUI.Instance != null) InventoryUI.Instance.RefreshDisplay();
        }

        Invoke(nameof(FinishPickUp), 1f);
        sr.enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    void FinishPickUp()
    {
        promptText.gameObject.SetActive(false);
        promptText.text = interactPrompt;
    }
}
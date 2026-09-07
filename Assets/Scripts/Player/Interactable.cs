using UnityEngine;
using TMPro;

public class Interactable : MonoBehaviour
{
    public TMP_Text promptText;

    [Header("Customize per item")]
    public string interactPrompt = "Press E to pick up";
    public string pickupMessage = "Picked up!";

    private bool playerInRange = false;
    private bool hasBeenPickedUp = false;

    void Start()
    {
        promptText.text = interactPrompt;
    }

    void Update()
    {
        if (playerInRange && !hasBeenPickedUp && Input.GetKeyDown(KeyCode.E))
        {
            PickUp();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasBeenPickedUp)
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
        Invoke(nameof(FinishPickUp), 1f);
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
    }

    void FinishPickUp()
    {
        promptText.gameObject.SetActive(false);
        promptText.text = interactPrompt; // reset for next time (if reused)
    }
}
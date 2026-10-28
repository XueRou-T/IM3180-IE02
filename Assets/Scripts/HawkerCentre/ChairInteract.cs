using UnityEngine;
using TMPro;

public class ChairInteract : MonoBehaviour
{
    public TMP_Text promptText;
    public GameObject promptContainer;
    public Sprite sittingSprite;

    // NEW: Adjust these X and Y values in the Inspector to nudge the player into the exact seat position
    public Vector3 sittingOffset = new Vector3(0f, 0.2f, 0f);

    // NEW: Type the exact name of your player's movement script here in the Inspector
    public string playerMovementScriptName = "PlayerMovement";

    private GameObject player;
    private SpriteRenderer playerSpriteRenderer;
    private Sprite originalSprite;
    private Animator playerAnimator;

    private bool playerInRange = false;
    private bool isSitting = false;

    void Start()
    {
        if (promptContainer != null) promptContainer.SetActive(false);
        if (promptText != null) promptText.text = "Press E to sit";
    }

    void Update()
    {
        if (playerInRange && !isSitting && Input.GetKeyDown(KeyCode.E))
        {
            SitDown();
        }
        else if (isSitting)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                StandUp();
            }
        }
    }

    void SitDown()
    {
        isSitting = true;

        if (playerAnimator != null) playerAnimator.enabled = false;

        MonoBehaviour movementScript = (MonoBehaviour)player.GetComponent(playerMovementScriptName);
        if (movementScript != null) movementScript.enabled = false;

        // NEW: Turn off player physics so the chair's Polygon Collider doesn't push them away
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        playerSpriteRenderer.sprite = sittingSprite;
        player.transform.position = transform.position + sittingOffset;

        if (promptContainer != null) promptContainer.SetActive(false);
    }

    void StandUp()
    {
        isSitting = false;

        playerSpriteRenderer.sprite = originalSprite;

        if (playerAnimator != null) playerAnimator.enabled = true;

        MonoBehaviour movementScript = (MonoBehaviour)player.GetComponent(playerMovementScriptName);
        if (movementScript != null) movementScript.enabled = true;

        // NEW: Turn player physics back on so they can collide with things again
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = true;

        if (promptContainer != null) promptContainer.SetActive(true);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (player == null)
            {
                player = other.gameObject;
                playerSpriteRenderer = player.GetComponentInChildren<SpriteRenderer>();
                playerAnimator = player.GetComponentInChildren<Animator>();
                originalSprite = playerSpriteRenderer.sprite;
            }

            if (!isSitting && promptContainer != null) promptContainer.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptContainer != null) promptContainer.SetActive(false);
        }
    }
}
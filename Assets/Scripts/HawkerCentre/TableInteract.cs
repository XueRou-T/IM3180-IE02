using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TableInteract : MonoBehaviour
{
    public static TableInteract activeTable;

    [Header("UI Prompts")]
    public GameObject interactPrompt;
    public TMP_Text promptText;
    public GameObject messageCanvas;
    public TMP_Text messageText;
    public GameObject inventoryPanel;

    [System.Serializable]
    public class EdibleFood
    {
        public string itemID;
        public Sprite tableSprite;
        public bool triggersSceneChange; // NEW: Tickbox to check if this food loads the next scene
    }

    [Header("Edible Items Setup")]
    public List<EdibleFood> validFoods;

    [Header("Cinematic & Chair Setup")]
    public Transform chairPosition;
    public string playerMovementScriptName = "PlayerMovement";
    public SpriteRenderer foodOnTable;

    [Header("Animation Setup")]
    public Sprite[] eatingSprites;
    public Sprite sittingSprite;
    public float animFrameRate = 0.2f;

    [Header("Scene Transition")]
    public string nextSceneName = "Memory1";

    private GameObject player;
    private SpriteRenderer playerSpriteRenderer;
    private Sprite originalSprite;
    private Animator playerAnimator;
    private Rigidbody2D playerRb;
    private MonoBehaviour playerMovementScript;

    private bool playerInRange = false;
    private bool isSelectingFood = false;
    private bool isSittingOnly = false;

    void Start()
    {
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (messageCanvas != null) messageCanvas.SetActive(false);
        if (foodOnTable != null) foodOnTable.enabled = false;
    }

    void Update()
    {
        if (isSittingOnly)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                StandUp();
            }
            return;
        }

        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isSelectingFood)
        {
            if (HasAnyFood())
            {
                StartFoodSelection();
            }
            else
            {
                SitDown();
            }
        }
    }

    private bool HasAnyFood()
    {
        if (GameManager.Instance == null) return false;
        foreach (EdibleFood food in validFoods)
        {
            if (GameManager.Instance.HasItem(food.itemID)) return true;
        }
        return false;
    }

    void SitDown()
    {
        isSittingOnly = true;

        if (playerAnimator != null) playerAnimator.enabled = false;
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerRb != null) playerRb.simulated = false;

        playerSpriteRenderer.sprite = sittingSprite;
        if (chairPosition != null) player.transform.position = chairPosition.position;

        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void StandUp()
    {
        isSittingOnly = false;

        playerSpriteRenderer.sprite = originalSprite;

        if (playerAnimator != null) playerAnimator.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (playerRb != null) playerRb.simulated = true;

        if (interactPrompt != null) interactPrompt.SetActive(true);
    }

    void StartFoodSelection()
    {
        isSelectingFood = true;
        activeTable = this;
        if (interactPrompt != null) interactPrompt.SetActive(false);

        // Uses the direct reference to turn on the UI
        if (inventoryPanel != null) inventoryPanel.SetActive(true);

        ShowMessage("Pick a food to eat", 3f);
    }

    public void ShowMessage(string msg, float duration)
    {
        StopCoroutine(nameof(MessageRoutine));
        StartCoroutine(MessageRoutine(msg, duration));
    }

    IEnumerator MessageRoutine(string msg, float duration)
    {
        messageCanvas.SetActive(true);
        messageText.text = msg;
        yield return new WaitForSeconds(duration);
        messageCanvas.SetActive(false);
    }

    public void TryEatItem(string clickedItemID)
    {
        EdibleFood foodToEat = validFoods.Find(f => f.itemID == clickedItemID);

        if (foodToEat != null)
        {
            isSelectingFood = false;
            activeTable = null;
            messageCanvas.SetActive(false);

            // Uses the direct reference to turn off the UI
            if (inventoryPanel != null) inventoryPanel.SetActive(false);

            StartCoroutine(EatingSequence(foodToEat));
        }
        else
        {
            ShowMessage("This item is not edible!", 2f);
        }
    }

    IEnumerator EatingSequence(EdibleFood foodToEat)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RemoveItem(foodToEat.itemID);
            if (InventoryUI.Instance != null) InventoryUI.Instance.RefreshDisplay();
        }

        if (playerAnimator != null) playerAnimator.enabled = false;
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (playerRb != null) playerRb.simulated = false;

        if (chairPosition != null) player.transform.position = chairPosition.position;
        if (foodOnTable != null && foodToEat.tableSprite != null)
        {
            foodOnTable.sprite = foodToEat.tableSprite;
            foodOnTable.enabled = true;
        }

        float timer = 0f;
        int frameIndex = 0;

        while (timer < 4f)
        {
            if (eatingSprites.Length > 0 && playerSpriteRenderer != null)
            {
                playerSpriteRenderer.sprite = eatingSprites[frameIndex];
                frameIndex = (frameIndex + 1) % eatingSprites.Length;
            }
            yield return new WaitForSeconds(animFrameRate);
            timer += animFrameRate;
        }

        // 5. Check what happens after eating
        if (foodToEat.triggersSceneChange)
        {
            // If the box is ticked, load the memory scene
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // If it is normal food, clear the table and stay seated
            if (foodOnTable != null) foodOnTable.enabled = false;

            playerSpriteRenderer.sprite = sittingSprite;
            isSittingOnly = true; // Returns them to the normal sitting state so they can stand up
        }
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
                playerRb = player.GetComponent<Rigidbody2D>();
                playerMovementScript = (MonoBehaviour)player.GetComponent(playerMovementScriptName);
                originalSprite = playerSpriteRenderer.sprite;
            }

            if (!isSelectingFood && !isSittingOnly && interactPrompt != null)
            {
                if (promptText != null)
                {
                    promptText.text = HasAnyFood() ? "Press E to eat" : "Press E to sit";
                }
                interactPrompt.SetActive(true);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            isSelectingFood = false;
            activeTable = null;

            // Uses the direct reference to turn off the UI if they walk away
            if (inventoryPanel != null) inventoryPanel.SetActive(false);

            if (interactPrompt != null) interactPrompt.SetActive(false);
            if (messageCanvas != null) messageCanvas.SetActive(false);
        }
    }
}
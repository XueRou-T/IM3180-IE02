using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    [Header("Sprites - Idle")]
    public Sprite idleUp;
    public Sprite idleDown;
    public Sprite idleLeft;
    public Sprite idleRight;

    [Header("Sprites - Walking Up")]
    public Sprite walkUp1;
    public Sprite walkUp2;

    [Header("Sprites - Walking Down")]
    public Sprite walkDown1;
    public Sprite walkDown2;

    [Header("Sprites - Walking Left")]
    public Sprite walkLeft1;
    public Sprite walkLeft2;

    [Header("Sprites - Walking Right")]
    public Sprite walkRight1;
    public Sprite walkRight2;

    [Header("Animation Settings")]
    public float frameRate = 0.2f; // time (seconds) between frame swaps

    [Header("SFX Reference")]
    public OverWorldSFX sfxManager;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 moveInput;
    private string lastDirection = "Down";

    private float frameTimer = 0f;
    private bool showingFirstFrame = true;

    public LaptopPuzzle puzzlePanel;

    [Header("UI Blocking")]
    public GameObject addressInputPanel; // Drag your House Address Popup here

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        bool isPuzzleOpen = puzzlePanel != null && puzzlePanel.IsPuzzleOpen;
        bool isAddressOpen = addressInputPanel != null && addressInputPanel.activeSelf;

        if (isPuzzleOpen || isAddressOpen || DestinationChoice.IsChoiceOpen || DialogueSystem.IsDialogueOpen || HawkerDialogueManager.IsDialogueOpen)
        {
            moveInput = Vector2.zero;
            if (sfxManager != null)
                sfxManager.StopAllSound();
            return;
        }

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput.Normalize();

        UpdateDirection();
        UpdateSprite();
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveInput * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateDirection()
    {
        if (moveInput.y > 0) lastDirection = "Up";
        else if (moveInput.y < 0) lastDirection = "Down";
        else if (moveInput.x < 0) lastDirection = "Left";
        else if (moveInput.x > 0) lastDirection = "Right";
    }

    void UpdateSprite()
    {
        bool isMoving = moveInput.sqrMagnitude > 0.01f;

        if (!isMoving)
        {
            if (sfxManager != null)
                sfxManager.StopAllSound();
            // Standing still - show idle sprite, reset animation timer
            frameTimer = 0f;
            showingFirstFrame = true;

            switch (lastDirection)
            {
                case "Up": sr.sprite = idleUp; break;
                case "Down": sr.sprite = idleDown; break;
                case "Left": sr.sprite = idleLeft; break;
                case "Right": sr.sprite = idleRight; break;
            }
        }
        else
        {
            if (sfxManager != null)
                sfxManager.PlayWalkingSound();
            // Moving - alternate between frame 1 and frame 2 based on timer
            frameTimer += Time.deltaTime;
            if (frameTimer >= frameRate)
            {
                frameTimer = 0f;
                showingFirstFrame = !showingFirstFrame;
            }

            switch (lastDirection)
            {
                case "Up": sr.sprite = showingFirstFrame ? walkUp1 : walkUp2; break;
                case "Down": sr.sprite = showingFirstFrame ? walkDown1 : walkDown2; break;
                case "Left": sr.sprite = showingFirstFrame ? walkLeft1 : walkLeft2; break;
                case "Right": sr.sprite = showingFirstFrame ? walkRight1 : walkRight2; break;
            }
        }
    }
}
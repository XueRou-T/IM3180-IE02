using TMPro;
using UnityEngine;
using System.Collections;

public class LaptopInteract : MonoBehaviour
{
    public TMP_Text promptText;
    public LaptopLogin loginPanel;
    public LaptopPuzzle puzzlePanel;
    public ObjectDialogue laptopDialogue;

    [Header("Lock Settings")]
    public bool requiresUnlock = true;
    public string[] requiredItems;

    [Header("Glow Settings")]
    public float pulseSpeed = 2f;
    public float minBrightness = 0.7f;
    public float maxBrightness = 1.3f;

    private bool playerInRange = false;
    private bool isUnlocked = false;
    private bool isSolved = false;
    private SpriteRenderer sr;
    private Color baseColor;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        baseColor = sr.color;
        promptText.text = "Laptop";

        if (!requiresUnlock)
        {
            isUnlocked = true;
        }

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("LaptopSolved"))
        {
            isUnlocked = true;
            isSolved = true;
        }
    }

    void Update()
    {
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        if (isUnlocked && !isSolved)
        {
            PulseGlow();
        }

        if (playerInRange && isUnlocked && !puzzlePanel.IsPuzzleOpen && Input.GetKeyDown(KeyCode.E))
        {
            if (isSolved)
            {
                OpenFiles();
            }
            else
            {
                OpenPuzzle();
            }
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

        if (allFound) isUnlocked = true;
    }

    void PulseGlow()
    {
        float brightness = Mathf.Lerp(minBrightness, maxBrightness, (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f);
        sr.color = baseColor * brightness;
    }

    void OpenPuzzle()
    {
        // --- FIRST-TIME LAPTOP DIALOGUE CHECK ---
        if (GameManager.Instance != null && !GameManager.Instance.HasFlag("LaptopFirstDialogueShown"))
        {
            GameManager.Instance.SetFlag("LaptopFirstDialogueShown");

            // 1. Open the laptop canvas / login panel IMMEDIATELY first!
            if (loginPanel != null)
            {
                loginPanel.gameObject.SetActive(true);
                puzzlePanel.IsPuzzleOpen = true;
            }

            // 2. Trigger the dialogue with a slight delay so it appears ON TOP of the laptop canvas
            if (laptopDialogue != null)
            {
                StartCoroutine(TriggerDialogueOverLaptop());
            }

            return;
        }
        // ----------------------------------------

        if (loginPanel != null)
        {
            loginPanel.gameObject.SetActive(true);
            puzzlePanel.IsPuzzleOpen = true;
        }
    }

    // Opens dialogue a split-second after the laptop canvas is already visible
    private IEnumerator TriggerDialogueOverLaptop()
    {
        // Wait a fraction of a second so the player registers the laptop opening first
        yield return new WaitForSeconds(0.5f);

        laptopDialogue.gameObject.SetActive(true);
        laptopDialogue.StartDialogue();

        // When the dialogue finishes, the laptop canvas is ALREADY open, 
        // so we don't need to open it again—just let the player interact with it!
        laptopDialogue.onDialogueFinished = () =>
        {
            // Dialogue closed, laptop canvas stays open and ready for input
        };
    }

    // Safely delays dialogue startup by 1 frame so UI canvas components wake up properly
    private IEnumerator SafelyStartFirstDialogue()
    {
        laptopDialogue.gameObject.SetActive(true);

        // Wait 1 frame so Unity's UI layout and TextMeshPro components initialize completely
        yield return null;

        laptopDialogue.StartDialogue();

        // Listen for when this first-time dialogue finishes
        laptopDialogue.onDialogueFinished = () =>
        {
            if (loginPanel != null)
            {
                loginPanel.gameObject.SetActive(true);
                puzzlePanel.IsPuzzleOpen = true;
            }
        };
    }

    void OpenFiles()
    {
        Debug.Log("Files screen open here.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isUnlocked)
        {
            playerInRange = true;
            promptText.text = isSolved ? "Access Files" : "Laptop";
            promptText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            promptText.gameObject.SetActive(false);
        }
    }
}
using UnityEngine;
using TMPro;

public class LaptopInteract : MonoBehaviour
{
    public TMP_Text promptText;
    public GameObject loginPanel;

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

        if (GameManager.Instance != null && GameManager.Instance.HasFlag("LaptopSolved"))
        {
            isSolved = true;
        }
    }

    void Update()
    {
<<<<<<< HEAD
        if (!isSolved && GameManager.Instance != null && GameManager.Instance.HasFlag("LaptopSolved"))
        {
            isSolved = true;
        }

=======
>>>>>>> parent of 3bfe626 (Merge branch 'minghui' into Afiq)
        if (requiresUnlock && !isUnlocked)
        {
            CheckUnlockCondition();
        }

        if (isUnlocked && !isSolved)
        {
            PulseGlow();
        }

        if (playerInRange && isUnlocked && !LaptopPuzzle.IsPuzzleOpen && Input.GetKeyDown(KeyCode.E))
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
        if (UIPersistence.Instance != null && UIPersistence.Instance.laptopLogin != null)
        {
            UIPersistence.Instance.laptopLogin.gameObject.SetActive(true);
            LaptopPuzzle.IsPuzzleOpen = true;
        }
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
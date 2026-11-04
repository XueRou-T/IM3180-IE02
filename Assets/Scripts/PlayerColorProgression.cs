using UnityEngine;

public class PlayerColorProgression : MonoBehaviour
{
    public static PlayerColorProgression Instance;

    [Header("Material Reference")]
    public SpriteRenderer playerSpriteRenderer;

    [Header("Saturation Stages (0 = Grayscale, 1 = Full Color)")]
    [Range(0f, 1f)] public float stage0_Start = 0.0f;       // Initial desaturated state
    [Range(0f, 1f)] public float stage1_AfterPuzzle1 = 0.33f; // After first puzzle
    [Range(0f, 1f)] public float stage2_AfterPuzzle2 = 0.66f; // After second puzzle
    [Range(0f, 1f)] public float stage3_FullColor = 1.00f;    // After final animation scene

    private Material instancedMaterial;
    private int saturationPropertyID;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (playerSpriteRenderer == null)
        {
            playerSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        // Cache the property ID for performance
        saturationPropertyID = Shader.PropertyToID("_SaturationAmount");

        if (playerSpriteRenderer != null)
        {
            // Create a unique material instance so it doesn't affect other sprites using the same shader
            instancedMaterial = playerSpriteRenderer.material;

            // Set initial stage
            SetSaturation(stage0_Start);
        }
    }

    // Call this when Puzzle 1 is completed
    public void CompletePuzzleOne()
    {
        SetSaturation(stage1_AfterPuzzle1);
        Debug.Log("[PlayerColor] Puzzle 1 completed. Color increased to Stage 1.");
    }

    // Call this when Puzzle 2 is completed
    public void CompletePuzzleTwo()
    {
        SetSaturation(stage2_AfterPuzzle2);
        Debug.Log("[PlayerColor] Puzzle 2 completed. Color increased to Stage 2.");
    }

    // Call this after the animation scene finishes
    public void CompleteAnimationScene()
    {
        SetSaturation(stage3_FullColor);
        Debug.Log("[PlayerColor] Animation scene completed. Full color restored!");
    }

    private void SetSaturation(float targetAmount)
    {
        if (instancedMaterial != null)
        {
            instancedMaterial.SetFloat(saturationPropertyID, targetAmount);
        }
    }
}
using UnityEngine;

public class PlayerColorProgression : MonoBehaviour
{
    public static PlayerColorProgression Instance;

    [Header("Material Reference")]
    public SpriteRenderer playerSpriteRenderer;

    [Range(0f, 1f)] public float saturationIncrease = 0.2f;

    private Material instancedMaterial;
    private int saturationPropertyID;
    private float targetAmount;

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
            float savedSaturation = PlayerPrefs.GetFloat("PlayerSaturation");
            SetSaturation(savedSaturation);
        }
    }

    private void Update()
    {
        float current = PlayerPrefs.GetFloat("PlayerSaturation");
        SetSaturation(targetAmount);
    }
    public void CompleteAnimationScene()
    {
        SetSaturation(1);
        Debug.Log("[PlayerColor] Animation scene completed. Full color restored!");
    }

    private void SetSaturation(float targetAmount)
    {
        if (instancedMaterial != null)
        {
            instancedMaterial.SetFloat(saturationPropertyID, targetAmount);
        }

        SaveSaturation(targetAmount);
    }

    private void SaveSaturation(float amount)
    {
        PlayerPrefs.SetFloat("PlayerSaturation", amount);
        PlayerPrefs.Save();
    }

    public void AddSaturation()
    {
        float current = PlayerPrefs.GetFloat("PlayerSaturation");

        targetAmount = current + saturationIncrease;
        targetAmount = Mathf.Clamp01(targetAmount);

        SetSaturation(targetAmount);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public float fadeDuration = 2f;
    private float alphaTimer = 0f;

    private Image uiImage;

    public bool fadeIn = true;
    public bool startTransition = false;

    void Start()
    {
        uiImage = GetComponent<Image>();

        Color color = uiImage.color;

        if (fadeIn)
        {
            color.a = 0f;
            alphaTimer = 0f;
        }
        else
        {
            color.a = 1f;
            alphaTimer = 1f;
        }
        uiImage.color = color;

    }
    void Update()
    {
        if (!startTransition) return;

        // Progress the timer based on fade direction
        if (fadeIn)
        {
            alphaTimer += Time.deltaTime / fadeDuration;
            alphaTimer = Mathf.Clamp01(alphaTimer);
        }
        else
        {
            alphaTimer -= Time.deltaTime / fadeDuration;
            alphaTimer = Mathf.Clamp01(alphaTimer);
        }

        Color color = uiImage.color;
        color.a = alphaTimer;
        uiImage.color = color;

        if (fadeIn && alphaTimer >= 1f)
        {
            startTransition = false; 
        }
        else if (!fadeIn && alphaTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public void StartTransition()
    {
        startTransition = true;
    }
}

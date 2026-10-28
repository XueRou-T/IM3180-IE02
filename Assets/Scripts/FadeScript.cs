using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    public float fadeDuration = 2f;
    private float alphaTimer = 1f;

    private Image uiImage;

    void Start()
    {
        uiImage = GetComponent<Image>();
    }
    void Update()
    {
        alphaTimer -= Time.deltaTime / fadeDuration;

        alphaTimer = Mathf.Max(alphaTimer, 0f);

        Color color = uiImage.color;
        color.a = alphaTimer;
        uiImage.color = color;

        if(alphaTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}

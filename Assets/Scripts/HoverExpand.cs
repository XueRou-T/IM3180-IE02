using UnityEngine;
using UnityEngine.EventSystems;

public class HoverExpand : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Vector3 targetScale;
    public float hoverExpandScale = 1.2f;
    public float hoverExpandDuration = 0.2f;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverExpandScale;
    }

    public void OnPointerExit(PointerEventData eventData) 
    {
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            hoverExpandDuration
        );
    }
}

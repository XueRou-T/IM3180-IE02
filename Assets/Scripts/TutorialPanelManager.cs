
using UnityEngine;

public class TutorialPanelManager : MonoBehaviour
{
    [Header("Panels")]
    public RectTransform topPanel;
    public RectTransform bottomPanel;
    public RectTransform leftPanel;
    public RectTransform rightPanel;

    [Header("References")]
    public RectTransform canvasRect;
    public RectTransform uiTarget;
    public Transform worldTarget;
    public Camera worldCamera;

    [Header("Spotlight Settings")]
    public Vector2 padding = new Vector2(20f, 20f);
    public Vector2 worldTargetSize = new Vector2(120f, 100f);

    public void ShowUITarget(RectTransform target)
    {
        uiTarget = target;
        worldTarget = null;

        gameObject.SetActive(true);
        UpdateSpotlight();
    }

    public void ShowWorldTarget(Transform target)
    {
        worldTarget = target;
        uiTarget = null;

        gameObject.SetActive(true);
        UpdateSpotlight();
    }

    public void HideSpotlight()
    {
        uiTarget = null;
        worldTarget = null;
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        UpdateSpotlight();
    }

    private void UpdateSpotlight()
    {
        if (canvasRect == null)
            return;

        Vector2 center;
        Vector2 size;

        if (uiTarget != null)
        {
            Vector3[] corners = new Vector3[4];
            uiTarget.GetWorldCorners(corners);

            Vector2 bottomLeft =
                canvasRect.InverseTransformPoint(corners[0]);

            Vector2 topRight =
                canvasRect.InverseTransformPoint(corners[2]);

            center = (bottomLeft + topRight) * 0.5f;
            size = topRight - bottomLeft;
        }
        else if (worldTarget != null)
        {
            Camera cam = canvasRect.GetComponentInParent<Canvas>()
                .rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay
                ? null
                : worldCamera;

            if (cam == null &&
                canvasRect.GetComponentInParent<Canvas>()
                    .rootCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
            {
                return;
            }

            Vector3 screenPos =
                cam != null
                    ? cam.WorldToScreenPoint(worldTarget.position)
                    : worldCamera.WorldToScreenPoint(worldTarget.position);

            if (screenPos.z <= 0)
                return;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                cam,
                out center
            );

            size = worldTargetSize;
        }
        else
        {
            return;
        }

        size += padding * 2f;

        Rect rect = canvasRect.rect;

        float left = Mathf.Clamp(
            center.x - size.x * 0.5f,
            rect.xMin, rect.xMax);

        float right = Mathf.Clamp(
            center.x + size.x * 0.5f,
            rect.xMin, rect.xMax);

        float bottom = Mathf.Clamp(
            center.y - size.y * 0.5f,
            rect.yMin, rect.yMax);

        float top = Mathf.Clamp(
            center.y + size.y * 0.5f,
            rect.yMin, rect.yMax);

        float canvasCenterX = (rect.xMin + rect.xMax) * 0.5f;
        float canvasCenterY = (rect.yMin + rect.yMax) * 0.5f;

        // TOP panel
        SetPanel(
            topPanel,
            new Vector2(
                canvasCenterX,
                (top + rect.yMax) * 0.5f),
            new Vector2(
                rect.width,
                Mathf.Max(0, rect.yMax - top))
        );

        SetPanel(
            bottomPanel,
            new Vector2(
                canvasCenterX,
                (rect.yMin + bottom) * 0.5f),
            new Vector2(
                rect.width,
                Mathf.Max(0, bottom - rect.yMin))
        );

        SetPanel(
            leftPanel,
            new Vector2(
                (rect.xMin + left) * 0.5f,
                canvasCenterY),
            new Vector2(
                Mathf.Max(0, left - rect.xMin),
                rect.height)
        );

        SetPanel(
            rightPanel,
            new Vector2(
                (right + rect.xMax) * 0.5f,
                canvasCenterY),
            new Vector2(
                Mathf.Max(0, rect.xMax - right),
                rect.height)
        );
    }

    private void SetPanel(
        RectTransform panel,
        Vector2 position,
        Vector2 size)
    {
        if (panel == null)
            return;

        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);

        panel.anchoredPosition = position;
        panel.sizeDelta = size;
    }
}

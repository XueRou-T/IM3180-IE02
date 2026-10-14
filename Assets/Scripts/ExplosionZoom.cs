using UnityEngine;

public class ExplosionZoom : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomIn = 2f;     
    [SerializeField] private float zoomOut = 8f;   
    [SerializeField] private float smoothTime = 0.25f;

    private float currentZoom;
    private float targetZoom;
    private float velocity = 0f;

    void Start()
    {
        if (cam == null)
            cam = Camera.main;

        currentZoom = cam.orthographicSize;
        targetZoom = currentZoom;
    }

    void Update()
    {
        // Left click toggles zoom target
        if (Input.GetMouseButtonDown(0))
        {
            targetZoom = Mathf.Approximately(targetZoom, zoomIn) ? zoomOut : zoomIn;
        }

        // Smoothly move camera towards target zoom
        cam.orthographicSize = Mathf.SmoothDamp(
            cam.orthographicSize,
            targetZoom,
            ref velocity,
            smoothTime
        );
    }
}

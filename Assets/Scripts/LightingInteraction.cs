using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightingInteraction : MonoBehaviour
{
    private Light2D lightSource;
    private BoxCollider2D boxCollider;
    private Canvas canvas;

    [Header("Starting Light Up?")]
    public bool lightOn;

    public bool playerNearby;

    void Start()
    {
        lightSource = GetComponent<Light2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        canvas = gameObject.GetComponentInChildren<Canvas>(true);

        if (canvas != null ) canvas.enabled = false;

        if (lightOn)
            lightSource.enabled = true;
        else 
            lightSource.enabled = false;
    }

    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            ToggleLight();
        }
    }

    private void ToggleLight()
    {
        lightOn = !lightOn;
        lightSource.enabled = lightOn;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
            if(canvas != null) canvas.enabled = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
            if(canvas != null) canvas.enabled = false;
        }
    }
}

using UnityEngine;

public class UIAudioManager : MonoBehaviour
{
    public static UIAudioManager Instance;

    public AudioClip hoverSFX;
    public AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayHoverSound()
    {
        if (hoverSFX != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSFX);
            Debug.Log("PlayHoverSound called");
        }
    }
}
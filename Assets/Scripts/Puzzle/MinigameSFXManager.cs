using UnityEngine;
using UnityEngine.Audio;

public class MinigameSFXManager : MonoBehaviour
{
    public AudioClip walking;

    [Header("Reference")]
    private AudioSource sfxAudioSource;

    private void Awake()
    {
        sfxAudioSource = GetComponent<AudioSource>();
    }

    public void PlayWalkingSound()
    {
        sfxAudioSource.PlayOneShot(walking);
    }

    public void StopAllSound()
    {
        sfxAudioSource.Stop();
    }
}

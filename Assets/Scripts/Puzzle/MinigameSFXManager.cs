using UnityEngine;
using UnityEngine.Audio;

public class MinigameSFXManager : MonoBehaviour
{
    public AudioClip walking;
    public AudioClip falling;
    public AudioClip dead;
    public AudioClip winning;

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
    public void PlayFallingSound()
    {
        sfxAudioSource.PlayOneShot(falling);
    }

    public void PlayDeathSound()
    {
        sfxAudioSource.PlayOneShot(dead);
    }

    public void PlayWinningSound()
    {
        sfxAudioSource.PlayOneShot(winning);
    }

    public void StopAllSound()
    {
        sfxAudioSource.Stop();
    }
}

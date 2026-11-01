using UnityEngine;

public class OverWorldSFX : MonoBehaviour
{
    public AudioClip walking;

    [Header("Reference")]
    private AudioSource sfxAudioSource;

    private void Awake()
    {
        sfxAudioSource = GetComponent<AudioSource>();

        sfxAudioSource.clip = walking;
        sfxAudioSource.loop = true;
    }

    public void PlayWalkingSound()
    {
        if (!sfxAudioSource.isPlaying)
        {
            sfxAudioSource.Play();
        }
    }

    public void StopAllSound()
    {
        sfxAudioSource.Stop();
    }
}

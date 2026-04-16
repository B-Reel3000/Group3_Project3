using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Player SFX")]
    public AudioClip shootSFX;
    public AudioClip laserSFX;
    public AudioClip hitSFX;

    [Header("Explosion SFX")]
    public AudioClip explosionSFX;

    [Header("Enemy SFX")]
    public AudioClip enemyShootSFX;

    [Header("UI SFX")]
    public AudioClip buttonBeepSFX;

    void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;

        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.PlayOneShot(clip);
        sfxSource.pitch = 1f;
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null || musicSource == null) return;

        musicSource.clip = musicClip;
        musicSource.Play();
    }
}
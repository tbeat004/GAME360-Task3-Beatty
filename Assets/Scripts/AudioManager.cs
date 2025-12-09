using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;

    [Header("SFX")]
    public AudioClip shootSFX;
    public AudioClip coinSFX;
    public AudioClip enemyHitSFX;

    private AudioSource musicSource;
    private AudioSource sfxSource;
    private AudioSource loopingSFXSource; // Dedicated source for looping SFX

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;

        sfxSource = gameObject.AddComponent<AudioSource>();
        
        loopingSFXSource = gameObject.AddComponent<AudioSource>();
        loopingSFXSource.loop = true;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    
    public void PlayLoopingSFX(AudioClip clip)
    {
        if (clip != null && loopingSFXSource != null)
        {
            loopingSFXSource.clip = clip;
            loopingSFXSource.Play();
        }
    }
    
    public void StopLoopingSFX()
    {
        if (loopingSFXSource != null)
        {
            loopingSFXSource.Stop();
            loopingSFXSource.clip = null;
        }
    }
}


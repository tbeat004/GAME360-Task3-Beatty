using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameplayMusic;
    public AudioClip earlyGameMusic;  // Early game stage music
    public AudioClip midGameMusic;    // Mid game stage music
    public AudioClip lateGameMusic;   // Late game stage primary music
    public AudioClip lateGameMusic2;  // Late game stage alternate music

    [Header("Gameplay SFX")]
    public AudioClip shootSFX;
    public AudioClip enemyHitSFX;
    
    [Header("Collectible SFX")]
    public AudioClip coinSFX;
    
    [Header("Power-Up SFX")]
    public AudioClip powerUpLoopSFX;
    
    [Header("Achievement & Combo SFX")]
    public AudioClip achievementUnlockSFX;
    public AudioClip comboRankUpSFX; // Used for all combo rank ups with pitch variation

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
        
        // Only change music if it's different from what's currently playing
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            return; // Already playing this track
        }
        
        musicSource.clip = clip;
        musicSource.Play();
        Debug.Log($"AudioManager: Now playing {clip.name}");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
    
    public void PlaySFXWithPitch(AudioClip clip, float pitch)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.pitch = pitch;
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f; // Reset to normal pitch
        }
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


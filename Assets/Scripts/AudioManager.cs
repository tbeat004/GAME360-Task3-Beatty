using UnityEngine;
using System.Collections;

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
    [Range(0f, 1f)]
    public float coinSFXVolume = 0.3f; // Lower volume for coin sounds
    
    [Header("Power-Up SFX")]
    public AudioClip powerUpLoopSFX;
    
    [Header("Achievement & Combo SFX")]
    public AudioClip achievementUnlockSFX;
    public AudioClip comboRankUpSFX; // Used for all combo rank ups with pitch variation
    
    [Header("Music Transition")]
    public float crossfadeDuration = 1.5f; // Smooth transition time

    private AudioSource musicSource;
    private AudioSource musicSourceAlt; // For crossfading
    private AudioSource sfxSource;
    private AudioSource loopingSFXSource; // Dedicated source for looping SFX
    private bool isCrossfading = false;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        
        musicSourceAlt = gameObject.AddComponent<AudioSource>();
        musicSourceAlt.loop = true;
        musicSourceAlt.volume = 0f;

        sfxSource = gameObject.AddComponent<AudioSource>();
        
        loopingSFXSource = gameObject.AddComponent<AudioSource>();
        loopingSFXSource.loop = true;
    }

    public void PlayBGM(AudioClip clip, bool useCrossfade = true, bool loop = true)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: PlayBGM called with null clip");
            return;
        }
        
        Debug.Log($"AudioManager: PlayBGM called - Clip: {clip.name}, Crossfade: {useCrossfade}, Loop: {loop}, Current playing: {musicSource.isPlaying}, Current clip: {musicSource.clip?.name ?? "none"}");
        
        // Only change music if it's different from what's currently playing
        if (musicSource.clip == clip && musicSource.isPlaying)
        {
            Debug.Log("AudioManager: Already playing this track, skipping");
            return; // Already playing this track
        }
        
        if (useCrossfade && musicSource.isPlaying && !isCrossfading)
        {
            Debug.Log("AudioManager: Starting crossfade");
            StartCoroutine(CrossfadeMusic(clip, loop));
        }
        else
        {
            Debug.Log("AudioManager: Playing music directly");
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.volume = 1f;
            musicSource.Play();
            Debug.Log($"AudioManager: Music started - isPlaying: {musicSource.isPlaying}");
        }
    }
    
    private IEnumerator CrossfadeMusic(AudioClip newClip, bool loop = true)
    {
        isCrossfading = true;
        
        // Start new music on alternate source
        musicSourceAlt.clip = newClip;
        musicSourceAlt.loop = loop;
        musicSourceAlt.volume = 0f;
        musicSourceAlt.Play();
        
        float elapsed = 0f;
        
        while (elapsed < crossfadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / crossfadeDuration;
            
            musicSource.volume = 1f - t;
            musicSourceAlt.volume = t;
            
            yield return null;
        }
        
        // Swap sources
        musicSource.Stop();
        AudioSource temp = musicSource;
        musicSource = musicSourceAlt;
        musicSourceAlt = temp;
        musicSourceAlt.volume = 0f;
        
        isCrossfading = false;
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip, volumeScale);
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
    
    public void StopAllMusic()
    {
        musicSource.Stop();
        musicSourceAlt.Stop();
        musicSource.clip = null;
        musicSourceAlt.clip = null;
        StopAllCoroutines();
        isCrossfading = false;
    }
}


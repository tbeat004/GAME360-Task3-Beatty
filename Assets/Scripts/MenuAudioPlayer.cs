using UnityEngine;

public class MenuAudioPlayer : MonoBehaviour
{
    [Header("Menu Music")]
    public AudioClip menuMusic;
    
    [Header("Audio Settings")]
    public float volume = 1f;
    
    private AudioSource audioSource;
    
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.loop = true;
        audioSource.playOnAwake = true;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        
        if (menuMusic != null)
        {
            audioSource.clip = menuMusic;
            audioSource.Play();
        }
    }
    
    public void SetVolume(float newVolume)
    {
        volume = newVolume;
        if (audioSource != null)
        {
            audioSource.volume = newVolume;
        }
    }
}

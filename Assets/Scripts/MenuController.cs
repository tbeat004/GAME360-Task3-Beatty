using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    
    private MenuAudioPlayer audioPlayer;
    
    void Start()
    {
        audioPlayer = FindFirstObjectByType<MenuAudioPlayer>();
        
        if (volumeSlider != null && audioPlayer != null)
        {
            volumeSlider.value = audioPlayer.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }
    
    public void PlayGame()
    {
        // Load the main game scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("TASK1_Beatty");
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();
    }
    
    private void OnVolumeChanged(float value)
    {
        if (audioPlayer != null)
        {
            audioPlayer.SetVolume(value);
        }
    }
}

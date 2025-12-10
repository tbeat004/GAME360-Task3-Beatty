using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuController : MonoBehaviour
{
    [Header("UI References")]
    public Slider volumeSlider;
    public TextMeshProUGUI highScoreText;
    
    private MenuAudioPlayer audioPlayer;
    
    void Start()
    {
        audioPlayer = FindFirstObjectByType<MenuAudioPlayer>();
        
        if (volumeSlider != null && audioPlayer != null)
        {
            volumeSlider.value = audioPlayer.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        
        // Display high score
        if (highScoreText != null)
        {
            int highScore = HighScoreManager.GetHighScore();
            highScoreText.text = $"High Score: {highScore}";
            Debug.Log($"MenuController: Displaying high score: {highScore}");
        }
        else
        {
            Debug.LogWarning("MenuController: highScoreText is not assigned in Inspector!");
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

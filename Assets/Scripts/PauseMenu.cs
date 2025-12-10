using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public Slider volumeSlider;
    
    private bool isPaused = false;
    private float previousTimeScale = 1f;
    
    void Start()
    {
        // Set initial volume slider value
        if (volumeSlider != null)
        {
            volumeSlider.value = AudioListener.volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
    }
    
    private void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
    
    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    public void Pause()
    {
        isPaused = true;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);
        
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f; // Freeze game
        AudioListener.pause = true; // Pause all audio
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    public void Resume()
    {
        isPaused = false;
        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
            
        Time.timeScale = previousTimeScale; // Resume game
        AudioListener.pause = false; // Resume audio
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public void QuitToMenu()
    {
        Time.timeScale = 1f; // Reset time scale before changing scenes
        AudioListener.pause = false; // Unpause audio system
        
        // Stop all game music
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.StopAllMusic();
        }
        
        SceneManager.LoadScene("Main Menu");
    }
    
    public bool IsPaused()
    {
        return isPaused;
    }
}

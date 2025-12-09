using UnityEngine;
using TMPro; // or using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI achievementText;

    private void Start()
    {
        // Subscribe to events
        EventManager.Instance.Subscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Subscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Subscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        EventManager.Instance.Unsubscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Unsubscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Unsubscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
    }

    private void OnScoreChanged(object data)
    {
        int score = (int)data;
        scoreText.text = $"Score: {score}";
        Debug.Log("UIManager: Updated score to " + score);
    }

    private void OnTimerTicked(object data)
    {
        float timeRemaining = (float)data;
        timerText.text = $"Time Left: {timeRemaining:F2}";
    }


    private void OnLevelComplete(object data)
    {
        levelCompletePanel.SetActive(true);
        Debug.Log("UIManager: Showing level complete screen");
    }

    private void OnAchievementUnlocked(object data)
    {
        string achievementName = (string)data;

        achievementText.enabled = true;
        achievementText.text = "Achievement Unlocked: " + achievementName;
        Debug.Log("UIManager: Showing achievement popup for " + achievementName);

        // Hide after 3 seconds 
        Invoke("HideAchievementPopup", 3f);
    }

    private void HideAchievementPopup()
    {
        achievementText.enabled = false;
    }
}

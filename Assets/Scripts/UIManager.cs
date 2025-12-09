using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI achievementText;
    
    private float previousTime;
    private Color originalTimerColor;
    private Vector3 originalTimerScale;

    private void Start()
    {
        // Store original timer properties
        if (timerText != null)
        {
            originalTimerColor = timerText.color;
            originalTimerScale = timerText.transform.localScale;
        }
        
        // Subscribe to events
        EventManager.Instance.Subscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Subscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Subscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onBulletMissed, OnBulletMissed);
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        EventManager.Instance.Unsubscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Unsubscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Unsubscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Unsubscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Unsubscribe(GameEvents.onBulletMissed, OnBulletMissed);
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
        previousTime = timeRemaining;
    }
    
    private void OnEnemyDefeated(object data)
    {
        // Timer gained time - flash green and scale up
        StartCoroutine(TimerFeedback(Color.green, 1.3f, 0.3f));
    }
    
    private void OnBulletMissed(object data)
    {
        // Timer lost time - flash red and scale down
        StartCoroutine(TimerFeedback(Color.red, 0.8f, 0.3f));
    }
    
    private IEnumerator TimerFeedback(Color flashColor, float targetScale, float duration)
    {
        if (timerText == null) yield break;
        
        float elapsed = 0f;
        Vector3 targetScaleVector = originalTimerScale * targetScale;
        
        // Animate to target color and scale
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);
            
            timerText.color = Color.Lerp(originalTimerColor, flashColor, t);
            timerText.transform.localScale = Vector3.Lerp(originalTimerScale, targetScaleVector, t);
            
            yield return null;
        }
        
        // Animate back to original
        elapsed = 0f;
        while (elapsed < duration / 2f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (duration / 2f);
            
            timerText.color = Color.Lerp(flashColor, originalTimerColor, t);
            timerText.transform.localScale = Vector3.Lerp(targetScaleVector, originalTimerScale, t);
            
            yield return null;
        }
        
        // Ensure we're back to original
        timerText.color = originalTimerColor;
        timerText.transform.localScale = originalTimerScale;
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

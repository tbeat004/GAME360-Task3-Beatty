using UnityEngine;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI chargesText;
    [SerializeField] private TextMeshProUGUI powerUpText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TextMeshProUGUI achievementText;
    [SerializeField] private TextMeshProUGUI stageTransitionText;
    
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
        EventManager.Instance.Subscribe(GameEvents.onChargesUpdated, OnChargesUpdated);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
        EventManager.Instance.Subscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
        
        // Initialize charge and power-up UI
        if (chargesText != null)
        {
            chargesText.text = "Charges: 0/10";
            Debug.Log("UIManager: Initialized charges text to: " + chargesText.text);
        }
        else
        {
            Debug.LogError("UIManager: chargesText is NULL! Assign it in the inspector!");
        }
        
        if (powerUpText != null)
            powerUpText.enabled = false;
            
        if (comboText != null)
            comboText.text = "";
            
        if (stageTransitionText != null)
            stageTransitionText.enabled = false;
    }
    
    void Update()
    {
        // Update combo display every frame
        if (comboText != null && ComboSystem.Instance != null)
        {
            ComboRank combo = ComboSystem.Instance.GetCurrentCombo();
            if (combo != ComboRank.None)
            {
                int hits = ComboSystem.Instance.GetConsecutiveHits();
                float timeLeft = ComboSystem.Instance.GetTimeUntilDecay();
                comboText.text = $"COMBO: {combo} ({hits} hits) - {timeLeft:F1}s";
                
                // Color based on rank
                comboText.color = GetComboColor(combo);
            }
            else
            {
                comboText.text = "";
            }
        }
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
        EventManager.Instance.Unsubscribe(GameEvents.onChargesUpdated, OnChargesUpdated);
        EventManager.Instance.Unsubscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Unsubscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
        EventManager.Instance.Unsubscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
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
        achievementText.text = "" + achievementName;
        Debug.Log("UIManager: Showing achievement popup for " + achievementName);

        // Hide after 3 seconds 
        Invoke("HideAchievementPopup", 3f);
    }

    private void HideAchievementPopup()
    {
        achievementText.enabled = false;
    }
    
    private void OnChargesUpdated(object data)
    {
        if (chargesText != null)
        {
            int charges = (int)data;
            chargesText.text = string.Format("Charges: {0}/10", charges);
            Debug.Log($"UIManager: Charges text updated to: {chargesText.text}");
        }
        else
        {
            Debug.LogError("UIManager: chargesText is null!");
        }
    }
    
    private void OnPowerUpActivated(object data)
    {
        float duration = (float)data;
        if (powerUpText != null)
        {
            powerUpText.enabled = true;
            powerUpText.color = Color.yellow;
            powerUpText.text = "POWER-UP ACTIVE! +20% Score!";
            Debug.Log("UIManager: Power-up activated!");
        }
        
        // Reset charges display when power-up activates
        if (chargesText != null)
        {
            chargesText.text = "Charges: 0/10";
        }
        
        StartCoroutine(UpdatePowerUpTimer());
    }
    
    private void OnPowerUpDeactivated(object data)
    {
        if (powerUpText != null)
        {
            powerUpText.enabled = false;
            Debug.Log("UIManager: Power-up deactivated");
        }
    }
    
    private Color GetComboColor(ComboRank combo)
    {
        switch (combo)
        {
            case ComboRank.C: return Color.white;
            case ComboRank.B: return Color.cyan;
            case ComboRank.A: return Color.green;
            case ComboRank.S: return Color.yellow;
            case ComboRank.SS: return new Color(1f, 0.5f, 0f); // Orange
            case ComboRank.SSS: return Color.red;
            default: return Color.white;
        }
    }
    
    // Updates the timer of the power up
    private IEnumerator UpdatePowerUpTimer()
    {
        while (GameManager.Instance.IsPowerUpActive())
        {
            float timeLeft = GameManager.Instance.GetPowerUpTimeLeft();
            if (powerUpText != null)
            {
                powerUpText.text = $"POWER-UP! +20% Score ({timeLeft:F1}s)";
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    private void OnGameStageChanged(object data)
    {
        GameStageData stageData = (GameStageData)data;
        
        // Don't show transition for early game (initial stage)
        if (stageData.stage == GameStage.Early)
            return;
            
        string message = "";
        Color textColor = Color.white;
        
        if (stageData.stage == GameStage.Mid)
        {
            message = "MID GAME\nDifficulty Rising!";
            textColor = new Color(1f, 0.65f, 0f); // Orange
        }
        else if (stageData.stage == GameStage.Late)
        {
            message = "LATE GAME\nSurvive!";
            textColor = new Color(1f, 0.2f, 0.2f); // Red
        }
        
        StartCoroutine(ShowStageTransition(message, textColor));
    }
    
    private IEnumerator ShowStageTransition(string message, Color color)
    {
        if (stageTransitionText == null)
            yield break;
            
        stageTransitionText.text = message;
        stageTransitionText.color = color;
        stageTransitionText.enabled = true;
        
        // Scale up animation
        stageTransitionText.transform.localScale = Vector3.zero;
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float scale = Mathf.Lerp(0f, 1.5f, t); // Overshoot to 1.5x
            stageTransitionText.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        // Settle to normal scale
        stageTransitionText.transform.localScale = Vector3.one * 1.2f;
        
        // Hold for 2 seconds
        yield return new WaitForSeconds(2f);
        
        // Fade out
        duration = 1f;
        elapsed = 0f;
        Color startColor = color;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            Color fadeColor = startColor;
            fadeColor.a = Mathf.Lerp(1f, 0f, t);
            stageTransitionText.color = fadeColor;
            yield return null;
        }
        
        stageTransitionText.enabled = false;
    }
}

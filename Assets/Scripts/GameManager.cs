using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;


    public int score = 0;

    [Header("UI Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Round Settings")]
    public float roundTime = 120f;
    private float timeLeft;
    private bool isRoundActive;
    
    [Header("Power-Up System")]
    public int chargesNeededForPowerUp = 10;
    public float powerUpDuration = 15f;
    public float powerUpScoreBoost = 0.20f; // 20% additive boost
    
    private int currentCharges = 0;
    private bool isPowerUpActive = false;
    private float powerUpTimeLeft = 0f;
    private float scoreMultiplier = 1.0f;
    private float stageScoreMultiplier = 1.0f; // Additional multiplier from game stage

 void Update()
{
    if (!isRoundActive) return;

    timeLeft -= Time.deltaTime;
    if (timeLeft < 0f) timeLeft = 0f;
    
    // Handle power-up duration
    if (isPowerUpActive)
    {
        powerUpTimeLeft -= Time.deltaTime;
        if (powerUpTimeLeft <= 0f)
        {
            DeactivatePowerUp();
        }
    }
    
    // Trigger event every frame for immediate UI updates
    EventManager.Instance.TriggerEvent(GameEvents.onTimerTicked, timeLeft);
    
    if (timeLeft <= 0f)
        EndRound();
}


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        score = 0;
        timeLeft = roundTime;
        isRoundActive = true;
        currentCharges = 0;
        scoreMultiplier = 1.0f;
        
        // Subscribe to events for timer modification
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onBulletMissed, OnBulletMissed);
        EventManager.Instance.Subscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Subscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.Instance?.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance?.Unsubscribe(GameEvents.onBulletMissed, OnBulletMissed);
        EventManager.Instance?.Unsubscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance?.Unsubscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
    }

    public void AddScore(int amount)
    {
        // Apply score multipliers: base (1.0) + power-up bonus (0.2) + stage bonus (0.5, 1.0, etc.)
        // Multipliers are additive: 1.0 + 0.2 (power-up) + 0.5 (mid game) = 1.7x total
        float totalMultiplier = scoreMultiplier + (stageScoreMultiplier - 1.0f);
        int finalAmount = Mathf.RoundToInt(amount * totalMultiplier);
        score += finalAmount;
        EventManager.Instance.TriggerEvent(GameEvents.onScoreChanged, score);
    }
    
    public int GetCurrentCharges()
    {
        return currentCharges;
    }
    
    public bool IsPowerUpActive()
    {
        return isPowerUpActive;
    }
    
    public float GetPowerUpTimeLeft()
    {
        return powerUpTimeLeft;
    }
    
    public float GetTimeLeft()
    {
        return timeLeft;
    }

    private void OnEnemyDefeated(object data)
    {
        // Add 3 seconds to timer when enemy is defeated
        timeLeft += 3f;
        Debug.Log("GameManager: Enemy defeated! Added 3 seconds to timer");
    }

    private void OnBulletMissed(object data)
    {
        // Get stage-based penalty from GameStageManager
        float penalty = 1f; // Default penalty
        if (GameStageManager.Instance != null)
        {
            penalty = GameStageManager.Instance.GetBulletMissPenalty();
        }
        
        // Remove time from timer when bullet misses
        timeLeft -= penalty;
        if (timeLeft < 0f) timeLeft = 0f;
        Debug.Log($"GameManager: Bullet missed! Removed {penalty} seconds from timer");
    }
    
    private void OnCollectibleCollected(object data)
    {
        // Get stage-based time bonus from GameStageManager
        float timeBonus = 2f; // Default bonus
        if (GameStageManager.Instance != null)
        {
            timeBonus = GameStageManager.Instance.GetCollectibleTimeBonus();
        }
        
        // Add time bonus for collecting collectibles
        timeLeft += timeBonus;
        Debug.Log($"GameManager: Collectible collected! Added {timeBonus} seconds to timer");
        
        // Don't collect charges while power-up is active
        if (isPowerUpActive)
        {
            Debug.Log("GameManager: Power-up active - charges not counted");
            return;
        }
        
        currentCharges++;
        Debug.Log($"GameManager: Charge collected! ({currentCharges}/{chargesNeededForPowerUp})");
        
        // Trigger charge update event with the NEW charge count
        EventManager.Instance.TriggerEvent(GameEvents.onChargesUpdated, currentCharges);
        
        // Activate power-up when reaching required charges
        if (currentCharges >= chargesNeededForPowerUp)
        {
            ActivatePowerUp();
        }
    }
    
    private void OnGameStageChanged(object data)
    {
        GameStageData stageData = (GameStageData)data;
        stageScoreMultiplier = stageData.scoreMultiplier;
        Debug.Log($"GameManager: Game stage changed to {stageData.stage}. Score multiplier now {stageScoreMultiplier}x");
    }
    
    private void ActivatePowerUp()
    {
        currentCharges = 0; // Reset charges
        isPowerUpActive = true;
        powerUpTimeLeft = powerUpDuration;
        scoreMultiplier = 1.0f + powerUpScoreBoost; // 1.2x score
        
        EventManager.Instance.TriggerEvent(GameEvents.onPowerUpActivated, powerUpDuration);
        Debug.Log($"GameManager: POWER-UP ACTIVATED! {powerUpScoreBoost * 100}% score boost for {powerUpDuration} seconds!");
    }
    
    private void DeactivatePowerUp()
    {
        isPowerUpActive = false;
        powerUpTimeLeft = 0f;
        scoreMultiplier = 1.0f; // Back to normal
        
        EventManager.Instance.TriggerEvent(GameEvents.onPowerUpDeactivated, null);
        Debug.Log("GameManager: Power-up deactivated");
    }


    private void EndRound()
    {
        isRoundActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Main Menu");
    }


    
}


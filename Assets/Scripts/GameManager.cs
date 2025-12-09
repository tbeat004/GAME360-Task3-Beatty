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
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.Instance?.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance?.Unsubscribe(GameEvents.onBulletMissed, OnBulletMissed);
        EventManager.Instance?.Unsubscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
    }

    public void AddScore(int amount)
    {
        // Apply score multiplier (1.0 normal, 1.2 during power-up)
        int finalAmount = Mathf.RoundToInt(amount * scoreMultiplier);
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
        // Remove 1 second from timer when bullet misses
        timeLeft -= 1f;
        if (timeLeft < 0f) timeLeft = 0f;
        Debug.Log("GameManager: Bullet missed! Removed 1 second from timer");
    }
    
    private void OnCollectibleCollected(object data)
    {
        currentCharges++;
        Debug.Log($"GameManager: Charge collected! ({currentCharges}/{chargesNeededForPowerUp})");
        
        // Activate power-up when reaching required charges
        if (currentCharges >= chargesNeededForPowerUp)
        {
            ActivatePowerUp();
        }
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


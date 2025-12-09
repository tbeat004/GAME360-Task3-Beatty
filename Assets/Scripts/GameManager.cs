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

 void Update()
{
    if (!isRoundActive) return;

    timeLeft -= Time.deltaTime;
    if (timeLeft < 0f) timeLeft = 0f;
    
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
        
        // Subscribe to events for timer modification
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onBulletMissed, OnBulletMissed);
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        EventManager.Instance?.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance?.Unsubscribe(GameEvents.onBulletMissed, OnBulletMissed);
    }

    public void AddScore(int amount)
    {
        score += amount;
        EventManager.Instance.TriggerEvent(GameEvents.onScoreChanged, score);
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


    private void EndRound()
    {
        isRoundActive = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("Main Menu");
    }


    
}


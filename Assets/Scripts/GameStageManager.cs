using UnityEngine;
using System.Collections;

public class GameStageManager : MonoBehaviour
{
    public static GameStageManager Instance;
    
    [Header("Stage Configuration")]
    public GameStageData[] stages;
    
    private GameStage currentStage = GameStage.Early;
    private int currentStageIndex = 0;
    private bool isAlternatingMusic = false;
    private float elapsedTime = 0f; // Independent timer tracking time since round start
    private bool roundActive = false;
    
    [Header("Late Game Speed Scaling")]
    public float lateGameSpeedInterval = 10f; // Increase speed every 10 seconds in late game
    public float lateGameSpeedIncrement = 0.1f; // Add 10% speed each interval
    public float lateGameSpeedCap = 2.5f; // Max 2.5x speed multiplier
    
    private float lateGameStartTime = 0f;
    private float currentLateGameSpeedBonus = 0f;
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Debug.Log("GameStageManager: Awake called");
    }
    
    void Start()
    {
        Debug.Log("GameStageManager: Start called");
        
        // Initialize default stages if not set in Inspector
        if (stages == null || stages.Length == 0)
        {
            Debug.Log("GameStageManager: Initializing default stages");
            stages = new GameStageData[]
            {
                new GameStageData(GameStage.Early, 0f, 1.0f, 0.0f, 1.0f) 
                { 
                    bulletMissPenalty = 1f,      // Early: -1s for missing
                    collectibleTimeBonus = 1.5f,  // Early: +1.5s for collectibles
                    stageMusic = AudioManager.Instance?.earlyGameMusic
                },
                new GameStageData(GameStage.Mid, 60f, 1.3f, 0.2f, 1.5f) 
                { 
                    bulletMissPenalty = 2f,      // Mid: -2s for missing  
                    collectibleTimeBonus = 1.5f,  // Mid: +1.5s for collectibles
                    stageMusic = AudioManager.Instance?.midGameMusic
                },
                new GameStageData(GameStage.Late, 180f, 1.6f, 0.5f, 2.0f) 
                { 
                    bulletMissPenalty = 3f,      // Late: -3s for missing
                    collectibleTimeBonus = 1.5f,  // Late: +1.5s for collectibles
                    stageMusic = AudioManager.Instance?.lateGameMusic,
                    alternateMusic = AudioManager.Instance?.lateGameMusic2
                }
            };
        }
        
        // Subscribe to timer events to check for stage transitions
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
        
        // Start in early game
        currentStage = GameStage.Early;
        currentStageIndex = 0;
        elapsedTime = 0f;
        roundActive = true;
        EventManager.Instance.TriggerEvent(GameEvents.onGameStageChanged, stages[0]);
        Debug.Log("GameStageManager: Started in Early Game stage");
    }
    
    void Update()
    {
        
        if (!roundActive) return;
        
        // Update elapsed time independently every frame
        elapsedTime += Time.deltaTime;
        
        // Debug log every second
        if (Mathf.FloorToInt(elapsedTime) != Mathf.FloorToInt(elapsedTime - Time.deltaTime))
        {
            Debug.Log($"GameStageManager: Elapsed time = {elapsedTime:F2}s, Current stage = {currentStage}, Stage index = {currentStageIndex}");
        }
        
        // Check if we should transition to the next stage
        for (int i = stages.Length - 1; i >= 0; i--)
        {
            if (elapsedTime >= stages[i].startTime && i > currentStageIndex)
            {
                Debug.Log($"GameStageManager: Triggering transition to stage {i} ({stages[i].stage})");
                TransitionToStage(i);
                break;
            }
        }
    }
    
    void OnDestroy()
    {
        EventManager.Instance?.Unsubscribe(GameEvents.onTimerTicked, OnTimerTicked);
        roundActive = false;
    }
    
    private void OnTimerTicked(object data)
    {
        // We still subscribe to this event to detect when the round ends
        float timeRemaining = (float)data;
        
        if (timeRemaining <= 0f)
        {
            roundActive = false;
        }
    }
    
    private void TransitionToStage(int stageIndex)
    {
        if (stageIndex < 0 || stageIndex >= stages.Length) return;
        
        currentStageIndex = stageIndex;
        currentStage = stages[stageIndex].stage;
        
        // Track when late game starts for progressive speed scaling
        if (currentStage == GameStage.Late)
        {
            lateGameStartTime = elapsedTime;
            currentLateGameSpeedBonus = 0f;
            Debug.Log($"GameStageManager: Late game started at {lateGameStartTime:F2}s");
        }
        
        Debug.Log($"GameStageManager: Transitioning to {currentStage} Game stage");
        
        // Trigger event with the new stage data
        EventManager.Instance.TriggerEvent(GameEvents.onGameStageChanged, stages[stageIndex]);
        
        // If this is Late game and has alternate music, start alternating
        if (currentStage == GameStage.Late && stages[stageIndex].alternateMusic != null)
        {
            StartCoroutine(AlternateLateGameMusic(stageIndex));
        }
    }
    
    private IEnumerator AlternateLateGameMusic(int stageIndex)
    {
        isAlternatingMusic = true;
        bool useAlternate = false;
        
        while (currentStageIndex == stageIndex && isAlternatingMusic)
        {
            AudioClip musicToPlay = useAlternate ? 
                stages[stageIndex].alternateMusic : 
                stages[stageIndex].stageMusic;
            
            if (musicToPlay != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBGM(musicToPlay);
                Debug.Log($"GameStageManager: Playing {(useAlternate ? "alternate" : "primary")} late game music");
            }
            
            // Wait for the music to finish
            if (musicToPlay != null)
            {
                yield return new WaitForSeconds(musicToPlay.length);
            }
            else
            {
                yield return new WaitForSeconds(30f); // Default wait time
            }
            
            useAlternate = !useAlternate;
        }
        
        isAlternatingMusic = false;
    }
    
    public GameStage GetCurrentStage()
    {
        return currentStage;
    }
    
    public GameStageData GetCurrentStageData()
    {
        if (currentStageIndex >= 0 && currentStageIndex < stages.Length)
            return stages[currentStageIndex];
        return null;
    }
    
    public float GetEnemySpeedMultiplier()
    {
        float baseMultiplier = stages[currentStageIndex].enemySpeedMultiplier;
        
        // Add progressive speed bonus in late game
        if (currentStage == GameStage.Late)
        {
            float totalMultiplier = baseMultiplier + currentLateGameSpeedBonus;
            // Cap at maximum speed
            return Mathf.Min(totalMultiplier, lateGameSpeedCap);
        }
        
        return baseMultiplier;
    }
    
    public float GetAirSpawnChance()
    {
        return stages[currentStageIndex].airSpawnChance;
    }
    
    public float GetScoreMultiplier()
    {
        return stages[currentStageIndex].scoreMultiplier;
    }
    
    public float GetBulletMissPenalty()
    {
        return stages[currentStageIndex].bulletMissPenalty;
    }
    
    public float GetCollectibleTimeBonus()
    {
        return stages[currentStageIndex].collectibleTimeBonus;
    }
}

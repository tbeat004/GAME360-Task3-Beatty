using UnityEngine;

public enum ComboRank
{
    None = 0,
    C = 1,
    B = 2,
    A = 3,
    S = 4,
    SS = 5,
    SSS = 6
}

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance;
    
    [Header("Combo Configuration")]
    public float baseDecayTime = 12f; // Base time before combo is lost (increased from 8s)
    public float decayMultiplierPerRank = 0.9f; // Each rank decays only slightly faster (90% of previous, was 85%)
    
    [Header("Combo Bonuses")]
    public float scoreMultiplierPerRank = 0.15f; // Each rank adds 15% score boost
    public float timeMultiplierPerRank = 0.1f; // Each rank adds 10% time boost
    
    private ComboRank currentCombo = ComboRank.None;
    private float timeSinceLastHit = 0f;
    private float currentDecayTime;
    private int consecutiveHits = 0;
    
    // Thresholds for ranking up
    private int[] rankUpThresholds = { 0, 3, 6, 10, 15, 22, 30 }; // Hits needed for C, B, A, S, SS, SSS
    
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        UpdateDecayTime();
    }
    
    void Start()
    {
        // Subscribe to enemy defeated events
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
    }
    
    void OnDestroy()
    {
        EventManager.Instance?.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
    }
    
    void Update()
    {
        if (currentCombo == ComboRank.None) return;
        
        timeSinceLastHit += Time.deltaTime;
        
        // Check if combo should decay
        if (timeSinceLastHit >= currentDecayTime)
        {
            DecayCombo();
            timeSinceLastHit = 0f;
        }
    }
    
    private void OnEnemyDefeated(object data)
    {
        consecutiveHits++;
        timeSinceLastHit = 0f;
        
        // Check if we rank up
        ComboRank previousRank = currentCombo;
        UpdateComboRank();
        
        if (currentCombo != previousRank)
        {
            Debug.Log($"ComboSystem: Combo rank up to {currentCombo}! ({consecutiveHits} hits)");
            UpdateDecayTime();
        }
        
        Debug.Log($"ComboSystem: Combo {currentCombo} - {consecutiveHits} consecutive hits");
    }
    
    private void UpdateComboRank()
    {
        // Determine rank based on consecutive hits
        for (int i = rankUpThresholds.Length - 1; i >= 0; i--)
        {
            if (consecutiveHits >= rankUpThresholds[i])
            {
                currentCombo = (ComboRank)i;
                return;
            }
        }
        currentCombo = ComboRank.None;
    }
    
    private void DecayCombo()
    {
        if (currentCombo == ComboRank.None) return;
        
        Debug.Log($"ComboSystem: Combo lost from {currentCombo}!");
        
        // Combo completely resets when time runs out
        ResetCombo();
    }
    
    private void ResetCombo()
    {
        Debug.Log("ComboSystem: Combo lost!");
        currentCombo = ComboRank.None;
        consecutiveHits = 0;
        timeSinceLastHit = 0f;
    }
    
    private void UpdateDecayTime()
    {
        if (currentCombo == ComboRank.None)
        {
            currentDecayTime = baseDecayTime;
            return;
        }
        
        // Higher ranks decay faster
        int rank = (int)currentCombo;
        currentDecayTime = baseDecayTime * Mathf.Pow(decayMultiplierPerRank, rank);
        
        Debug.Log($"ComboSystem: Decay time for {currentCombo}: {currentDecayTime:F2}s");
    }
    
    public ComboRank GetCurrentCombo()
    {
        return currentCombo;
    }
    
    public float GetScoreMultiplier()
    {
        if (currentCombo == ComboRank.None) return 0f;
        return (int)currentCombo * scoreMultiplierPerRank;
    }
    
    public float GetTimeMultiplier()
    {
        if (currentCombo == ComboRank.None) return 0f;
        return (int)currentCombo * timeMultiplierPerRank;
    }
    
    public int GetConsecutiveHits()
    {
        return consecutiveHits;
    }
    
    public float GetTimeUntilDecay()
    {
        if (currentCombo == ComboRank.None) return 0f;
        return Mathf.Max(0f, currentDecayTime - timeSinceLastHit);
    }
}

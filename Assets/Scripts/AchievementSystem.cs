using UnityEngine;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour
{
    private int enemiesDefeated = 0;
    private int collectiblesCollected = 0;
    private int currentScore = 0;
    
    private HashSet<string> unlockedAchievements = new HashSet<string>();

    private void Start()
    {
        // Subscribe to events
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Subscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        EventManager.Instance.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Unsubscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Unsubscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Unsubscribe(GameEvents.onTimerTicked, OnTimerTicked);
    }

    private void OnEnemyDefeated(object data)
    {
        enemiesDefeated++;
        Debug.Log($"AchievementSystem: Enemies defeated: {enemiesDefeated}");
        
        if (enemiesDefeated >= 10 && !unlockedAchievements.Contains("Enemy Hunter"))
        {
            UnlockAchievement("Enemy Hunter");
        }
    }

    private void OnCollectibleCollected(object data)
    {
        collectiblesCollected++;
        Debug.Log($"AchievementSystem: Collectibles collected: {collectiblesCollected}");
        
        if (collectiblesCollected >= 5 && !unlockedAchievements.Contains("Collector"))
        {
            UnlockAchievement("Collector");
        }
    }

    private void OnScoreChanged(object data)
    {
        currentScore = (int)data;
        Debug.Log($"AchievementSystem: Score updated: {currentScore}");
        
        if (currentScore >= 1000 && !unlockedAchievements.Contains("High Scorer"))
        {
            UnlockAchievement("High Scorer");
        }
    }

    private void OnTimerTicked(object data)
    {
        float timeRemaining = (float)data;
        
        // Check for Enemy Hunter Achievement (complete with 85+ seconds remaining)
        if (timeRemaining >= 85f && !unlockedAchievements.Contains("Enemy hunter"))
        {
            UnlockAchievement("Enemy Destroyer");
        }
        
    }

    private void UnlockAchievement(string achievementName)
    {
        unlockedAchievements.Add(achievementName);
        Debug.Log($"AchievementSystem: Achievement Unlocked - {achievementName}!");
        
        // Trigger achievement unlocked event
        EventManager.Instance.TriggerEvent(GameEvents.onAchievementUnlocked, achievementName);
    }
}

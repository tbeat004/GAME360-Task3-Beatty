using UnityEngine;
using System.Collections.Generic;

public class AchievementSystem : MonoBehaviour
{
    private int enemiesDefeated = 0;
    private int collectiblesCollected = 0;
    private int currentScore = 0;
    private bool reachedMidGame = false;
    private bool reachedLateGame = false;
    private int bulletsFired = 0;
    private int bulletsHit = 0;
    
    private HashSet<string> unlockedAchievements = new HashSet<string>();

    private void Start()
    {
        // Subscribe to events
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Subscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Subscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
        EventManager.Instance.Subscribe(GameEvents.onBulletShot, OnBulletShot);
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        EventManager.Instance.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Unsubscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Unsubscribe(GameEvents.onScoreChanged, OnScoreChanged);
        EventManager.Instance.Unsubscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Unsubscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Unsubscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
        EventManager.Instance.Unsubscribe(GameEvents.onBulletShot, OnBulletShot);
    }
    
    private void OnBulletShot(object data)
    {
        bulletsFired++;
    }

    private void OnEnemyDefeated(object data)
    {
        enemiesDefeated++;
        bulletsHit++;
        Debug.Log($"AchievementSystem: Enemies defeated: {enemiesDefeated}");
        
        // Tier 1: First Blood
        if (enemiesDefeated >= 1 && !unlockedAchievements.Contains("First Blood"))
        {
            UnlockAchievement("First Blood");
        }
        
        // Tier 2: Enemy Hunter
        if (enemiesDefeated >= 25 && !unlockedAchievements.Contains("Enemy Hunter"))
        {
            UnlockAchievement("Enemy Hunter");
        }
        
        // Tier 3: Enemy Slayer
        if (enemiesDefeated >= 50 && !unlockedAchievements.Contains("Enemy Slayer"))
        {
            UnlockAchievement("Enemy Slayer");
        }
        
        // Tier 4: Enemy Destroyer
        if (enemiesDefeated >= 100 && !unlockedAchievements.Contains("Enemy Destroyer"))
        {
            UnlockAchievement("Enemy Destroyer");
        }
        
        // Tier 5: Enemy Annihilator
        if (enemiesDefeated >= 200 && !unlockedAchievements.Contains("Enemy Annihilator"))
        {
            UnlockAchievement("Enemy Annihilator");
        }
    }

    private void OnCollectibleCollected(object data)
    {
        collectiblesCollected++;
        Debug.Log($"AchievementSystem: Collectibles collected: {collectiblesCollected}");
        
        // Tier 1: Coin Collector
        if (collectiblesCollected >= 10 && !unlockedAchievements.Contains("Coin Collector"))
        {
            UnlockAchievement("Coin Collector");
        }
        
        // Tier 2: Treasure Hunter
        if (collectiblesCollected >= 25 && !unlockedAchievements.Contains("Treasure Hunter"))
        {
            UnlockAchievement("Treasure Hunter");
        }
        
        // Tier 3: Hoarder
        if (collectiblesCollected >= 50 && !unlockedAchievements.Contains("Hoarder"))
        {
            UnlockAchievement("Hoarder");
        }
    }

    private void OnScoreChanged(object data)
    {
        currentScore = (int)data;
        Debug.Log($"AchievementSystem: Score updated: {currentScore}");
        
        // Score Tier 1
        if (currentScore >= 500 && !unlockedAchievements.Contains("Score Starter"))
        {
            UnlockAchievement("Score Starter");
        }
        
        // Score Tier 2
        if (currentScore >= 1000 && !unlockedAchievements.Contains("High Scorer"))
        {
            UnlockAchievement("High Scorer");
        }
        
        // Score Tier 3
        if (currentScore >= 2500 && !unlockedAchievements.Contains("Score Master"))
        {
            UnlockAchievement("Score Master");
        }
        
        // Score Tier 4
        if (currentScore >= 5000 && !unlockedAchievements.Contains("Point Legend"))
        {
            UnlockAchievement("Point Legend");
        }
        
        // Score Tier 5
        if (currentScore >= 10000 && !unlockedAchievements.Contains("Ultimate Scorer"))
        {
            UnlockAchievement("Ultimate Scorer");
        }
    }

    private void OnTimerTicked(object data)
    {
        float timeRemaining = (float)data;
        
        // Check combo achievements
        if (ComboSystem.Instance != null)
        {
            ComboRank currentCombo = ComboSystem.Instance.GetCurrentCombo();
            
            // Combo rank achievements
            if (currentCombo >= ComboRank.B && !unlockedAchievements.Contains("Combo Starter"))
            {
                UnlockAchievement("Combo Starter");
            }
            
            if (currentCombo >= ComboRank.S && !unlockedAchievements.Contains("Combo Master"))
            {
                UnlockAchievement("Combo Master");
            }
            
            if (currentCombo >= ComboRank.SSS && !unlockedAchievements.Contains("Combo God"))
            {
                UnlockAchievement("Combo God");
            }
        }
    }
    
    private void OnGameStageChanged(object data)
    {
        GameStageData stageData = (GameStageData)data;
        
        if (stageData.stage == GameStage.Mid && !reachedMidGame)
        {
            reachedMidGame = true;
            if (!unlockedAchievements.Contains("Mid-Game Warrior"))
            {
                UnlockAchievement("Mid-Game Warrior");
            }
        }
        
        if (stageData.stage == GameStage.Late && !reachedLateGame)
        {
            reachedLateGame = true;
            if (!unlockedAchievements.Contains("Late-Game Legend"))
            {
                UnlockAchievement("Late-Game Legend");
            }
            
            // Survival achievement - reaching late game with specific score
            if (currentScore >= 3000 && !unlockedAchievements.Contains("Elite Survivor"))
            {
                UnlockAchievement("Elite Survivor");
            }
        }
    }
    
    private void OnPowerUpActivated(object data)
    {
        // Achievement for activating power-up for the first time
        if (!unlockedAchievements.Contains("Charged Up"))
        {
            UnlockAchievement("Charged Up");
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

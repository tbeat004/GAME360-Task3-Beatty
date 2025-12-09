using UnityEngine;

public class SoundTriggerSystem : MonoBehaviour
{
    private bool isPowerUpSoundPlaying = false;
    private ComboRank lastComboRank = ComboRank.None;

    private void Start()
    {
        // Subscribe to events
        EventManager.Instance.Subscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Subscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Subscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Subscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Subscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Subscribe(GameEvents.onBulletShot, OnBulletShot);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
        EventManager.Instance.Subscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
    }

    private void OnDestroy()
    {
        // Unsubscribe to events
        EventManager.Instance.Unsubscribe(GameEvents.onEnemyDefeated, OnEnemyDefeated);
        EventManager.Instance.Unsubscribe(GameEvents.onCollectibleCollected, OnCollectibleCollected);
        EventManager.Instance.Unsubscribe(GameEvents.onAchievementUnlocked, OnAchievementUnlocked);
        EventManager.Instance.Unsubscribe(GameEvents.onLevelComplete, OnLevelComplete);
        EventManager.Instance.Unsubscribe(GameEvents.onTimerTicked, OnTimerTicked);
        EventManager.Instance.Unsubscribe(GameEvents.onBulletShot, OnBulletShot);
        EventManager.Instance.Unsubscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Unsubscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
        EventManager.Instance.Unsubscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
    }
    
    private void Update()
    {
        // Check for combo rank changes to play sound
        if (ComboSystem.Instance != null)
        {
            ComboRank currentRank = ComboSystem.Instance.GetCurrentCombo();
            
            if (currentRank != lastComboRank && currentRank > lastComboRank)
            {
                // Combo ranked up! Play sound with pitch based on rank
                float pitch = 1f + ((int)currentRank * 0.1f); // C=1.1, B=1.2, A=1.3, S=1.4, SS=1.5, SSS=1.6
                AudioManager.Instance.PlaySFXWithPitch(AudioManager.Instance.comboRankUpSFX, pitch);
                Debug.Log($"SoundTriggerSystem: Combo rank up to {currentRank}! Pitch: {pitch}");
            }
            
            lastComboRank = currentRank;
        }
    }

    private void OnEnemyDefeated(object data)
    {
        Debug.Log("SoundTriggerSystem: Enemy defeated - triggering sound");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.enemyHitSFX);
    }

    private void OnCollectibleCollected(object data)
    {
        Debug.Log("SoundTriggerSystem: Collectible collected - triggering sound");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.coinSFX);
    }

    private void OnAchievementUnlocked(object data)
    {
        Debug.Log("SoundTriggerSystem: Achievement unlocked - triggering sound");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.achievementUnlockSFX);
    }

    private void OnLevelComplete(object data)
    {
        Debug.Log("SoundTriggerSystem: Level complete - triggering sound");
    }

    private void OnTimerTicked(object data)
    {
        float timeRemaining = (float)data;
        
        // Play tick sound for last 10 seconds
        if (timeRemaining <= 10f )
        {
            Debug.Log("SoundTriggerSystem: Last 10 - triggering sounds");
           
        }
    }

    private void OnBulletShot(object data)
    {
        Debug.Log("SoundTriggerSystem: Bullet shot - triggering sound");
        AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSFX);
    }
    
    private void OnPowerUpActivated(object data)
    {
        Debug.Log("SoundTriggerSystem: Power-up activated - starting loop sound");
        
        if (AudioManager.Instance.powerUpLoopSFX != null)
        {
            AudioManager.Instance.PlayLoopingSFX(AudioManager.Instance.powerUpLoopSFX);
            isPowerUpSoundPlaying = true;
        }
    }
    
    private void OnPowerUpDeactivated(object data)
    {
        Debug.Log("SoundTriggerSystem: Power-up deactivated - stopping loop sound");
        if (isPowerUpSoundPlaying)
        {
            AudioManager.Instance.StopLoopingSFX();
            isPowerUpSoundPlaying = false;
        }
    }
    
    private void OnGameStageChanged(object data)
    {
        GameStageData stageData = (GameStageData)data;
        Debug.Log($"SoundTriggerSystem: Game stage changed to {stageData.stage}");
        
        // Play the music for this stage (GameStageManager handles alternating for Late game)
        if (stageData.stageMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayBGM(stageData.stageMusic);
        }
    }
}

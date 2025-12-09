using UnityEngine;

public class SoundTriggerSystem : MonoBehaviour
{
    [Header("Sound Effects")]
    [SerializeField] private AudioClip enemyDefeatSound;
    [SerializeField] private AudioClip collectibleSound;
    [SerializeField] private AudioClip achievementSound;
    [SerializeField] private AudioClip levelCompleteSound;
    [SerializeField] private AudioClip timerWarningSound;
    [SerializeField] private AudioClip bulletShotSound;
    [SerializeField] private AudioClip powerUpLoopSound;
    
    private bool isPowerUpSoundPlaying = false;

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
        if (powerUpLoopSound != null)
        {
            AudioManager.Instance.PlayLoopingSFX(powerUpLoopSound);
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
}

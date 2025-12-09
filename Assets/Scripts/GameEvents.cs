using UnityEngine;

public static class GameEvents
{
    public const string onScoreChanged = "onScoreChanged";
    public const string onEnemyDefeated = "onEnemyDefeated";
    public const string onCollectibleCollected = "onCollectibleCollected";
    public const string onLevelComplete = "onLevelComplete";
    public const string onPlayerHealthChanged = "onPlayerHealthChanged";
    public const string onPowerUpCollected = "onPowerUpCollected";
    public const string onAchievementUnlocked = "onAchievementUnlocked";
    public const string onTimerTicked = "onTimerTicked";

    public const string onBulletShot = "onBulletShot";
    public const string onBulletMissed = "onBulletMissed";
    public const string onPowerUpActivated = "onPowerUpActivated";
    public const string onPowerUpDeactivated = "onPowerUpDeactivated";
    public const string onChargesUpdated = "onChargesUpdated";
    public const string onGameStageChanged = "onGameStageChanged";
}

using UnityEngine;

public enum GameStage
{
    Early,
    Mid,
    Late
}

[System.Serializable]
public class GameStageData
{
    public GameStage stage;
    public float startTime;              // When this stage starts (seconds into round)
    public float enemySpeedMultiplier;   // Multiplier for enemy movement speed
    public float airSpawnChance;         // Chance (0-1) for enemies to spawn in air
    public float scoreMultiplier;        // Multiplier for all score gains
    public AudioClip stageMusic;         // Music for this stage
    public AudioClip alternateMusic;     // Optional second music (for Late stage alternating)
    public float bulletMissPenalty;      // Time penalty for missing a shot
    public float collectibleTimeBonus;   // Time bonus for collecting a collectible
    
    public GameStageData(GameStage stage, float startTime, float enemySpeed, float airChance, float scoreBoost)
    {
        this.stage = stage;
        this.startTime = startTime;
        this.enemySpeedMultiplier = enemySpeed;
        this.airSpawnChance = airChance;
        this.scoreMultiplier = scoreBoost;
        this.bulletMissPenalty = 1f; // Default 1 second penalty
        this.collectibleTimeBonus = 2f; // Default 2 second bonus
    }
}

using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;     
    public Transform[] spawnPoints;    
    public float airSpawnHeightOffset = 2f; // How much higher to spawn air enemies
    private GameObject currentEnemy;

    void Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
        
        if (currentEnemy == null)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null) return;

        Transform spawn;
        
        // Check if we should spawn in the air based on current game stage
        bool spawnInAir = false;
        if (GameStageManager.Instance != null)
        {
            float airSpawnChance = GameStageManager.Instance.GetAirSpawnChance();
            spawnInAir = Random.value < airSpawnChance;
        }
        
        // Choose a random spawn point
        int index = Random.Range(0, spawnPoints.Length);
        spawn = spawnPoints[index];
        
        // Calculate spawn position (add height offset if air spawn)
        Vector3 spawnPosition = spawn.position;
        if (spawnInAir)
        {
            spawnPosition.y += airSpawnHeightOffset;
            Debug.Log($"EnemySpawner: Spawning enemy in the air at height {spawnPosition.y}!");
        }

        currentEnemy = Instantiate(enemyPrefab, spawnPosition, spawn.rotation);
        
        // If spawned in air, disable gravity
        if (spawnInAir)
        {
            EnemyController enemyController = currentEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.SetAsAirSpawn();
            }
        }
    }
}

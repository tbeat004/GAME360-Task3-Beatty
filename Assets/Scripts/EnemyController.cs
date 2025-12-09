using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public int maxHealth = 1;
    public int scoreOnDeath = 50;


    public float maxRange = 5f;      
    public float moveSpeed = 2f;    
    public float pauseTime = 0.5f;
    public float airSpawnSpeedMultiplier = 0.6f; // Air enemies move at 60% speed

    private int health;
    private Vector3 startPos;
    private Vector3 strafeAxis;      
    private Vector3 targetPos;
    private bool moving;
    private float currentMoveSpeed; // Actual speed with stage multiplier applied

    void Awake()
    {
        health = maxHealth;
        currentMoveSpeed = moveSpeed; // Initialize to base speed
    }
    
    public void SetAsAirSpawn()
    {
        // Disable gravity for air spawns
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            Debug.Log("EnemyController: Set as air spawn - gravity disabled");
        }
        
        // Reduce speed for air spawns
        currentMoveSpeed *= airSpawnSpeedMultiplier;
        Debug.Log($"EnemyController: Air spawn speed reduced to {currentMoveSpeed}");
        
        // Change color to blue for air spawns
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.blue;
            Debug.Log("EnemyController: Air spawn set to blue color");
        }
    }

    void Start()
    {
        startPos = transform.position;

        strafeAxis = transform.right;
        strafeAxis.y = 0f;
        if (strafeAxis.sqrMagnitude < 0.0001f) strafeAxis = Vector3.right; 
        strafeAxis.Normalize();

        // Apply current game stage speed multiplier
        ApplyStageSpeedMultiplier();
        
        // Subscribe to stage changes to update speed dynamically
        EventManager.Instance.Subscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
        
        ChooseNewTarget();
    }
    
    void OnDestroy()
    {
        // Unsubscribe from stage changes
        EventManager.Instance?.Unsubscribe(GameEvents.onGameStageChanged, OnGameStageChanged);
    }

    void Update()
    {
        if (!moving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, currentMoveSpeed * Time.deltaTime);

        if ((transform.position - targetPos).sqrMagnitude <= 0.0025f) 
        {
            moving = false;
            Invoke(nameof(ChooseNewTarget), pauseTime);
        }
    }

    void ChooseNewTarget()
    {
        if (maxRange <= 0f) return;


        float offset = Random.Range(-maxRange, maxRange);
        targetPos = startPos + strafeAxis * offset;
        moving = true;
    }
    
    private void ApplyStageSpeedMultiplier()
    {
        // Get the current stage multiplier from GameStageManager
        if (GameStageManager.Instance != null)
        {
            float stageMultiplier = GameStageManager.Instance.GetEnemySpeedMultiplier();
            currentMoveSpeed = moveSpeed * stageMultiplier;
            Debug.Log($"EnemyController: Applied stage speed multiplier {stageMultiplier}x. Speed: {currentMoveSpeed}");
        }
        else
        {
            currentMoveSpeed = moveSpeed;
        }
    }
    
    private void OnGameStageChanged(object data)
    {
        // Update speed when stage changes
        ApplyStageSpeedMultiplier();
    }


    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0) Die();
    }

    private void Die()
    {
        GameManager.Instance?.AddScore(scoreOnDeath);
        EventManager.Instance.TriggerEvent(GameEvents.onEnemyDefeated);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
    }

}


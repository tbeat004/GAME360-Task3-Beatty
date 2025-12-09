using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded = false;
    private float baseMoveSpeed; // Store original speed

    public GameObject bulletPrefab;   
    public Transform firePoint;       
    public float fireRate = 0.25f;

    private float nextFire = 0f;
    private StateMachine stateMachine;
    
    // Store state instances
    public IdleState idleState;
    public MovingState movingState;
    public JumpingState jumpingState;
    public AttackingState attackingState;
    
    // Called when script instance is being loaded
    void Awake()
    {
        // Get the Rigidbody component attached to this GameObject
        rb = GetComponent<Rigidbody>();
        stateMachine = GetComponent<StateMachine>();
        
        // Verify components were found
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on Player!");
        }
        else
        {
            Debug.Log("PlayerController Awake - Rigidbody initialized");
        }
        
        if (stateMachine == null)
        {
            Debug.LogError("No StateMachine found on Player!");
        }
        
        // Create state instances ONCE
        idleState = new IdleState(stateMachine, this);
        movingState = new MovingState(stateMachine, this);
        jumpingState = new JumpingState(stateMachine, this);
        attackingState = new AttackingState(stateMachine, this);
    }
    
    // Called before the first frame update
    void Start()
    {
        // Initialize with Idle state
        stateMachine.ChangeState(idleState);
        
        // Store base speed and subscribe to power-up events
        baseMoveSpeed = moveSpeed;
        EventManager.Instance.Subscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance.Subscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
    }
    
    void OnDestroy()
    {
        EventManager.Instance?.Unsubscribe(GameEvents.onPowerUpActivated, OnPowerUpActivated);
        EventManager.Instance?.Unsubscribe(GameEvents.onPowerUpDeactivated, OnPowerUpDeactivated);
    }
    
    private void OnPowerUpActivated(object data)
    {
        moveSpeed = baseMoveSpeed * 2f; // 2x speed during power-up
        Debug.Log($"PlayerController: Power-up activated! Speed boosted to {moveSpeed}");
    }
    
    private void OnPowerUpDeactivated(object data)
    {
        moveSpeed = baseMoveSpeed; // Restore normal speed
        Debug.Log($"PlayerController: Power-up deactivated. Speed restored to {moveSpeed}");
    }
    
    // Called once per frame
    void Update()
    {
        // Update move speed based on combo rank
        UpdateSpeedFromCombo();
        
        // State machine now handles calling the appropriate methods
        // HandleMovement();
        // HandleJumping();
        // HandleShooting();
    }
    
    private void UpdateSpeedFromCombo()
    {
        if (ComboSystem.Instance == null) return;
        
        ComboRank combo = ComboSystem.Instance.GetCurrentCombo();
        float comboSpeedBonus = 0f;
        
        // Add speed bonus based on combo rank (5% per rank)
        if (combo != ComboRank.None)
        {
            comboSpeedBonus = (int)combo * 0.05f; // C=5%, B=10%, A=15%, S=20%, SS=25%, SSS=30%
        }
        
        // Apply combo bonus on top of base/power-up speed
        float powerUpMultiplier = (moveSpeed / baseMoveSpeed > 1.5f) ? 2f : 1f; // Check if power-up active
        moveSpeed = baseMoveSpeed * powerUpMultiplier * (1f + comboSpeedBonus);
    }
    
    public void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); 
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(horizontal, 0f, vertical).normalized;

        if (movement.magnitude > 0f)
        {
            transform.Translate(movement * moveSpeed * Time.deltaTime, Space.Self);
        }
    }
    
    public void HandleJumping()
    {
        // Check for jump input
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
    
            Debug.Log("Player jumped!");
        }
    }

    public void HandleShooting()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFire) 
        {
            nextFire = Time.time + fireRate;

            // pick spawn point
            Vector3 spawnPos = firePoint ? firePoint.position : transform.position + transform.forward * 1f;

            Instantiate(bulletPrefab, spawnPos, transform.rotation);
            EventManager.Instance.TriggerEvent(GameEvents.onBulletShot);

            Debug.Log("Bullet fired!");
        }
    }

    // Public getter for isGrounded
    public bool IsGrounded()
    {
        return isGrounded;
    }
    

   
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            Debug.Log("Player landed on ground");
        }
    }
    
    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            Debug.Log("Player left ground");
        }
    }
}

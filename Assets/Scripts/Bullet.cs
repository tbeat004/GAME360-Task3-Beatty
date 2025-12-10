using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 150f;   
    public float lifetime = 2f;
    private bool hitEnemy = false;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void FixedUpdate()
    {
        // Use MovePosition with the Rigidbody for physics-based movement
        if (rb != null)
        {
            Vector3 newPosition = rb.position + transform.forward * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        
    if (other.CompareTag("Enemy"))
    {
        hitEnemy = true;
        Destroy(gameObject);
    }
    else if (!other.CompareTag("Player") && !other.CompareTag("Coin"))
    {
        Destroy(gameObject); 
    }
    }

    void OnDestroy()
    {
        // If bullet is destroyed without hitting an enemy, trigger miss event
        if (!hitEnemy)
        {
            EventManager.Instance?.TriggerEvent(GameEvents.onBulletMissed, null);
        }
    }
}

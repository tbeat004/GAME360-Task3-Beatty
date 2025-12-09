using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 150f;   
    public float lifetime = 2f;
    private bool hitEnemy = false;

    void Awake()
    {
        // Set collision detection to ContinuousDynamic to prevent fast bullets from phasing through objects
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifetime); 
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
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

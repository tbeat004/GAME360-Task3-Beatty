using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 50f;   
    public float lifetime = 2f;
    private bool hitEnemy = false;

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

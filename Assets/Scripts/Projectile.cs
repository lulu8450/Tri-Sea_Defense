
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Stats")]
    public float speed = 5f;
    public float damage = 2f;
    public float lifetime = 3f;

    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.isKinematic = false;
        rb.freezeRotation = true;
    }

    /// <summary>
    /// Called by the turret to set the projectile's target and start movement.
    /// </summary>
    // Static helper to ensure the script is present and set the target
    public static Projectile EnsureAndSetTarget(GameObject projectileObj, Transform newTarget, float speed, float lifetime)
    {
        Projectile proj = projectileObj.GetComponent<Projectile>();
        if (proj == null)
        {
            proj = projectileObj.AddComponent<Projectile>();
        }
        proj.speed = speed;
        proj.lifetime = lifetime;
        proj.SetTarget(newTarget);
        return proj;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            rb.linearVelocity = dir * speed;
            Destroy(gameObject, lifetime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyMover enemy = other.GetComponent<EnemyMover>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
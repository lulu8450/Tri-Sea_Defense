using UnityEngine;

/// <summary>
/// Improved Projectile: supports damage, pooling, and clear English comments.
/// </summary>
public class ProjectileChange : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;
    public int damage = 1;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Destroy projectile if target is gone
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            // Deal damage to enemy if possible
            var enemy = target.GetComponent<EnemyMoverChange>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                // Fallback for old EnemyMover
                Destroy(target.gameObject);
                GameManager.Instance?.AddPearls(GameManager.Instance.pearlsPerEnemy);
            }
            Destroy(gameObject);
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}

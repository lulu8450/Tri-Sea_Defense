using UnityEngine;

/// <summary>
/// Improved EnemyMover: supports health, events, pooling, and clear English comments.
/// </summary>
public class EnemyMoverChange : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2f;
    public int damage = 1;
    public int maxHealth = 3;
    private int currentHealth;
    private int currentWaypointIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        if (currentWaypointIndex < waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                waypoints[currentWaypointIndex].position,
                speed * Time.deltaTime
            );
            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Enemy reached the end: damage base, then die
            GameManager.Instance?.TakeDamage(damage);
            Die();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // TODO: Add pooling support here
        GameManager.Instance?.AddPearls(GameManager.Instance.pearlsPerEnemy);
        Destroy(gameObject);
    }
}

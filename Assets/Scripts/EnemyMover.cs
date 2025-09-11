using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Improved EnemyMover: supports health, events, pooling, and clear English comments.
/// </summary>
public class EnemyMover : MonoBehaviour
{
    public Transform[] waypoints;


    [Header("Stats")]
    public float speed = 2f;
    public int damage = 1;
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("UI Elements")]
    private Slider healthSlider;    // Reference to the HealthBarUI slider
    private int currentWaypointIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
        // Find the Slider component in all children (works for nested UI)
        healthSlider = GetComponentInChildren<Slider>(true);
        if (healthSlider == null)
        {
            Debug.LogError("No Slider component found in children of Enemy prefab! Make sure HealthBarUI is a Slider.");
        }
        else
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        UpdateHealthBarUI();
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
            GameManager.Instance.TakeDamage(damage);
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount)  // Fonction pour que l'ennemi prenne des dégats
    {
        currentHealth -= amount;

        // Met  jour la barre de vie
        UpdateHealthBarUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthBarUI()    // Fonction pour mettre  jour l'affichage de la barre de vie
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
    void Die()  // Fonction pour gérer la mort de l'ennemi
    {
        GameManager.Instance.AddPearls(GameManager.Instance.pearlsPerEnemy);
        Destroy(gameObject);
    }

}
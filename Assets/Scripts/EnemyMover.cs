using UnityEngine;
using UnityEngine.UI;

public class EnemyMover : MonoBehaviour
{
    public Transform[] waypoints;
    

    [Header("Stats")]
    public float speed = 2f;
    public int damage = 1;
    public float maxHealth = 10f;
    private float currentHealth;
    [Header("UI Elements")]
    // Réference au Prefab de la barre de vie que tu viens de crer
    public GameObject healthBarPrefab; 
    private Slider healthSlider; // Réference au composant Slider

    private int currentWaypointIndex = 0;

    void Start()
    {
        currentHealth = maxHealth;
        
        // On crée une instance de la barre de vie
        GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, transform);
        
        // On rcupre le composant Slider sur l'instance
        healthSlider = healthBarInstance.GetComponent<Slider>();
        
        // On s'assure que le Slider existe bien
        if (healthSlider == null)
        {
            Debug.LogError("Le Prefab de barre de vie ne contient pas de composant Slider!");
        }
        
        // On positionne la barre de vie au-dessus de l'ennemi
        // Tu peux ajuster ces valeurs pour qu'elle soit bien place.
        healthBarInstance.transform.localPosition = new Vector3(0, 1.5f, 0); 
        
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
            // La valeur du slider est une échelle de 0 à 10.
            // On calcule le ratio (vie actuelle / vie max)
            healthSlider.value = currentHealth / maxHealth;
        }
    }
    void Die()  // Fonction pour gérer la mort de l'ennemi
    {
        GameManager.Instance.AddPearls(GameManager.Instance.pearlsPerEnemy);
        Destroy(gameObject); 
    }
    
}
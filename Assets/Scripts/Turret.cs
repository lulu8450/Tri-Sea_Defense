using UnityEngine;

public class Turret : MonoBehaviour
{
    // Vitesse de tir (un tir toutes les 0.5 secondes)
    public float fireRate = 2f; 
    // Portée de détection des ennemis
    public float detectionRange = 5f; 
    // public GameObject projectilePrefab;
    public string enemyTag = "Enemy";
    public Transform firePoint;
    public GameObject projectilePrefab; // Référence du projectile
    private GameObject loadedProjectilePrefab;
    private float nextFireTime;
    private Transform currentTarget;

    void Start()
    {
        // On prefab de projectile dans les Assets par son nom
        loadedProjectilePrefab = Resources.Load<GameObject>("Projectile"); 
    }

    void Update()
    {
        // 1. Chercher la cible
        // FindTarget();
        FindClosestTarget(); // 1. Chercher la cible la plus proche

        if (currentTarget != null && Time.time >= nextFireTime) // 2. Si une cible est trouvée et que le temps de tir est dépassé
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FindClosestTarget()
    {
        // On trouve tous les ennemis grace  leur tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        currentTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies) // Boucle sur tous les ennemis trouvés
        {
            float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= detectionRange && distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                currentTarget = enemy.transform;
            }
        }
    }

    // void Shoot()
    // {
    //     // On crée un nouveau projectile
    //     GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    //     // On le fait se déplacer vers la cible
    //     // projectile.GetComponent<Projectile>().SetTarget(target); 
    // }
    void Shoot()
    {
        if (loadedProjectilePrefab == null)
        {
            Debug.LogError("Projectile prefab non trouv dans le dossier Resources.");
            return;
        }
        
        GameObject projectile = Instantiate(loadedProjectilePrefab, transform.position, Quaternion.identity);
        // On passe la cible au script du projectile
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetTarget(currentTarget);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Pour voir la portée de la tourelle dans l'éditeur Unity (en bleu)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
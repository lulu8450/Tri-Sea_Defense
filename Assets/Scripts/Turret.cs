using System.Collections;
using UnityEngine;
using System.Linq; // Pour utiliser OrderBy

/// <summary>
/// Improved Turret script: modular, upgrade-ready, event-friendly, rotation, stable aim detection and clear English comments.
/// </summary>
public class Turret : MonoBehaviour
{
    [Header("Turret Stats")]
    /// <summary> Cadence de tir </summary>
    public float fireRate = 2f; 
    /// <summary> Portée de détection des ennemis </summary>
    public float detectionRange = 5f;
    /// <summary> Dégâts par tir </summary>
    public float damage = 10f;
    /// <summary> Vitesse de rotation </summary>
    public float rotationSpeed = 5f;
    /// <summary> Niveau de la tourelle </summary>
    public int level = 1;
    /// <summary> Multiplicateur d'amélioration </summary>
    public float upgradeMultiplier = 1.5f;

    [Header("References")]
    /// <summary> Référence du point de tir du projectile </summary>
    public Transform firePoint;
    /// <summary> Référence du projectile </summary>
    public GameObject projectilePrefab;
    /// <summary> Tag utilisé pour identifier les ennemis </summary>
    public string enemyTag = "Enemy";
    /// <summary> Temps du prochain tir </summary>
    private float nextFireTime;
    /// <summary> Cible actuelle de la tourelle </summary>
    private Transform currentTarget;
    /// <summary> Coroutine de recherche de cible </summary>
    private Coroutine findTargetCoroutine;

    void Start()
    {
        // On cherche le FirePoint s'il n'est pas assigné  dans l'Inspector
        if (firePoint == null)
        {
            firePoint = transform.Find("FirePoint");
            if (firePoint == null)
            {
                Debug.LogError("FirePoint child object not found. Please assign it or add a child named 'FirePoint'.");
            }
        }

        // On cherche le Prefab du projectile s'il n'est pas assigné
        if (projectilePrefab == null)
        {
            projectilePrefab = Resources.Load<GameObject>("Projectile");
            if (projectilePrefab == null)
            {
                Debug.LogWarning("ProjectilePrefab not assigned or not found in Resources folder.");
            }
        }

        // Démarrer la coroutine de recherche de cible
        StartCoroutine(FindClosestTargetCoroutine());
    }

    void Update()
    {
        // On s'assure que la cible est toujours valide et  portée
        if (currentTarget != null && Vector3.Distance(transform.position, currentTarget.position) > detectionRange)
        {
            currentTarget = null;
        }

        if (currentTarget != null)
        {
            // On fait face  la cible avec une rotation fluide
            RotateTowardsTarget();

            // Si le temps de tir est passé, on tire
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + 1f / fireRate;
            }
        }
        else
        {
            // Si pas de cible, on retourne  la rotation par défaut (vers le haut dans un jeu 2D)
            Vector3 defaultRotation = new Vector3(0, 0, 90);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(defaultRotation), Time.deltaTime * rotationSpeed);
        }
    }

    void RotateTowardsTarget()
    {
        // Calcule la direction vers la cible
        Vector3 direction = currentTarget.position - transform.position;
        // Calcule l'angle en degrés
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Crée une rotation de destination
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));

        // NOUVEAU: Utilisation de Quaternion.Slerp pour une rotation fluide
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    // IEnumerator FindClosestTargetCoroutine()    // NOUVEAU: Coroutine pour une recherche moins fréquente et plus performante
    // {
    //     // On ne va chercher la cible que toutes les 0.2 secondes
    //     float findTargetInterval = 0.2f;

    //     while (true)
    //     {
    //         // On trouve tous les ennemis grace  leur tag
    //         GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
    //         currentTarget = null;
    //         float shortestDistance = Mathf.Infinity;

    //         foreach (GameObject enemy in enemies)
    //         {
    //             float distanceToEnemy = Vector2.Distance(transform.position, enemy.transform.position);
    //             if (distanceToEnemy <= detectionRange && distanceToEnemy < shortestDistance)
    //             {
    //                 shortestDistance = distanceToEnemy;
    //                 currentTarget = enemy.transform;
    //             }
    //         }

    //         yield return new WaitForSeconds(findTargetInterval);
    //     }
    // }
    IEnumerator FindClosestTargetCoroutine()
    {
        while (true)
        {
            // On ne cherche une cible que si on n'en a pas une valide actuellement
            if (currentTarget == null)
            {
                // Utilise Physics2D.OverlapCircleAll pour plus de performance que FindGameObjectsWithTag
                Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRange);
                
                Transform closest = null;
                float shortestDistance = Mathf.Infinity;

                foreach (var hit in hits)
                {
                    // Vrifie si l'objet a le tag ennemi
                    if (hit.CompareTag(enemyTag))
                    {
                        float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                        if (distanceToEnemy < shortestDistance)
                        {
                            shortestDistance = distanceToEnemy;
                            closest = hit.transform;
                        }
                    }
                }
                currentTarget = closest;
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

    // void Shoot()
    // {
    //     if (projectilePrefab == null || firePoint == null || currentTarget == null)
    //     {
    //         Debug.LogError("Prefab de projectile ou FirePoint non assigné !");
    //         return;
    //     }
    //     // TODO: Use object pooling for projectiles

    //     // On instancie le projectile  la position du FirePoint (le bout du canon)
    //     GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

    //     // On passe la cible au script du projectile
    //     Projectile projectileScript = newProjectile.GetComponent<Projectile>();
    //     if (projectileScript != null)
    //     {
    //         projectileScript.SetTarget(currentTarget);
    //     }
    //     else
    //     {
    //         Debug.LogError("Le prefab de projectile n'a pas le script 'Projectile'!");
    //     }
    // }
    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile prefab ou FirePoint manquant !");
            return;
        }

        // On instancie le projectile  la position du FirePoint (le bout du canon)
        GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // On passe la cible au script du projectile
        Projectile projectileScript = newProjectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            // On passe la cible actuelle
            projectileScript.SetTarget(currentTarget);
            // projectileScript.damage = damage; // On passe aussi les dégâts
        }
        else
        {
            Debug.LogError("Le prefab de projectile n'a pas le script 'Projectile'!");
        }
    }

    public void UpgradeTurret() // Logique d'amélioration
    {
        level++;
        fireRate *= upgradeMultiplier;
        damage *= upgradeMultiplier;
        detectionRange *= 1.1f;
        // Add VFX/SFX/UI feedback here
    }

    void OnDrawGizmosSelected()
    {
        // Pour voir la porte de la tourelle dans l'diteur Unity (en bleu)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
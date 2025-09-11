using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine;
using System.Linq; // Pour utiliser OrderBy
using TMPro;


/// <summary>
/// Improved Turret script: modular, upgrade-ready, event-friendly, rotation, stable aim detection and clear English comments.
/// </summary>

public class Turret : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private GameManager gameManager;
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

    [Header("UI")]
    public TextMeshProUGUI LevelText;
    public TextMeshProUGUI UpgradeCostText;

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
        // Assign GameManager reference
        gameManager = GameManager.Instance;

        // Hide upgrade cost text by default
        SetUpgradeCostTextVisible(false);

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

        // Auto-assign LevelText if not set (search recursively for LevelText)
        if (LevelText == null)
        {
            var go = GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name == "LevelText");
            if (go != null) LevelText = go;
        }
        // Auto-assign UpgradeCostText if not set (search recursively for UpgradeCostText)
        if (UpgradeCostText == null)
        {
            var go = GetComponentsInChildren<TextMeshProUGUI>(true)
                .FirstOrDefault(t => t.name == "UpgradeCostText");
            if (go != null) UpgradeCostText = go;
        }
        UpdateLevelText();
        UpdateUpgradeCostText();
        // Démarrer la coroutine de recherche de cible
        StartCoroutine(FindClosestTargetCoroutine());
    }

    // Show upgrade cost text on mouse hover
    public void OnPointerEnter(PointerEventData eventData)
    {
        SetUpgradeCostTextVisible(true);
    }

    // Hide upgrade cost text when mouse leaves
    public void OnPointerExit(PointerEventData eventData)
    {
    SetUpgradeCostTextVisible(false);
    }

    private void UpdateUpgradeCostText()
    {
        if (UpgradeCostText != null)
            UpgradeCostText.text = $"Cost {GetUpgradeCost()} pearls.";
    }

    // Call this to show/hide the upgrade cost (for hover logic)
    public void SetUpgradeCostTextVisible(bool visible)
    {
        if (UpgradeCostText != null)
            UpgradeCostText.gameObject.SetActive(visible);
    }

    private void UpdateLevelText()
    {
        if (LevelText != null)
            LevelText.text = level.ToString();
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
    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null)
        {
            Debug.LogError("Projectile prefab ou FirePoint manquant !");
            return;
        }

    // Instantiate the projectile and ensure the script is present, then set the target
    GameObject newProjectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    Projectile.EnsureAndSetTarget(newProjectile, currentTarget, 10f, 3f); // Replace 10f and 3f with your desired speed/lifetime if needed
    }

    public void UpgradeTurret() // Logique d'amélioration
    {
        Debug.Log($"UpgradeTurret called on {gameObject.name}");
        level++;
        fireRate *= upgradeMultiplier;
        damage *= upgradeMultiplier;
        detectionRange *= 1.1f;
        UpdateLevelText();
        UpdateUpgradeCostText();
        if (gameManager != null)
            gameManager.UpdateTurretText();
        // Change color of all children of firePoint to #D63230
        Color upgradeColor;
        ColorUtility.TryParseHtmlString("#D63230", out upgradeColor);
        if (firePoint != null)
        {
            bool changed = false;
            foreach (Transform child in firePoint)
            {
                var sr = child.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = upgradeColor;
                    changed = true;
                }
            }
            if (!changed) Debug.LogWarning($"No SpriteRenderer found on firePoint children for {gameObject.name}");
        }
        else
        {
            Debug.LogWarning($"firePoint is null on {gameObject.name}");
        }
        Debug.Log($"Turret upgraded to level {level}!");
    }

    void OnDrawGizmosSelected()
    {
        // Pour voir la porte de la tourelle dans l'éditeur Unity (en bleu)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
    public int GetUpgradeCost()
    {
        return 2 * level; // Upgrade cost increases with level
    }
}
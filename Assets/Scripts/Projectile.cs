using UnityEngine;

public class Projectile : MonoBehaviour
{
    // Rférences publiques, visible dans l'Inspector de Unity
    [Header("Projectile Stats")]
    public float speed = 10f;
    public float damage = 20f;
    public float lifetime = 3f;

    // Rférences prives, gres par le script lui-mme
    private Rigidbody2D rb;
    private Transform target;

    void Awake()
    {
        // On rcupre le composant Rigidbody2D au dmarrage
        rb = GetComponent<Rigidbody2D>();

        // Si on n'a pas de Rigidbody, on l'ajoute. C'est une bonne pratique de scurit.
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }

        // On s'assure que la physique ne gne pas notre mouvement
        rb.gravityScale = 0;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        // Le projectile se dtruit tout seul aprs un certain temps
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Initialise le projectile avec une cible. Appelé par la tourelle.
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            // Calcule la direction vers la cible
            Vector2 direction = (target.position - transform.position).normalized;

            // Applique une vélocité (vitesse et direction) au Rigidbody2D
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // Si la tourelle a perdu sa cible avant de tirer, on détruit le projectile
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Si la cible est détruite pendant le vol du projectile
        if (target == null)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifie si le projectile entre en collision avec un ennemi
        if (other.CompareTag("Enemy"))
        {
            // // Tente de récupérer le script 'Enemy'
            // Enemy enemy = other.GetComponent<Enemy>();
            // Tente de récupérer le script 'EnemyMover'
            EnemyMover enemy = other.GetComponent<EnemyMover>();

            if (enemy != null)
            {
                // Appelle la méthode TakeDamage de l'ennemi
                enemy.TakeDamage(damage);
            }
            
            // Détruit le projectile après l'impact
            Destroy(gameObject);
        }
    }
}
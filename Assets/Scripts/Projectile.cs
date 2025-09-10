using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    public float speed = 10f;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // Dtruit le projectile si la cible a t dtruite
            return;
        }
        
        // Dplace le projectile vers la cible
        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Vrifie si le projectile a atteint sa cible
        if (Vector2.Distance(transform.position, target.position) < 0.2f)
        {
            // On rcupre le script de l'ennemi pour lui infliger des dgts
            // Note : Pour l'instant on le dtruit directement, tu peux ajouter une fonction TakeDamage()
            GameManager.Instance.AddPearls(GameManager.Instance.pearlsPerEnemy);
            Destroy(target.gameObject);
            Destroy(gameObject);
        }
    }
    
    // Fonction appele par la tourelle pour dfinir la cible
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
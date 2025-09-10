using UnityEngine;

public class EnemyMover : MonoBehaviour
{
    public Transform[] waypoints;  // Tableau pour stocker les points de passage que l'ennemi va suivre
    public float speed = 2f; // Vitesse de déplacement de l'ennemi
    public int damage = 1; // Dégats que l'ennemi fait

    private int currentWaypointIndex = 0;

    // void Update()
    // {
    //     // On vérifie qu'il y a encore des points de passage à atteindre
    //     if (currentWaypointIndex < waypoints.Length)
    //     {
    //         // Calcule la nouvelle position pour se déplacer vers le prochain waypoint
    //         transform.position = Vector2.MoveTowards(
    //             transform.position,
    //             waypoints[currentWaypointIndex].position,
    //             speed * Time.deltaTime
    //         );

    //         // Si l'ennemi est très proche du waypoint, on passe au suivant
    //         if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
    //         {
    //             currentWaypointIndex++;
    //         }
    //     }
    //     else
    //     {
    //         // L'ennemi a atteint la fin du chemin
    //         // C'est ici que tu peux réduire la vie de ta base (à coder dans GameManager)
    //         // et détruire l'ennemi
    //         Debug.Log("Un ennemi a atteint la fin !");
    //         Destroy(gameObject);
    //     }
    // }
        void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (currentWaypointIndex < waypoints.Length) // On vérifie qu'il y a encore des points de passage à atteindre
        {
            // Déplacement vers le prochain point de passage
            transform.position = Vector2.MoveTowards(
                transform.position,
                waypoints[currentWaypointIndex].position,
                speed * Time.deltaTime
            );

            // Si l'ennemi est très proche du waypoint, on passe au suivant
            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {
            // Si l'ennemi a atteint la fin du chemin et est toujours en vie : il inflige des dègats
            // On fait appel  la fonction du GameManager par code
            GameManager.Instance.TakeDamage(damage); 
            Destroy(gameObject); 
        }
    }
}
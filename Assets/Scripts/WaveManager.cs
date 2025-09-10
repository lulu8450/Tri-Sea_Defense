using System.Collections;
using UnityEngine;
using System.Linq; // Ncessaire pour utiliser .FirstOrDefault()

public class WaveManager : MonoBehaviour
{
    // On rcupre les rfrences par le nom du prefab
    private GameObject enemyPrefab; 
    
    // Le point de dpart des ennemis (on ne le cherche plus par tag)
    // On va le dfinir dynamiquement.
    private Transform spawnPoint; 

    // Les points de passage  transmettre  l'ennemi
    private Transform[] waypoints; 

    public int enemiesPerWave = 5;
    public float timeBetweenSpawns = 1f;
    public float timeBetweenWaves = 5f;

    void Start()
    {
        // On cherche le prefab de l'ennemi dans le dossier Resources
        enemyPrefab = Resources.Load<GameObject>("Enemy");
        if (enemyPrefab == null)
        {
            Debug.LogError("Le Prefab d'ennemi 'Enemy' n'a pas t trouv dans le dossier Resources. Vrifie le nom et le chemin !");
            return; 
        }
        

    }

    public void StartWave()
    {
        // Démarre le cycle de vagues
        StartCoroutine(StartWaveCycle());
    }
    // private IEnumerator StartWaveCycle()
    // {
    //     while (true)
    //     {
    //         // On rcupre le chemin de la grille avant de dclencher la vague
    //         waypoints = HexGridManager.Instance.GetPathWaypoints();

    //         // On vrifie qu'il y a un chemin avant de lancer la vague
    //         if (waypoints == null || waypoints.Length == 0)
    //         {
    //             Debug.LogWarning("Aucun chemin d'ennemi n'est dfinis. Les ennemis ne peuvent pas spawn.");
    //             // On attend la prochaine vague pour vrifier  nouveau
    //             yield return new WaitForSeconds(timeBetweenWaves);
    //             continue; // Passe  l'itration suivante de la boucle
    //         }

    //         // On dfinit le point de dpart comme le premier waypoint
    //         spawnPoint = waypoints.FirstOrDefault();

    //         yield return StartCoroutine(SpawnWave());
    //         Debug.Log("Vague termine. Prochaine vague dans " + timeBetweenWaves + " secondes.");
    //         yield return new WaitForSeconds(timeBetweenWaves);
    //     }
    // }
        private IEnumerator StartWaveCycle()
        {
            while (true)
            {
                waypoints = HexGridManager.Instance.GetPathWaypoints();

                if (waypoints == null || waypoints.Length == 0)
                {
                    Debug.LogWarning("Aucun chemin d'ennemi n'est défini. Les ennemis ne peuvent pas spawn.");
                    yield return new WaitForSeconds(timeBetweenWaves);
                    continue;
                }

                // NOUVEAU: On inverse la liste de waypoints pour que les ennemis partent du dernier hexagone
                System.Array.Reverse(waypoints);

                spawnPoint = waypoints.FirstOrDefault();

                yield return StartCoroutine(SpawnWave());
                Debug.Log("Vague terminée. Prochaine vague dans " + timeBetweenWaves + " secondes.");
                yield return new WaitForSeconds(timeBetweenWaves);
            }
        }

    private IEnumerator SpawnWave()
    {
        if (spawnPoint == null)
        {
            Debug.LogError("Le point de départ (premier waypoint) n'a pas été trouvé. Impossible de spawner les ennemis.");
            yield break;
        }

        for (int i = 0; i < enemiesPerWave; i++)
        {
            // On instancie l'ennemi au point de spawn
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

            // On rcupre le script de mouvement et on lui assigne les waypoints
            EnemyMover enemyMover = newEnemy.GetComponent<EnemyMover>();

            if (enemyMover != null)
            {
                enemyMover.waypoints = waypoints;
            }
            else
            {
                Debug.LogError("Le prefab d'ennemi 'Enemy' n'a pas le script 'EnemyMover' !");
                Destroy(newEnemy);
                yield break;
            }

            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
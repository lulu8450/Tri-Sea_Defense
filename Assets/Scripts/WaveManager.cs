
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // Ncessaire pour utiliser .FirstOrDefault()

public class WaveManager : MonoBehaviour
{
    [Header("UI")]
    public Button StartWaveButton;
    private bool waveStarted = false;
    // On récupère les références par le nom du prefab
    private GameObject enemyPrefab; 
    
    // Le point de départ des ennemis (on ne le cherche plus par tag)
    // On va le définir dynamiquement.
    private Transform spawnPoint; 

    // Les points de passage  transmettre  l'ennemi
    private Transform[] waypoints; 

    public int enemiesPerWave = 5;
    public float timeBetweenSpawns = 1f;
    public float timeBetweenWaves = 5f;
    private int waveLevel = 1;

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
        if (waveStarted) return;
        waveStarted = true;
        // Disable the start button
        if (StartWaveButton == null)
        {
            var go = GameObject.Find("StartWaveButton");
            if (go != null) StartWaveButton = go.GetComponent<Button>();
        }
        if (StartWaveButton != null)
            StartWaveButton.gameObject.SetActive(false);

        // Démarre le cycle de vagues seulement si tout est placé
        if (GameManager.Instance.AllPlacedBeforeWave())
        {
            StartCoroutine(StartWaveCycle());
        }
        else
        {
            Debug.Log("You must place all available turrets and paths before starting the wave!");
        }
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

            // Calculate enemies per wave based on number of paths placed
            int pathCount = GameManager.Instance.availablePaths;
            enemiesPerWave = Mathf.CeilToInt(enemiesPerWave * 1.2f) + (pathCount - 1) * 2; // +2 enemies per extra path
            Debug.Log($"Starting wave {waveLevel} with {enemiesPerWave} enemies (paths: {pathCount})");
            yield return StartCoroutine(SpawnWave());
            waveLevel++;
            GameManager.Instance.AddAvailablePath(); // Add 1 more path for next wave
            GameManager.Instance.availableTurrets = 3; // Reset turrets for next wave if you want
            GameManager.Instance.availablePaths = GameManager.Instance.availablePaths; // Paths already incremented
            Debug.Log($"Vague terminée. Prochaine vague dans {timeBetweenWaves} secondes. Next wave: {waveLevel}");
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

        // Decide how many big enemies this wave (e.g., 1 + waveLevel/3)
        int bigEnemiesCount = Mathf.Min(waveLevel / 3 + 1, enemiesPerWave);
        HashSet<int> bigEnemyIndices = new HashSet<int>();
        while (bigEnemyIndices.Count < bigEnemiesCount)
        {
            bigEnemyIndices.Add(Random.Range(0, enemiesPerWave));
        }

        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            EnemyMover enemyMover = newEnemy.GetComponent<EnemyMover>();
            if (enemyMover != null)
            {
                enemyMover.waypoints = waypoints;
                if (bigEnemyIndices.Contains(i))
                {
                    enemyMover.maxHealth = Mathf.RoundToInt(enemyMover.maxHealth * (1.5f + waveLevel * 0.2f));
                    enemyMover.speed *= 0.7f;
                    newEnemy.transform.localScale *= 1.3f;
                }
                else
                {
                    enemyMover.maxHealth = Mathf.RoundToInt(enemyMover.maxHealth * (1f + waveLevel * 0.1f));
                }
                // Reset currentHealth to maxHealth so enemy spawns alive
                var currentHealthField = enemyMover.GetType().GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (currentHealthField != null)
                {
                    currentHealthField.SetValue(enemyMover, enemyMover.maxHealth);
                }
                enemyMover.TakeDamage(0); // Force health bar update
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